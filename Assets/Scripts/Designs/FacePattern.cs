using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FacePattern
{
	public bool hexagon;
	public int v0, v1;
	public int p0, p1, p2, p3, p4, p5;
	public int c0, c1, c2, c3, c4, c5;

	#region Initialize
	public FacePattern(int p0, int p1, int p2, int p3, int v0, int v1)
	{
		this.hexagon = false;
		this.p0 = p0;
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;
		this.p4 = 0;
		this.p5 = 0;
		this.c0 = -1;
		this.c1 = -1;
		this.c2 = -1;
		this.c3 = -1;
		this.c4 = -1;
		this.c5 = -1;
		this.v0 = v0;
		this.v1 = v1;
	}
	public FacePattern(int p0, int p1, int p2, int p3, int c0, int c1, int c2, int c3, int v0, int v1)
	{
		this.hexagon = false;
		this.p0 = p0;
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;
		this.p4 = 0;
		this.p5 = 0;
		this.c0 = c0;
		this.c1 = c1;
		this.c2 = c2;
		this.c3 = c3;
		this.c4 = -1;
		this.c5 = -1;
		this.v0 = v0;
		this.v1 = v1;
	}
	public FacePattern(int p0, int p1, int p2, int p3, int p4, int p5, int v0, int v1)
	{
		this.hexagon = true;
		this.p0 = p0;
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;
		this.p4 = p4;
		this.p5 = p5;
		this.c0 = -1;
		this.c1 = -1;
		this.c2 = -1;
		this.c3 = -1;
		this.c4 = -1;
		this.c5 = -1;
		this.v0 = v0;
		this.v1 = v1;
	}
	public FacePattern(int p0, int p1, int p2, int p3, int p4, int p5, int c0, int c1, int c2, int c3, int c4, int c5, int v0, int v1)
	{
		this.hexagon = true;
		this.p0 = p0;
		this.p1 = p1;
		this.p2 = p2;
		this.p3 = p3;
		this.p4 = p4;
		this.p5 = p5;
		this.c0 = c0;
		this.c1 = c1;
		this.c2 = c2;
		this.c3 = c3;
		this.c4 = c4;
		this.c5 = c5;
		this.v0 = v0;
		this.v1 = v1;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is FacePattern)) {
			return false;
		}
		return this.Equals((FacePattern)obj);
	}
	public bool Equals(FacePattern other)
	{
		bool hexagon_bool = hexagon && other.hexagon && (v0 == other.v0) && (v1 == other.v1) && (
			((p0 == other.p0) && (p1 == other.p1) && (p2 == other.p2) && (p3 == other.p3) && (p4 == other.p4) && (p5 == other.p5)) ||
			((p0 == other.p5) && (p1 == other.p0) && (p2 == other.p1) && (p3 == other.p2) && (p4 == other.p3) && (p5 == other.p4)) ||
			((p0 == other.p4) && (p1 == other.p5) && (p2 == other.p0) && (p3 == other.p1) && (p4 == other.p2) && (p5 == other.p3)) ||
			((p0 == other.p3) && (p1 == other.p4) && (p2 == other.p5) && (p3 == other.p0) && (p4 == other.p1) && (p5 == other.p2)) ||
			((p0 == other.p2) && (p1 == other.p3) && (p2 == other.p4) && (p3 == other.p5) && (p4 == other.p0) && (p5 == other.p1)) ||
			((p0 == other.p1) && (p1 == other.p2) && (p2 == other.p3) && (p3 == other.p4) && (p4 == other.p5) && (p5 == other.p0))
		);
		bool rect_bool = !hexagon && !other.hexagon && (
			((p0 == other.p0) && (p1 == other.p1) && (p2 == other.p2) && (p3 == other.p3) && (v0 == other.v0) && (v1 == other.v1)) ||
			((p0 == other.p2) && (p1 == other.p3) && (p2 == other.p0) && (p3 == other.p1) && (v0 == other.v1) && (v1 == other.v0))
		);
		return (hexagon_bool || rect_bool);
	}

	public static bool operator ==(FacePattern fp0, FacePattern fp1)
	{
		return fp0.Equals(fp1);
	}
	public static bool operator !=(FacePattern fp0, FacePattern fp1)
	{
		return !fp0.Equals(fp1);
	}
	#endregion

	public static FacePattern emptyRect {
		get {
			return new FacePattern(0, 0, 0, 0, 0, 0);
		}
	}
	public static FacePattern emptyHexagon {
		get {
			return new FacePattern(0, 0, 0, 0, 0, 0, 0, 0);
		}
	}

	public bool IsEmpty()
	{
		return (p0 == 0) && (p1 == 0) && (p2 == 0) && (p3 == 0) && (!hexagon || ((p4 == 0) && (p5 == 0))) && (v0 == 0) && (v1 == 0);
	}

	public bool IsValid()
	{
		return (p0 > 0) && (p1 > 0) && (p2 > 0) && (p3 > 0) && (!hexagon || ((p4 > 0) && (p5 > 0))) && (v0 >= 0) && (v1 >= 0);
	}

	public override int GetHashCode()
	{
		int code = 0;
		if (hexagon) {
			code += 5 * (p0 + 3) * (p1 + 3) * (p2 + 3) * (p3 + 3) * (p4 + 3) * (p5 + 3);
			code += 13 * (v0 + 9) * (v1 + 9);
		}
		else {
			code += 9 * (((p0 + 11) * (p2 + 11)) + ((p1 + 7) * (p3 + 7)));
			code += 17 * (v0 + 17) * (v1 + 17);
		}
		return code;
	}

	public static FacePattern FlipAxi(FacePattern pat)
	{
		if (pat.hexagon) {
			return new FacePattern(pat.p0, pat.p1, pat.p2, pat.p3, pat.p4, pat.p5, pat.v0, pat.v1);
		}
		else {
			return new FacePattern(pat.p2, pat.p3, pat.p0, pat.p1, pat.v1, pat.v0);
		}
	}

	public static int CalculateShift(FacePattern from, FacePattern to)
	{
		//quick shift match check
		if (from.hexagon != to.hexagon) {
			return -1;
		}
		if (!from.hexagon) {
			if ((from.p0 == to.p0) && (from.p1 == to.p1) && (from.p2 == to.p2) && (from.p3 == to.p3) && (from.v0 == to.v0) && (from.v1 == to.v1)) {
				return 0;
			}
			else if ((from.p0 == to.p2) && (from.p1 == to.p3) && (from.p2 == to.p0) && (from.p3 == to.p1) && (from.v0 == to.v1) && (from.v1 == to.v0)) {
				return 3;
			}
			return -1;
		}
		if ((from.p0 == to.p0) && (from.p1 == to.p1) && (from.p2 == to.p2) && (from.p3 == to.p3) && (from.p4 == to.p4) && (from.p5 == to.p5)) {
			return 0;
		}
		else if ((from.p0 == to.p5) && (from.p1 == to.p0) && (from.p2 == to.p1) && (from.p3 == to.p2) && (from.p4 == to.p3) && (from.p5 == to.p4)) {
			return 1;
		}
		else if ((from.p0 == to.p4) && (from.p1 == to.p5) && (from.p2 == to.p0) && (from.p3 == to.p1) && (from.p4 == to.p2) && (from.p5 == to.p3)) {
			return 2;
		}
		else if ((from.p0 == to.p3) && (from.p1 == to.p4) && (from.p2 == to.p5) && (from.p3 == to.p0) && (from.p4 == to.p1) && (from.p5 == to.p2)) {
			return 3;
		}
		else if ((from.p0 == to.p2) && (from.p1 == to.p3) && (from.p2 == to.p4) && (from.p3 == to.p5) && (from.p4 == to.p0) && (from.p5 == to.p1)) {
			return 4;
		}
		else if ((from.p0 == to.p1) && (from.p1 == to.p2) && (from.p2 == to.p3) && (from.p3 == to.p4) && (from.p4 == to.p5) && (from.p5 == to.p0)) {
			return 5;
		}
		else {
			return -1;
		}
	}
}
