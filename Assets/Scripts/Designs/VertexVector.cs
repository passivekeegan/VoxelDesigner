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
	/// <returns>
	/// A Vector3 struct representing a vector to be added to a reference point.
	/// </returns>
	public Vector3 GenerateVertexVector(int shift)
	{
		Vector3 vector = Vector3.zero;
		if (operations != null) {
			for (int index = 0; index < operations.Count; index++) {
				vector += operations[index].CalculateVector(shift);
			}
		}
		return vector;
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
