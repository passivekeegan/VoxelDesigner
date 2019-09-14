using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectPatternPatternMap : Dictionary<RectFacePattern, RectFacePattern>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<RectFacePattern> _keys = new List<RectFacePattern>();
	[SerializeField]
	private List<RectFacePattern> _values = new List<RectFacePattern>();


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
		foreach (KeyValuePair<RectFacePattern, RectFacePattern> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}
