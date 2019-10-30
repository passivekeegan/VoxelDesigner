using System;

[Serializable]
public struct IJL : IEquatable<IJL>
{
	public int i;
	public int j;
	public int l;

	#region Initialization
	public IJL(int i, int j, int l)
	{
		this.i = i;
		this.j = j;
		this.l = l;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is IJL)) {
			return false;
		}
		IJL other = (IJL)obj;
		return (i == other.i) && (j == other.j) && (l == other.l);
	}
	public bool Equals(IJL other)
	{
		return (i == other.i) && (j == other.j) && (l == other.l);
	}
	public static bool operator ==(IJL a, IJL b)
	{
		return (a.i == b.i) && (a.j == b.j) && (a.l == b.l);
	}
	public static bool operator !=(IJL a, IJL b)
	{
		return (a.i != b.i) || (a.j != b.j) || (a.l != b.l);
	}
	#endregion

	#region Addition
	public static IJL operator +(IJL a, IJL b)
	{
		return new IJL(a.i + b.i, a.j + b.j, a.l + b.l);
	}
	public static IJL operator +(int c, IJL a)
	{
		return new IJL(a.i + c, a.j + c, a.l + c);
	}
	public static IJL operator +(IJL a, int c)
	{
		return new IJL(a.i + c, a.j + c, a.l + c);
	}
	#endregion

	#region Subtraction
	public static IJL operator -(IJL a)
	{
		return new IJL(-a.i, -a.j, -a.l);
	}
	public static IJL operator -(IJL a, IJL b)
	{
		return new IJL(a.i - b.i, a.j - b.j, a.l - b.l);
	}
	public static IJL operator -(int c, IJL a)
	{
		return new IJL(c - a.i, c - a.j, c - a.l);
	}
	public static IJL operator -(IJL a, int c)
	{
		return new IJL(a.i - c, a.j - c, a.l - c);
	}
	#endregion

	#region Multiplication
	public static IJL operator *(IJL a, IJL b)
	{
		return new IJL(a.i * b.i, a.j * b.j, a.l * b.l);
	}
	public static IJL operator *(int c, IJL a)
	{
		return new IJL(c * a.i, c * a.j, c * a.l);
	}
	public static IJL operator *(IJL a, int c)
	{
		return new IJL(c * a.i, c * a.j, c * a.l);
	}
	#endregion

	#region Division
	public static IJL operator /(IJL a, IJL b)
	{
		return new IJL(a.i / b.i, a.j / b.j, a.l / b.l);
	}
	public static IJL operator /(int c, IJL a)
	{
		return new IJL(c / a.i, c / a.j, c / a.l);
	}
	public static IJL operator /(IJL a, int c)
	{
		return new IJL(a.i / c, a.j / c, a.l / c);
	}
	#endregion

	#region Hash Code
	public override int GetHashCode()
	{
		return ((i % 1100) * 880000) + ((j % 1100) * 800) + (l % 800);
	}
	#endregion

	#region String
	public override string ToString()
	{
		return "IJL(" + i + "," + j + "," + l + ")";
	}
	#endregion

	#region Properties
	public static IJL zero {
		get {
			return new IJL(0, 0, 0);
		}
	}
	public static IJL one {
		get {
			return new IJL(1, 1, 1);
		}
	}
	public IJL ijl {
		get {
			return new IJL(i, j, l);
		}
	}
	public IJL ilj {
		get {
			return new IJL(i, l, j);
		}
	}
	public IJL jil {
		get {
			return new IJL(j, i, l);
		}
	}
	public IJL lij {
		get {
			return new IJL(l, i, j);
		}
	}
	public IJL jli {
		get {
			return new IJL(j, l, i);
		}
	}
	public IJL lji {
		get {
			return new IJL(l, j, i);
		}
	}
	#endregion
}
