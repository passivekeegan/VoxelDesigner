[System.Serializable]
public struct Voxel : System.IEquatable<Voxel>
{
	public readonly uint id;
	public readonly VoxelAxi axi;

	public Voxel(uint id)
	{
		this.id = id;
		this.axi = VoxelAxi.AD0;
	}

	public Voxel(uint id, VoxelAxi axi)
	{
		this.id = id;
		this.axi = axi;
	}

		
	#region Comparison
	public override bool Equals(object obj)
	{
		if (!(obj is Voxel)) {
			return false;
		}
		return this.Equals((Voxel)obj);
	}
	public bool Equals(Voxel other)
	{
		return (id == other.id);
	}
	public static bool operator ==(Voxel a, Voxel b)
	{

		return (a.id == b.id);
	}
	public static bool operator !=(Voxel a, Voxel b)
	{
		return (a.id != b.id);
	}
	#endregion

	#region HashCode
	public override int GetHashCode()
	{
		return (int) id;
	}
	#endregion

	public bool IsEmpty()
	{
		return (id <= 0) || (axi == VoxelAxi.None);
	}

	public static Voxel GetEmpty()
	{
		return new Voxel(0, VoxelAxi.None);
	}
	
}
