
/// <summary>
/// The TriIndex struct is a structure designed for encoding and 
/// decoding information describing 1 of 3 different types of 
/// vertex indices. It can be of type "Vertex", "Corner Plug", 
/// or "Edge Plug".
/// </summary>
public struct TriIndex
{
	private const ushort UPPER_MASK = 65280;
	private const ushort LOWER_MASK = 255;
	private const int AXI_SHIFT = 8;

	public readonly TriIndexType type;
	public readonly byte axi_index;
	public readonly byte socket_index;

	/// <summary>
	/// Creates and initializes a "Vertex" type TriIndex.
	/// </summary>
	/// <param name="vertex_index">
	/// A ushort that represents the index of a vertex.
	/// </param>
	public TriIndex(ushort vertex_index)
	{
		this.type = TriIndexType.Vertex;
		this.axi_index = 0;
		this.socket_index = 0;
		this.axi_index = DecodeAxiIndex(vertex_index);
		this.socket_index = DecodeIndex(vertex_index);
	}

	/// <summary>
	/// Creates and initializes a specified type of TriIndex, 
	/// encoding version.
	/// </summary>
	/// <param name="type">
	/// A TriIndexType variable that specifies what kind of 
	/// TriIndex should be created and initialized.
	/// </param>
	/// <param name="vertex_index">
	/// A ushort to be encoded depending on the type of 
	/// TriIndex created.
	/// </param>
	public TriIndex(TriIndexType type, ushort vertex_index)
	{
		this.type = type;
		this.axi_index = 0;
		this.socket_index = 0;
		this.axi_index = DecodeAxiIndex(vertex_index);
		this.socket_index = DecodeIndex(vertex_index);
	}

	/// <summary>
	/// Creates and initializes a specified type of TriIndex,
	/// without encoding version.
	/// </summary>
	/// <param name="type">
	/// A TriIndexType variable that describes what kind of 
	/// TriIndex this should be interpreted as.
	/// </param>
	/// <param name="axis_index">
	/// A byte variable whose value is stored directly, can be a part of an
	/// encoded vertex index or the index of an axi.
	/// </param>
	/// <param name="index">
	/// A byte variable whose value is stored directly, can be a part of an 
	/// encoded vertex index or a socket index.
	/// </param>
	public TriIndex(TriIndexType type, byte axis_index, byte index)
	{
		this.type = type;
		this.axi_index = axis_index;
		this.socket_index = index;
	}

	/// <summary>
	/// Encodes a TriIndex struct into a ushort representation based on 
	/// its TriIndexType value.
	/// </summary>
	/// <param name="index"></param>
	/// <returns></returns>
	public static ushort EncodeIndex(TriIndex index)
	{
		return EncodeIndex(index.axi_index, index.socket_index);
	}

	/// <summary>
	/// Encodes an axi index and socket index into a ushort representation.
	/// </summary>
	/// <param name="axi_index">
	/// A byte variable representing an axi index.
	/// </param>
	/// <param name="socket_index">
	/// A byte variable representing a socket index.
	/// </param>
	/// <returns></returns>
	public static ushort EncodeIndex(byte axi_index, byte socket_index)
	{
		//shift and add axis index to vertex
		ushort vertex = (ushort)(((ushort)(axi_index << AXI_SHIFT)) & UPPER_MASK);
		//add vertex index and return
		return (ushort)(vertex | (ushort)(socket_index & LOWER_MASK));
	}

	/// <summary>
	/// Decodes an encoded ushort variable into a byte variable representing the axi index.
	/// </summary>
	/// <param name="encoded_value">
	/// A ushort variable that has been encoded to contain an axi index and socket index.
	/// </param>
	/// <returns>
	/// Returns a byte variable representing the axi index.
	/// </returns>
	public static byte DecodeAxiIndex(ushort encoded_value)
	{
		return (byte)(((ushort)(encoded_value >> AXI_SHIFT)) & LOWER_MASK);
	}

	/// <summary>
	/// Decodes an encoded ushort variable into a byte variable representing the socket index.
	/// </summary>
	/// <param name="encoded_value">
	/// A ushort variable that has been encoded to contain an axi index and socket index.
	/// </param>
	/// <returns>
	/// Returns a byte variable representing the socket index.
	/// </returns>
	public static byte DecodeIndex(ushort encoded_value)
	{
		return (byte)(encoded_value & LOWER_MASK);
	}
}

/// <summary>
/// An Enum that is used to distinguish different types of vertex indices referenced 
/// in a Triangle.
/// </summary>
public enum TriIndexType
{
	Vertex = 0,
	CornerPlug = 1,
	EdgePlug = 2
}