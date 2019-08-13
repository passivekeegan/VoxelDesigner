using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The FaceDesign class is an object that is designed to store 
/// information on how to construct a voxel face.
/// </summary>
public class FaceDesign : VoxelComponent
{
	public const int CORNERPLUG_CNT = 6;
	public const int EDGEPLUG_CNT = 6;

	/// <summary>
	/// Initializes the interal state of the FaceDesign object. 
	/// Should be called when this object is created.
	/// </summary>
	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = 0;
		_facesocket_cnt = 0;
		_cornerplug_cnt = CORNERPLUG_CNT;
		_edgeplug_cnt = EDGEPLUG_CNT;
		//initialize lists normally
		InitializeComponentLists();
	}
}
