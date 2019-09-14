using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectPatternIntMap : Dictionary<RectFacePattern, int>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<RectFacePattern> _keys = new List<RectFacePattern>();
	[SerializeField]
	private List<int> _values = new List<int>();


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
		foreach (KeyValuePair<RectFacePattern, int> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}
