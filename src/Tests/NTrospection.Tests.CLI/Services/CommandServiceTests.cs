using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Models;
using NTrospection.CLI.Services;

namespace NTrospection.Tests.CLI.Models
{
    [TestClass]
    public class CommandServiceTests
    {
	private Mock<IParameterService> _mockParameterService;
	private CommandService _service;
	private CommandMethod _command;
	private CommandMethod _nonCommand;
	
	[TestInitialize]
	public void Init()
	{
	    _mockParameterService = new Mock<IParameterService>();
	    _service = new CommandService(_mockParameterService.Object);

	    _command = new CommandMethod(typeof(CommandServiceTests).GetMethod("FakeMethod"));
	    _nonCommand = new CommandMethod(typeof(CommandServiceTests).GetMethod("FakeMethodNonCommand"));
	}

	// === Fake Method For Reflection ===
	[CliCommand("Fake", "Fake Method For Reflection")]
	public void FakeMethod(
			       [CliParameter('f', "")]int foo,
			       string bar,
			       [CliParameter("")]int foobar)
	{
	    
	}

	// === Fake Method For Reflection ===
	public void FakeMethodNonCommand()
	{
	    
	}

	[TestMethod]
	public void GetCommandName_ReturnsName_WhenAttributeIsProvided()
	{
	    var expected = "Fake";
	    var actual = _service.GetCommandName(_command);

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetCommandName_ReturnsNull_WhenAttributeIsNull()
	{
	    string expected = null;
	    var actual = _service.GetCommandName(_nonCommand);

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetParameterErrors_ReturnsEmptyList_WhenParamtersAreValid()
	{
	    var args = new List<CommandLineArgument>
	    {
		new CommandLineArgument
		{
		    Command = "foo",
		    Value = "5",
		    Order = 1,
		},
		new CommandLineArgument
		{
		    Command = "bar",
		    Value = "something",
		    Order = 2,
		}
	    };

	    var expected = new List<string>();
	    var actual = _service.GetParameterErrors(_command, args);

	    CollectionAssert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetParameterErrors_ReturnsListOfErrors_WhenParamtersAreInvalid()
	{
	    var args = new List<CommandLineArgument>
	    {
		new CommandLineArgument
		{
		    Command = "fool",
		    Value = "5",
		    Order = 1,
		},
		new CommandLineArgument
		{
		    Command = "blah",
		    Value = "something",
		    Order = 2,
		}
	    };

	    var expected = new List<string>
	    {
		"The parameter 'fool' is not a valid parameter",
		"The parameter 'blah' is not a valid parameter"
	    };
	    var actual = _service.GetParameterErrors(_command, args);

	    CollectionAssert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void ParameterMatchesArg_ReturnsTrue_WhenArgumentMatchesParameter()
	{
	    var arg = new CommandLineArgument
	    {
		Command = "foo",
		Value = "5",
		Order = 1,
	    };
	    var pi = _command.Info.GetParameters()[0];

	    var expected = true;
	    var actual = _service.ParameterMatchesArg(_command, pi, arg);

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void ParameterMatchesArg_ReturnsTrue_WhenArgumentMatchesParameterAlias()
	{
	    var arg = new CommandLineArgument
	    {
		Command = "f",
		Value = "5",
		Order = 1,
	    };
	    var pi = _command.Info.GetParameters()[0];

	    var expected = true;
	    var actual = _service.ParameterMatchesArg(_command, pi, arg);

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void ParameterMatchesArg_ReturnsFalse_WhenArgumentDoesNotMatchParameter()
	{
	    var arg = new CommandLineArgument
	    {
		Command = "barf",
		Value = "something",
		Order = 1,
	    };
	    var pi = _command.Info.GetParameters()[1];

	    var expected = false;
	    var actual = _service.ParameterMatchesArg(_command, pi, arg);

	    Assert.AreEqual(expected, actual);
	}
    }
}
