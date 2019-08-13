using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CornerPatternPatternMap : Dictionary<CornerPattern, CornerPattern>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<CornerPattern> _keys = new List<CornerPattern>();
	[SerializeField]
	private List<CornerPattern> _values = new List<CornerPattern>();


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
		foreach (KeyValuePair<CornerPattern, CornerPattern> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}
