using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LateralPatternPatternMap : Dictionary<LatEdgePattern, LatEdgePattern>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<LatEdgePattern> _keys = new List<LatEdgePattern>();
	[SerializeField]
	private List<LatEdgePattern> _values = new List<LatEdgePattern>();


	public void OnAfterDeserialize()
	{
		Clear();
		for (int k = 0; k < _keys.Count; k++) {
			this.Add(_keys[k], _values[k]);
		}
	}

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();
		foreach (KeyValuePair<LatEdgePattern, LatEdgePattern> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}
