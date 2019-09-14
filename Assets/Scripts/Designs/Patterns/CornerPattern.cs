using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CornerPattern : IEquatable<CornerPattern>
{
	public bool xflip;
	public bool yflip;
	public int a0, a1, a2;
	public int b0, b1, b2;

	#region Initialization
	public CornerPattern(int a0, int a1, int a2, int b0, int b1, int b2)
	{
		this.xflip = false;
		this.yflip = false;
		this.a0 = a0;
		this.a1 = a1;
		this.a2 = a2;
		this.b0 = b0;
		this.b1 = b1;
		this.b2 = b2;
	}
	public CornerPattern(bool xflip, bool yflip, int a0, int a1, int a2, int b0, int b1, int b2)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.a0 = a0;
		this.a1 = a1;
		this.a2 = a2;
		this.b0 = b0;
		this.b1 = b1;
		this.b2 = b2;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is CornerPattern)) {
			return false;
		}
		return this.Equals((CornerPattern)obj);
	}
	public bool Equals(CornerPattern other)
	{
		bool vertical = yflip || other.yflip;
		bool horizontal = xflip || other.xflip;
		//normal
		bool equality = (
			(a0 == other.a0 && b0 == other.b0 && a1 == other.a1 && b1 == other.b1 && a2 == other.a2 && b2 == other.b2) ||
			(a0 == other.a1 && b0 == other.b1 && a1 == other.a2 && b1 == other.b2 && a2 == other.a0 && b2 == other.b0) ||
			(a0 == other.a2 && b0 == other.b2 && a1 == other.a0 && b1 == other.b0 && a2 == other.a1 && b2 == other.b1)
		);
		//horizontal
		equality = equality || (horizontal && (
			(a0 == other.a0 && b0 == other.b0 && a1 == other.a2 && b1 == other.b2 && a2 == other.a1 && b2 == other.b1) ||
			(a0 == other.a1 && b0 == other.b1 && a1 == other.a0 && b1 == other.b0 && a2 == other.a2 && b2 == other.b2) ||
			(a0 == other.a2 && b0 == other.b2 && a1 == other.a1 && b1 == other.b1 && a2 == other.a0 && b2 == other.b0)
		));
		//vertical
		equality = equality || (vertical && (
			(a0 == other.b0 && b0 == other.a0 && a1 == other.b1 && b1 == other.a1 && a2 == other.b2 && b2 == other.a2) ||
			(a0 == other.b1 && b0 == other.a1 && a1 == other.b2 && b1 == other.a2 && a2 == other.b0 && b2 == other.a0) ||
			(a0 == other.b2 && b0 == other.a2 && a1 == other.b0 && b1 == other.a0 && a2 == other.b1 && b2 == other.a1)
		));
		//vertical and horizontal
		return equality || (vertical && horizontal && (
			(a0 == other.b0 && b0 == other.a0 && a1 == other.b2 && b1 == other.a2 && a2 == other.b1 && b2 == other.a1) ||
			(a0 == other.b1 && b0 == other.a1 && a1 == other.b0 && b1 == other.a0 && a2 == other.b2 && b2 == other.a2) ||
			(a0 == other.b2 && b0 == other.a2 && a1 == other.b1 && b1 == other.a1 && a2 == other.b0 && b2 == other.a0)
		));
	}
	public static bool operator ==(CornerPattern cp0, CornerPattern cp1)
	{
		return cp0.Equals(cp1);
	}
	public static bool operator !=(CornerPattern cp0, CornerPattern cp1)
	{
		return !cp0.Equals(cp1);
	}
	#endregion

	public bool IsEmpty()
	{
		return a0 == 0 && a1 == 0 && a2 == 0 && b0 == 0 && b1 == 0 && b2 == 0;
	}
	public bool IsValid()
	{
		return a0 >= 0 && a1 >= 0 && a2 >= 0 && b0 >= 0 && b1 >= 0 && b2 >= 0;
	}

	public static CornerPattern empty {
		get {
			return new CornerPattern(0, 0, 0, 0, 0, 0);
		}
	}

	public override int GetHashCode()
	{
		int a = ((a0 + 1) % 100) + ((b0 + 1) % 100);
		int b = ((a1 + 1) % 100) + ((b1 + 1) % 100);
		int c = ((a2 + 1) % 100) + ((b2 + 1) % 100);
		return (a + b + c + 3) % 100;
	}

	public static PatternMatch CalculateTransform(CornerPattern from, CornerPattern to, bool topheavy)
	{
		bool vertical = from.yflip || to.yflip;
		bool horizontal = from.xflip || to.xflip;
		//normal
		int heavyshift = 0;
		if (topheavy) {
			heavyshift = 1;
		}
		if (from.a0 == to.a0 && from.b0 == to.b0 && from.a1 == to.a1 && from.b1 == to.b1 && from.a2 == to.a2 && from.b2 == to.b2) {
			return new PatternMatch(false, false, Vx.ND[heavyshift, 0]);
		}
		else if (from.a0 == to.a1 && from.b0 == to.b1 && from.a1 == to.a2 && from.b1 == to.b2 && from.a2 == to.a0 && from.b2 == to.b0) {
			return new PatternMatch(false, false, Vx.ND[heavyshift, 2]);
		}
		else if (from.a0 == to.a2 && from.b0 == to.b2 && from.a1 == to.a0 && from.b1 == to.b0 && from.a2 == to.a1 && from.b2 == to.b1) {
			return new PatternMatch(false, false, Vx.ND[heavyshift, 4]);
		}
		if (horizontal) {
			if (from.a0 == to.a0 && from.b0 == to.b0 && from.a1 == to.a2 && from.b1 == to.b2 && from.a2 == to.a1 && from.b2 == to.b1) {
				return new PatternMatch(true, false, Vx.ND[heavyshift, 4]);
			}
			else if (from.a0 == to.a1 && from.b0 == to.b1 && from.a1 == to.a0 && from.b1 == to.b0 && from.a2 == to.a2 && from.b2 == to.b2) {
				return new PatternMatch(true, false, Vx.ND[heavyshift, 0]);
			}
			else if (from.a0 == to.a2 && from.b0 == to.b2 && from.a1 == to.a1 && from.b1 == to.b1 && from.a2 == to.a0 && from.b2 == to.b0) {
				return new PatternMatch(true, false, Vx.ND[heavyshift, 2]);
			}
		}
		if (vertical) {
			if (from.a0 == to.b0 && from.b0 == to.a0 && from.a1 == to.b1 && from.b1 == to.a1 && from.a2 == to.b2 && from.b2 == to.a2) {
				return new PatternMatch(false, true, Vx.ND[heavyshift, 0]);
			}
			else if (from.a0 == to.b1 && from.b0 == to.a1 && from.a1 == to.b2 && from.b1 == to.a2 && from.a2 == to.b0 && from.b2 == to.a0) {
				return new PatternMatch(false, true, Vx.ND[heavyshift, 2]);
			}
			else if (from.a0 == to.b2 && from.b0 == to.a2 && from.a1 == to.b0 && from.b1 == to.a0 && from.a2 == to.b1 && from.b2 == to.a1) {
				return new PatternMatch(false, true, Vx.ND[heavyshift, 4]);
			}
		}
		if (vertical && horizontal) {
			if (from.a0 == to.b0 && from.b0 == to.a0 && from.a1 == to.b2 && from.b1 == to.a2 && from.a2 == to.b1 && from.b2 == to.a1) {
				return new PatternMatch(true, true, Vx.ND[heavyshift, 4]);
			}
			else if (from.a0 == to.b1 && from.b0 == to.a1 && from.a1 == to.b0 && from.b1 == to.a0 && from.a2 == to.b2 && from.b2 == to.a2) {
				return new PatternMatch(true, true, Vx.ND[heavyshift, 0]);
			}
			else if (from.a0 == to.b2 && from.b0 == to.a2 && from.a1 == to.b1 && from.b1 == to.a1 && from.a2 == to.b0 && from.b2 == to.a0) {
				return new PatternMatch(true, true, Vx.ND[heavyshift, 2]);
			}
		}
		return new PatternMatch(false, false, -1);
	}
}

