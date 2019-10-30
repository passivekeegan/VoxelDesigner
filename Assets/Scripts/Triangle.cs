using System;
using UnityEngine;

[Serializable]
public struct Triangle : IEquatable<Triangle>
{
	public int vertex0;
	public int vertex1;
	public int vertex2;

	public Triangle(int vertex0, int vertex1, int vertex2)
	{
		this.vertex0 = vertex0;
		this.vertex1 = vertex1;
		this.vertex2 = vertex2;
	}

	#region Equality
	public override bool Equals(object obj)
	{
		if (!(obj is Triangle)) {
			return false;
		}
		return Equals((Triangle)obj);
	}
	public bool Equals(Triangle other)
	{
		return ((vertex0 == other.vertex0) && (vertex1 == other.vertex1) && (vertex2 == other.vertex2)) ||
			   ((vertex0 == other.vertex1) && (vertex1 == other.vertex2) && (vertex2 == other.vertex0)) ||
			   ((vertex0 == other.vertex2) && (vertex1 == other.vertex0) && (vertex2 == other.vertex1));
	}
	public static bool operator ==(Triangle a, Triangle b)
	{
		return a.Equals(b);
	}
	public static bool operator !=(Triangle a, Triangle b)
	{
		return !a.Equals(b);
	}
	#endregion

	public override int GetHashCode()
	{
		return (vertex0 + vertex1 + vertex2) % 1000;
	}

	public bool IsValid(int vertex_count)
	{
		return (vertex0 != vertex1) && (vertex0 != vertex2) && (vertex1 != vertex2) && 
			(vertex0 >= 0 && vertex0 < vertex_count) && 
			(vertex1 >= 0 && vertex1 < vertex_count) && 
			(vertex2 >= 0 && vertex2 < vertex_count);
	}

	public static Triangle Flip(Triangle triangle)
	{
		return new Triangle(triangle.vertex0, triangle.vertex2, triangle.vertex1);
	}

	public static Vector3 Normal(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2)
	{
		return Vector3.Cross(vertex1 - vertex0, vertex2 - vertex0).normalized;
	}
}
