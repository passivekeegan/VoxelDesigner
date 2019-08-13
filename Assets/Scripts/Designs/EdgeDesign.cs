using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EdgeDesign class is an object that is designed to store 
/// information on how to construct a voxel edge.
/// </summary>
public class EdgeDesign : VoxelComponent
{
	public const int FACESOCKET_CNT = 4;
	public const int CORNERPLUG_CNT = 2;

	/// <summary>
	/// Initializes the interal state of the EdgeDesign object. 
	/// Should be called when this object is created.
	/// </summary>
	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = 0;
		_facesocket_cnt = FACESOCKET_CNT;
		_cornerplug_cnt = CORNERPLUG_CNT;
		_edgeplug_cnt = 0;
		//initialize lists
		InitializeComponentLists();
	}
}
