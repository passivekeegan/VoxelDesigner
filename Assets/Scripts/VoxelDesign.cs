using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoxelDesign
{
	[SerializeField]
	private List<Point> _vertices;
	[SerializeField]
	private List<Triangle> _triangles;
	[SerializeField]
	private List<int> _shared;

	public VoxelDesign()
	{
		_vertices = new List<Point>();
		_triangles = new List<Triangle>();
		_shared = new List<int>();
	}

	public List<Point> vertices {
		get {
			return _vertices;
		}
	}

	public List<Triangle> triangles {
		get {
			return _triangles;
		}
	}

	public void AddVertex(Point point)
	{
		_vertices.Add(point);
		int vertex_index = _vertices.Count - 1;
		if (point.shared) {
			_shared.Add(vertex_index);
		}
	}

	public void UpdateVertex(int index, Point point)
	{
		if (index < 0 || index >= _vertices.Count) {
			return;
		}
		Point oldpoint = _vertices[index];
		_vertices[index] = point;
		//shared
		if (!oldpoint.shared && point.shared) {
			_shared.Add(index);
		}
		else if (oldpoint.shared && !point.shared) {
			_shared.Remove(index);
		}
	}

	public void DeleteVertex(int index)
	{
		if (index < 0 || index >= _vertices.Count) {
			return;
		}
		Point point = _vertices[index];
		_vertices.RemoveAt(index);
		//update axi lists
		if (point.shared) {
			_shared.Remove(index);
		}
		//update shared list
		for (int k = 0;k < _shared.Count;k++) {
			if (_shared[k] > index) {
				_shared[k] -= 1;
			}
		}
		//update triangles
		for (int k = 0;k < _triangles.Count;k++) {
			Triangle tri = _triangles[k];
			//vertex 0
			int vertex0 = tri.vertex0;
			if (vertex0 == index) {
				vertex0 = -1;
			}
			else if (vertex0 > index) {
				vertex0 -= 1;
			}
			//vertex 1
			int vertex1 = tri.vertex1;
			if (vertex1 == index) {
				vertex1 = -1;
			}
			else if (vertex1 > index) {
				vertex1 -= 1;
			}
			//vertex 2
			int vertex2 = tri.vertex2;
			if (vertex2 == index) {
				vertex2 = -1;
			}
			else if (vertex2 > index) {
				vertex2 -= 1;
			}
			//update triangle
			_triangles[k] = new Triangle(vertex0, vertex1, vertex2);
		}
	}

	public void AddTriangle(Triangle triangle)
	{
		if (_triangles.Contains(triangle)) {
			return;
		}
		_triangles.Add(triangle);
	}

	public void UpdateTriangle(int index, Triangle triangle)
	{
		if (index < 0 || index >= _triangles.Count) {
			return;
		}
		if (_triangles.Contains(triangle) && triangle != _triangles[index]) {
			return;
		}
		_triangles[index] = triangle;
	}

	public void DeleteTriangle(int index)
	{
		if (index < 0 || index >= _triangles.Count) {
			return;
		}
		_triangles.RemoveAt(index);
	}

	public bool isValid {
		get {
			return (_vertices != null) && (_triangles != null) && (_shared != null);
		}
	}
}
