using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Core;
using NTrospection.CLI.Models;
using NTrospection.CLI.Repositories;

namespace NTrospection.Tests.CLI.Repositories
{
    [TestClass]
    public class ArgumentRepositoryTests
    {
        private ArgumentRepository _service;

        private Mock<ISettings> _mockSettings;

        private string _help = "?";
        private string _pre = "--";
        
        [TestInitialize]
        public void Init()
        {
            _mockSettings = new Mock<ISettings>();
            
            _service = new ArgumentRepository(_mockSettings.Object);

            _mockSettings.Setup(s => s.HelpString()).Returns(_help);
            _mockSettings.Setup(s => s.ArgumentPrefix()).Returns(_pre);
        }

        [TestMethod]
        public void TryGetArg_ReturnsNull_WhenIndexOutOfBounds()
        {
            var expected = (string)null;
            var args = new List<string>();

            var actual = _service.TryGetArg(args, 0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TryGetArg_ReturnsNull_ArgMatchesHelpString()
        {
            var expected = (string)null;
            var args = new List<string>{ _help };

            var actual = _service.TryGetArg(args, 0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TryGetArg_ReturnsArg_WhenArgIsFoundAndNotHelp()
        {
            var expected = "foo";
            var args = new List<string>{ expected };

            var actual = _service.TryGetArg(args, 0);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetArguments_ReturnsListWithOnlyNull_WhenPassedEmptyArgs()
        {
            var expected = new List<CommandLineArgument>{ null };
            var args = new List<string>();

            var actual = _service.SetArguments(args).ToList();
            
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetArguments_ReturnsArgumentWithValues_WhenPassed()
        {
            var expectedCommand = "foo";
            var expectedArg = new CommandLineArgument
            {
                Command = expectedCommand,
                Order = 1,
                Values = new List<string>
                {
                    "1",
                    "2",
                    "3"
                }
            };
            var expected = new List<CommandLineArgument>{ expectedArg };
            var args = new List<string>
            {
                $"{_pre}{expectedCommand}",
                "1",
                "2",
                "3"
            };

            var actual = _service.SetArguments(args).ToList();
            
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetArguments_ReturnsArgumentsWithValues_WhenPassedMultipleArgs()
        {
            var command1 = "foo";
            var values1 = new List<string>{ "1" };
            
            var command2 = "bar";
            var values2 = new List<string>{ "1", "2", "3" };
            
            var expectedArg1 = new CommandLineArgument
            {
                Command = command1,
                Order = 1,
                Values = new List<string>
                {
                    "1"
                }
            };
            var expectedArg2 = new CommandLineArgument
            {
                Command = command2,
                Order = 2,
                Values = new List<string>
                {
                    "1",
                    "2",
                    "3"
                }
            };
            
            var expected = new List<CommandLineArgument>{ expectedArg1, expectedArg2 };
            var args = new List<string>{ $"{_pre}{command1}" };
            args.AddRange(values1);
            args.Add($"{_pre}{command2}");
            args.AddRange(values2);

            var actual = _service.SetArguments(args).ToList();
            
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
