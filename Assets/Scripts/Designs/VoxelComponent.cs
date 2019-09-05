using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class designed to be implemented as a component to be used in the 
/// construction of a voxel. It contains geometry information as well as concepts 
/// such as sockets and plugs ment to help voxel components hook up to one another.
/// </summary>
public abstract class VoxelComponent : VoxelObject, ISerializationCallbackReceiver
{
	//vertices variables
	[SerializeField]
	public List<VertexVector> vertices;
	//triangles variables
	[SerializeField]
	public List<Triangle> triangles;
	//edge sockets variables
	public List<List<int>> edgesockets;
	[SerializeField]
	protected int _edgesocket_cnt;
	//face sockets variables
	public List<List<int>> facesockets;
	[SerializeField]
	protected int _facesocket_cnt;
	//corner plugs variables
	[SerializeField]
	public List<int> cornerplugs;
	[SerializeField]
	protected int _cornerplug_cnt;
	//edge plugs variables
	[SerializeField]
	public List<int> edgeplugs;
	[SerializeField]
	protected int _edgeplug_cnt;


	/// <summary>
	/// Initializes the internal lists in the VoxelComponent object based on 
	/// socket and plug count variables that should be set before calling this 
	/// method. 
	/// </summary>
	protected void InitializeComponentLists()
	{
		//geometry
		vertices = new List<VertexVector>();
		triangles = new List<Triangle>();
		//edge sockets
		_edgesocket_cnt = Mathf.Max(0, _edgesocket_cnt);
		edgesockets = new List<List<int>>(_edgesocket_cnt);
		for (int k = 0; k < _edgesocket_cnt; k++) {
			edgesockets.Add(new List<int>());
		}
		//face sockets
		_facesocket_cnt = Mathf.Max(0, _facesocket_cnt);
		facesockets = new List<List<int>>(_facesocket_cnt);
		for (int k = 0; k < _facesocket_cnt; k++) {
			facesockets.Add(new List<int>());
		}
		//corner plugs
		_cornerplug_cnt = Mathf.Max(0, _cornerplug_cnt);
		cornerplugs = new List<int>(_cornerplug_cnt);
		for (int k = 0; k < _cornerplug_cnt; k++) {
			cornerplugs.Add(-1);
		}
		//edge plugs
		_edgeplug_cnt = Mathf.Max(0, _edgeplug_cnt);
		edgeplugs = new List<int>(_edgeplug_cnt);
		for (int k = 0; k < _edgeplug_cnt; k++) {
			edgeplugs.Add(-1);
		}
	}

	/// <summary>
	/// A check to ensure that the lists are not null or do not have a different 
	/// number of sockets or plugs than they should.
	/// </summary>
	/// <returns>
	/// Returns a boolean, a true value represents that all lists are not null and 
	/// are the appropriate size.
	/// </returns>
	public override bool IsValid()
	{
		return (vertices != null) && (triangles != null) &&
			(edgesockets != null) && (edgesockets.Count == _edgesocket_cnt) &&
			(facesockets != null) && (facesockets.Count == _facesocket_cnt) &&
			(cornerplugs != null) && (cornerplugs.Count == _cornerplug_cnt) &&
			(edgeplugs != null) && (edgeplugs.Count == _edgeplug_cnt);
	}

	#region Corner and Edge Plugs
	/// <summary>
	/// A boolean value that represents if this object has edge sockets enabled.
	/// </summary>
	/// <value>
	/// Returns a true boolean if edge socket use is enabled and false if it is
	/// disabled or if the internal edge socket format is incorrect.
	/// </value>
	public bool hasEdgeSocket {
		get {
			return (_edgesocket_cnt > 0) && (edgesockets != null) && (edgesockets.Count == _edgesocket_cnt);
		}
	}

	/// <summary>
	/// A boolean value that represents if this object has face sockets enabled.
	/// </summary>
	/// <value>
	/// Returns a true boolean if face socket use is enabled and false if it is
	/// disabled or if the internal face socket format is incorrect.
	/// </value>
	public bool hasFaceSocket {
		get {
			return (_facesocket_cnt > 0) && (facesockets != null) && (facesockets.Count == _facesocket_cnt);
		}
	}

	/// <summary>
	/// A boolean value that represents if this object has corner plugs enabled.
	/// </summary>
	/// <value>
	/// Returns a true boolean if corner plug use is enabled and false if it is
	/// disabled or if the internal corner plug format is incorrect.
	/// </value>
	public bool hasCornerPlug
	{
		get {
			return (_cornerplug_cnt > 0) && (cornerplugs != null) && (cornerplugs.Count == _cornerplug_cnt);
		}
	}

	/// <summary>
	/// A boolean value that represents if this object has edge plugs enabled.
	/// </summary>
	/// <value>
	/// Returns a true boolean if edge plug use is enabled and false if it is
	/// disabled or if the internal edge plug format is incorrect.
	/// </value>
	public bool hasEdgePlug {
		get {
			return (_edgeplug_cnt > 0) && (edgeplugs != null) && (edgeplugs.Count == _edgeplug_cnt);
		}
	}
	#endregion

	#region Edge and Face Sockets
	/// <summary>
	/// Returns the axi index of the edge socket at a given axi. 
	/// </summary>
	/// <param name="axi">
	/// An integer in the range of [0, 7]. It represents an axi direction.
	/// </param>
	/// <returns>
	/// Returns an integer representing the axi index of the edge socket. 
	/// Returns -1 if the object doesn't have an edge socket at that axi.
	/// </returns>
	public virtual int EdgeSocketIndex(int axi)
	{
		return -1;
	}

	/// <summary>
	/// Returns the axi index of the face socket at a given level and axi. 
	/// </summary>
	/// <param name="level">
	/// An integer value that represents 1 of 3 face socket levels: 
	/// above, middle and below. Above is level &gt 0. Middle is 
	/// level = 0. Below is level &lt 0.
	/// </param>
	/// <param name="axi">
	/// An integer in the range of [0, 7]. It represents an axi 
	/// direction.
	/// </param>
	/// <returns>
	/// Returns an integer representing the axi index of the face socket. 
	/// Returns -1 if the object doesn't have a face socket at that level 
	/// and axi.
	/// </returns>
	public virtual int FaceSocketIndex(int level, int axi)
	{
		return -1;
	}

	/// <summary>
	/// Returns the number of vertices in the edge socket at the given axi.
	/// </summary>
	/// <param name="axi">
	/// An integer in the range of [0, 7]. It represents an axi direction.
	/// </param>
	/// <returns>
	/// Returns an integer representing the socket vertex count. Returns -1 
	/// if the object doesn't have an edge socket at that axi.
	/// </returns>
	public int GetEdgeSocketCount(int axi)
	{
		int axi_index = EdgeSocketIndex(axi);
		if (axi_index < 0 || axi_index >= edgesockets.Count || edgesockets[axi_index] == null) {
			return -1;
		}
		return edgesockets[axi_index].Count;
	}

	/// <summary>
	/// Returns the number of vertices in the face socket at a given 
	/// level and axi. 
	/// </summary>
	/// <param name="level">
	/// An integer value that represents 1 of 3 face socket levels: 
	/// above, middle and below. Above is level &gt 0. Middle is 
	/// level = 0. Below is level &lt 0.
	/// </param>
	/// <param name="axi">
	/// An integer in the range of [0, 7]. It represents an axi 
	/// direction.
	/// </param>
	/// <returns>
	/// Returns an integer representing the socket vertex count. 
	/// Returns -1 if the object doesn't have a face socket at 
	/// that level and axi.
	/// </returns>
	public int GetFaceSocketCount(int level, int axi)
	{
		int axi_index = FaceSocketIndex(level, axi);
		if (axi_index < 0 || axi_index >= facesockets.Count || facesockets[axi_index] == null) {
			return -1;
		}
		return facesockets[axi_index].Count;
	}

	/// <summary>
	/// Gets a vertex index from an edge socket by axi.
	/// </summary>
	/// <param name="axi">
	/// An integer in the range of [0, 7]. It represents an axi direction.
	/// </param>
	/// <param name="socket_index">
	/// An integer representing an index of a vertex index in an edge socket.
	/// </param>
	/// <returns>
	/// Returns an integer representing a vertex index in the corresponding
	/// edge socket. Returns -1 if the object doesn't have an edge socket at 
	/// that axi, if the socket index is out of bounds, or if the vertex 
	/// index in the edge socket is out of bounds.
	/// </returns>
	public int GetEdgeSocketByAxi(int axi, int socket_index)
	{
		return GetSocket(EdgeSocketIndex(axi), socket_index, edgesockets);
	}


	/// <summary>
	/// Gets a vertex index from an edge socket by index.
	/// </summary>
	/// <param name="axi_index">
	/// An integer representing the index of an edge socket in this objects 
	/// list of edge sockets.
	/// </param>
	/// <param name="socket_index">
	/// An integer representing an index of a vertex index in an edge socket.
	/// </param>
	/// <returns>
	/// Returns an integer representing a vertex index in the corresponding
	/// edge socket. Returns -1 if the axi index or socket index is out of 
	/// bounds, or if the vertex index in the edge socket is out of bounds.
	/// </returns>
	public int GetEdgeSocketByIndex(int axi_index, int socket_index)
	{
		return GetSocket(axi_index, socket_index, edgesockets);
	}

	/// <summary>
	/// Gets a vertex index from a face socket by axi.
	/// </summary>
	/// <param name="level">
	/// An integer value that represents 1 of 3 face socket levels: 
	/// above, middle and below. Above is level &gt 0. Middle is 
	/// level = 0. Below is level &lt 0.
	/// </param>
	/// <param name="axi">
	/// An integer in the range of [0, 7]. It represents an axi 
	/// direction.
	/// </param>
	/// <param name="socket_index">
	/// An integer representing an index of a vertex index in a face socket.
	/// </param>
	/// <returns>
	/// Returns an integer representing a vertex index in the corresponding
	/// face socket. Returns -1 if the object doesn't have a face socket at 
	/// that level and axi, the socket index is out of bounds or if the 
	/// vertex index in the face socket is out of bounds.
	/// </returns>
	public int GetFaceSocketByAxi(int level, int axi, int socket_index)
	{
		return GetSocket(FaceSocketIndex(level, axi), socket_index, facesockets);
	}

	/// <summary>
	/// Gets a vertex index from a face socket by index.
	/// </summary>
	/// <param name="axi_index">
	/// An integer representing the index of a face socket in this objects 
	/// list of face sockets.
	/// </param>
	/// <param name="socket_index">
	/// An integer representing an index of a vertex index in a face socket.
	/// </param>
	/// <returns>
	/// Returns an integer representing a vertex index in the corresponding
	/// face socket. Returns -1 if the axi index or socket index is out of 
	/// bounds, or if the vertex index in the face socket is out of bounds.
	/// </returns>
	public int GetFaceSocketByIndex(int axi_index, int socket_index)
	{
		return GetSocket(axi_index, socket_index, facesockets);
	}

	/// <summary>
	/// Gets a vertex index from a socket.
	/// </summary>
	/// <param name="axi_index">
	/// An integer representing the index of a socket in the list of sockets.
	/// </param>
	/// <param name="socket_index">
	/// An integer representing an index of a vertex index in a socket.
	/// </param>
	/// <param name="sockets">
	/// A list of integer lists. The upper level list represents a list of 
	/// sockets corresponding to some axi. The lower level list of integers 
	/// represents the sockets vertex indices pointing to vertices in the 
	/// object.
	/// </param>
	/// <returns>
	/// Returns an integer representing a vertex index. Returns -1 if the 
	/// axis_index or socket_index is out of bounds, if sockets is null of 
	/// improperly formated, or if the vertex index in the socket is out 
	/// of bounds.
	/// </returns>
	private int GetSocket(int axi_index, int socket_index, List<List<int>> sockets)
	{
		//validate that axi index is within bounds and socket is not null
		if (axi_index < 0 || axi_index >= sockets.Count || sockets[axi_index] == null) {
			return -1;
		}
		//validate that the socket index is within bounds
		if (socket_index < 0 || socket_index >= sockets[axi_index].Count) {
			return -1;
		}
		//get the vertex index in the socket
		int vertex_index = sockets[axi_index][socket_index];
		//validate the vertex index is within bounds
		if (vertex_index < 0 || vertex_index >= vertices.Count) {
			return -1;
		}
		return vertex_index;
	}

	public List<int> GetSocket(SocketType type, int axi_index)
	{
		switch(type) {
			case SocketType.Edge:
				if (axi_index < 0 || axi_index >= _edgesocket_cnt) {
					return null;
				}
				return edgesockets[axi_index];
			case SocketType.Face:
				if (axi_index < 0 || axi_index >= _facesocket_cnt) {
					return null;
				}
				return facesockets[axi_index];
			default:
				return null;
		}
	}

	#endregion

	#region Serialization
	[SerializeField]
	private List<int> _serial_edgecounts;
	[SerializeField]
	private List<int> _serial_edgesockets;
	[SerializeField]
	private List<int> _serial_facecounts;
	[SerializeField]
	private List<int> _serial_facesockets;

	public void OnBeforeSerialize()
	{
		//edge socket serialization
		if (edgesockets == null) {
			edgesockets = new List<List<int>>();
		}
		if (_serial_edgecounts == null) {
			_serial_edgecounts = new List<int>();
		}
		else {
			_serial_edgecounts.Clear();
		}
		if (_serial_edgesockets == null) {
			_serial_edgesockets = new List<int>();
		}
		else {
			_serial_edgesockets.Clear();
		}
		for (int i = 0; i < _edgesocket_cnt; i++) {
			_serial_edgecounts.Add(0);
			if (i >= edgesockets.Count || edgesockets[i] == null) {
				continue;
			}
			_serial_edgecounts[i] = edgesockets[i].Count;
			for (int j = 0; j < _serial_edgecounts[i]; j++) {
				_serial_edgesockets.Add(edgesockets[i][j]);
			}
		}
		//face socket serialization
		if (facesockets == null) {
			facesockets = new List<List<int>>();
		}
		if (_serial_facecounts == null) {
			_serial_facecounts = new List<int>();
		}
		else {
			_serial_facecounts.Clear();
		}
		if (_serial_facesockets == null) {
			_serial_facesockets = new List<int>();
		}
		else {
			_serial_facesockets.Clear();
		}
		for (int i = 0; i < _facesocket_cnt; i++) {
			_serial_facecounts.Add(0);
			if (i >= facesockets.Count || facesockets[i] == null) {
				continue;
			}
			_serial_facecounts[i] = facesockets[i].Count;
			for (int j = 0; j < _serial_facecounts[i]; j++) {
				_serial_facesockets.Add(facesockets[i][j]);
			}
		}
	}
	public void OnAfterDeserialize()
	{
		//edge socket deserialization
		if (_serial_edgecounts == null || _serial_edgesockets == null) {
			_serial_edgecounts = new List<int>();
			_serial_edgesockets = new List<int>();
		}
		if (edgesockets == null) {
			edgesockets = new List<List<int>>(_edgesocket_cnt);
		}
		else {
			edgesockets.Clear();
		}
		for (int i = 0, index = 0; i < _edgesocket_cnt; i++) {
			edgesockets.Add(new List<int>());
			if (i >= _serial_edgecounts.Count) {
				continue;
			}
			int cnt = Mathf.Max(0, _serial_edgecounts[i]);
			for (int j = 0; j < cnt && index < _serial_edgesockets.Count; j++, index += 1) {
				edgesockets[i].Add(_serial_edgesockets[index]);
			}
		}
		//face socket deserialization
		if (_serial_facecounts == null || _serial_facesockets == null) {
			_serial_facecounts = new List<int>();
			_serial_facesockets = new List<int>();
		}
		if (facesockets == null) {
			facesockets = new List<List<int>>(_facesocket_cnt);
		}
		else {
			facesockets.Clear();
		}
		for (int i = 0, index = 0; i < _facesocket_cnt; i++) {
			facesockets.Add(new List<int>());
			if (i >= _serial_facecounts.Count) {
				continue;
			}
			int cnt = Mathf.Max(0, _serial_facecounts[i]);
			for (int j = 0; j < cnt && index < _serial_facesockets.Count; j++, index += 1) {
				facesockets[i].Add(_serial_facesockets[index]);
			}
		}
	}
	#endregion
}


public enum SocketType
{
	Edge,
	Face
}