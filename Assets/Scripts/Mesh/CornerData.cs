using UnityEngine;

public struct CornerData
{
	public readonly bool draw;
	public readonly int id;
	public readonly int shift;
	public readonly Vector3 vertex;

	public CornerData(bool draw, int id, int shift, Vector3 vertex)
	{
		this.draw = draw;
		this.id = id;
		this.shift = Mathf.Clamp(shift, 0, 5);
		this.vertex = vertex;
	}
}