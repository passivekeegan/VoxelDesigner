using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct VertexData
{
	public bool hastri;
	public int index;
	public Vector3 vertex;
	public Vector3 normal;

	public VertexData(bool hastri, int index, Vector3 vertex, Vector3 normal)
	{
		this.hastri = hastri;
		this.index = index;
		this.vertex = vertex;
		this.normal = normal;
	}
}

public struct TriangleData
{
	public readonly int vertex0;
	public readonly int vertex1;
	public readonly int vertex2;

	public TriangleData(int vertex0, int vertex1, int vertex2)
	{
		this.vertex0 = vertex0;
		this.vertex1 = vertex1;
		this.vertex2 = vertex2;
	}
}
