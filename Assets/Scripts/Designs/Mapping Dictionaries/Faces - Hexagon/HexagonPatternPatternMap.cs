﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HexagonPatternPatternMap : Dictionary<HexagonFacePattern, HexagonFacePattern>, ISerializationCallbackReceiver
{
	[SerializeField]
	private List<HexagonFacePattern> _keys = new List<HexagonFacePattern>();
	[SerializeField]
	private List<HexagonFacePattern> _values = new List<HexagonFacePattern>();


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
		foreach (KeyValuePair<HexagonFacePattern, HexagonFacePattern> kv in this) {
			_keys.Add(kv.Key);
			_values.Add(kv.Value);
		}
	}
}
