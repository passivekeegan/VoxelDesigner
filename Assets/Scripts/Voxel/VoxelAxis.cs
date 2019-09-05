//public enum VoxelAxi : ushort
//{
//	None = 0,
//	AD0 = 1,
//	AD1 = 2,
//	AD2 = 4,
//	AD3 = 8,
//	AD4 = 16,
//	AD5 = 32,
//	BD0 = 64,
//	BD1 = 128,
//	BD2 = 256,
//	BD3 = 512,
//	BD4 = 1024,
//	BD5 = 2048
//}

//[System.Flags]
//public enum VoxelAxe : ushort
//{
//	None = 0,
//	AD0 = 1,
//	AD1 = 2,
//	AD2 = 4,
//	AD3 = 8,
//	AD4 = 16,
//	AD5 = 32,
//	BD0 = 64,
//	BD1 = 128,
//	BD2 = 256,
//	BD3 = 512,
//	BD4 = 1024,
//	BD5 = 2048
//}

//public static class VoxelAxesExt
//{
//	public static ushort ALL {
//		get {
//			return 4095;
//		}
//	}

//	public static string Name(this VoxelAxe axis)
//	{
//		bool AD0 = (axis & VoxelAxe.AD0) > 0;
//		bool AD1 = (axis & VoxelAxe.AD1) > 0;
//		bool AD2 = (axis & VoxelAxe.AD2) > 0;
//		bool AD3 = (axis & VoxelAxe.AD3) > 0;
//		bool AD4 = (axis & VoxelAxe.AD4) > 0;
//		bool AD5 = (axis & VoxelAxe.AD5) > 0;
//		bool BD0 = (axis & VoxelAxe.BD0) > 0;
//		bool BD1 = (axis & VoxelAxe.BD1) > 0;
//		bool BD2 = (axis & VoxelAxe.BD2) > 0;
//		bool BD3 = (axis & VoxelAxe.BD3) > 0;
//		bool BD4 = (axis & VoxelAxe.BD4) > 0;
//		bool BD5 = (axis & VoxelAxe.BD5) > 0;
//		bool A = AD0 && AD1 && AD2 && AD3 && AD4 && AD5;
//		bool NA = !AD0 && !AD1 && !AD2 && !AD3 && !AD4 && !AD5;
//		bool B = BD0 && BD1 && BD2 && BD3 && BD4 && BD5;
//		bool NB = !BD0 && !BD1 && !BD2 && !BD3 && !BD4 && !BD5;
//		if (A && B) {
//			return "ALL";
//		}
//		else if (NA && NB) {
//			return "NONE";
//		}
//		else {
//			string name = "";
//			if (A) {
//				name += "A-ALL";
//			}
//			else if (!NA) {
//				name += "A";
//				if (AD0) {
//					name += "0";
//				}
//				if (AD1) {
//					name += "1";
//				}
//				if (AD2) {
//					name += "2";
//				}
//				if (AD3) {
//					name += "3";
//				}
//				if (AD4) {
//					name += "4";
//				}
//				if (AD5) {
//					name += "5";
//				}
//			}
//			if (!NA && !NB) {
//				name += "-";
//			}
//			if (B) {
//				name += "B-ALL";
//			}
//			else if (!NB) {
//				name += "B";
//				if (BD0) {
//					name += "0";
//				}
//				if (BD1) {
//					name += "1";
//				}
//				if (BD2) {
//					name += "2";
//				}
//				if (BD3) {
//					name += "3";
//				}
//				if (BD4) {
//					name += "4";
//				}
//				if (BD5) {
//					name += "5";
//				}
//			}
//			return name;
//		}
//	}
//}