using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Point
{
	public bool shared;
	public Vector3 vertex;

	public Point(bool shared, Vector3 vertex)
	{
		this.shared = shared;
		this.vertex = vertex;
	}
}
