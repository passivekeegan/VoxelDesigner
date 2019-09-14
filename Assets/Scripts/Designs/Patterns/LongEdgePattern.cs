using System;

[Serializable]
public struct LongEdgePattern : IEquatable<LongEdgePattern>
{
	public bool xflip;
	public bool yflip;
	public int p0, p1;
	public int cid0, cid1;
	public int in0, out0, out1;

	#region Initialization
	public LongEdgePattern(bool xflip, bool yflip, int p0, int p1, int in0, int out0, int out1)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.p0 = p0;
		this.p1 = p1;
		this.cid0 = -1;
		this.cid1 = -1;
		this.in0 = in0;
		this.out0 = out0;
		this.out1 = out1;
	}
	public LongEdgePattern(bool xflip, bool yflip, int p0, int p1, int cid0, int cid1, int in0, int out0, int out1)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.p0 = p0;
		this.p1 = p1;
		this.cid0 = cid0;
		this.cid1 = cid1;
		this.in0 = in0;
		this.out0 = out0;
		this.out1 = out1;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is LongEdgePattern)) {
			return false;
		}
		return this.Equals((LongEdgePattern)obj);
	}
	public bool Equals(LongEdgePattern other)
	{
		bool vertical = yflip || other.yflip;
		bool horizontal = xflip || other.xflip;
		//normal
		bool equality = p0 == other.p0 && p1 == other.p1 && cid0 == other.cid0 && cid1 == other.cid1 && (
			(in0 == other.in0 && out0 == other.out0 && out1 == other.out1) ||
			(in0 == other.out0 && out0 == other.out1 && out1 == other.in0) ||
			(in0 == other.out1 && out0 == other.in0 && out1 == other.out0)
		);
		//horizontal
		equality = equality || (horizontal &&
			p0 == other.p0 && p1 == other.p1 && cid0 == other.cid0 && cid1 == other.cid1 && (
			(in0 == other.in0 && out0 == other.out1 && out1 == other.out0) ||
			(in0 == other.out1 && out0 == other.out0 && out1 == other.in0) ||
			(in0 == other.out0 && out0 == other.in0 && out1 == other.out1)
		));
		//vertical
		equality = equality || (vertical &&
			p0 == other.p1 && p1 == other.p0 && cid0 == other.cid1 && cid1 == other.cid0 && (
			(in0 == other.in0 && out0 == other.out0 && out1 == other.out1) ||
			(in0 == other.out0 && out0 == other.out1 && out1 == other.in0) ||
			(in0 == other.out1 && out0 == other.in0 && out1 == other.out0)
		));
		//vertical and horizontal
		return equality || (vertical && horizontal &&
			p0 == other.p1 && p1 == other.p0 && cid0 == other.cid1 && cid1 == other.cid0 && (
			(in0 == other.in0 && out0 == other.out1 && out1 == other.out0) ||
			(in0 == other.out1 && out0 == other.out0 && out1 == other.in0) ||
			(in0 == other.out0 && out0 == other.in0 && out1 == other.out1)
		));
	}


	public static bool operator ==(LongEdgePattern pat0, LongEdgePattern pat1)
	{
		return pat0.Equals(pat1);
	}
	public static bool operator !=(LongEdgePattern pat0, LongEdgePattern pat1)
	{
		return !pat0.Equals(pat1);
	}
	#endregion

	public bool IsEmpty()
	{
		return p0 == 0 && p1 == 0 && in0 == 0 && out0 == 0 && out1 == 0;
	}
	public bool IsValid()
	{
		return p0 >= 0 && p1 >= 0 && in0 >= 0 && out0 >= 0 && out1 >= 0;
	}
	public static LongEdgePattern empty {
		get {
			return new LongEdgePattern(false, false, 0, 0, 0, 0, 0);
		}
	}

	public LongEdgePattern GetPatternCopy(bool keep_cid0, bool keep_cid1)
	{
		int c0 = -1;
		if (keep_cid0) {
			c0 = cid0;
		}
		int c1 = -1;
		if (keep_cid1) {
			c1 = cid1;
		}
		return new LongEdgePattern(xflip, yflip, p0, p1, c0, c1, in0, out0, out1);
	}

	public override int GetHashCode()
	{
		int a = 11 * (in0 + 1) * (out0 + 1) * (out1 + 1);
		int b = (3 * (cid0 + 2) * (cid1 + 2));
		int c = (7 * (p0 + 1) * (p1 + 1));
		return (a % 10000) + (b % 1000) + (c % 100);
	}

	public static PatternMatch CalculateTransform(LongEdgePattern from, LongEdgePattern to, bool topheavy)
	{
		bool vertical = from.yflip || to.yflip;
		bool horizontal = from.xflip || to.xflip;
		int heavyshift = 0;
		if (topheavy) {
			heavyshift = 1;
		}
		if (from.p0 == to.p0 && from.p1 == to.p1 && from.cid0 == to.cid0 && from.cid1 == to.cid1) {
			if (from.in0 == to.in0 && from.out0 == to.out0 && from.out1 == to.out1) {
				return new PatternMatch(false, false, Vx.ND[heavyshift, 0]);
			}
			else if (from.in0 == to.out0 && from.out0 == to.out1 && from.out1 == to.in0) {
				return new PatternMatch(false, false, Vx.ND[heavyshift, 2]);
			}
			else if (from.in0 == to.out1 && from.out0 == to.in0 && from.out1 == to.out0) {
				return new PatternMatch(false, false, Vx.ND[heavyshift, 4]);
			}
			if (horizontal) {
				if (from.in0 == to.in0 && from.out0 == to.out1 && from.out1 == to.out0) {
					return new PatternMatch(true, false, Vx.ND[heavyshift, 4]);
				}
				else if (from.in0 == to.out1 && from.out0 == to.out0 && from.out1 == to.in0) {
					return new PatternMatch(true, false, Vx.ND[heavyshift, 0]);
				}
				else if (from.in0 == to.out0 && from.out0 == to.in0 && from.out1 == to.out1) {
					return new PatternMatch(true, false, Vx.ND[heavyshift, 2]);
				}
			}
		} 
		if (vertical && from.p0 == to.p1 && from.p1 == to.p0 && from.cid0 == to.cid1 && from.cid1 == to.cid0) {
			if (from.in0 == to.in0 && from.out0 == to.out0 && from.out1 == to.out1) {
				return new PatternMatch(false, true, Vx.ND[heavyshift, 0]);
			}
			else if (from.in0 == to.out0 && from.out0 == to.out1 && from.out1 == to.in0) {
				return new PatternMatch(false, true, Vx.ND[heavyshift, 2]);
			}
			else if (from.in0 == to.out1 && from.out0 == to.in0 && from.out1 == to.out0) {
				return new PatternMatch(false, true, Vx.ND[heavyshift, 4]);
			}
			if (horizontal) {
				if (from.in0 == to.in0 && from.out0 == to.out1 && from.out1 == to.out0) {
					return new PatternMatch(true, true, Vx.ND[heavyshift, 4]);
				}
				else if (from.in0 == to.out1 && from.out0 == to.out0 && from.out1 == to.in0) {
					return new PatternMatch(true, true, Vx.ND[heavyshift, 0]);
				}
				else if (from.in0 == to.out0 && from.out0 == to.in0 && from.out1 == to.out1) {
					return new PatternMatch(true, true, Vx.ND[heavyshift, 2]);
				}
			}
		}
		return new PatternMatch(false, false, -1);
	}
}
