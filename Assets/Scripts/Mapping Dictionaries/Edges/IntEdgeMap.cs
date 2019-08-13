using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntEdgeMap : Dictionary<int, EdgeDesign>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<EdgeDesign> _values = new List<EdgeDesign>();


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
		foreach (KeyValuePair<int, EdgeDesign> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}