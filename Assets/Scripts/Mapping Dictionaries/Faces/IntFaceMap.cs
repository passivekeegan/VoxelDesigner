using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntFaceMap : Dictionary<int, FaceDesign>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<int> _keys = new List<int>();
	[SerializeField]
	private List<FaceDesign> _values = new List<FaceDesign>();


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
		foreach (KeyValuePair<int, FaceDesign> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}