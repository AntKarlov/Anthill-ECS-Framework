using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using Anthill.Utils;

public class AntMathTest
{
	// [SetUp]
	// public void SomeSetupBeforeTesting()
	// {
		
	// }

	[Test]
	public void InRange()
	{
		Assert.IsTrue(AntMath.InRange(1.0f, 0.0f, 10.0f), "1.0f not in the range (0-10)!");
		Assert.IsFalse(AntMath.InRange(11.0f, 0.0f, 10.0f), "11.0f in the range (0-10)!");
	}

	[Test]
	public void Closest()
	{
		Assert.IsTrue(AntMath.Closest(1.0f, 0.0f, 10.0f) == 0.0f, "Incorrect closest value!");
		Assert.IsTrue(AntMath.Closest(8.0f, 0.0f, 10.0f) == 10.0f, "Incorrect closest value!");
	}

	[Test]
	public void Limit()
	{
		float value = AntMath.Limit(10.0f, 9.0f);
		A.Log($"Limit {value}");
		Assert.IsTrue(value == 9.0f, "Incorrect limit value!");
		
		value = AntMath.Limit(-10.0f, 9.0f);
		A.Log($"Limit {value}");
		Assert.IsTrue(value == -9.0f, "Incorrect limit value!");

		// Assert.IsNotNull()
		// Assert.Fail();
	}
}
