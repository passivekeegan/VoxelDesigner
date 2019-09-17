using System;

[Serializable]
public struct RectPattern : IEquatable<RectPattern>
{
	public bool xflip;
	public bool yflip;
	public int a0_p, b0_p, a1_p, b1_p;
	public int a0_cid, b0_cid, a1_cid, b1_cid;
	public int vin, vout;

	#region Initialization
	public RectPattern(bool xflip, bool yflip, int a0_p, int b0_p, int a1_p, int b1_p, int vin, int vout)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.a0_p = a0_p;
		this.b0_p = b0_p;
		this.a1_p = a1_p;
		this.b1_p = b1_p;
		this.a0_cid = -1;
		this.b0_cid = -1;
		this.a1_cid = -1;
		this.b1_cid = -1;
		this.vin = vin;
		this.vout = vout;
	}
	public RectPattern(bool xflip, bool yflip, int a0_p, int b0_p, int a1_p, int b1_p, int a0_cid, int b0_cid, int a1_cid, int b1_cid, int vin, int vout)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.a0_p = a0_p;
		this.b0_p = b0_p;
		this.a1_p = a1_p;
		this.b1_p = b1_p;
		this.a0_cid = a0_cid;
		this.b0_cid = b0_cid;
		this.a1_cid = a1_cid;
		this.b1_cid = b1_cid;
		this.vin = vin;
		this.vout = vout;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is RectPattern)) {
			return false;
		}
		return this.Equals((RectPattern)obj);
	}
	public bool Equals(RectPattern other)
	{
		bool vertical = yflip || other.yflip;
		bool horizontal = xflip || other.xflip;

		//if (a0_p == other.a0_p && b0_p == other.b0_p &&
		//	a1_p == other.a1_p && b1_p == other.b1_p &&
		//	a0_cid == other.a0_cid && b0_cid == other.b0_cid &&
		//	a1_cid == other.a1_cid && b1_cid == other.b1_cid
		//) {
		//	if (vin == other.vin && vout == other.vout) {
		//		return true;
		//	}
		//	else if (horizontal && vin == other.vout && vout == other.vin) {
		//		return true;
		//	}
		//}
		//if (a0_p == other.a1_p && b0_p == other.b1_p &&
		//	a1_p == other.a0_p && b1_p == other.b0_p &&
		//	a0_cid == other.a1_cid && b0_cid == other.b1_cid &&
		//	a1_cid == other.a0_cid && b1_cid == other.b0_cid
		//) {
		//	if (vin == other.vout && vout == other.vin) {
		//		return true;
		//	}
		//	else if (horizontal && vin == other.vin && vout == other.vout) {
		//		return true;
		//	}
		//}
		//if (vertical &&
		//	a0_p == other.b0_p && b0_p == other.a0_p &&
		//	a1_p == other.b1_p && b1_p == other.a1_p &&
		//	a0_cid == other.b0_cid && b0_cid == other.a0_cid &&
		//	a1_cid == other.b1_cid && b1_cid == other.a1_cid
		//) {
		//	if (vin == other.vin && vout == other.vout) {
		//		return true;
		//	}
		//	else if (horizontal && vin == other.vout && vout == other.vin) {
		//		return true;
		//	}
		//}
		//if (vertical &&
		//	a0_p == other.b1_p && b0_p == other.a1_p &&
		//	a1_p == other.b0_p && b1_p == other.a0_p &&
		//	a0_cid == other.b1_cid && b0_cid == other.a1_cid &&
		//	a1_cid == other.b0_cid && b1_cid == other.a0_cid
		//) {
		//	if (vin == other.vout && vout == other.vin) {
		//		return true;
		//	}
		//	else if (horizontal && vin == other.vin && vout == other.vout) {
		//		return true;
		//	}
		//}
		return CalculateTransform(this, other, 0).shift >= 0;
	}


	public static bool operator ==(RectPattern pat0, RectPattern pat1)
	{
		return pat0.Equals(pat1);
	}
	public static bool operator !=(RectPattern pat0, RectPattern pat1)
	{
		return !pat0.Equals(pat1);
	}
	#endregion

	public bool IsEmpty()
	{
		return a0_p == 0 && b0_p == 0 && a1_p == 0 && b1_p == 0 && vin == 0 && vout == 0;
	}
	public bool IsValid()
	{
		return a0_p >= 0 && b0_p >= 0 && a1_p >= 0 && b1_p >= 0 && vin >= 0 && vout >= 0;
	}
	public static RectPattern empty {
		get {
			return new RectPattern(false, false, 0, 0, 0, 0, 0, 0);
		}
	}

	public override int GetHashCode()
	{
		int code = 9 * (((a0_p + 11) * (b0_p + 11)) + ((a1_p + 11) * (b1_p + 11)));
		code += 5 * (((a0_cid + 3) * (b0_cid + 3)) + ((a1_cid + 3) * (b1_cid + 3)));
		code += 17 * (vin + 17) * (vout + 17);
		return code % 1000;
	}

	public static PatternMatch CalculateTransform(RectPattern from, RectPattern to, int d)
	{
		bool vertical = from.yflip || to.yflip;
		bool horizontal = from.xflip || to.xflip;

		//normal
		if (from.vin == to.vin && from.vout == to.vout) {
			if (from.a0_p == to.a0_p && from.b0_p == to.b0_p &&
				from.a1_p == to.a1_p && from.b1_p == to.b1_p) {
				return new PatternMatch(false, false, d);
			}
			else if (horizontal &&
				from.a0_p == to.a1_p && from.b0_p == to.b1_p &&
				from.a1_p == to.a0_p && from.b1_p == to.b0_p) {
				return new PatternMatch(true, false, d);
			}
			else if (vertical &&
				from.a0_p == to.b0_p && from.b0_p == to.a0_p &&
				from.a1_p == to.b1_p && from.b1_p == to.a1_p) {
				return new PatternMatch(false, true, d);
			}
			else if (horizontal && vertical &&
				from.a0_p == to.b1_p && from.b0_p == to.a1_p &&
				from.a1_p == to.b0_p && from.b1_p == to.a0_p) {
				return new PatternMatch(true, true, d);
			}
		}
		//flipped
		if (from.vin == to.vout && from.vout == to.vin) {
			if (from.a0_p == to.a1_p && from.b0_p == to.b1_p &&
				from.a1_p == to.a0_p && from.b1_p == to.b0_p) {
				return new PatternMatch(false, false, Vx.D[3,d]);
			}
			else if (horizontal &&
				from.a0_p == to.a0_p && from.b0_p == to.b0_p &&
				from.a1_p == to.a1_p && from.b1_p == to.b1_p) {
				return new PatternMatch(true, false, Vx.D[3, d]);
			}
			else if (vertical &&
				from.a0_p == to.b1_p && from.b0_p == to.a1_p &&
				from.a1_p == to.b0_p && from.b1_p == to.a0_p) {
				return new PatternMatch(false, true, Vx.D[3, d]);
			}
			else if (horizontal && vertical &&
				from.a0_p == to.b0_p && from.b0_p == to.a0_p &&
				from.a1_p == to.b1_p && from.b1_p == to.a1_p) {
				return new PatternMatch(true, true, Vx.D[3, d]);
			}
		}


		//if (from.a0_p == to.a0_p && from.b0_p == to.b0_p &&
		//	from.a1_p == to.a1_p && from.b1_p == to.b1_p &&
		//	from.a0_cid == to.a0_cid && from.b0_cid == to.b0_cid &&
		//	from.a1_cid == to.a1_cid && from.b1_cid == to.b1_cid
		//) {
		//	if (from.vin == to.vin && from.vout == to.vout) {
		//		return new PatternMatch(false, false, d);
		//	}
		//	else if (horizontal && from.vin == to.vout && from.vout == to.vin) {
		//		return new PatternMatch(true, false, Vx.D[3, d]);
		//	}
		//}
		//if (from.a0_p == to.a1_p && from.b0_p == to.b1_p &&
		//	from.a1_p == to.a0_p && from.b1_p == to.b0_p &&
		//	from.a0_cid == to.a1_cid && from.b0_cid == to.b1_cid &&
		//	from.a1_cid == to.a0_cid && from.b1_cid == to.b0_cid
		//) {
		//	if (from.vin == to.vout && from.vout == to.vin) {
		//		return new PatternMatch(false, false, Vx.D[3, d]);
		//	}
		//	else if (horizontal && from.vin == to.vin && from.vout == to.vout) {
		//		return new PatternMatch(true, false, d);
		//	}
		//}
		//if (vertical &&
		//	from.a0_p == to.b0_p && from.b0_p == to.a0_p &&
		//	from.a1_p == to.b1_p && from.b1_p == to.a1_p &&
		//	from.a0_cid == to.b0_cid && from.b0_cid == to.a0_cid &&
		//	from.a1_cid == to.b1_cid && from.b1_cid == to.a1_cid
		//) {
		//	if (from.vin == to.vin && from.vout == to.vout) {
		//		return new PatternMatch(false, true, d);
		//	}
		//	else if (horizontal && from.vin == to.vout && from.vout == to.vin) {
		//		return new PatternMatch(true, true, Vx.D[3, d]);
		//	}
		//}
		//if (vertical &&
		//	from.a0_p == to.b1_p && from.b0_p == to.a1_p &&
		//	from.a1_p == to.b0_p && from.b1_p == to.a0_p &&
		//	from.a0_cid == to.b1_cid && from.b0_cid == to.a1_cid &&
		//	from.a1_cid == to.b0_cid && from.b1_cid == to.a0_cid
		//) {
		//	if (from.vin == to.vout && from.vout == to.vin) {
		//		return new PatternMatch(false, true, Vx.D[3, d]);
		//	}
		//	else if (horizontal && from.vin == to.vin && from.vout == to.vout) {
		//		return new PatternMatch(true, true, d);
		//	}
		//}
		return new PatternMatch(false, false, -1);
	}
}