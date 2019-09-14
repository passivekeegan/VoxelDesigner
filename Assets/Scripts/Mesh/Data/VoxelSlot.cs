public struct VoxelSlot
{
	public readonly bool draw;
	public readonly Voxel voxel;

	public VoxelSlot(bool draw, Voxel voxel)
	{
		this.draw = draw;
		this.voxel = voxel;
	}
}