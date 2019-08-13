using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The CornerDesign class is an object that is designed to store 
/// information on how to construct a voxel corner.
/// </summary>
public class CornerDesign : VoxelComponent
{
	public const int EDGESOCKET_CNT = 5;
	public const int FACESOCKET_CNT = 15;

	/// <summary>
	/// Initializes the interal state of the CornerDesign object. 
	/// Should be called when this object is created.
	/// </summary>
	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = EDGESOCKET_CNT;
		_facesocket_cnt = FACESOCKET_CNT;
		_cornerplug_cnt = 0;
		_edgeplug_cnt = 0;
		//initialize lists
		InitializeComponentLists();
	}

	/// <summary>
	/// Returns the axi index of the edge socket at a given axi. 
	/// </summary>
	/// <param name="axi">
	/// An integer in the range of [0, 7]. It represents an axi direction.
	/// </param>
	/// <returns>
	/// Returns an integer representing the axi index of the edge socket. 
	/// Returns -1 if the CornerDesign object doesn't have an edge socket 
	/// at that axi.
	/// </returns>
	public override int EdgeSocketIndex(int axi)
	{
		switch (axi) {
			case 0:// above
				return 0;
			case 1:// below
				return 1;
			case 2:// d0
				return 2;
			case 4:// d2
				return 3;
			case 6:// d4
				return 4;
			default:
				return -1;
		}
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
	/// Returns -1 if the CornerDesign object doesn't have a face socket 
	/// at that level and axi.
	/// </returns>
	public override int FaceSocketIndex(int level, int axi)
	{
		//above
		if (level > 0) {
			switch (axi) {
				case 2:
					return 0;
				case 3:
					return 2;
				case 4:
					return 4;
				case 5:
					return 6;
				case 6:
					return 8;
				case 7:
					return 10;
				default:
					return -1;
			}
		}
		//below
		else if (level < 0) {
			switch (axi) {
				case 2:
					return 1;
				case 3:
					return 3;
				case 4:
					return 5;
				case 5:
					return 7;
				case 6:
					return 9;
				case 7:
					return 11;
				default:
					return -1;
			}
		}
		//middle
		else {
			switch (axi) {
				case 5:
					return 12;
				case 7:
					return 13;
				case 3:
					return 14;
				default:
					return -1;
			}
		}
	}
}
