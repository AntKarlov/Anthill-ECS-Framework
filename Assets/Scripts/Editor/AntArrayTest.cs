using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Anthill.Utils;
using System;

public class AntArrayTest
{
	private string[] _testArray = { "value1", "value2", "value3" };

	// [SetUp]
	// public void SomeSetupBeforeTesting()
	// {
		
	// }

	[Test]
	public void ArrayAdd()
	{
		AntArray.Add(ref _testArray, "value4");

		Assert.IsTrue(_testArray.Length == 4, "Incorrect array size!");
		Assert.IsTrue(_testArray[_testArray.Length - 1].Equals("value4"), "Incorrect array value!");

		// Assert.IsNotNull()
		// Assert.Fail();
	}

	[Test]
	public void ArrayContains()
	{
		Assert.IsTrue(AntArray.Contains(ref _testArray, "value4"), "Array not contains value!");
	}

	[Test]
	public void ArrayRemove()
	{
		AntArray.RemoveAt(ref _testArray, 2);
		Assert.IsTrue(_testArray.Length == 3, "Array have length high than expected!");
	}

	[Test]
	public void ArrayGetRandom()
	{
		A.Log(AntArray.GetRandom(ref _testArray));
	}

	[Test]
	public void GetFirstAndLast()
	{
		Assert.IsTrue(AntArray.First(ref _testArray).Equals("value1"), "First value is not equals to `value1`!");
		Assert.IsTrue(AntArray.Last(ref _testArray).Equals("value4"), "First value is not equals to `value4`!");
	}
}
