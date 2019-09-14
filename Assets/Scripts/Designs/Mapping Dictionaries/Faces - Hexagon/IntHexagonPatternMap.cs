using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntHexagonPatternMap : Dictionary<int, List<HexagonFacePattern>>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<HexagonFacePattern> _values = new List<HexagonFacePattern>();


	public void OnAfterDeserialize()
	{
		Clear();
		for (int k = 0; k < _keys.Count; k++) {
			if (!this.ContainsKey(_keys[k])) {
				this.Add(_keys[k], new List<HexagonFacePattern>());
			}
			if (_values[k].IsValid() && !_values[k].IsEmpty()) {
				this[_keys[k]].Add(_values[k]);
			}
		}
	}

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();
		foreach (KeyValuePair<int, List<HexagonFacePattern>> kv in this) {
			if (kv.Value.Count > 0) {
				foreach (HexagonFacePattern pat in kv.Value) {
					_keys.Add(kv.Key);
					_values.Add(pat);
				}
			}
			else {
				_keys.Add(kv.Key);
				_values.Add(HexagonFacePattern.empty);
			}
		}
	}
}
