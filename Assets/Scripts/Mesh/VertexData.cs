using UnityEngine;

public struct VertexData
{
	public readonly bool hastri;
	public readonly int index;
	public readonly Vector3 vertex;
	public readonly Vector3 normal;

	public VertexData(Vector3 vertex)
	{
		this.hastri = false;
		this.index = -1;
		this.vertex = vertex;
		this.normal = Vector3.zero;
	}

	public VertexData(bool hastri, int index, Vector3 vertex, Vector3 normal)
	{
		this.hastri = hastri;
		this.index = index;
		this.vertex = vertex;
		this.normal = normal;
	}
}