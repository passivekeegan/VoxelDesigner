using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EdgePattern : IEquatable<EdgePattern>
{
	public bool vertical;
	public int p0, p1;
	public int c0, c1;
	public int v0, v1, v2, v3;


	#region Initialization
	public EdgePattern(bool vertical, int p0, int p1, int v0, int v1, int v2, int v3)
	{
		this.vertical = vertical;
		this.p0 = p0;
		this.p1 = p1;
		this.v0 = v0;
		this.c0 = -1;
		this.c1 = -1;
		this.v1 = v1;
		this.v2 = v2;
		this.v3 = v3;
	}
	public EdgePattern(bool vertical, int p0, int p1, int c0, int c1, int v0, int v1, int v2, int v3)
	{
		this.vertical = vertical;
		this.p0 = p0;
		this.p1 = p1;
		this.v0 = v0;
		this.c0 = c0;
		this.c1 = c1;
		this.v1 = v1;
		this.v2 = v2;
		this.v3 = v3;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is EdgePattern)) {
			return false;
		}
		return this.Equals((EdgePattern)obj);
	}

	public bool Equals(EdgePattern other)
	{
		bool h = vertical || (!vertical && (
			((v0 == other.v0) && (v1 == other.v1) && (v2 == other.v2) && (v3 == other.v3)) ||
			((v0 == other.v2) && (v1 == other.v3) && (v2 == other.v0) && (v3 == other.v1))
		));
		bool v = !vertical || (vertical && (
			((v0 == other.v0) && (v1 == other.v1) && (v2 == other.v2)) ||
			((v0 == other.v2) && (v1 == other.v0) && (v2 == other.v1)) ||
			((v0 == other.v1) && (v1 == other.v2) && (v2 == other.v0))
		));
		return (
			(vertical == other.vertical) && h && v &&
			((
				(p0 == other.p0) &&
				(p1 == other.p1) &&
				(c0 == other.c0) &&
				(c1 == other.c1)
			) || (
				(p0 == other.p1) &&
				(p1 == other.p0) &&
				(c0 == other.c1) &&
				(c1 == other.c0)
			))
		);
	}

	public EdgePattern GetPatternCopy(bool keep_c0, bool keep_c1)
	{
		if (keep_c0 && keep_c1) {
			return new EdgePattern(vertical, p0, p1, c0, c1, v0, v1, v2, v3);
		} 
		else if (keep_c0) {
			return new EdgePattern(vertical, p0, p1, c0, -1, v0, v1, v2, v3);
		}
		else if (keep_c1) {
			return new EdgePattern(vertical, p0, p1, -1, c1, v0, v1, v2, v3);
		}
		else {
			return new EdgePattern(vertical, p0, p1, -1, -1, v0, v1, v2, v3);
		}
	}

	public static bool operator ==(EdgePattern ep0, EdgePattern ep1)
	{
		return ep0.Equals(ep1);
	}
	public static bool operator !=(EdgePattern ep0, EdgePattern ep1)
	{
		return !ep0.Equals(ep1);
	}
	#endregion

	public static EdgePattern empty {
		get {
			return new EdgePattern(false, 0, 0, 0, 0, 0, 0);
		}
	}

	public bool IsEmpty()
	{
		return (p0 == 0) && (p1 == 0) && (c0 < 0) && (c1 < 0) && (v0 == 0) && (v1 == 0) && (v2 == 0) && (vertical || (v3 == 0));
	}

	public bool valid {
		get {
			return (p0 >= 0) && (p1 >= 0) && (v0 >= 0) && (v1 >= 0) && (v2 >= 0) && (vertical || (v3 >= 0));
		}
	}
	public override int GetHashCode()
	{
		int a = (v0 + 1) * (v1 + 1) * (v2 + 1);
		if (vertical) {
			a *= 9;
		}
		else {
			a *= 5 * (v3 + 1);
		}
		int b = (3 * (c0 + 2) * (c1 + 2));
		int c = (7 * (p0 + 1) * (p1 + 1));
		return (a % 10000) + (b % 1000) + (c % 100);
	}

	public static bool CalculateAxiFlip(EdgePattern from, EdgePattern to)
	{
		//axi flip only available for horizontal axi
		if (from.vertical != to.vertical || from.vertical) {
			return false;
		}
		//check if horizontal voxel check is mirrored
		if ((from.v0 == from.v2) && (from.v1 == from.v3)) {
			return false;
		}
		//axi flip
		return (from.v0 == to.v2) && (from.v1 == to.v3) && (from.v2 == to.v0) && (from.v3 == to.v1);
	}

	public static EdgePattern FlipAxi(EdgePattern pat)
	{
		return new EdgePattern(pat.vertical, pat.p1, pat.p0, pat.c1, pat.c0, pat.v2, pat.v3, pat.v0, pat.v1);
	}

	//MAKE THIS FUNCTION CLEARER, DOESN'T DO WHAT YOU THINK IT DOES
	public static int CalculateVerticalShift(EdgePattern from, EdgePattern to, IJL edge_key)
	{
		if (!from.vertical || !to.vertical) {
			return -1;
		}
		int shift = 0;
		if (Vx.IsEdgeTopHeavy(edge_key)) {
			shift += 1;
		}
		if ((from.v0 == to.v0) && (from.v1 == to.v1) && (from.v2 == to.v2)) {
			return shift;
		}
		else if ((from.v0 == to.v2) && (from.v1 == to.v0) && (from.v2 == to.v1)) {
			return shift + 2;
		}
		else if ((from.v0 == to.v1) && (from.v1 == to.v2) && (from.v2 == to.v0)) {
			return shift + 4;
		}
		else {
			return -1;
		}
	}

	public static int CalculateHorizontalShift(EdgePattern from, EdgePattern to, int to_axi)
	{
		if (from.vertical || to.vertical) {
			return -1;
		}
		int d = to_axi - 2;
		if (d < 0 || d > 5) {
			return -1;
		}
		int shift = Vx.ND[2, d];
		if ((from.v0 == to.v0) && (from.v1 == to.v1) && (from.v2 == to.v2) && (from.v3 == to.v3)) {
			return shift;
		}
		else if ((from.v0 == to.v2) && (from.v1 == to.v3) && (from.v2 == to.v0) && (from.v3 == to.v1)) {
			return Vx.D[3, shift];
		}
		else {
			return -1;
		}
	}
}