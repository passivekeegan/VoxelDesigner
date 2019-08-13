using UnityEngine;

/// <summary>
/// A struct that represents an an operation that forms part 
/// of a vector. It was designed to be horizontally directional 
/// invariant and can have its reference vector changed easily.
/// </summary>
[System.Serializable]
public struct VectorOperation
{
	public bool use_custom;
	public float custom_modifier;
	public ModifierType modifier;
	public VectorType vector;

	/// <summary>
	/// Create and initialize a VectorOperation using a custom 
	/// modifier.
	/// </summary>
	/// <param name="modifier">
	/// A float variable that gets multiplied to the vectors 
	/// components.
	/// </param>
	/// <param name="vector">
	/// A VectorType enum type that specifies a directionally 
	/// shifted vector to use in the operation.
	/// </param>
	public VectorOperation(float modifier, VectorType vector)
	{
		this.use_custom = true;
		this.custom_modifier = modifier;
		this.modifier = default(ModifierType);
		if (System.Enum.IsDefined(typeof(VectorType), vector)) {
			this.vector = vector;
		}
		else {
			this.vector = default(VectorType);
		}
	}

	/// <summary>
	/// Create and initialize a VectorOperation using a special 
	/// modifier value.
	/// </summary>
	/// <param name="modifier">
	/// A ModifierType enum type that specifies a special float 
	/// value to multiply to the vectors components.
	/// </param>
	/// <param name="vector">
	/// A VectorType enum type that specifies a directionally 
	/// shifted vector to use in the operation.
	/// </param>
	public VectorOperation(ModifierType modifier, VectorType vector)
	{
		this.use_custom = false;
		this.custom_modifier = 1f;
		if (System.Enum.IsDefined(typeof(ModifierType), modifier)) {
			this.modifier = modifier;
		}
		else {
			this.modifier = default(ModifierType);
		}
		if (System.Enum.IsDefined(typeof(VectorType), vector)) {
			this.vector = vector;
		}
		else {
			this.vector = default(VectorType);
		}
	}

	/// <summary>
	/// Create and initialize a VectorOperation directly.
	/// </summary>
	/// <param name="use_custom">
	/// A boolean determining whether this operation uses the 
	/// specified float multiplier or the ModifierType multiplier
	/// for its operation.
	/// </param>
	/// <param name="custom_modifier">
	/// A float variable that gets multiplied to the vectors 
	/// components.
	/// </param>
	/// <param name="modifier">
	/// A ModifierType enum type that specifies a special float 
	/// value to multiply to the vectors components.
	/// </param>
	/// <param name="vector">
	/// A VectorType enum type that specifies a directionally 
	/// shifted vector to use in the operation.
	/// </param>
	public VectorOperation(bool use_custom, float custom_modifier, ModifierType modifier, VectorType vector)
	{
		this.use_custom = use_custom;
		this.custom_modifier = custom_modifier;
		if (System.Enum.IsDefined(typeof(ModifierType), modifier)) {
			this.modifier = modifier;
		}
		else {
			this.modifier = default(ModifierType);
		}
		if (System.Enum.IsDefined(typeof(VectorType), vector)) {
			this.vector = vector;
		}
		else {
			this.vector = default(VectorType);
		}
	}

	/// <summary>
	/// Returns an empty VectorOperation.
	/// </summary>
	public static VectorOperation empty {
		get {
			return new VectorOperation(default(ModifierType), default(VectorType));
		}
	}
}

/// <summary>
/// An enum representing vectors that can be either shifted 
/// easily or don't need to be shifted. Up and Down vectors
/// point directly up and down, D vectors point in the 
/// direction of hexagon corners each 60 degrees apart and
/// M vectors point between D vectors towards a hexagons 
/// outer faces. All are unit vectors.
/// </summary>
public enum VectorType
{
	UP,
	DOWN,
	D0,
	D1,
	D2,
	D3,
	D4,
	D5,
	M0,
	M1,
	M2,
	M3,
	M4,
	M5
}

/// <summary>
/// A static class containing functional extensions 
/// to the VectorType enum.
/// </summary>
public static class VectorTypeExt
{
	/// <summary>
	/// Gets the vector that the provided VectorType specifies and 
	/// shifts its direction.
	/// </summary>
	/// <param name="type">
	/// A VectorType enum type that specifies a directionally 
	/// shifted vector.
	/// </param>
	/// <param name="shift">
	/// An integer in the range of [0, 5] that represents a shift 
	/// around 6 directions horizontally.
	/// </param>
	/// <returns>
	/// Returns a Vector3 struct representing the final vector after 
	/// being shifted.
	/// </returns>
	public static Vector3 Vector(this VectorType type, int shift)
	{
		if (shift < 0 || shift > 5) {
			return Vector3.zero;
		}
		switch (type) {
			case VectorType.UP:
				return Vx.YAxis;
			case VectorType.DOWN:
				return -Vx.YAxis;
			case VectorType.D0:
				return Vx.Hexagon[shift];
			case VectorType.D1:
				return Vx.Hexagon[Vx.D[1, shift]];
			case VectorType.D2:
				return Vx.Hexagon[Vx.D[2, shift]];
			case VectorType.D3:
				return Vx.Hexagon[Vx.D[3, shift]];
			case VectorType.D4:
				return Vx.Hexagon[Vx.D[4, shift]];
			case VectorType.D5:
				return Vx.Hexagon[Vx.D[5, shift]];
			case VectorType.M0:
				return Vx.MidHexagon[shift];
			case VectorType.M1:
				return Vx.MidHexagon[Vx.D[1, shift]];
			case VectorType.M2:
				return Vx.MidHexagon[Vx.D[2, shift]];
			case VectorType.M3:
				return Vx.MidHexagon[Vx.D[3, shift]];
			case VectorType.M4:
				return Vx.MidHexagon[Vx.D[4, shift]];
			case VectorType.M5:
				return Vx.MidHexagon[Vx.D[5, shift]];
			default:
				return Vector3.zero;
		}
	}
}

/// <summary>
/// An enum representing special or important float \
/// modifiers to be used in vector operations.
/// </summary>
public enum ModifierType
{
	One,
	N_One,
	Bevel,
	N_Bevel,
	Space,
	N_Space,
	SpHyp,
	N_SpHyp,
	SpAdj,
	N_SpAdj,
	HBevel
}

/// <summary>
/// A static class containing functional extensions 
/// to the ModifierType enum.
/// </summary>
public static class ModifierTypeExt
{
	/// <summary>
	/// Gets the float value of the corresponding special modifier type.
	/// </summary>
	/// <param name="type">
	/// A ModifierType enum type that specifies a special float 
	/// value that corresponds to it.
	/// </param>
	/// <param name="args">
	/// A reference to a struct contained some of the special variables 
	/// potentially required.
	/// </param>
	/// <returns>
	/// Returns the float value. Returns 1 if the modifier type is not 
	/// recognized.
	/// </returns>
	public static float Value(this ModifierType type, ref VertexArgs args)
	{
		switch (type) {
			case ModifierType.Bevel:
				return args.bevel;
			case ModifierType.N_Bevel:
				return -args.bevel;
			case ModifierType.Space:
				return args.space;
			case ModifierType.N_Space:
				return -args.space;
			case ModifierType.SpHyp:
				return args.space / Vx.SQRT3;
			case ModifierType.N_SpHyp:
				return -args.space / Vx.SQRT3;
			case ModifierType.SpAdj:
				return args.space / (2 * Vx.SQRT3);
			case ModifierType.N_SpAdj:
				return -args.space / (2 * Vx.SQRT3);
			case ModifierType.HBevel:
				return args.bevel / 2f;
			default:
				return 1f;
		}
	}
}