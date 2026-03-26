using chess_client;
using Shared.Logger;

namespace UnitTesting.Shared;

using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

/// <summary>
/// Unit tests for the <see cref="GameLogger"/> class
/// The tests simulate console in and outputs using <see cref="Console.SetIn(TextReader)"/>
/// and <see cref="Console.SetOut(TextWriter)"/>
/// </summary>
[DoNotParallelize]
[TestClass]
public sealed class LoggerTests
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
        GameLogger.Configure(LogLevel.Debug, true, false);
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
    /// Verifies that <see cref="GameLogger.Debug(string)"/>
    /// prints the passed message to the console
    /// </summary>
    [TestMethod]
    public void TestGameLogger_DebugLevelOutput()
    {
        // Arrange
        var test = "Test debug message";

        // Act
        GameLogger.Debug(test);

        // Assert
        var levelString = "[DEBUG] ";
        var filteredLevel = _output.ToString().Substring(_output.ToString().Length - test.Length - 1 - levelString.Length, levelString.Length);
        Assert.AreEqual(levelString, filteredLevel);

        var filteredMessage = _output.ToString().Substring(_output.ToString().Length - test.Length - 1, test.Length);
        Assert.AreEqual(test, filteredMessage);
    }
    
    /// <summary>
    /// Verifies that <see cref="GameLogger.Info(string)"/>
    /// prints the passed message to the console
    /// </summary>
    [TestMethod]
    public void TestGameLogger_InfoLevelOutput()
    {
        // Arrange
        var test = "Test info message";

        // Act
        GameLogger.Info(test);

        // Assert
        var levelString = "[INFO] ";
        var filteredLevel = _output.ToString().Substring(_output.ToString().Length - test.Length - 1 - levelString.Length, levelString.Length);
        Assert.AreEqual(levelString, filteredLevel);

        var filteredMessage = _output.ToString().Substring(_output.ToString().Length - test.Length - 1, test.Length);
        Assert.AreEqual(test, filteredMessage);
    }
    
    /// <summary>
    /// Verifies that <see cref="GameLogger.Warning(string)"/>
    /// prints the passed message to the console
    /// </summary>
    [TestMethod]
    public void TestGameLogger_WarningLevelOutput()
    {
        // Arrange
        var test = "Test warning message";

        // Act
        GameLogger.Warning(test);

        // Assert
        var levelString = "[WARNING] ";
        var filteredLevel = _output.ToString().Substring(_output.ToString().Length - test.Length - 1 - levelString.Length, levelString.Length);
        Assert.AreEqual(levelString, filteredLevel);

        var filteredMessage = _output.ToString().Substring(_output.ToString().Length - test.Length - 1, test.Length);
        Assert.AreEqual(test, filteredMessage);
    }
    
    /// <summary>
    /// Verifies that <see cref="GameLogger.Error(string, Exception)"/>
    /// prints the passed message to the console
    /// </summary>
    [TestMethod]
    public void TestGameLogger_ErrorLevelOutput()
    {
        // Arrange
        var test = "Test error message";

        // Act
        GameLogger.Error(test);

        // Assert
        var levelString = "[ERROR] ";
        var filteredLevel = _output.ToString().Substring(_output.ToString().Length - test.Length - 1 - levelString.Length, levelString.Length);
        Assert.AreEqual(levelString, filteredLevel);

        var filteredMessage = _output.ToString().Substring(_output.ToString().Length - test.Length - 1, test.Length);
        Assert.AreEqual(test, filteredMessage);
    }
    
    /// <summary>
    /// Verifies that <see cref="GameLogger.Fatal(string, Exception)"/>
    /// prints the passed message to the console
    /// </summary>
    [TestMethod]
    public void TestGameLogger_FatalLevelOutput()
    {
        // Arrange
        var test = "Test fatal message";

        // Act
        GameLogger.Fatal(test);

        // Assert
        var levelString = "[FATAL] ";
        var filteredLevel = _output.ToString().Substring(_output.ToString().Length - test.Length - 1 - levelString.Length, levelString.Length);
        Assert.AreEqual(levelString, filteredLevel);

        var filteredMessage = _output.ToString().Substring(_output.ToString().Length - test.Length - 1, test.Length);
        Assert.AreEqual(test, filteredMessage);
    }
    
    /// <summary>
    /// Verifies that <see cref="GameLogger.Debug(string)"/>
    /// prints no message if the log level is set higher than Debug
    /// </summary>
    [TestMethod]
    public void TestGameLogger_DebugLevelNoOutput()
    {
        // Arrange
        GameLogger.Configure(LogLevel.Fatal, true, false);
        
        var test = "Test debug message";

        // Act
        GameLogger.Debug(test);

        // Assert
        Assert.AreEqual("", _output.ToString());
    }
}