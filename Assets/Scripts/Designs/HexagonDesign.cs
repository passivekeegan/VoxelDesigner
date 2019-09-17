public class HexagonDesign : VoxelComponent
{
	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = 0;
		_facesocket_cnt = 0;
		_cornerplug_cnt = 6;
		_edgeplug_cnt = 6;
		//initialize lists
		InitializeComponentLists();
	}
}
