using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CornerMap : ComponentMap<CornerDesign>
{
	[SerializeField]
	private List<int> _pids;
	[SerializeField]
	private List<CornerPattern> _patterns;

	//play mode structures
	private Dictionary<int, List<CornerPattern>> _idpatterns;
	private Dictionary<CornerPattern, int> _patternid;
	private Dictionary<CornerPattern, CornerPattern> _patternpattern;

	public CornerMap()
	{
		_ids = new List<int>();
		_components = new List<CornerDesign>();
		_pids = new List<int>();
		_patterns = new List<CornerPattern>();
	}

	public override void Start()
	{
		_compid = new Dictionary<CornerDesign, int>();
		_idcomp = new Dictionary<int, CornerDesign>();
		_idpatterns = new Dictionary<int, List<CornerPattern>>();
		_patternid = new Dictionary<CornerPattern, int>();
		_patternpattern = new Dictionary<CornerPattern, CornerPattern>();
		for (int k = 0;k < _ids.Count;k++) {
			int id = _ids[k];
			CornerDesign comp = _components[k];
			if (comp == null || id < 1) {
				continue;
			}
			_compid.Add(comp, id);
			_idcomp.Add(id, comp);
			_idpatterns.Add(id, new List<CornerPattern>());
		}
		for (int k = 0;k < _pids.Count;k++) {
			int id = _pids[k];
			CornerPattern pattern = _patterns[k];
			if (!pattern.IsValid() || pattern.IsEmpty()) {
				continue;
			}
			if (_patternid.ContainsKey(pattern) || !_idpatterns.ContainsKey(id)) {
				continue;
			}
			_patternid.Add(pattern, id);
			_patternpattern.Add(pattern, pattern);
			_idpatterns[id].Add(pattern);
		}
	}

	public void AddPattern(CornerDesign component, CornerPattern pattern)
	{
		if (component == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			if (!_compid.ContainsKey(component) || _patternid.ContainsKey(pattern)) {
				return;
			}
			int id = _compid[component];
			if (id < 1) {
				return;
			}
			_patternid.Add(pattern, id);
			_idpatterns[id].Add(pattern);
			_patternpattern.Add(pattern, pattern);
		
		#if UNITY_EDITOR
		}
		else {
			int comp_index = _components.IndexOf(component);
			if (comp_index < 0) {
				return;
			}
			if (_patterns.Contains(pattern)) {
				return;
			}
			_pids.Add(_ids[comp_index]);
			_patterns.Add(pattern);
		}
		#endif
	}
	public void DeletePattern(CornerDesign component, CornerPattern pattern)
	{
		if (component == null || !pattern.IsValid() || pattern.IsEmpty()) {
			return;
		}
		#if UNITY_EDITOR
		if (Application.isPlaying) {
		#endif
			if (!_compid.ContainsKey(component) || !_patternid.ContainsKey(pattern)) {
				return;
			}
			_patternid.Remove(pattern);
			_idpatterns[_compid[component]].Remove(pattern);
			_patternpattern.Remove(pattern);
		#if UNITY_EDITOR
		}
		else {
			int comp_index = _components.IndexOf(component);
			if (comp_index < 0) {
				return;
			}
			int id = _ids[comp_index];
			int pat_index = _patterns.IndexOf(pattern);
			if (pat_index < 0) {
				return;
			}
			if (_pids[pat_index] != id) {
				return;
			}
			_pids.RemoveAt(pat_index);
			_patterns.RemoveAt(pat_index);
		}
		#endif
	}
	public bool PatternExists(CornerPattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return false;
		}
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			return _patternid.ContainsKey(pattern);
#if UNITY_EDITOR
		}
		else {
			return _patterns.Contains(pattern);
		}
#endif
	}
	public int GetComponentID(CornerPattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return 0;
		}
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			if (!_patternid.ContainsKey(pattern)) {
				return 0;
			}
			return _patternid[pattern];
#if UNITY_EDITOR
		}
		else {
			int index = _patterns.IndexOf(pattern);
			if (index < 0) {
				return 0;
			}
			return _pids[index];
		}
#endif
	}
	public CornerDesign GetComponent(CornerPattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return null;
		}
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			if (!_patternid.ContainsKey(pattern)) {
				return null;
			}
			int id = _patternid[pattern];
			if (!_idcomp.ContainsKey(id)) {
				return null;
			}
			return _idcomp[id];
#if UNITY_EDITOR
		}
		else {
			int pat_index = _patterns.IndexOf(pattern);
			if (pat_index < 0) {
				return null;
			}
			int id = _pids[pat_index];
			int comp_index = _ids.IndexOf(id);
			if (comp_index < 0) {
				return null;
			}
			return _components[comp_index];
		}
#endif
	}
	public CornerPattern GetPattern(CornerPattern pattern)
	{
		if (!pattern.IsValid() || pattern.IsEmpty()) {
			return CornerPattern.empty;
		}
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			if (!_patternpattern.ContainsKey(pattern)) {
				return CornerPattern.empty;
			}
			return _patternpattern[pattern];
#if UNITY_EDITOR
		}
		else {
			int index = _patterns.IndexOf(pattern);
			if (index < 0) {
				return CornerPattern.empty;
			}
			return _patterns[index];
		}
#endif
	}

#if UNITY_EDITOR
	public void AddPatternsToList(CornerDesign component, List<CornerPattern> list)
	{
		if (component == null || list == null || _pids.Count != _patterns.Count) {
			return;
		}
		int index = _components.IndexOf(component);
		if (index < 0) {
			return;
		}
		int id = _ids[index];
		for (int k = 0; k < _patterns.Count; k++) {
			if (_pids[k] != id) {
				continue;
			}
			list.Add(_patterns[k]);
		}
	}
#endif
	protected override void ComponentAdded(int id)
	{
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			_idpatterns.Add(id, new List<CornerPattern>());
#if UNITY_EDITOR
		}
#endif
	}

	protected override void ComponentDeleted(int id)
	{
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			List<CornerPattern> patterns = _idpatterns[id];
			for (int k = 0; k < patterns.Count; k++) {
				_patternid.Remove(patterns[k]);
				_patternpattern.Remove(patterns[k]);
			}
			_idpatterns.Remove(id);
#if UNITY_EDITOR
		}
#endif
	}
}
