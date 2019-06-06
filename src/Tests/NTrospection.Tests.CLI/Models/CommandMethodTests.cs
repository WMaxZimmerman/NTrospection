using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using NTrospection.Tests.CLI.Common;

namespace NTrospection.Tests.CLI.Models
{
    [TestClass]
    public class CommandMethodTests: BaseCliTest
    {
	MethodInfo _nothingMethod;
	MethodInfo _errorMethod;
	MethodInfo _parameterMethod;

	[TestInitialize]
	public void Init()
	{
	    var me = typeof(CommandMethodTests);
	    _nothingMethod = me.GetMethod("MethodThatDoesNothing");
	    _errorMethod = me.GetMethod("MethodThatThrowsError");
	    _parameterMethod = me.GetMethod("MethodThatHasParameter");
	}

	

	// === Test Methods ===
	// Kept here to avoid line number changes in stack trace
	
	public void MethodThatDoesNothing()
	{
	    // Just use for above tests to call
	}
	
	public void MethodThatThrowsError()
	{
	    throw new Exception("I went boom!");
	}
	
	public void MethodThatHasParameter(int something)
	{
	    throw new Exception("I went boom!");
	}
	
	// === Test Methods ===
	
        [TestMethod]
        public void InvokeWillReturnTrueWhenMethodSucceeds()
        {
	    var cm = new CommandMethod(_nothingMethod);
	    var actual = cm.Invoke(new List<CommandLineArgument>());
	    var expected = true;

	    Assert.AreEqual(expected, actual.WasSuccess);
        }
	
        [TestMethod]
        public void InvokeWillReturnFalseWhenMethodThrowsException()
        {
	    var cm = new CommandMethod(_errorMethod);
	    var expected = false;

            var consoleLines = new List<string>
            {
                "An error occurred while executing the command.",
                "Message: I went boom!",
                GetStackTraceForException("Models", "CommandMethodTests", "MethodThatThrowsError", "", 39)
            };

	    var actual = cm.Invoke(new List<CommandLineArgument>());

	    Assert.AreEqual(expected, actual.WasSuccess);
	    AssertCollectionsAreEqual(consoleLines, actual.Messages);
        }

        [TestMethod]
        public void InvokeWillReturnFalseWhenMethodIsNotCalledWithParameters()
        {
	    var cm = new CommandMethod(_parameterMethod);
	    var expected = false;

            var consoleLines = new List<string>();
	    
	    var actual = cm.Invoke(new List<CommandLineArgument>());

	    Assert.AreEqual(expected, actual.WasSuccess);
	    AssertCollectionsAreEqual(consoleLines, actual.Messages);
        }

        [TestMethod]
        public void InvokeWillReturnFalseWhenMethodIsNotCalledCorrectParameters()
        {
	    var cm = new CommandMethod(_parameterMethod);
	    var expected = false;

            var consoleLines = new List<string>
            {
                "An error occurred while attempting to execute the command.",
                "This is most likely due to invalid arguments.",
                // $"Please verify the command usage with '{_settings.Object.HelpString()}' and try again."
                $"Please verify the command usage with '?' and try again."
            };

	    var parameter = new CommandLineArgument();
	    parameter.Value = "kaboom";
	    
	    var actual = cm.Invoke(new List<CommandLineArgument>{ parameter });

	    Assert.AreEqual(expected, actual.WasSuccess);
	    AssertCollectionsAreEqual(consoleLines, actual.Messages);
        }
    }
}
