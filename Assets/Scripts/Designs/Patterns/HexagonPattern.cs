using System;

[Serializable]
public struct HexagonPattern : IEquatable<HexagonPattern>
{
	public bool xflip;
	public bool yflip;
	public int c0_p, c1_p, c2_p, c3_p, c4_p, c5_p;
	public int c0_cid, c1_cid, c2_cid, c3_cid, c4_cid, c5_cid;
	public int va, vb;

	#region Initialization
	public HexagonPattern(bool xflip, bool yflip, int c0_p, int c1_p, int c2_p, int c3_p, int c4_p, int c5_p, int va, int vb)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.c0_p = c0_p;
		this.c1_p = c1_p;
		this.c2_p = c2_p;
		this.c3_p = c3_p;
		this.c4_p = c4_p;
		this.c5_p = c5_p;
		this.c0_cid = -1;
		this.c1_cid = -1;
		this.c2_cid = -1;
		this.c3_cid = -1;
		this.c4_cid = -1;
		this.c5_cid = -1;
		this.va = va;
		this.vb = vb;
	}
	public HexagonPattern(bool xflip, bool yflip, int c0_p, int c1_p, int c2_p, int c3_p, int c4_p, int c5_p, int c0_cid, int c1_cid, int c2_cid, int c3_cid, int c4_cid, int c5_cid, int va, int vb)
	{
		this.xflip = xflip;
		this.yflip = yflip;
		this.c0_p = c0_p;
		this.c1_p = c1_p;
		this.c2_p = c2_p;
		this.c3_p = c3_p;
		this.c4_p = c4_p;
		this.c5_p = c5_p;
		this.c0_cid = c0_cid;
		this.c1_cid = c1_cid;
		this.c2_cid = c2_cid;
		this.c3_cid = c3_cid;
		this.c4_cid = c4_cid;
		this.c5_cid = c5_cid;
		this.va = va;
		this.vb = vb;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is HexagonPattern)) {
			return false;
		}
		return this.Equals((HexagonPattern)obj);
	}
	public bool Equals(HexagonPattern other)
	{
		bool vertical = yflip || other.yflip;
		bool horizontal = xflip || other.xflip;
		if (c0_p == other.c0_p && c0_cid == other.c0_cid &&
			c1_p == other.c1_p && c1_cid == other.c1_cid &&
			c2_p == other.c2_p && c2_cid == other.c2_cid &&
			c3_p == other.c3_p && c3_cid == other.c3_cid &&
			c4_p == other.c4_p && c4_cid == other.c4_cid &&
			c5_p == other.c5_p && c5_cid == other.c5_cid
		) {
			//shift 0
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (c0_p == other.c1_p && c0_cid == other.c1_cid &&
			c1_p == other.c2_p && c1_cid == other.c2_cid &&
			c2_p == other.c3_p && c2_cid == other.c3_cid &&
			c3_p == other.c4_p && c3_cid == other.c4_cid &&
			c4_p == other.c5_p && c4_cid == other.c5_cid &&
			c5_p == other.c0_p && c5_cid == other.c0_cid
		) {
			//shift 1
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (c0_p == other.c2_p && c0_cid == other.c2_cid &&
			c1_p == other.c3_p && c1_cid == other.c3_cid &&
			c2_p == other.c4_p && c2_cid == other.c4_cid &&
			c3_p == other.c5_p && c3_cid == other.c5_cid &&
			c4_p == other.c0_p && c4_cid == other.c0_cid &&
			c5_p == other.c1_p && c5_cid == other.c1_cid
		) {
			//shift 2
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (c0_p == other.c3_p && c0_cid == other.c3_cid &&
			c1_p == other.c4_p && c1_cid == other.c4_cid &&
			c2_p == other.c5_p && c2_cid == other.c5_cid &&
			c3_p == other.c0_p && c3_cid == other.c0_cid &&
			c4_p == other.c1_p && c4_cid == other.c1_cid &&
			c5_p == other.c2_p && c5_cid == other.c2_cid
		) {
			//shift 3
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (c0_p == other.c4_p && c0_cid == other.c4_cid &&
			c1_p == other.c5_p && c1_cid == other.c5_cid &&
			c2_p == other.c0_p && c2_cid == other.c0_cid &&
			c3_p == other.c1_p && c3_cid == other.c1_cid &&
			c4_p == other.c2_p && c4_cid == other.c2_cid &&
			c5_p == other.c3_p && c5_cid == other.c3_cid
		) {
			//shift 4
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (c0_p == other.c5_p && c0_cid == other.c5_cid &&
			c1_p == other.c0_p && c1_cid == other.c0_cid &&
			c2_p == other.c1_p && c2_cid == other.c1_cid &&
			c3_p == other.c2_p && c3_cid == other.c2_cid &&
			c4_p == other.c3_p && c4_cid == other.c3_cid &&
			c5_p == other.c4_p && c5_cid == other.c4_cid
		) {
			//shift 5
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (horizontal &&
			c0_p == other.c0_p && c0_cid == other.c0_cid &&
			c1_p == other.c5_p && c1_cid == other.c5_cid &&
			c2_p == other.c4_p && c2_cid == other.c4_cid &&
			c3_p == other.c3_p && c3_cid == other.c3_cid &&
			c4_p == other.c2_p && c4_cid == other.c2_cid &&
			c5_p == other.c1_p && c5_cid == other.c1_cid
		) {
			//shift 0
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (horizontal &&
			c0_p == other.c5_p && c0_cid == other.c5_cid &&
			c1_p == other.c4_p && c1_cid == other.c4_cid &&
			c2_p == other.c3_p && c2_cid == other.c3_cid &&
			c3_p == other.c2_p && c3_cid == other.c2_cid &&
			c4_p == other.c1_p && c4_cid == other.c1_cid &&
			c5_p == other.c0_p && c5_cid == other.c0_cid
		) {
			//shift 1
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (horizontal &&
			c0_p == other.c4_p && c0_cid == other.c4_cid &&
			c1_p == other.c3_p && c1_cid == other.c3_cid &&
			c2_p == other.c2_p && c2_cid == other.c2_cid &&
			c3_p == other.c1_p && c3_cid == other.c1_cid &&
			c4_p == other.c0_p && c4_cid == other.c0_cid &&
			c5_p == other.c5_p && c5_cid == other.c5_cid
		) {
			//shift 2
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (horizontal &&
			c0_p == other.c3_p && c0_cid == other.c3_cid &&
			c1_p == other.c2_p && c1_cid == other.c2_cid &&
			c2_p == other.c1_p && c2_cid == other.c1_cid &&
			c3_p == other.c0_p && c3_cid == other.c0_cid &&
			c4_p == other.c5_p && c4_cid == other.c5_cid &&
			c5_p == other.c4_p && c5_cid == other.c4_cid
		) {
			//shift 3
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (horizontal &&
			c0_p == other.c2_p && c0_cid == other.c2_cid &&
			c1_p == other.c1_p && c1_cid == other.c1_cid &&
			c2_p == other.c0_p && c2_cid == other.c0_cid &&
			c3_p == other.c5_p && c3_cid == other.c5_cid &&
			c4_p == other.c4_p && c4_cid == other.c4_cid &&
			c5_p == other.c3_p && c5_cid == other.c3_cid
		) {
			//shift 4
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		if (horizontal &&
			c0_p == other.c1_p && c0_cid == other.c1_cid &&
			c1_p == other.c0_p && c1_cid == other.c0_cid &&
			c2_p == other.c5_p && c2_cid == other.c5_cid &&
			c3_p == other.c4_p && c3_cid == other.c4_cid &&
			c4_p == other.c3_p && c4_cid == other.c3_cid &&
			c5_p == other.c2_p && c5_cid == other.c2_cid
		) {
			//shift 5
			if (va == other.va && vb == other.vb) {
				return true;
			}
			else if (vertical && va == other.vb && vb == other.va) {
				return true;
			}
		}
		return false;
	}
	public static bool operator ==(HexagonPattern pat0, HexagonPattern pat1)
	{
		return pat0.Equals(pat1);
	}
	public static bool operator !=(HexagonPattern pat0, HexagonPattern pat1)
	{
		return !pat0.Equals(pat1);
	}
	#endregion

	public bool IsEmpty()
	{
		return c0_p == 0 && c1_p == 0 && c2_p == 0 && c3_p == 0 && c4_p == 0 && c5_p == 0 && va == 0 && vb == 0;
	}
	public bool IsValid()
	{
		return c0_p >= 0 && c1_p >= 0 && c2_p >= 0 && c3_p >= 0 && c4_p >= 0 && c5_p >= 0 && va >= 0 && vb >= 0;
	}
	public static HexagonPattern empty {
		get {
			return new HexagonPattern(false, false, 0, 0, 0, 0, 0, 0, 0, 0);
		}
	}

	public override int GetHashCode()
	{
		int code = 5 * (c0_p + 3) * (c1_p + 3) * (c2_p + 3) * (c3_p + 3) * (c4_p + 3) * (c5_p + 3);
		code += 13 * (va + 9) * (vb + 9);
		return code % 1000;
	}

	public static PatternMatch CalculateTransform(HexagonPattern from, HexagonPattern to)
	{
		bool vertical = from.yflip || to.yflip;
		bool horizontal = from.xflip || to.xflip;
		if (from.c0_p == to.c0_p && from.c0_cid == to.c0_cid &&
			from.c1_p == to.c1_p && from.c1_cid == to.c1_cid &&
			from.c2_p == to.c2_p && from.c2_cid == to.c2_cid &&
			from.c3_p == to.c3_p && from.c3_cid == to.c3_cid &&
			from.c4_p == to.c4_p && from.c4_cid == to.c4_cid &&
			from.c5_p == to.c5_p && from.c5_cid == to.c5_cid
		) {
			//shift 0
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(false, false, 0);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(false, true, 0);
			}
		}
		if (from.c0_p == to.c1_p && from.c0_cid == to.c1_cid &&
			from.c1_p == to.c2_p && from.c1_cid == to.c2_cid &&
			from.c2_p == to.c3_p && from.c2_cid == to.c3_cid &&
			from.c3_p == to.c4_p && from.c3_cid == to.c4_cid &&
			from.c4_p == to.c5_p && from.c4_cid == to.c5_cid &&
			from.c5_p == to.c0_p && from.c5_cid == to.c0_cid
		) {
			//shift 1
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(false, false, 1);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(false, true, 1);
			}
		}
		if (from.c0_p == to.c2_p && from.c0_cid == to.c2_cid &&
			from.c1_p == to.c3_p && from.c1_cid == to.c3_cid &&
			from.c2_p == to.c4_p && from.c2_cid == to.c4_cid &&
			from.c3_p == to.c5_p && from.c3_cid == to.c5_cid &&
			from.c4_p == to.c0_p && from.c4_cid == to.c0_cid &&
			from.c5_p == to.c1_p && from.c5_cid == to.c1_cid
		) {
			//shift 2
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(false, false, 2);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(false, true, 2);
			}
		}
		if (from.c0_p == to.c3_p && from.c0_cid == to.c3_cid &&
			from.c1_p == to.c4_p && from.c1_cid == to.c4_cid &&
			from.c2_p == to.c5_p && from.c2_cid == to.c5_cid &&
			from.c3_p == to.c0_p && from.c3_cid == to.c0_cid &&
			from.c4_p == to.c1_p && from.c4_cid == to.c1_cid &&
			from.c5_p == to.c2_p && from.c5_cid == to.c2_cid
		) {
			//shift 3
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(false, false, 3);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(false, true, 3);
			}
		}
		if (from.c0_p == to.c4_p && from.c0_cid == to.c4_cid &&
			from.c1_p == to.c5_p && from.c1_cid == to.c5_cid &&
			from.c2_p == to.c0_p && from.c2_cid == to.c0_cid &&
			from.c3_p == to.c1_p && from.c3_cid == to.c1_cid &&
			from.c4_p == to.c2_p && from.c4_cid == to.c2_cid &&
			from.c5_p == to.c3_p && from.c5_cid == to.c3_cid
		) {
			//shift 4
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(false, false, 4);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(false, true, 4);
			}
		}
		if (from.c0_p == to.c5_p && from.c0_cid == to.c5_cid &&
			from.c1_p == to.c0_p && from.c1_cid == to.c0_cid &&
			from.c2_p == to.c1_p && from.c2_cid == to.c1_cid &&
			from.c3_p == to.c2_p && from.c3_cid == to.c2_cid &&
			from.c4_p == to.c3_p && from.c4_cid == to.c3_cid &&
			from.c5_p == to.c4_p && from.c5_cid == to.c4_cid
		) {
			//shift 5
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(false, false, 5);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(false, true, 5);
			}
		}
		if (horizontal &&
			from.c0_p == to.c0_p && from.c0_cid == to.c0_cid &&
			from.c1_p == to.c5_p && from.c1_cid == to.c5_cid &&
			from.c2_p == to.c4_p && from.c2_cid == to.c4_cid &&
			from.c3_p == to.c3_p && from.c3_cid == to.c3_cid &&
			from.c4_p == to.c2_p && from.c4_cid == to.c2_cid &&
			from.c5_p == to.c1_p && from.c5_cid == to.c1_cid
		) {
			//shift 0
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(true, false, 0);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(true, true, 0);
			}
		}
		if (horizontal &&
			from.c0_p == to.c5_p && from.c0_cid == to.c5_cid &&
			from.c1_p == to.c4_p && from.c1_cid == to.c4_cid &&
			from.c2_p == to.c3_p && from.c2_cid == to.c3_cid &&
			from.c3_p == to.c2_p && from.c3_cid == to.c2_cid &&
			from.c4_p == to.c1_p && from.c4_cid == to.c1_cid &&
			from.c5_p == to.c0_p && from.c5_cid == to.c0_cid
		) {
			//shift 1
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(true, false, 1);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(true, true, 1);
			}
		}
		if (horizontal &&
			from.c0_p == to.c4_p && from.c0_cid == to.c4_cid &&
			from.c1_p == to.c3_p && from.c1_cid == to.c3_cid &&
			from.c2_p == to.c2_p && from.c2_cid == to.c2_cid &&
			from.c3_p == to.c1_p && from.c3_cid == to.c1_cid &&
			from.c4_p == to.c0_p && from.c4_cid == to.c0_cid &&
			from.c5_p == to.c5_p && from.c5_cid == to.c5_cid
		) {
			//shift 2
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(true, false, 2);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(true, true, 2);
			}
		}
		if (horizontal &&
			from.c0_p == to.c3_p && from.c0_cid == to.c3_cid &&
			from.c1_p == to.c2_p && from.c1_cid == to.c2_cid &&
			from.c2_p == to.c1_p && from.c2_cid == to.c1_cid &&
			from.c3_p == to.c0_p && from.c3_cid == to.c0_cid &&
			from.c4_p == to.c5_p && from.c4_cid == to.c5_cid &&
			from.c5_p == to.c4_p && from.c5_cid == to.c4_cid
		) {
			//shift 3
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(true, false, 3);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(true, true, 3);
			}
		}
		if (horizontal &&
			from.c0_p == to.c2_p && from.c0_cid == to.c2_cid &&
			from.c1_p == to.c1_p && from.c1_cid == to.c1_cid &&
			from.c2_p == to.c0_p && from.c2_cid == to.c0_cid &&
			from.c3_p == to.c5_p && from.c3_cid == to.c5_cid &&
			from.c4_p == to.c4_p && from.c4_cid == to.c4_cid &&
			from.c5_p == to.c3_p && from.c5_cid == to.c3_cid
		) {
			//shift 4
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(true, false, 4);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(true, true, 4);
			}
		}
		if (horizontal &&
			from.c0_p == to.c1_p && from.c0_cid == to.c1_cid &&
			from.c1_p == to.c0_p && from.c1_cid == to.c0_cid &&
			from.c2_p == to.c5_p && from.c2_cid == to.c5_cid &&
			from.c3_p == to.c4_p && from.c3_cid == to.c4_cid &&
			from.c4_p == to.c3_p && from.c4_cid == to.c3_cid &&
			from.c5_p == to.c2_p && from.c5_cid == to.c2_cid
		) {
			//shift 5
			if (from.va == to.va && from.vb == to.vb) {
				return new PatternMatch(true, false, 5);
			}
			else if (vertical && from.va == to.vb && from.vb == to.va) {
				return new PatternMatch(true, true, 5);
			}
		}
		return new PatternMatch(false, false, -1);
	}
}
