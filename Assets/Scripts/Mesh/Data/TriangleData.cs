using UnityEngine;

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