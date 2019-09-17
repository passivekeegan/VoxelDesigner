using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ComponentMap<T> where T: VoxelComponent
{
	public const int CAPACITY_LIMIT = 10000;

	[SerializeField]
	protected List<int> _ids;
	[SerializeField]
	protected List<T> _components;

	//play mode structures
	protected Dictionary<T, int> _compid;
	protected Dictionary<int, T> _idcomp;

	public void AddComponent(T component)
	{
		if (component == null) {
			return;
		}
		int id;
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			if (_compid.ContainsKey(component)) {
				return;
			}
			id = GenerateUniqueComponentID();
			if (id < 1) {
				return;
			}
			_compid.Add(component, id);
			_idcomp.Add(id, component);
		#if UNITY_EDITOR
		}
		else {
			int index = _components.IndexOf(component);
			if (index >= 0) {
				return;
			}
			id = GenerateUniqueComponentID();
			if (id < 1) {
				return;
			}
			_ids.Add(id);
			_components.Add(component);
		}
		#endif
		//component was added callback
		ComponentAdded(id);
	}
	public void ReplaceComponent(T oldcomp, T newcomp)
	{
		if (oldcomp == null || newcomp == null || oldcomp == newcomp) {
			return;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			if (!_compid.ContainsKey(oldcomp)) {
				return;
			}
			int oldid = _compid[oldcomp];
			if (_compid.ContainsKey(newcomp)) {
				int newid = _compid[newcomp];
				_compid[oldcomp] = newid;
				_compid[newcomp] = oldid;
				_idcomp[oldid] = newcomp;
				_idcomp[newid] = oldcomp;
			}
			else {
				_compid.Remove(oldcomp);
				_compid.Add(newcomp, oldid);
				_idcomp[oldid] = newcomp;
			}
		#if UNITY_EDITOR
		}
		else {
			//check if old component exists
			int old_index = _components.IndexOf(oldcomp);
			if (old_index < 0) {
				return;
			}
			//check if new component exists
			int new_index = _components.IndexOf(newcomp);
			if (new_index >= 0) {
				_components[old_index] = newcomp;
				_components[new_index] = oldcomp;
			}
			else {
				_components[old_index] = newcomp;
			}
		}
		#endif
	}
	public void DeleteComponent(T component)
	{
		if (component == null) {
			return;
		}
		int id;
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			if (!_compid.ContainsKey(component)) {
				return;
			}
			id = _compid[component];
			_compid.Remove(component);
			_idcomp.Remove(id);
		#if UNITY_EDITOR
		}
		else {
			int index = _components.IndexOf(component);
			if (index < 0) {
				return;
			}
			id = _ids[index];
			_ids.RemoveAt(index);
			_components.RemoveAt(index);
		}
		#endif
		//call deleted component calllback
		ComponentDeleted(id);
	}

	public bool ComponentExists(T component)
	{
		if (component == null) {
			return false;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			return _compid.ContainsKey(component);
		#if UNITY_EDITOR
		}
		else {
			return _components.Contains(component);
		}
		#endif
		
	}

	public T GetComponent(int id)
	{
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			if (id <= 0 || !_idcomp.ContainsKey(id)) {
				return null;
			}
			return _idcomp[id];
		#if UNITY_EDITOR
		}
		else {
			if (id <= 0) {
				return null;
			}
			int index = _ids.IndexOf(id);
			if (index < 0) {
				return null;
			}
			return _components[index];
		}
		#endif
	}

	#if UNITY_EDITOR
	public void AddComponentsToList(List<VoxelComponent> list)
	{
		for (int k = 0;k < _components.Count; k++) {
			if (_components[k] == null) {
				continue;
			}
			list.Add(_components[k]);
		}
	}
	public void AddComponentsToList(List<T> list)
	{
		for (int k = 0; k < _components.Count; k++) {
			if (_components[k] == null) {
				continue;
			}
			list.Add(_components[k]);
		}
	}
	#endif

	private int GenerateUniqueComponentID()
	{
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			for (int id = 1; id <= CAPACITY_LIMIT; id++) {
				if (!_idcomp.ContainsKey(id)) {
					return id;
				}
			}
		#if UNITY_EDITOR
		}
		else {
			for (int id = 1; id <= CAPACITY_LIMIT; id++) {
				if (!_ids.Contains(id)) {
					return id;
				}
			}
		}
		#endif
		return -1;
	}

	protected virtual void ComponentAdded(int id) { }
	protected virtual void ComponentDeleted(int id) { }
	public abstract void Start();
}
