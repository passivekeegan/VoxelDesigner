using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntLateralMap : Dictionary<int, LatEdgeDesign>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<LatEdgeDesign> _values = new List<LatEdgeDesign>();


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
		foreach (KeyValuePair<int, LatEdgeDesign> kv in this) {
			if (kv.Value == null) {
				continue;
			}
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}