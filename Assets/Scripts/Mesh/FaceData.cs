using UnityEngine;

public struct FaceData
{
	public readonly bool draw;
	public readonly int id;
	public readonly int axi;
	public readonly int shift;
	public readonly Vector3 vertex;

	public FaceData(bool draw, int id, int axi, int shift, Vector3 vertex)
	{
		this.draw = draw;
		this.id = id;
		this.axi = Mathf.Clamp(axi, 0, 8);
		this.shift = Mathf.Clamp(shift, 0, 5);
		this.vertex = vertex;
	}

	public bool isHexagon {
		get {
			return (axi == 0 || axi == 1);
		}
	}
}
