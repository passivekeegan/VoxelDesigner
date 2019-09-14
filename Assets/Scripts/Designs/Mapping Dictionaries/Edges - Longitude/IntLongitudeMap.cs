using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntLongitudeMap : Dictionary<int, LongEdgeDesign>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<LongEdgeDesign> _values = new List<LongEdgeDesign>();


	public void OnAfterDeserialize()
	{
		Clear();
		for (int k = 0; k < _keys.Count; k++) {
			if (_values[k] == null) {
				continue;
			}
			this.Add(_keys[k], _values[k]);
		}
	}

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();
		foreach (KeyValuePair<int, LongEdgeDesign> kv in this) {
			if (kv.Value == null) {
				continue;
			}
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}