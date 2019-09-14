using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LongitudePatternPatternMap : Dictionary<LongEdgePattern, LongEdgePattern>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<LongEdgePattern> _keys = new List<LongEdgePattern>();
	[SerializeField]
	private List<LongEdgePattern> _values = new List<LongEdgePattern>();


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
		foreach (KeyValuePair<LongEdgePattern, LongEdgePattern> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}
