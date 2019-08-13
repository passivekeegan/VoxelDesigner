using UnityEngine;

public struct IJ
{
	public int i;
	public int j;

	#region Initialization
	public IJ(int i, int j)
	{
		this.i = i;
		this.j = j;
	}

	public IJ(Vector2Int v)
	{
		this.i = v.x;
		this.j = v.y;
	}
	public IJ(Vector3Int v)
	{
		this.i = v.x;
		this.j = v.y;
	}
	public IJ(IJ ij)
	{
		this.i = ij.i;
		this.j = ij.j;
	}
	#endregion

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is IJ)) {
			return false;
		}
		IJ other = (IJ)obj;
		return (i == other.i) && (j == other.j);
	}
	public static bool operator ==(IJ a, IJ b)
	{
		return (a.i == b.i) && (a.j == b.j);
	}
	public static bool operator ==(IJ a, object obj)
	{
		if (!(obj is IJ)) {
			return false;
		}
		IJ other = (IJ)obj;
		return (a.i == other.i) && (a.j == other.j);
	}
	public static bool operator ==(object obj, IJ a)
	{
		if (!(obj is IJ)) {
			return false;
		}
		IJ other = (IJ)obj;
		return (a.i == other.i) && (a.j == other.j);
	}
	public static bool operator !=(IJ a, IJ b)
	{
		return (a.i != b.i) || (a.j != b.j);
	}
	public static bool operator !=(IJ a, object obj)
	{
		if (!(obj is IJ)) {
			return true;
		}
		IJ other = (IJ)obj;
		return (a.i != other.i) || (a.j != other.j);
	}
	public static bool operator !=(object obj, IJ a)
	{
		if (!(obj is IJ)) {
			return true;
		}
		IJ other = (IJ)obj;
		return (a.i != other.i) || (a.j != other.j);
	}
	#endregion

	#region Addition
	public static IJ operator +(IJ a, IJ b)
	{
		return new IJ(a.i + b.i, a.j + b.j);
	}
	public static IJ operator +(int c, IJ a)
	{
		return new IJ(a.i + c, a.j + c);
	}
	public static IJ operator +(IJ a, int c)
	{
		return new IJ(a.i + c, a.j + c);
	}
	public static IJ operator +(Vector2Int v, IJ a)
	{
		return new IJ(a.i + v.x, a.j + v.y);
	}
	public static IJ operator +(IJ a, Vector2Int v)
	{
		return new IJ(a.i + v.x, a.j + v.y);
	}
	public static IJ operator +(Vector3Int v, IJ a)
	{
		return new IJ(a.i + v.x, a.j + v.y);
	}
	public static IJ operator +(IJ a, Vector3Int v)
	{
		return new IJ(a.i + v.x, a.j + v.y);
	}
	#endregion

	#region Subtraction
	public static IJ operator -(IJ a, IJ b)
	{
		return new IJ(a.i - b.i, a.j - b.j);
	}
	public static IJ operator -(int c, IJ a)
	{
		return new IJ(c - a.i, c - a.j);
	}
	public static IJ operator -(IJ a, int c)
	{
		return new IJ(a.i - c, a.j - c);
	}
	public static IJ operator -(Vector2Int v, IJ a)
	{
		return new IJ(v.x - a.i, v.y - a.j);
	}
	public static IJ operator -(IJ a, Vector2Int v)
	{
		return new IJ(a.i - v.x, a.j - v.y);
	}
	public static IJ operator -(Vector3Int v, IJ a)
	{
		return new IJ(v.x - a.i, v.y - a.j);
	}
	public static IJ operator -(IJ a, Vector3Int v)
	{
		return new IJ(a.i - v.x, a.j - v.y);
	}
	#endregion

	#region Multiplication
	public static IJ operator *(IJ a, IJ b)
	{
		return new IJ(a.i * b.i, a.j * b.j);
	}
	public static IJ operator *(int c, IJ a)
	{
		return new IJ(c * a.i, c * a.j);
	}
	public static IJ operator *(IJ a, int c)
	{
		return new IJ(a.i * c, a.j * c);
	}
	public static IJ operator *(Vector2Int v, IJ a)
	{
		return new IJ(v.x * a.i, v.y * a.j);
	}
	public static IJ operator *(IJ a, Vector2Int v)
	{
		return new IJ(a.i * v.x, a.j * v.y);
	}
	public static IJ operator *(Vector3Int v, IJ a)
	{
		return new IJ(v.x * a.i, v.y * a.j);
	}
	public static IJ operator *(IJ a, Vector3Int v)
	{
		return new IJ(a.i * v.x, a.j * v.y);
	}

	public static Vector2 operator *(float c, IJ a)
	{
		return new Vector2(c * a.i, c * a.j);
	}
	public static Vector2 operator *(IJ a, float c)
	{
		return new Vector2(c * a.i, c * a.j);
	}
	#endregion

	#region Conversions
	public static explicit operator IJ(IJL value)
	{
		return new IJ(value.i, value.j);
	}
	public static explicit operator IJ(Vector2Int value)
	{
		return new IJ(value.x, value.y);
	}
	public static explicit operator IJ(Vector3Int value)
	{
		return new IJ(value.x, value.y);
	}
	public static explicit operator Vector2Int(IJ a)
	{
		return new Vector2Int(a.i, a.j);
	}
	public static explicit operator Vector3Int(IJ a)
	{
		return new Vector3Int(a.i, a.j, 0);
	}
	public static explicit operator Vector2(IJ a)
	{
		return new Vector2(a.i, a.j);
	}
	public static explicit operator Vector3(IJ a)
	{
		return new Vector3(a.i, a.j, 0);
	}
	#endregion

	#region Hash Code
	public override int GetHashCode()
	{
		return ((i % 30000) * 30000) + (j % 30000);
	}
	#endregion

	#region String
	public override string ToString()
	{
		return "WildIJ(" + i.ToString() + "," + j.ToString() + ")";
	}
	#endregion

	#region Properties
	public static IJ zero {
		get {
			return new IJ(0, 0);
		}
	}
	public static IJ one {
		get {
			return new IJ(1, 1);
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
	public IJL ijl {
		get {
			return new IJL(i, j, 0);
		}
	}
	#endregion
}