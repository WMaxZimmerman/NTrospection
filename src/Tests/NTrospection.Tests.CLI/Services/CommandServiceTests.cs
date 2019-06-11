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
	public void FakeMethod()
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
    }
}
