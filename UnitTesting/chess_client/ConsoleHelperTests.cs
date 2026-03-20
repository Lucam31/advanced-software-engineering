using chess_client;
using UnitTesting.Mock;

namespace UnitTesting.chess_client;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Unit tests for the <see cref="ConsoleHelper"/> class
/// The tests simulate console in and outputs using <see cref="Console.SetIn(TextReader)"/>
/// and <see cref="Console.SetOut(TextWriter)"/>
/// </summary>
[DoNotParallelize]
[TestClass]
public class ConsoleHelperTest
{
    /// <summary>
    /// Saved original console input stream, restored in test cleanup
    /// </summary>
    private TextReader? _originalIn;
    private TextWriter? _originalOut;
    private readonly StringWriter _output = new StringWriter();
    private readonly MockConsoleWriter _mockOutput = new MockConsoleWriter();

    /// <summary>
    /// Initializes each test by saving the current console streams and
    /// redirecting the console output to a <see cref="StringWriter"/>
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        Console.SetOut(_output);
        ConsoleHelper.SetConsole(new SystemConsoleAdapterProxy());
    }

    /// <summary>
    /// Restores the original console in and outputs after each test
    /// executed automatically after every test
    /// </summary>
    [TestCleanup]
    public void TearDown()
    {
        if (_originalIn != null) Console.SetIn(_originalIn);
        if (_originalOut != null) Console.SetOut(_originalOut);
        ConsoleHelper.ResetConsole();
    }

    /// <summary>
    /// Verifies that <see cref="ConsoleHelper.PrintConsole(string)"/> outputs the expected string to the console
    /// </summary>
    [TestMethod]
    public void TestPrintConsole_OutputEqualsInput()
    {
        ConsoleHelper.PrintConsole("Test");

        Assert.AreEqual("Test", _output.ToString());
    }

    /// <summary>
    /// Verifies that <see cref="ConsoleHelper.PrintConsoleNewline(string)"/> outputs the expected string with a newline
    /// to the console
    /// </summary>
    [TestMethod]
    public void TestPrintConsoleNewline_OutputEqualsNewlineInput()
    {
        ConsoleHelper.PrintConsoleNewline("TestNewline");
        
        Assert.AreEqual("\nTestNewline", _output.ToString());
    }
    
    /// <summary>
    /// Verifies that <see cref="ConsoleHelper.ClearCurrentConsoleLine"/> clears the current console line
    /// </summary>
    [TestMethod]
    public void TestClearCurrentConsoleLine_OutputClearsLine()
    {
        ConsoleHelper.SetConsole(_mockOutput);
        ConsoleHelper.PrintConsole("TestLine");
        ConsoleHelper.ClearCurrentConsoleLine();
        // Assert.AreEqual(string.Empty, _mockOutput.GetLine(_mockOutput.CursorTop));
        Assert.AreEqual(string.Empty, _mockOutput.GetLine(0));
    }
    
    /// <summary>
    /// Verifies that <see cref="ConsoleHelper.OverwriteLine(string)"/> overwrites the current console line
    /// with the expected string
    /// </summary>
    [TestMethod]
    public void TestOverwriteLine_OutputEqualsInput()
    {
        ConsoleHelper.SetConsole(_mockOutput);
        ConsoleHelper.PrintConsole("OldLine");
        ConsoleHelper.OverwriteLine("NewLine");
        Assert.AreEqual("NewLine", _mockOutput.GetLine(_mockOutput.CursorTop));
    }
    
    /// <summary>
    /// Verifies that <see cref="ConsoleHelper.WriteErrorMessage(string)"/> outputs the expected error message
    /// to the console
    /// </summary>
    [TestMethod]
    public void TestWriteErrorMessage_OutputEqualsInput()
    {
        ConsoleHelper.SetConsole(_mockOutput);
        ConsoleHelper.PrintConsoleNewline("SomeMessage");
        ConsoleHelper.WriteErrorMessage("ErrorMessage");
        Assert.AreEqual("ErrorMessage", _mockOutput.GetLine(0));
    }

    /// <summary>
    /// A class to make testing with the system console easier
    /// </summary>
    private sealed class SystemConsoleAdapterProxy : IConsoleAdapter
    {
        public int CursorTop => Console.CursorTop;
        public int WindowWidth => Console.WindowWidth;
        public int WindowHeight => Console.WindowHeight;
        public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);
        public void Write(string value) => Console.Write(value);
    }
}