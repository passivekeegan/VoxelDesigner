using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An abstract class designed to be implemented as a component to be used in the 
/// construction of a voxel. It contains geometry information as well as concepts 
/// such as sockets and plugs ment to help voxel components hook up to one another.
/// </summary>
[System.Serializable]
public abstract class VoxelComponent : VoxelObject
{
	//vertices variables
	[SerializeField]
	public List<Vector3> vertices;
	//triangles variables
	[SerializeField]
	public List<Triangle> triangles;
	//edge sockets variables
	[SerializeField]
	public List<AxiSocket> edgesockets;
	[SerializeField]
	protected int _edgesocket_cnt;
	//face sockets variables
	[SerializeField]
	public List<AxiSocket> facesockets;
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
		vertices = new List<Vector3>();
		triangles = new List<Triangle>();
		//edge sockets
		_edgesocket_cnt = Mathf.Max(0, _edgesocket_cnt);
		edgesockets = new List<AxiSocket>(_edgesocket_cnt);
		for (int k = 0; k < _edgesocket_cnt; k++) {
			edgesockets.Add(new AxiSocket(new List<int>(), new List<int>(), new List<int>(), new List<int>()));
		}
		//face sockets
		_facesocket_cnt = Mathf.Max(0, _facesocket_cnt);
		facesockets = new List<AxiSocket>(_facesocket_cnt);
		for (int k = 0; k < _facesocket_cnt; k++) {
			facesockets.Add(new AxiSocket(new List<int>(), new List<int>(), new List<int>(), new List<int>()));
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
	public int GetEdgeSocketCount(bool invx, bool invy, int axi)
	{
		return GetEdgeSocketCountByIndex(invx, invy, EdgeSocketIndex(axi));
	}


	public int GetEdgeSocketCountByIndex(bool invx, bool invy, int axi_index)
	{
		if (axi_index < 0 || axi_index >= edgesockets.Count || !edgesockets[axi_index].IsValid()) {
			return -1;
		}
		List<int> sockets = edgesockets[axi_index][invx, invy];
		if (sockets == null) {
			return -1;
		}
		return sockets.Count;
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
	public int GetFaceSocketCount(bool invx, bool invy, int level, int axi)
	{
		return GetFaceSocketCountByIndex(invx, invy, FaceSocketIndex(level, axi));
	}

	public int GetFaceSocketCountByIndex(bool invx, bool invy, int axi_index)
	{
		if (axi_index < 0 || axi_index >= facesockets.Count || !facesockets[axi_index].IsValid()) {
			return -1;
		}
		List<int> sockets = facesockets[axi_index][invx, invy];
		if (sockets == null) {
			return -1;
		}
		return sockets.Count;
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
	public int GetEdgeSocketByAxi(bool invx, bool invy, int axi, int socket_index)
	{
		return GetSocket(invx, invy, EdgeSocketIndex(axi), socket_index, edgesockets);
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
	public int GetEdgeSocketByIndex(bool invx, bool invy, int axi_index, int socket_index)
	{
		return GetSocket(invx, invy, axi_index, socket_index, edgesockets);
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
	public int GetFaceSocketByAxi(bool invx, bool invy, int level, int axi, int socket_index)
	{
		return GetSocket(invx, invy, FaceSocketIndex(level, axi), socket_index, facesockets);
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
	public int GetFaceSocketByIndex(bool invx, bool invy, int axi_index, int socket_index)
	{
		return GetSocket(invx, invy, axi_index, socket_index, facesockets);
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
	private int GetSocket(bool invx, bool invy, int axi_index, int socket_index, List<AxiSocket> axisockets)
	{
		//validate that axi index is within bounds and socket is not null
		if (axi_index < 0 || axi_index >= axisockets.Count || !axisockets[axi_index].IsValid()) {
			return -1;
		}
		List<int> sockets = axisockets[axi_index][invx, invy];
		if (sockets == null) {
			return -1;
		}
		//validate that the socket index is within bounds
		if (socket_index < 0 || socket_index >= sockets.Count) {
			return -1;
		}
		//get the vertex index in the socket
		int vertex_index = sockets[socket_index];
		//validate the vertex index is within bounds
		if (vertex_index < 0 || vertex_index >= vertices.Count) {
			return -1;
		}
		return vertex_index;
	}

	public List<int> GetEdgeSocketList(bool invx, bool invy, int axi_index)
	{
		return GetSocketList(invx, invy, axi_index, edgesockets);
	}

	public List<int> GetFaceSocketList(bool invx, bool invy, int axi_index)
	{
		return GetSocketList(invx, invy, axi_index, facesockets);
	}

	private List<int> GetSocketList(bool invx, bool invy, int axi_index, List<AxiSocket> axisockets)
	{

		//validate that axi index is within bounds and socket is not null
		if (axi_index < 0 || axi_index >= axisockets.Count || !axisockets[axi_index].IsValid()) {
			return null;
		}
		return axisockets[axi_index][invx, invy];
	}
	#endregion
}
