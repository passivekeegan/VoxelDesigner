using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerDesign : VoxelComponent
{
	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = 5;
		_facesocket_cnt = 9;
		_cornerplug_cnt = 0;
		_edgeplug_cnt = 0;
		//initialize lists
		InitializeComponentLists();
	}

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

	public override int FaceSocketIndex(int level, int axi)
	{
		//above
		if (level > 0) {
			switch (axi) {
				case 2:
					return 0;
				case 4:
					return 2;
				case 6:
					return 4;
				default:
					return -1;
			}
		}
		//below
		else if (level < 0) {
			switch (axi) {
				case 2:
					return 1;
				case 4:
					return 3;
				case 6:
					return 5;
				default:
					return -1;
			}
		}
		//middle
		else {
			switch (axi) {
				case 5:
					return 6;
				case 7:
					return 7;
				case 3:
					return 8;
				default:
					return -1;
			}
		}
	}
}
