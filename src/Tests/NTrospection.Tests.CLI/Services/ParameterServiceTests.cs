using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Attributes;
using NTrospection.CLI.Core;
using NTrospection.CLI.Services;
using NTrospection.Tests.CLI.App.Models;

namespace NTrospection.Tests.CLI.Models
{
    [TestClass]
    public class ParameterServiceTests
    {
	const char _expectedAlias = 'b';
	
	[TestInitialize]
	public void Init()
	{
	}

	// === Fake Method For Reflection ===
	public void FakeMethod(
			       [CliParameter("sets foo")]int foo,
			       [CliParameter(_expectedAlias, "sets bar")]int bar,
			       string foobar)
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
	    var expected = $" | {Settings.ArgumentPrefix}{_expectedAlias}";

	    Assert.AreEqual(expected, actual);
	}
    }
}
