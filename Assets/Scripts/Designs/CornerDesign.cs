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
		_facesocket_cnt = 15;
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
