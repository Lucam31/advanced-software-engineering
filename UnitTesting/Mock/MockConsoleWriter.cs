using System.Text;
using System.Linq;
using chess_client;

namespace UnitTesting.Mock;

/// <summary>
/// A mock console implementation that simulates a text console with cursor positioning
/// and line clearing/overwriting behavior.
/// </summary>
public class MockConsoleWriter : IConsoleAdapter
{
    private readonly List<StringBuilder> _lines = new();
    private int _cursorTop;
    private int _cursorLeft;

    public int CursorTop => _cursorTop;
    /// <summary>
    /// sets the width and height of the mock console
    /// the width equals the number of characters per line
    /// the height equals the number of lines
    /// </summary>
    public int WindowWidth { get; set; } = 120;
    public int WindowHeight { get; set; } = 60;

    public override string ToString() => string.Join("\n", _lines.Select(sb => sb.ToString()));

    /// <summary>
    /// sets the cursor position within the mock console
    /// </summary>
    /// <param name="left"></param>
    /// <param name="top"></param>
    public void SetCursorPosition(int left, int top)
    {
        _cursorLeft = Math.Max(0, left);
        _cursorTop = Math.Max(0, top);
        EnsureLine(_cursorTop);
    }

    /// <summary>
    /// Writes a string to the mock console at the current cursor position.
    /// </summary>
    /// <param name="value"></param>
    public void Write(string value)
    {
        foreach (var ch in value)
        {
            if (ch == '\n')
            {
                _cursorTop++;
                _cursorLeft = 0;
                EnsureLine(_cursorTop);
                continue;
            }

            EnsureLine(_cursorTop);
            var line = _lines[_cursorTop];
            while (line.Length < _cursorLeft)
            {
                line.Append(' ');
            }

            if (_cursorLeft < line.Length)
            {
                line[_cursorLeft] = ch;
            }
            else
            {
                line.Append(ch);
            }

            _cursorLeft++;
        }
    }

    /// <summary>
    /// ensures that the internal line list has at least the specified index
    /// </summary>
    /// <param name="index"></param>
    private void EnsureLine(int index)
    {
        while (_lines.Count <= index)
        {
            _lines.Add(new StringBuilder());
        }
    }

    /// <summary>
    /// helper methods to get the content of a specific line
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public string GetLine(int index) => index >= 0 && index < _lines.Count ? _lines[index].ToString().Trim() : string.Empty;
    
    /// <summary>
    /// helper method to get the entire buffer content as a string
    /// </summary>
    public string GetBuffer() => ToString();
}