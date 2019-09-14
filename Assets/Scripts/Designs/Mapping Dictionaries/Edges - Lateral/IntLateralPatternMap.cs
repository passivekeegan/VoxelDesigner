using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntLateralPatternMap : Dictionary<int, List<LatEdgePattern>>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<LatEdgePattern> _values = new List<LatEdgePattern>();


	public void OnAfterDeserialize()
	{
		Clear();
		for (int k = 0; k < _keys.Count; k++) {
			if (!this.ContainsKey(_keys[k])) {
				this.Add(_keys[k], new List<LatEdgePattern>());
			}
			if (!_values[k].IsEmpty()) {
				this[_keys[k]].Add(_values[k]);
			}
		}
	}

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();
		foreach (KeyValuePair<int, List<LatEdgePattern>> kv in this) {
			if (kv.Value.Count > 0) {
				foreach (LatEdgePattern pat in kv.Value) {
					_keys.Add(kv.Key);
					_values.Add(pat);
				}
			}
			else {
				_keys.Add(kv.Key);
				_values.Add(LatEdgePattern.empty);
			}
		}
	}
}
