public class LatEdgeDesign : VoxelComponent
{
	public const int EDGESOCKET_CNT = 0;
	public const int FACESOCKET_CNT = 4;
	public const int CORNERPLUG_CNT = 2;
	public const int EDGEPLUG_CNT = 0;

	public override void Initialize()
	{
		objname = "";
		//setup list arguments
		_edgesocket_cnt = 0;
		_facesocket_cnt = 4;
		_cornerplug_cnt = 2;
		_edgeplug_cnt = 0;
		//initialize lists
		InitializeComponentLists();
	}

}
