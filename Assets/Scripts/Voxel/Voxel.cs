[System.Serializable]
public struct Voxel : System.IEquatable<Voxel>
{
	public readonly uint id;

	public Voxel(uint id)
	{
		this.id = id;
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
		return (id <= 0);
	}

	public static Voxel GetEmpty()
	{
		return new Voxel(0);
	}
	
}
