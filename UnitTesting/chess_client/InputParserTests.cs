using chess_client;
using Shared;

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
public sealed class InputParserTest
{
    /// <summary>
    /// Saved original console input stream, restored in test cleanup
    /// </summary>
    private TextReader? _originalIn;
    private TextWriter? _originalOut;
    private readonly StringWriter _output = new StringWriter();

    /// <summary>
    /// Initializes each test by saving the current console streams and
    /// redirecting the console output to a <see cref="StringWriter"/>
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        Console.SetOut(_output); // suppress console output during tests
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
    }

    /// <summary>
    /// Verifies that <see cref="InputParser.ReadInput(string,bool,string[])"/>
    /// accepts a single valid input (here B) and returns it in uppercase
    /// </summary>
    [TestMethod]
    public void TestReadInput_SingleValidEntry_ReturnsValue()
    {
        // simulate user input B (\n represents pressing Enter)
        Console.SetIn(new StringReader("B\n")); 

        var result = InputParser.ReadInput("Test input: ", false, "A", "B", "C");

        Assert.AreEqual("B", result);
    }

    /// <summary>
    /// Verifies that invalid inputs are ignored first and the next
    /// valid input (here C) is returned.
    /// </summary>
    [TestMethod]
    public void TestReadInput_InvalidThenValid_ReturnsSecondValue()
    {
        // simulate user input: first invalid (x), then valid (C) (\n represents Enter)
        Console.SetIn(new StringReader("x\nC\n"));
        string prompt = "Test input: ";

        var result = InputParser.ReadInput(prompt, false, "A", "B", "C");
        
        Assert.AreEqual("C", result);
    }

    /// <summary>
    /// Ensures that the prompt text is actually written to the console
    /// before reading the input
    /// </summary>
    [TestMethod]
    public void TestReadInput_PromptIsPrintedToConsole()
    {
        Console.SetIn(new StringReader("C\n"));
        string prompt = "Test input: ";
        
        var result = InputParser.ReadInput(prompt, false, "A", "B", "C");
        
        var promptOutput = _output.ToString().Substring(0, prompt.Length);
        Assert.AreEqual(promptOutput, prompt);
    }
    
    /// <summary>
    /// Verifies that an <see cref="ArgumentException"/> is thrown
    /// when no validInputs array is provided
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
        "validInputs must contain at least one valid input.")]
    public void TestReadInput_NoValidInputs_ThrowsException()
    {
        var result = InputParser.ReadInput("Test input: ", false, []);
    }
    
    /// <summary>
    /// Verifies that <see cref="InputParser.ReadMove()"/> correctly
    /// turns a valid move string (e2e4) into a <c>Move</c> object with From=E2 and To=E4
    /// </summary>
    [TestMethod]
    public void TestReadMove_ValidMove_ReturnsMoveObject()
    {
        // simulate user input e2e4 (\n represents Enter)
        Console.SetIn(new StringReader("e2e4\n"));

        var move = InputParser.ReadMove();

        Assert.IsNotNull(move);
        Assert.AreEqual("E2", move.From);
        Assert.AreEqual("E4", move.To);
    }
    
    /// <summary>
    /// Verifies that an <see cref="ArgumentException"/> is thrown
    /// when an invalid coordinate (here x2x4) is entered
    /// </summary>
    [TestMethod]
    [ExpectedException(typeof(ArgumentException),
        "Input was not valid")]
    public void TestReadMove_InvalidMove_ThrowsException()
    {
        // simulate user input x2x4 (\n represents Enter)
        Console.SetIn(new StringReader("x2x4\n"));

        var move = InputParser.ReadMove();
    }
}