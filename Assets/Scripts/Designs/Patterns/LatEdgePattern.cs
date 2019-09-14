using System;

[Serializable]
public struct LatEdgePattern : IEquatable<LatEdgePattern>
{
	public bool xflip;
	public bool yflip;
	public int p0, p1;
	public int cid0, cid1;
	public int a0, b0, a1, b1;

	#region Initialization
	public LatEdgePattern(bool xflip, bool yflip, int p0, int p1, int a0, int b0, int a1, int b1)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.p0 = p0;
		this.p1 = p1;
		this.cid0 = -1;
		this.cid1 = -1;
		this.a0 = a0;
		this.b0 = b0;
		this.a1 = a1;
		this.b1 = b1;
	}
	public LatEdgePattern(bool xflip, bool yflip, int p0, int p1, int cid0, int cid1, int a0, int b0, int a1, int b1)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.p0 = p0;
		this.p1 = p1;
		this.cid0 = cid0;
		this.cid1 = cid1;
		this.a0 = a0;
		this.b0 = b0;
		this.a1 = a1;
		this.b1 = b1;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is LatEdgePattern)) {
			return false;
		}
		return this.Equals((LatEdgePattern)obj);
	}
	public bool Equals(LatEdgePattern other)
	{
		bool vertical = yflip || other.yflip;
		bool horizontal = xflip || other.xflip;

		if (a0 == other.a0 && b0 == other.b0 && a1 == other.a1 && b1 == other.b1) {
			if (p0 == other.p0 && p1 == other.p1 && cid0 == other.cid0 && cid1 == other.cid1) {
				return true;
			}
			else if (horizontal && p0 == other.p1 && p1 == other.p0 && cid0 == other.cid1 && cid1 == other.cid0) {
				return true;
			}
		}
		if (a0 == other.a1 && b0 == other.b1 && a1 == other.a0 && b1 == other.b0) {
			if (p0 == other.p1 && p1 == other.p0 && cid0 == other.cid1 && cid1 == other.cid0) {
				return true;
			}
			else if (horizontal && p0 == other.p0 && p1 == other.p1 && cid0 == other.cid0 && cid1 == other.cid1) {
				return true;
			}
		}
		if (vertical && a0 == other.b0 && b0 == other.a0 && a1 == other.b1 && b1 == other.a1) {
			if (p0 == other.p0 && p1 == other.p1 && cid0 == other.cid0 && cid1 == other.cid1) {
				return true;
			}
			else if (horizontal && p0 == other.p1 && p1 == other.p0 && cid0 == other.cid1 && cid1 == other.cid0) {
				return true;
			}
		}
		if (vertical && a0 == other.b1 && b0 == other.a1 && a1 == other.b0 && b1 == other.a0) {
			if (p0 == other.p1 && p1 == other.p0 && cid0 == other.cid1 && cid1 == other.cid0) {
				return true;
			}
			else if (horizontal && p0 == other.p0 && p1 == other.p1 && cid0 == other.cid0 && cid1 == other.cid1) {
				return true;
			}
		}
		return false;
	}


	public static bool operator ==(LatEdgePattern pat0, LatEdgePattern pat1)
	{
		return pat0.Equals(pat1);
	}
	public static bool operator !=(LatEdgePattern pat0, LatEdgePattern pat1)
	{
		return !pat0.Equals(pat1);
	}
	#endregion

	public bool IsEmpty()
	{
		return p0 == 0 && p1 == 0 && a0 == 0 && b0 == 0 && a1 == 0 && b1 == 0;
	}

	public bool IsValid()
	{
		return p0 >= 0 && p1 >= 0 && a0 >= 0 && b0 >= 0 && a1 >= 0 && b1 >= 0;
	}

	public static LatEdgePattern empty {
		get {
			return new LatEdgePattern(false, false, 0, 0, 0, 0, 0, 0);
		}
	}

	public LatEdgePattern GetPatternCopy(bool keep_cid0, bool keep_cid1)
	{
		int c0 = -1;
		if (keep_cid0) {
			c0 = cid0;
		}
		int c1 = -1;
		if (keep_cid1) {
			c1 = cid1;
		}
		return new LatEdgePattern(xflip, yflip, p0, p1, c0, c1, a0, b0, a1, b1);
	}

	public override int GetHashCode()
	{
		int a = 11 * (a0 + 1) * (a1 + 1) * (b0 + 1) * (b1 + 1);
		int b = (3 * (cid0 + 2) * (cid1 + 2));
		int c = (7 * (p0 + 1) * (p1 + 1));
		return (a + b + c) % 1000;
	}

	public static PatternMatch CalculateTransform(LatEdgePattern from, LatEdgePattern to, int d)
	{
		bool vertical = from.yflip || to.yflip;
		bool horizontal = from.xflip || to.xflip;

		if (from.a0 == to.a0 && from.b0 == to.b0 && from.a1 == to.a1 && from.b1 == to.b1) {
			if (from.p0 == to.p0 && from.p1 == to.p1 && from.cid0 == to.cid0 && from.cid1 == to.cid1) {
				return new PatternMatch(false, false, d);
			}
			else if (horizontal && from.p0 == to.p1 && from.p1 == to.p0 && from.cid0 == to.cid1 && from.cid1 == to.cid0) {
				return new PatternMatch(true, false, d);
			}
		}
		if (from.a0 == to.a1 && from.b0 == to.b1 && from.a1 == to.a0 && from.b1 == to.b0) {
			if (from.p0 == to.p1 && from.p1 == to.p0 && from.cid0 == to.cid1 && from.cid1 == to.cid0) {
				return new PatternMatch(false, false, Vx.D[3, d]);
			}
			else if (horizontal && from.p0 == to.p0 && from.p1 == to.p1 && from.cid0 == to.cid0 && from.cid1 == to.cid1) {
				return new PatternMatch(true, false, Vx.D[3, d]);
			}
		}
		if (vertical && from.a0 == to.b0 && from.b0 == to.a0 && from.a1 == to.b1 && from.b1 == to.a1) {
			if (from.p0 == to.p0 && from.p1 == to.p1 && from.cid0 == to.cid0 && from.cid1 == to.cid1) {
				return new PatternMatch(false, true, d);
			}
			else if (horizontal && from.p0 == to.p1 && from.p1 == to.p0 && from.cid0 == to.cid1 && from.cid1 == to.cid0) {
				return new PatternMatch(true, true, d);
			}
		}
		if (vertical && from.a0 == to.b1 && from.b0 == to.a1 && from.a1 == to.b0 && from.b1 == to.a0) {
			if (from.p0 == to.p1 && from.p1 == to.p0 && from.cid0 == to.cid1 && from.cid1 == to.cid0) {
				return new PatternMatch(false, true, Vx.D[3, d]);
			}
			else if (horizontal && from.p0 == to.p0 && from.p1 == to.p1 && from.cid0 == to.cid0 && from.cid1 == to.cid1) {
				return new PatternMatch(true, true, Vx.D[3, d]);
			}
		}
		return new PatternMatch(false, false, -1);
	}
}
