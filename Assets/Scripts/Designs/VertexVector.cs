using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct designed to store a sequence of operations that can be executed to 
/// produce a vector that when added to a reference point produces a mesh vertex.
/// </summary>
[System.Serializable]
public struct VertexVector
{
	public List<VectorOperation> operations;

	/// <summary>
	/// Produces a vector by iterating and executing the vector operations 
	/// sequentially.
	/// </summary>
	/// <param name="shift">
	/// An integer in the range of [0, 5] that represents a shift 
	/// around 6 directions horizontally.
	/// </param>
	/// <param name="args">
	/// A reference to a struct contained some of the special variables 
	/// potentially required for the operations.
	/// </param>
	/// <returns>
	/// A Vector3 struct representing a vector to be added to a reference point.
	/// </returns>
	public Vector3 GenerateVertexVector(int shift, ref VertexArgs args)
	{
		Vector3 point = Vector3.zero;
		if (operations != null && shift >= 0 && shift < 6) {
			for (int index = 0;index < operations.Count;index++) {
				float value;
				if (operations[index].use_custom) {
					value = operations[index].custom_modifier;
				}
				else {
					value = operations[index].modifier.Value(ref args);
				}
				point += value * operations[index].vector.Vector(shift);
			}
		}
		return point;
	}

	/// <summary>
	/// Returns an empty VertexVector with 0 operations.
	/// </summary>
	public static VertexVector empty {
		get {
			return new VertexVector() { operations = new List<VectorOperation>() };
		}
	}
}

/// <summary>
/// A struct designed to hold important values needed to construct 
/// vertices.
/// </summary>
public struct VertexArgs
{
	public float bevel;
	public float space;
}
