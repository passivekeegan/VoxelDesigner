using UnityEngine;

public struct IJL
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

	public IJL(int i, int j)
	{
		this.i = i;
		this.j = j;
		this.l = 0;
	}
	public IJL(Vector2Int v)
	{
		this.i = v.x;
		this.j = v.y;
		this.l = 0;
	}
	public IJL(Vector3Int v)
	{
		this.i = v.x;
		this.j = v.y;
		this.l = v.z;
	}
	public IJL(IJ ij)
	{
		this.i = ij.i;
		this.j = ij.j;
		this.l = 0;
	}
	public IJL(IJ ij, int l)
	{
		this.i = ij.i;
		this.j = ij.j;
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
	public static bool operator ==(IJL a, IJL b)
	{
		return (a.i == b.i) && (a.j == b.j) && (a.l == b.l);
	}
	public static bool operator ==(IJL a, object obj)
	{
		if (!(obj is IJL)) {
			return false;
		}
		IJL b = (IJL)obj;
		return (a.i == b.i) && (a.j == b.j) && (a.l == b.l);
	}
	public static bool operator ==(object obj, IJL a)
	{
		if (!(obj is IJL)) {
			return false;
		}
		IJL b = (IJL)obj;
		return (a.i == b.i) && (a.j == b.j) && (a.l == b.l);
	}
	public static bool operator !=(IJL a, IJL b)
	{
		return (a.i != b.i) || (a.j != b.j) || (a.l != b.l);
	}
	public static bool operator !=(IJL a, object obj)
	{
		if (!(obj is IJL)) {
			return true;
		}
		IJL b = (IJL)obj;
		return (a.i != b.i) || (a.j != b.j) || (a.l != b.l);
	}
	public static bool operator !=(object obj, IJL a)
	{
		if (!(obj is IJL)) {
			return true;
		}
		IJL b = (IJL)obj;
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
	public static IJL operator +(Vector2Int v, IJL a)
	{
		return new IJL(a.i + v.x, a.j + v.y, a.l);
	}
	public static IJL operator +(IJL a, Vector2Int v)
	{
		return new IJL(a.i + v.x, a.j + v.y, a.l);
	}
	public static IJL operator +(Vector3Int v, IJL a)
	{
		return new IJL(a.i + v.x, a.j + v.y, a.l + v.z);
	}
	public static IJL operator +(IJL a, Vector3Int v)
	{
		return new IJL(a.i + v.x, a.j + v.y, a.l + v.z);
	}
	public static IJL operator +(IJL a, IJ b)
	{
		return new IJL(a.i + b.i, a.j + b.j, a.l);
	}
	public static IJL operator +(IJ a, IJL b)
	{
		return new IJL(a.i + b.i, a.j + b.j, b.l);
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
	public static IJL operator -(Vector2Int v, IJL a)
	{
		return new IJL(v.x - a.i, v.y - a.j, a.l);
	}
	public static IJL operator -(IJL a, Vector2Int v)
	{
		return new IJL(a.i - v.x, a.j - v.y, a.l);
	}
	public static IJL operator -(Vector3Int v, IJL a)
	{
		return new IJL(v.x - a.i, v.y - a.j, v.z - a.l);
	}
	public static IJL operator -(IJL a, Vector3Int v)
	{
		return new IJL(a.i - v.x, a.j - v.y, a.l - v.z);
	}
	public static IJL operator -(IJL a, IJ b)
	{
		return new IJL(a.i - b.i, a.j - b.j, a.l);
	}
	public static IJL operator -(IJ a, IJL b)
	{
		return new IJL(a.i - b.i, a.j - b.j, b.l);
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
	public static IJL operator *(Vector2Int v, IJL a)
	{
		return new IJL(v.x * a.i, v.y * a.j, a.l);
	}
	public static IJL operator *(IJL a, Vector2Int v)
	{
		return new IJL(v.x * a.i, v.y * a.j, a.l);
	}
	public static IJL operator *(Vector3Int v, IJL a)
	{
		return new IJL(v.x * a.i, v.y * a.j, v.z * a.l);
	}
	public static IJL operator *(IJL a, Vector3Int v)
	{
		return new IJL(v.x * a.i, v.y * a.j, v.z * a.l);
	}
	public static Vector3 operator *(float c, IJL a)
	{
		return new Vector3(c * a.i, c * a.j, c * a.l);
	}
	public static Vector3 operator *(IJL a, float c)
	{
		return new Vector3(c * a.i, c * a.j, c * a.l);
	}
	public static IJL operator *(IJL a, IJ b)
	{
		return new IJL(a.i * b.i, a.j * b.j, a.l);
	}
	public static IJL operator *(IJ a, IJL b)
	{
		return new IJL(a.i * b.i, a.j * b.j, b.l);
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
	public static IJL operator /(Vector2Int v, IJL a)
	{
		return new IJL(v.x / a.i, v.y / a.j, a.l);
	}
	public static IJL operator /(IJL a, Vector2Int v)
	{
		return new IJL(a.i / v.x, a.j / v.y, a.l);
	}
	public static IJL operator /(Vector3Int v, IJL a)
	{
		return new IJL(v.x / a.i, v.y / a.j, v.z / a.l);
	}
	public static IJL operator /(IJL a, Vector3Int v)
	{
		return new IJL(a.i / v.x, a.j / v.y, a.l / v.z);
	}
	public static Vector3 operator /(float c, IJL a)
	{
		return new Vector3(c / a.i, c / a.j, c / a.l);
	}
	public static Vector3 operator /(IJL a, float c)
	{
		return new Vector3(a.i / c, a.j / c, a.l / c);
	}
	public static IJL operator /(IJL a, IJ b)
	{
		return new IJL(a.i / b.i, a.j / b.j, a.l);
	}
	public static IJL operator /(IJ a, IJL b)
	{
		return new IJL(a.i / b.i, a.j / b.j, b.l);
	}
	#endregion

	#region Conversions
	public static explicit operator IJL(IJ a)
	{
		return new IJL(a.i, a.j, 0);
	}
	public static explicit operator IJL(Vector2Int v)
	{
		return new IJL(v.x, v.y, 0);
	}
	public static explicit operator IJL(Vector3Int v)
	{
		return new IJL(v.x, v.y, v.z);
	}
	public static explicit operator Vector2Int(IJL a)
	{
		return new Vector2Int(a.i, a.j);
	}
	public static explicit operator Vector3Int(IJL a)
	{
		return new Vector3Int(a.i, a.j, a.l);
	}
	public static explicit operator Vector2(IJL a)
	{
		return new Vector2(a.i, a.j);
	}
	public static explicit operator Vector3(IJL a)
	{
		return new Vector3(a.i, a.j, a.l);
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
		return "WildIJL(" + i + "," + j + "," + l + ")";
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
	public IJ ij {
		get {
			return new IJ(i, j);
		}
	}
	public IJ ji {
		get {
			return new IJ(j, i);
		}
	}
	public IJ il {
		get {
			return new IJ(i, l);
		}
	}
	public IJ li {
		get {
			return new IJ(l, i);
		}
	}
	public IJ jl {
		get {
			return new IJ(j, l);
		}
	}
	public IJ lj {
		get {
			return new IJ(l, j);
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
