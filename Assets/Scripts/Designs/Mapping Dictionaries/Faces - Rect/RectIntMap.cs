using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RectIntMap : Dictionary<RectFaceDesign, int>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<RectFaceDesign> _keys = new List<RectFaceDesign>();
	[SerializeField]
	private List<int> _values = new List<int>();


	public void OnAfterDeserialize()
	{
		Clear();
		for (int k = 0; k < _keys.Count; k++) {
			if (_keys[k] == null) {
				continue;
			}
			this.Add(_keys[k], _values[k]);
		}
	}

	public void OnBeforeSerialize()
	{
		_keys.Clear();
		_values.Clear();
		foreach (KeyValuePair<RectFaceDesign, int> kv in this) {
			if (kv.Key == null) {
				continue;
			}
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}