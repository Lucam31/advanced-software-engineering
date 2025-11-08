using chess_client;
using UnitTesting.Mock;

namespace UnitTesting.chess_client;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Unit tests for the <see cref="InputParser"/> class
/// The tests simulate console in and outputs using <see cref="Console.SetIn(TextReader)"/>
/// and <see cref="Console.SetOut(TextWriter)"/>
/// </summary>
[DoNotParallelize]
[TestClass]
public class CliOutputTest
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
        CliOutput.SetConsole(new SystemConsoleAdapterProxy());
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
        CliOutput.ResetConsole();
    }

    /// <summary>
    /// Verifies that <see cref="CliOutput.PrintConsole(string)"/> outputs the expected string to the console
    /// </summary>
    [TestMethod]
    public void TestPrintConsole_OutputEqualsInput()
    {
        CliOutput.PrintConsole("Test");

        Assert.AreEqual("Test", _output.ToString());
    }

    /// <summary>
    /// Verifies that <see cref="CliOutput.PrintConsoleNewline(string)"/> outputs the expected string with a newline
    /// to the console
    /// </summary>
    [TestMethod]
    public void TestPrintConsoleNewline_OutputEqualsNewlineInput()
    {
        CliOutput.PrintConsoleNewline("TestNewline");
        
        Assert.AreEqual("\nTestNewline", _output.ToString());
    }
    
    /// <summary>
    /// Verifies that <see cref="CliOutput.ClearCurrentConsoleLine"/> clears the current console line
    /// </summary>
    [TestMethod]
    public void TestClearCurrentConsoleLine_OutputClearsLine()
    {
        CliOutput.SetConsole(_mockOutput);
        CliOutput.PrintConsole("TestLine");
        CliOutput.ClearCurrentConsoleLine();
        // Assert.AreEqual(string.Empty, _mockOutput.GetLine(_mockOutput.CursorTop));
        Assert.AreEqual(string.Empty, _mockOutput.GetLine(0));
    }
    
    /// <summary>
    /// Verifies that <see cref="CliOutput.OverwriteLine(string)"/> overwrites the current console line
    /// with the expected string
    /// </summary>
    [TestMethod]
    public void TestOverwriteLine_OutputEqualsInput()
    {
        CliOutput.SetConsole(_mockOutput);
        CliOutput.PrintConsole("OldLine");
        CliOutput.OverwriteLine("NewLine");
        Assert.AreEqual("NewLine", _mockOutput.GetLine(_mockOutput.CursorTop));
    }
    
    /// <summary>
    /// Verifies that <see cref="CliOutput.WriteErrorMessage(string)"/> outputs the expected error message
    /// to the console
    /// </summary>
    [TestMethod]
    public void TestWriteErrorMessage_OutputEqualsInput()
    {
        CliOutput.SetConsole(_mockOutput);
        CliOutput.PrintConsoleNewline("SomeMessage");
        CliOutput.WriteErrorMessage("ErrorMessage");
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