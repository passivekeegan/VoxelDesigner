using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A struct that stores geometry information of a mesh triangle 
/// in either encoded or decoded form.
/// </summary>
[System.Serializable]
public struct Triangle
{
	public TriIndexType type0;
	public TriIndexType type1;
	public TriIndexType type2;
	public ushort vertex0;
	public ushort vertex1;
	public ushort vertex2;


	/// <summary>
	/// Create a Triangle object where all the vertex index types are 
	/// refering to this objects vertices.
	/// </summary>
	/// <param name="vertex0">
	/// A ushort representing a vertex index.
	/// </param>
	/// <param name="vertex1">
	/// A ushort representing a vertex index.
	/// </param>
	/// <param name="vertex2">
	/// A ushort representing a vertex index.
	/// </param>
	public Triangle(ushort vertex0, ushort vertex1, ushort vertex2)
	{
		this.type0 = TriIndexType.Vertex;
		this.type1 = TriIndexType.Vertex;
		this.type2 = TriIndexType.Vertex;
		this.vertex0 = vertex0;
		this.vertex1 = vertex1;
		this.vertex2 = vertex2;
	}

	/// <summary>
	/// Create a Triangle object from 3 vertex indices of variable type.
	/// </summary>
	/// <param name="index0">
	/// A TriIndex struct representing 1 of 3 vertex index types and its value.
	/// </param>
	/// <param name="index1">
	/// A TriIndex struct representing 1 of 3 vertex index types and its value.
	/// </param>
	/// <param name="index2">
	/// A TriIndex struct representing 1 of 3 vertex index types and its value.
	/// </param>
	public Triangle(TriIndex index0, TriIndex index1, TriIndex index2)
	{
		this.type0 = index0.type;
		this.type1 = index1.type;
		this.type2 = index2.type;
		this.vertex0 = 0;
		this.vertex1 = 0;
		this.vertex2 = 0;
		this.vertex0 = TriIndex.EncodeIndex(index0);
		this.vertex1 = TriIndex.EncodeIndex(index1);
		this.vertex2 = TriIndex.EncodeIndex(index2);
	}

	/// <summary>
	/// Determines if the vertex indices of the triangle are valid and inbounds.
	/// </summary>
	/// <param name="vertex_count">
	/// An integer representing the number of vertices of an object.
	/// </param>
	/// <param name="cornerplugs">
	/// A list of corner plugs from an object. Each element corresponds to an axi 
	/// and the value at that element represents the number of vertices of a 
	/// matching socket.
	/// </param>
	/// <param name="edgeplugs">
	/// A list of edge plugs from an object. Each element corresponds to an axi 
	/// and the value at that element represents the number of vertices of a 
	/// matching socket.
	/// </param>
	/// <returns>
	/// A boolean value where true represents that the triangle is properly formated
	/// and is in bounds of the given parameters.
	/// </returns>
	public bool IsValid(int vertex_count, List<int> cornerplugs, List<int> edgeplugs)
	{
		//vertex 0 validity check
		bool valid0 = (type0 == TriIndexType.Vertex && vertex0 < vertex_count);
		int axis_index = (int)TriIndex.DecodeAxiIndex(vertex0);
		int vertex_index = TriIndex.DecodeIndex(vertex0);
		valid0 = valid0 || (type0 == TriIndexType.CornerPlug && cornerplugs != null && axis_index < cornerplugs.Count && vertex_index < cornerplugs[axis_index]);
		valid0 = valid0 || (type0 == TriIndexType.EdgePlug && edgeplugs != null && axis_index < edgeplugs.Count && vertex_index < edgeplugs[axis_index]);
		//vertex 1 validity check
		bool valid1 = (type1 == TriIndexType.Vertex && vertex1 < vertex_count);
		axis_index = (int)TriIndex.DecodeAxiIndex(vertex1);
		vertex_index = TriIndex.DecodeIndex(vertex1);
		valid1 = valid1 || (type1 == TriIndexType.CornerPlug && cornerplugs != null && axis_index < cornerplugs.Count && vertex_index < cornerplugs[axis_index]);
		valid1 = valid1 || (type1 == TriIndexType.EdgePlug && edgeplugs != null && axis_index < edgeplugs.Count && vertex_index < edgeplugs[axis_index]);
		//vertex 2 validity check
		bool valid2 = (type2 == TriIndexType.Vertex && vertex2 < vertex_count);
		axis_index = (int)TriIndex.DecodeAxiIndex(vertex2);
		vertex_index = TriIndex.DecodeIndex(vertex2);
		valid2 = valid2 || (type2 == TriIndexType.CornerPlug && cornerplugs != null && axis_index < cornerplugs.Count && vertex_index < cornerplugs[axis_index]);
		valid2 = valid2 || (type2 == TriIndexType.EdgePlug && edgeplugs != null && axis_index < edgeplugs.Count && vertex_index < edgeplugs[axis_index]);
		//return validity results
		return valid0 && valid1 && valid2;
	}

	/// <summary>
	/// Returns an blank and empty Triangle struct.
	/// </summary>
	public static Triangle empty {
		get {
			return new Triangle(0, 0, 0);
		}
	}
}
