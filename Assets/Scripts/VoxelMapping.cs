using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMapping : ScriptableObject
{
	[SerializeField]
	private int _ids;
	[SerializeField]
	private float _weld_distance;
	[SerializeField]
	private List<VoxelPattern> _patterns;
	[SerializeField]
	private List<VoxelDesign> _designs;

	public void Initialize()
	{
		_ids = 1;
		_weld_distance = 0.0001f;
		_patterns = new List<VoxelPattern>();
		_designs = new List<VoxelDesign>();

		AddPatterns(_ids, _patterns, _designs);
	}

	public bool isValid {
		get {
			return (_ids > 0) && (_patterns != null && _patterns.Count > 0) && (_designs != null && _designs.Count > 0);
		}
	}

	public float vertexWeldDistance {
		get {
			return _weld_distance;
		}
		set {
			_weld_distance = Mathf.Clamp01(value);
		}
	}

	public void GetPatternList(List<VoxelPattern> patterns)
	{
		if (patterns == null) {
			return;
		}
		for (int k = 0;k < _patterns.Count;k++) {
			patterns.Add(_patterns[k]);
		}
	}

	public VoxelDesign GetVoxelDesign(VoxelPattern pattern)
	{
		if (pattern.isEmpty) {
			return null;
		}
		int index = _patterns.IndexOf(pattern);
		if (index < 0) {
			return null;
		}
		return _designs[index];
	}

	public VoxelPattern GetVoxelPattern(VoxelPattern pattern)
	{
		if (pattern.isEmpty) {
			return new VoxelPattern();
		}
		int index = _patterns.IndexOf(pattern);
		if (index < 0) {
			return new VoxelPattern();
		}
		return _patterns[index];
	}

	public static void AddPatterns(int ids, List<VoxelPattern> patterns, List<VoxelDesign> designs)
	{
		if (ids <= 0 || patterns == null || designs == null || patterns.Count != designs.Count) {
			return;
		}
		HashSet<VoxelPattern> set = new HashSet<VoxelPattern>(patterns);
		for (int a0 = 0;a0 <= ids;a0++) {
			for (int a1 = 0; a1 <= ids; a1++) {
				for (int a2 = 0; a2 <= ids; a2++) {
					for (int b0 = 0; b0 <= ids; b0++) {
						for (int b1 = 0; b1 <= ids; b1++) {
							for (int b2 = 0; b2 <= ids; b2++) {
								VoxelPattern pattern = new VoxelPattern(a0, a1, a2, b0, b1, b2);
								if (pattern.isEmpty || !set.Add(pattern)) {
									continue;
								}
								patterns.Add(pattern);
								designs.Add(new VoxelDesign());
							}
						}
					}
				}
			}
		}
	}

	public static void DeleteIds(int deleteid, List<VoxelPattern> patterns, List<VoxelDesign> designs)
	{
		if (deleteid <= 1 || patterns == null || designs == null || patterns.Count != designs.Count) {
			return;
		}
		int index = 0;
		while (index < patterns.Count) {
			bool deletepattern = (patterns[index].a0 >= deleteid);
			deletepattern = deletepattern || (patterns[index].a1 >= deleteid);
			deletepattern = deletepattern || (patterns[index].a2 >= deleteid);
			deletepattern = deletepattern || (patterns[index].b0 >= deleteid);
			deletepattern = deletepattern || (patterns[index].b1 >= deleteid);
			deletepattern = deletepattern || (patterns[index].b2 >= deleteid);
			if (deletepattern) {
				patterns.RemoveAt(index);
				designs.RemoveAt(index);
			}
			else {
				index += 1;
			}
		}
	}
}
