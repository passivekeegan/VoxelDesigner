public class RectDesign: VoxelComponent
{
	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = 0;
		_facesocket_cnt = 0;
		_cornerplug_cnt = 4;
		_edgeplug_cnt = 4;
		//initialize lists
		InitializeComponentLists();
	}
}
