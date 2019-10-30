using System;

[Serializable]
public struct VoxelPattern : IEquatable<VoxelPattern>
{
	public int a0, a1, a2;
	public int b0, b1, b2;

	public VoxelPattern(int a0, int a1, int a2, int b0, int b1, int b2)
	{
		this.a0 = a0;
		this.a1 = a1;
		this.a2 = a2;
		this.b0 = b0;
		this.b1 = b1;
		this.b2 = b2;
	}

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is VoxelPattern)) {
			return false;
		}
		return Equals((VoxelPattern)obj);
	}
	public bool Equals(VoxelPattern other)
	{
		return CalculateTransform(this, other, false).shift >= 0;
	}
	public static bool operator ==(VoxelPattern vp0, VoxelPattern vp1)
	{
		return vp0.Equals(vp1);
	}
	public static bool operator !=(VoxelPattern vp0, VoxelPattern vp1)
	{
		return !vp0.Equals(vp1);
	}
	#endregion

	public override int GetHashCode()
	{
		int a = (a0 % 100) + (b0 % 100);
		int b = (a1 % 100) + (b1 % 100);
		int c = (a2 % 100) + (b2 % 100);
		return (a + b + c) % 100;
	}

	public bool isEmpty {
		get {
			return a0 == 0 && a1 == 0 && a2 == 0 && b0 == 0 && b1 == 0 && b2 == 0;
		}
	}

	public override string ToString()
	{
		return "Pattern(A:" + a0.ToString() + a1.ToString() + a2.ToString() + " B:" + b0.ToString() + b1.ToString() + b2.ToString() + ")";
	}

	public static PatternMatch CalculateTransform(VoxelPattern from, VoxelPattern to, bool topheavy)
	{
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
		else if (from.a0 == to.a0 && from.b0 == to.b0 && from.a1 == to.a2 && from.b1 == to.b2 && from.a2 == to.a1 && from.b2 == to.b1) {
			return new PatternMatch(true, false, Vx.ND[heavyshift, 4]);
		}
		else if (from.a0 == to.a1 && from.b0 == to.b1 && from.a1 == to.a0 && from.b1 == to.b0 && from.a2 == to.a2 && from.b2 == to.b2) {
			return new PatternMatch(true, false, Vx.ND[heavyshift, 0]);
		}
		else if (from.a0 == to.a2 && from.b0 == to.b2 && from.a1 == to.a1 && from.b1 == to.b1 && from.a2 == to.a0 && from.b2 == to.b0) {
			return new PatternMatch(true, false, Vx.ND[heavyshift, 2]);
		}
		else if (from.a0 == to.b0 && from.b0 == to.a0 && from.a1 == to.b1 && from.b1 == to.a1 && from.a2 == to.b2 && from.b2 == to.a2) {
			return new PatternMatch(false, true, Vx.ND[heavyshift, 0]);
		}
		else if (from.a0 == to.b1 && from.b0 == to.a1 && from.a1 == to.b2 && from.b1 == to.a2 && from.a2 == to.b0 && from.b2 == to.a0) {
			return new PatternMatch(false, true, Vx.ND[heavyshift, 2]);
		}
		else if (from.a0 == to.b2 && from.b0 == to.a2 && from.a1 == to.b0 && from.b1 == to.a0 && from.a2 == to.b1 && from.b2 == to.a1) {
			return new PatternMatch(false, true, Vx.ND[heavyshift, 4]);
		}
		else if (from.a0 == to.b0 && from.b0 == to.a0 && from.a1 == to.b2 && from.b1 == to.a2 && from.a2 == to.b1 && from.b2 == to.a1) {
			return new PatternMatch(true, true, Vx.ND[heavyshift, 4]);
		}
		else if (from.a0 == to.b1 && from.b0 == to.a1 && from.a1 == to.b0 && from.b1 == to.a0 && from.a2 == to.b2 && from.b2 == to.a2) {
			return new PatternMatch(true, true, Vx.ND[heavyshift, 0]);
		}
		else if (from.a0 == to.b2 && from.b0 == to.a2 && from.a1 == to.b1 && from.b1 == to.a1 && from.a2 == to.b0 && from.b2 == to.a0) {
			return new PatternMatch(true, true, Vx.ND[heavyshift, 2]);
		}
		return new PatternMatch(false, false, -1);
	}
}
