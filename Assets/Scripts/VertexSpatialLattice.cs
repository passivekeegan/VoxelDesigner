using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityLattice
{
	private const float _OVERLAPFACTOR = 0.1f;

	private float _proximity;
	private float _chunkaxis;
	private float _chunkhalfaxis;
	private float _chunkradius;
	private Dictionary<IJL, List<LatticeData>> _chunkdata;

	private List<IJL> _w_chunkarr;
	private HashSet<IJL> _w_chunkset;

	public ProximityLattice(float vertex_proximity, float latticeradius)
	{
		_proximity = Mathf.Max(0, vertex_proximity * vertex_proximity);
		_chunkradius = Mathf.Max(0.0001f, latticeradius);
		_chunkaxis = (2 * _chunkradius) * (1 - _OVERLAPFACTOR);
		_chunkhalfaxis = _chunkaxis / 2f;
		_chunkdata = new Dictionary<IJL, List<LatticeData>>();

		_w_chunkarr = new List<IJL>(8);
		_w_chunkset = new HashSet<IJL>();
	}

	public float proximity {
		set {
			_proximity = Mathf.Max(0, value * value);
		}
	}

	public void Clear(bool delete_empty)
	{
		if (delete_empty) {
			_chunkdata.Clear();
		}
		else {
			foreach (KeyValuePair<IJL, List<LatticeData>> pair in _chunkdata) {
				pair.Value.Clear();
			}
		}
		_w_chunkarr.Clear();
		_w_chunkset.Clear();
	}

	public bool AddVertex(int index, Vector3 vertex)
	{
		FillChunkIJL(vertex);
		if (_w_chunkarr.Count <= 0) {
			return false;
		}
		//check for close vertex
		for (int ak = 0;ak < _w_chunkarr.Count;ak++) {
			IJL chunk_ijl = _w_chunkarr[ak];
			if (!_chunkdata.ContainsKey(chunk_ijl)) {
				continue;
			}
			List<LatticeData> chunkdata = _chunkdata[chunk_ijl];
			for (int bk = 0; bk < chunkdata.Count; bk++) {
				LatticeData data = chunkdata[bk];
				if ((vertex - data.vertex).sqrMagnitude > _proximity) {
					continue;
				}
				return false;
			}
		}
		for (int k = 0; k < _w_chunkarr.Count; k++) {
			IJL chunk_ijl = _w_chunkarr[k];
			if (!_chunkdata.ContainsKey(chunk_ijl)) {
				_chunkdata.Add(chunk_ijl, new List<LatticeData>());
			}
			_chunkdata[chunk_ijl].Add(new LatticeData(index, vertex));
		}
		return true;
	}


	public bool DeleteVertex(Vector3 vertex, bool delete_empty)
	{
		FillChunkIJL(vertex);
		if (_w_chunkarr.Count <= 0) {
			return false;
		}
		//check for close vertex
		bool removed = false;
		for (int ak = 0; ak < _w_chunkarr.Count; ak++) {
			IJL chunk_ijl = _w_chunkarr[ak];
			if (!_chunkdata.ContainsKey(chunk_ijl)) {
				continue;
			}
			int index = 0;
			List<LatticeData> chunkdata = _chunkdata[chunk_ijl];
			while (index < chunkdata.Count) {
				LatticeData data = chunkdata[index];
				if ((vertex - data.vertex).sqrMagnitude > _proximity) {
					chunkdata.RemoveAt(index);
					removed = true;
				}
				else {
					index += 1;
				}
			}
			if (delete_empty && chunkdata.Count <= 0) {
				_chunkdata.Remove(chunk_ijl);
			}
		}
		return removed;
	}

	public int GetCloseVertexIndex(Vector3 vertex)
	{
		FillChunkIJL(vertex);
		for (int ak = 0;ak < _w_chunkarr.Count;ak++) {
			IJL chunk_ijl = _w_chunkarr[ak];
			if (!_chunkdata.ContainsKey(chunk_ijl)) {
				continue;
			}
			List<LatticeData> chunkdata = _chunkdata[chunk_ijl];
			for (int bk = 0;bk < chunkdata.Count;bk++) {
				LatticeData data = chunkdata[bk];
				if ((vertex - data.vertex).sqrMagnitude > _proximity) {
					continue;
				}
				return data.index;
			}
		}
		return -1;
	}

	private void FillChunkIJL(Vector3 vertex)
	{
		_w_chunkarr.Clear();
		_w_chunkset.Clear();
		float chunky = (vertex.y + _chunkhalfaxis) / _chunkaxis;
		float chunkx = (vertex.x + _chunkhalfaxis) / _chunkaxis;
		float chunkz = (vertex.z + _chunkhalfaxis) / _chunkaxis;
		int maxy = Mathf.CeilToInt(chunky);
		int miny = Mathf.FloorToInt(chunky);
		int maxx = Mathf.CeilToInt(chunkx);
		int minx = Mathf.FloorToInt(chunkx);
		int maxz = Mathf.CeilToInt(chunkz);
		int minz = Mathf.FloorToInt(chunkz);
		bool is_minx = (vertex.x - (minx * _chunkaxis) <= _chunkradius);
		bool is_maxx = (vertex.x - (maxx * _chunkaxis) <= _chunkradius);
		bool is_miny = (vertex.y - (miny * _chunkaxis) <= _chunkradius);
		bool is_maxy = (vertex.y - (maxy * _chunkaxis) <= _chunkradius);
		bool is_minz = (vertex.z - (minz * _chunkaxis) <= _chunkradius);
		bool is_maxz = (vertex.z - (maxz * _chunkaxis) <= _chunkradius);
		IJL chunk_ijl;
		if (is_miny) {
			if (is_minx) {
				if (is_minz) {
					chunk_ijl = new IJL(minz, minx, miny);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
				if (is_maxz) {
					chunk_ijl = new IJL(maxz, minx, miny);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
			}
			if (is_maxx) {
				if (is_minz) {
					chunk_ijl = new IJL(minz, maxx, miny);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
				if (is_maxz) {
					chunk_ijl = new IJL(maxz, maxx, miny);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
			}
		}
		if (is_maxy) {
			if (is_minx) {
				if (is_minz) {
					chunk_ijl = new IJL(minz, minx, maxy);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
				if (is_maxz) {
					chunk_ijl = new IJL(maxz, minx, maxy);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
			}
			if (is_maxx) {
				if (is_minz) {
					chunk_ijl = new IJL(minz, maxx, maxy);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
				if (is_maxz) {
					chunk_ijl = new IJL(maxz, maxx, maxy);
					if (_w_chunkset.Add(chunk_ijl)) {
						_w_chunkarr.Add(chunk_ijl);
					}
				}
			}
		}
	}

	private bool AllChunksExist()
	{
		for (int k = 0; k < _w_chunkarr.Count; k++) {
			if (!_chunkdata.ContainsKey(_w_chunkarr[k])) {
				return false;
			}
		}
		return true;
	}
}


public struct LatticeData
{
	public int index;
	public Vector3 vertex;

	public LatticeData(int index, Vector3 vertex)
	{
		this.index = index;
		this.vertex = vertex;
	}
}
