using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CornerPacket
{
	public readonly CornerDesign design;
	public readonly int socketaxi_index;
	public readonly int slotindex;
	public readonly bool invx;
	public readonly bool invy;

	public CornerPacket(CornerDesign design, int slotindex, int socketaxi_index, bool invx, bool invy)
	{
		this.design = design;
		this.socketaxi_index = socketaxi_index;
		this.slotindex = slotindex;
		this.invx = invx;
		this.invy = invy;
	}

	public bool isValid {
		get {
			return (design != null) && (socketaxi_index >= 0) && (slotindex >= 0);
		}
	}

	public static CornerPacket empty {
		get {
			return new CornerPacket(null, -1, -1, false, false);
		}
	}

	public int GetEdgeSocketSlotIndex(int socket_index, bool flip)
	{
		if (design == null || slotindex < 0) {
			return -1;
		}
		int socketcount = design.GetEdgeSocketCountByIndex(invx, invy, socketaxi_index);
		if (socketcount <= 0) {
			return -1;
		}
		if (flip) {
			socket_index = socketcount - 1 - socket_index;
		}
		int offset = design.GetEdgeSocketByIndex(invx, invy, socketaxi_index, socket_index);
		if (offset < 0) {
			return -1;
		}
		return slotindex + offset;
	}

	public int GetFaceSocketSlotIndex(int socket_index, bool flip)
	{
		if (design == null || slotindex < 0) {
			return -1;
		}
		int socketcount = design.GetFaceSocketCountByIndex(invx, invy, socketaxi_index);
		if (socketcount <= 0) {
			return -1;
		}
		if (flip) {
			socket_index = socketcount - 1 - socket_index;
		}
		int offset = design.GetFaceSocketByIndex(invx, invy, socketaxi_index, socket_index);
		if (offset < 0) {
			return -1;
		}
		return slotindex + offset;
	}
}
