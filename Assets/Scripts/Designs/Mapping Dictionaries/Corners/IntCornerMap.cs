using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntCornerMap : Dictionary<int, CornerDesign>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<CornerDesign> _values = new List<CornerDesign>();


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
		foreach (KeyValuePair<int, CornerDesign> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}