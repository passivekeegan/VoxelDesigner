using UnityEngine;

/// <summary>
/// A struct that represents an an operation that forms part 
/// of a vector. It was designed to be horizontally directional 
/// invariant and can have its reference vector changed easily.
/// </summary>
[System.Serializable]
public struct VectorOperation
{
	public float scale;
	public VType vector_type;
	public Vector3 vector;

	public VectorOperation(float scale, VType vector_type, Vector3 vector)
	{
		this.scale = scale;
		this.vector_type = vector_type;
		this.vector = vector;
	}

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

	public Vector3 CalculateVector(int d)
	{
		switch (vector_type) {
			case VType.Custom:
				return scale * (Vx.AxiRot[Vx.D[1, d]] * vector);
			case VType.Up:
				return scale * Vector3.up;
			case VType.Down:
				return scale * Vector3.down;
			case VType.D0:
				return scale * Vx.HexagonUnit[2 * d];
			case VType.D1:
				return scale * Vx.HexagonUnit[2 * Vx.D[1, d]];
			case VType.D2:
				return scale * Vx.HexagonUnit[2 * Vx.D[2, d]];
			case VType.D3:
				return scale * Vx.HexagonUnit[2 * Vx.D[3, d]];
			case VType.D4:
				return scale * Vx.HexagonUnit[2 * Vx.D[4, d]];
			case VType.D5:
				return scale * Vx.HexagonUnit[2 * Vx.D[5, d]];
			case VType.M0:
				return scale * Vx.HexagonUnit[(2 * d) + 1];
			case VType.M1:
				return scale * Vx.HexagonUnit[(2 * Vx.D[1, d]) + 1];
			case VType.M2:
				return scale * Vx.HexagonUnit[(2 * Vx.D[2, d]) + 1];
			case VType.M3:
				return scale * Vx.HexagonUnit[(2 * Vx.D[3, d]) + 1];
			case VType.M4:
				return scale * Vx.HexagonUnit[(2 * Vx.D[4, d]) + 1];
			case VType.M5:
				return scale * Vx.HexagonUnit[(2 * Vx.D[5, d]) + 1];
			default:
				return Vector3.zero;
		}
	}

	/// <summary>
	/// Returns an empty VectorOperation.
	/// </summary>
	public static VectorOperation empty {
		get {
			return new VectorOperation(1f, default(VType), Vector3.zero);
		}
	}
}

public enum VType
{
	Custom,
	Up,
	Down,
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

