public class LongitudeDesign : VoxelComponent
{
	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = 0;
		_facesocket_cnt = 3;
		_cornerplug_cnt = 2;
		_edgeplug_cnt = 0;
		//initialize lists
		InitializeComponentLists();
	}

}