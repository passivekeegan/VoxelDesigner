using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CornerPacket
{
	public readonly CornerDesign design;
	public readonly int socket_axi_index;
	public readonly int index;
	public readonly int axi;

	public CornerPacket(CornerDesign design, int index, int socket_axi_index, int axi)
	{
		this.design = design;
		this.socket_axi_index = socket_axi_index;
		this.index = index;
		this.axi = axi;
	}

	public static CornerPacket empty {
		get {
			return new CornerPacket(null, -1, -1, 0);
		}
	}
}





