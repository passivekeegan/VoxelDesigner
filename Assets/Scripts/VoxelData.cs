using System;

[Serializable]
public struct VoxelData
{
	public bool draw;
	public int id;

	public VoxelData(bool draw, int id)
	{
		this.draw = draw;
		this.id = id;
	}
}
