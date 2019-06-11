using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Core;
using NTrospection.CLI.Services;
using NTrospection.Tests.CLI.App.Models;
using NTrospection.Tests.CLI.Common;

namespace NTrospection.Tests.CLI.Models
{
    [TestClass]
    public class ParameterServiceTests: BaseCliTest
    {
	const char _expectedAlias = 'b';
	
	[TestInitialize]
	public void Init()
	{
	}

	// === Fake Method For Reflection ===
	public void FakeMethod(
			       [CliParameter("sets foo")]int foo,
			       [CliParameter(_expectedAlias, "sets bar")]int? bar,
			       string foobar = "null",
			       List<int> barfoo = null,
			       SampleEnum enumb = SampleEnum.EnumOne,
			       List<SampleEnum> enumbs = null)
	{
	    
	}

	[TestMethod]
	public void GetParamValue_ReturnsEnum_WhenPassedValidEnum()
	{
	    var service = new ParameterService();
	    var val = service.GetParamValue("EnumOne", typeof(SampleEnum));

	    Assert.AreEqual(SampleEnum.EnumOne, val);
	}

	[DataTestMethod]
	[DataRow("5", 5, typeof(int?))]
	[DataRow("true", true, typeof(bool?))]
	[DataRow("EnumOne", SampleEnum.EnumOne, typeof(SampleEnum?))]
	public void GetParamValue_ReturnsUnderlyingValue_WhenPassedValidNullable(string value, dynamic expected, Type type)
	{
	    var service = new ParameterService();
	    var val = service.GetParamValue(value, type);

	    Assert.AreEqual(expected, val);
	}

	[DataTestMethod]
	[DataRow(typeof(string[]))]
	[DataRow(typeof(List<string>))]
	[DataRow(typeof(IEnumerable<string>))]
	public void IsEnumerable_ReturnsTrue_WhenPassedEnumerableType(Type type)
	{
	    var service = new ParameterService();
	    var actual = service.IsEnumerable(type);

	    Assert.AreEqual(true, actual);
	}

	[DataTestMethod]
	[DataRow(typeof(string))]
	[DataRow(typeof(int?))]
	[DataRow(typeof(SampleEnum))]
	public void IsEnumerable_ReturnsFalse_WhenPassedNonEnumerableType(Type type)
	{
	    var service = new ParameterService();
	    var actual = service.IsEnumerable(type);

	    Assert.AreEqual(false, actual);
	}

	[TestMethod]
	public void GetAliasString_ReturnsEmptyString_WhenPassedNoAlias()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[0];
	    
	    var actual = service.GetAliasString(pi);

	    Assert.AreEqual("", actual);
	}

	[TestMethod]
	public void GetAliasString_ReturnsEmptyString_WhenPassedParameterWithoutAttribute()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[2];
	    
	    var actual = service.GetAliasString(pi);

	    Assert.AreEqual("", actual);
	}

	[TestMethod]
	public void GetAliasString_ReturnsString_WhenPassedAlias()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[1];
	    
	    var actual = service.GetAliasString(pi);
	    // var expected = $" | {Settings.ArgumentPrefix}{_expectedAlias}";
	    var expected = $" | --{_expectedAlias}";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetPriorityString_ReturnsOptional_WhenParameterHasDefaultValue()
	{	    
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[2];
	    
	    var actual = service.GetPriorityString(pi);
	    var expected = "Optional";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetPriorityString_ReturnsRequired_WhenParameterHasNoDefault()
	{	    
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[1];
	    
	    var actual = service.GetPriorityString(pi);
	    var expected = "Required";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetPriorityString_ReturnsOptionalAndDefault_WhenParameterHasDefaultAndVerboseFlagSet()
	{
	    var mockSettings = new Mock<ISettings>();
	    mockSettings.Setup(s => s.ParamDetail()).Returns("detailed");
	    
	    var service = new ParameterService(mockSettings.Object);
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[2];
	    
	    var actual = service.GetPriorityString(pi);
	    var expected = "Optional with a default value of null";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetPriorityString_ReturnsOptionalAndDefault_WhenParameterHasDefaultAndVerboseFlagSetAndDefaultIsNull()
	{
	    var mockSettings = new Mock<ISettings>();
	    mockSettings.Setup(s => s.ParamDetail()).Returns("detailed");
	    
	    var service = new ParameterService(mockSettings.Object);
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[3];
	    
	    var actual = service.GetPriorityString(pi);
	    var expected = "Optional with a default value of null";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetTypeString_ReturnsNameOfType_WhenParameterIsNotCollection()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[0];
	    
	    var actual = service.GetTypeString(pi);
	    var expected = "Int32";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetTypeString_ReturnsNameOfUnderType_WhenParameterIsNotCollectionAndNullable()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[1];
	    
	    var actual = service.GetTypeString(pi);
	    var expected = "Int32";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetTypeString_ReturnsListOf_WhenParameterIsCollection()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[3];
	    
	    var actual = service.GetTypeString(pi);
	    var expected = "List of Int32";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetDescriptionString_ReturnsEmptyString_WhenPassedParameterWithoutAttribute()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[2];
	    
	    var actual = service.GetDescriptionString(pi);

	    Assert.AreEqual("", actual);
	}

	[TestMethod]
	public void GetDescriptionString_ReturnsString_WhenPassedDescription()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[0];
	    
	    var actual = service.GetDescriptionString(pi);
	    var expected = $"Description: sets foo";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetDocString_ReturnsString_WhenPassedNonEnum()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[0];

	    var actual = service.GetDocString(pi, "Required", "type", " | --f");
	    var expected = "--foo | --f (type): This parameter is Required";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetDocString_ReturnsStringWithOptions_WhenPassedEnum()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[4];

	    var actual = service.GetDocString(pi, "Required", "type", " | --e");
	    var expected = "--enumb | --e (type): This parameter is Required and must be one of the following (EnumOne, EnumTwo, EnumThree)";

	    Assert.AreEqual(expected, actual);
	}

	[TestMethod]
	public void GetDocString_ReturnsStringWithCollectionOptions_WhenPassedEnumCollection()
	{
	    var service = new ParameterService();
	    var pi = typeof(ParameterServiceTests)
		.GetMethod("FakeMethod")
		.GetParameters()[5];

	    var actual = service.GetDocString(pi, "Required", "type", " | --e");
	    var expected = "--enumbs | --e (type): This parameter is Required and must be a collection of one of the following (EnumOne, EnumTwo, EnumThree)";

	    Assert.AreEqual(expected, actual);
	}
    }
}
