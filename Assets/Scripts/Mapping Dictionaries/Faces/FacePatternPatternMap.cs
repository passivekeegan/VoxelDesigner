using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FacePatternPatternMap : Dictionary<FacePattern, FacePattern>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<FacePattern> _keys = new List<FacePattern>();
	[SerializeField]
	private List<FacePattern> _values = new List<FacePattern>();


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
		foreach (KeyValuePair<FacePattern, FacePattern> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}
