using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NTrospection.CLI.Services;
using NTrospection.Tests.CLI.App.Models;

namespace NTrospection.Tests.CLI.Models
{
    [TestClass]
    public class ParameterServiceTests
    {
	[TestInitialize]
	public void Init()
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
    }
}
