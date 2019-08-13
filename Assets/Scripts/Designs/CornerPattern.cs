using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[System.Serializable]
public struct CornerPattern : IEquatable<CornerPattern>
{
	public int a0, a1, a2;
	public int b0, b1, b2;

	#region Initialization
	/// <summary>
	/// 
	/// </summary>
	/// <param name="a0"></param>
	/// <param name="a1"></param>
	/// <param name="a2"></param>
	/// <param name="b0"></param>
	/// <param name="b1"></param>
	/// <param name="b2"></param>
	public CornerPattern(int a0, int a1, int a2, int b0, int b1, int b2)
	{
		this.a0 = a0;
		this.a1 = a1;
		this.a2 = a2;
		this.b0 = b0;
		this.b1 = b1;
		this.b2 = b2;
	}
	#endregion

	#region Equality
	/// <summary>
	/// 
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public override bool Equals(object obj)
	{
		if (!(obj is CornerPattern)) {
			return false;
		}
		return this.Equals((CornerPattern)obj);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool Equals(CornerPattern other)
	{
		bool eq = (a0 == other.a0) &&
				  (a1 == other.a1) &&
				  (a2 == other.a2) &&
				  (b0 == other.b0) &&
				  (b1 == other.b1) &&
				  (b2 == other.b2);
		eq = eq ||
			 ((a0 == other.a1) &&
			  (a1 == other.a2) &&
			  (a2 == other.a0) &&
			  (b0 == other.b1) &&
			  (b1 == other.b2) &&
			  (b2 == other.b0));
		eq = eq ||
			 ((a0 == other.a2) &&
			  (a1 == other.a0) &&
			  (a2 == other.a1) &&
			  (b0 == other.b2) &&
			  (b1 == other.b0) &&
			  (b2 == other.b1));
		return eq;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="cp0"></param>
	/// <param name="cp1"></param>
	/// <returns></returns>
	public static bool operator == (CornerPattern cp0, CornerPattern cp1)
	{
		return cp0.Equals(cp1);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="cp0"></param>
	/// <param name="cp1"></param>
	/// <returns></returns>
	public static bool operator != (CornerPattern cp0, CornerPattern cp1)
	{
		return !cp0.Equals(cp1);
	}
	#endregion

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool IsEmpty()
	{
		return a0 == 0 && a1 == 0 && a2 == 0 && b0 == 0 && b1 == 0 && b2 == 0;
	}

	/// <summary>
	/// 
	/// </summary>
	public static CornerPattern empty {
		get {
			return new CornerPattern(0, 0, 0, 0, 0, 0);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public override int GetHashCode()
	{
		int a = ((100 * (a0 % 100)) + (b0 % 100));
		int b = ((100 * (a1 % 100)) + (b1 % 100));
		int c = ((100 * (a2 % 100)) + (b2 % 100));
		return (17 * (a + b + c)) + (a * b) + (a * c) + (b * c) + (a * b * c);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="from"></param>
	/// <param name="to"></param>
	/// <returns></returns>
	public static int CalculateShift(CornerPattern from, CornerPattern to)
	{
		if ((from.a0 == to.a0) &&
			(from.a1 == to.a1) &&
			(from.a2 == to.a2) &&
			(from.b0 == to.b0) &&
			(from.b1 == to.b1) &&
			(from.b2 == to.b2)) 
		{
			return 0;
		}
		else if ((from.a0 == to.a1) &&
			  (from.a1 == to.a2) &&
			  (from.a2 == to.a0) &&
			  (from.b0 == to.b1) &&
			  (from.b1 == to.b2) &&
			  (from.b2 == to.b0)) 
		{
			return 2;
		}
		else if ((from.a0 == to.a2) &&
			  (from.a1 == to.a0) &&
			  (from.a2 == to.a1) &&
			  (from.b0 == to.b2) &&
			  (from.b1 == to.b0) &&
			  (from.b2 == to.b1)) {
			return 4;
		}
		return -1;
	}
}
