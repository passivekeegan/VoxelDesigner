using UnityEngine;

public struct ComponentData
{
	public readonly bool draw;
	public readonly int id;
	public readonly Vector3 vertex;
	public readonly PatternMatch match;

	public ComponentData(bool draw, int id, Vector3 vertex, PatternMatch match)
	{
		this.draw = draw;
		this.id = id;
		this.vertex = vertex;
		this.match = new PatternMatch(match.invx, match.invy, Mathf.Clamp(match.shift, 0, 5));
	}

	public static ComponentData empty {
		get {
			return new ComponentData(false, 0, Vector3.zero, PatternMatch.empty);
		}
	}
}
