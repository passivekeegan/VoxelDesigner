using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntFacePatternMap : Dictionary<int, List<FacePattern>>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<FacePattern> _values = new List<FacePattern>();


	public void OnAfterDeserialize()
	{
		Clear();
		for (int k = 0; k < _keys.Count; k++) {
			if (!this.ContainsKey(_keys[k])) {
				this.Add(_keys[k], new List<FacePattern>());
			}
			if (_values[k].IsValid()) {
				this[_keys[k]].Add(_values[k]);
			}
		}
	}

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();
		foreach (KeyValuePair<int, List<FacePattern>> kv in this) {
			if (kv.Value.Count > 0) {
				foreach (FacePattern pat in kv.Value) {
					_keys.Add(kv.Key);
					_values.Add(pat);
				}
			}
			else {
				_keys.Add(kv.Key);
				_values.Add(FacePattern.emptyHexagon);
			}
		}
	}
}