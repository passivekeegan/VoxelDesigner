using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObject : VoxelObject
{
	[SerializeField]
	private CornerMap _corner;
	[SerializeField]
	private LateralMap _lateral;
	[SerializeField]
	private LongitudeMap _longitude;
	[SerializeField]
	private RectMap _rect;
	[SerializeField]
	private HexagonMap _hexagon;

#if UNITY_EDITOR
	public override void Initialize()
	{
		objname = "";
		_corner = new CornerMap();
		_lateral = new LateralMap();
		_longitude = new LongitudeMap();
		_rect = new RectMap();
		_hexagon = new HexagonMap();
	}
#endif

	public void Start()
	{
#if UNITY_EDITOR
		if (Application.isPlaying) {
#endif
			_corner.Start();
			_lateral.Start();
			_longitude.Start();
			_rect.Start();
			_hexagon.Start();
#if UNITY_EDITOR
		}
#endif
	}

	public override bool IsValid()
	{
		return _corner != null && _lateral != null && _longitude != null && _rect != null && _hexagon != null;
	}

#region Corners
#region Modify Map
	public void AddCorner(CornerDesign corner)
	{
		_corner.AddComponent(corner);
	}
	public void ReplaceCorner(CornerDesign oldcorner, CornerDesign newcorner)
	{
		_corner.ReplaceComponent(oldcorner, newcorner);
	}
	public void DeleteCorner(CornerDesign corner)
	{
		_corner.DeleteComponent(corner);
	}
	public void AddCornerPattern(CornerDesign corner, CornerPattern pattern)
	{
		_corner.AddPattern(corner, pattern);
	}
	public void DeleteCornerPattern(CornerDesign corner, CornerPattern pattern)
	{
		_corner.DeletePattern(corner, pattern);
	}

#endregion

#region Query Map
	public bool CornerExists(CornerDesign corner)
	{
		return _corner.ComponentExists(corner);
	}
	public bool CornerPatternExists(CornerPattern pattern)
	{
		return _corner.PatternExists(pattern);
	}

#endregion

#region Getters
	public int GetCornerID(CornerPattern pattern)
	{
		return _corner.GetComponentID(pattern);
	}
	public CornerDesign GetCorner(CornerPattern pattern)
	{
		return _corner.GetComponent(pattern);
	}
	public CornerDesign GetCorner(int id)
	{
		return _corner.GetComponent(id);
	}
	public CornerPattern GetCornerPattern(CornerPattern pattern)
	{
		return _corner.GetPattern(pattern);
	}
#endregion

#if UNITY_EDITOR
#region List Getters
	public void AddCornersToList(List<VoxelComponent> list)
	{
		_corner.AddComponentsToList(list);
	}
	public void AddCornersToList(List<CornerDesign> list)
	{
		_corner.AddComponentsToList(list);
	}
	public void AddCornerPatternsToList(CornerDesign corner, List<CornerPattern> list)
	{
		_corner.AddPatternsToList(corner, list);
	}
#endregion
#endif
#endregion

#region Laterals
#region Modify Map
	public void AddLateral(LateralDesign lateral)
	{
		_lateral.AddComponent(lateral);
	}
	public void ReplaceLateral(LateralDesign oldlateral, LateralDesign newlateral)
	{
		_lateral.ReplaceComponent(oldlateral, newlateral);
	}
	public void DeleteLateral(LateralDesign lateral)
	{
		_lateral.DeleteComponent(lateral);
	}
	public void AddLateralPattern(LateralDesign lateral, LateralPattern pattern)
	{
		_lateral.AddPattern(lateral, pattern);
	}
	public void DeleteLateralPattern(LateralDesign lateral, LateralPattern pattern)
	{
		_lateral.DeletePattern(lateral, pattern);
	}

#endregion

#region Query Map
	public bool LateralExists(LateralDesign lateral)
	{
		return _lateral.ComponentExists(lateral);
	}
	public bool LateralPatternExists(LateralPattern pattern)
	{
		return _lateral.PatternExists(pattern);
	}

#endregion

#region Getters
	public int GetLateralID(LateralPattern pattern)
	{
		return _lateral.GetComponentID(pattern);
	}
	public LateralDesign GetLateral(LateralPattern pattern)
	{
		return _lateral.GetComponent(pattern);
	}
	public LateralDesign GetLateral(int id)
	{
		return _lateral.GetComponent(id);
	}
	public LateralPattern GetLateralPattern(LateralPattern pattern)
	{
		return _lateral.GetPattern(pattern);
	}
#endregion

#if UNITY_EDITOR
#region List Getters
	public void AddLateralsToList(List<VoxelComponent> list)
	{
		_lateral.AddComponentsToList(list);
	}
	public void AddLateralsToList(List<LateralDesign> list)
	{
		_lateral.AddComponentsToList(list);
	}
	public void AddLateralPatternsToList(LateralDesign lateral, List<LateralPattern> list)
	{
		_lateral.AddPatternsToList(lateral, list);
	}
#endregion
#endif
#endregion

#region Longitudes
#region Modify Map
	public void AddLongitude(LongitudeDesign longitude)
	{
		_longitude.AddComponent(longitude);
	}
	public void ReplaceLongitude(LongitudeDesign oldlongitude, LongitudeDesign newlongitude)
	{
		_longitude.ReplaceComponent(oldlongitude, newlongitude);
	}
	public void DeleteLongitude(LongitudeDesign longitude)
	{
		_longitude.DeleteComponent(longitude);
	}
	public void AddLongitudePattern(LongitudeDesign longitude, LongitudePattern pattern)
	{
		_longitude.AddPattern(longitude, pattern);
	}
	public void DeleteLongitudePattern(LongitudeDesign longitude, LongitudePattern pattern)
	{
		_longitude.DeletePattern(longitude, pattern);
	}

#endregion

#region Query Map
	public bool LongitudeExists(LongitudeDesign longitude)
	{
		return _longitude.ComponentExists(longitude);
	}
	public bool LongitudePatternExists(LongitudePattern pattern)
	{
		return _longitude.PatternExists(pattern);
	}
#endregion

#region Getters
	public int GetLongitudeID(LongitudePattern pattern)
	{
		return _longitude.GetComponentID(pattern);
	}
	public LongitudeDesign GetLongitude(LongitudePattern pattern)
	{
		return _longitude.GetComponent(pattern);
	}
	public LongitudeDesign GetLongitude(int id)
	{
		return _longitude.GetComponent(id);
	}
	public LongitudePattern GetLongitudePattern(LongitudePattern pattern)
	{
		return _longitude.GetPattern(pattern);
	}
#endregion

#if UNITY_EDITOR
#region List Getters
	public void AddLongitudesToList(List<VoxelComponent> list)
	{
		_longitude.AddComponentsToList(list);
	}
	public void AddLongitudesToList(List<LongitudeDesign> list)
	{
		_longitude.AddComponentsToList(list);
	}
	public void AddLongitudePatternsToList(LongitudeDesign longitude, List<LongitudePattern> list)
	{
		_longitude.AddPatternsToList(longitude, list);
	}
#endregion
#endif
#endregion

#region Rects
#region Modify Map
	public void AddRect(RectDesign rect)
	{
		_rect.AddComponent(rect);
	}
	public void ReplaceRect(RectDesign oldrect, RectDesign newrect)
	{
		_rect.ReplaceComponent(oldrect, newrect);
	}
	public void DeleteRect(RectDesign rect)
	{
		_rect.DeleteComponent(rect);
	}
	public void AddRectPattern(RectDesign rect, RectPattern pattern)
	{
		_rect.AddPattern(rect, pattern);
	}
	public void DeleteRectPattern(RectDesign rect, RectPattern pattern)
	{
		_rect.DeletePattern(rect, pattern);
	}

#endregion

#region Query Map
	public bool RectExists(RectDesign rect)
	{
		return _rect.ComponentExists(rect);
	}
	public bool RectPatternExists(RectPattern pattern)
	{
		return _rect.PatternExists(pattern);
	}

#endregion

#region Getters
	public int GetRectID(RectPattern pattern)
	{
		return _rect.GetComponentID(pattern);
	}
	public RectDesign GetRect(RectPattern pattern)
	{
		return _rect.GetComponent(pattern);
	}
	public RectDesign GetRect(int id)
	{
		return _rect.GetComponent(id);
	}
	public RectPattern GetRectPattern(RectPattern pattern)
	{
		return _rect.GetPattern(pattern);
	}
#endregion

#if UNITY_EDITOR
#region List Getters
	public void AddRectsToList(List<VoxelComponent> list)
	{
		_rect.AddComponentsToList(list);
	}
	public void AddRectsToList(List<RectDesign> list)
	{
		_rect.AddComponentsToList(list);
	}
	public void AddRectPatternsToList(RectDesign rect, List<RectPattern> list)
	{
		_rect.AddPatternsToList(rect, list);
	}
#endregion
#endif
#endregion

#region Hexagons
#region Modify Map
	public void AddHexagon(HexagonDesign hexagon)
	{
		_hexagon.AddComponent(hexagon);
	}
	public void ReplaceHexagon(HexagonDesign oldhexagon, HexagonDesign newhexagon)
	{
		_hexagon.ReplaceComponent(oldhexagon, newhexagon);
	}
	public void DeleteHexagon(HexagonDesign hexagon)
	{
		_hexagon.DeleteComponent(hexagon);
	}
	public void AddHexagonPattern(HexagonDesign hexagon, HexagonPattern pattern)
	{
		_hexagon.AddPattern(hexagon, pattern);
	}
	public void DeleteHexagonPattern(HexagonDesign hexagon, HexagonPattern pattern)
	{
		_hexagon.DeletePattern(hexagon, pattern);
	}

#endregion

#region Query Map
	public bool HexagonExists(HexagonDesign hexagon)
	{
		return _hexagon.ComponentExists(hexagon);
	}
	public bool HexagonPatternExists(HexagonPattern pattern)
	{
		return _hexagon.PatternExists(pattern);
	}

#endregion

#region Getters
	public int GetHexagonID(HexagonPattern pattern)
	{
		return _hexagon.GetComponentID(pattern);
	}
	public HexagonDesign GetHexagon(HexagonPattern pattern)
	{
		return _hexagon.GetComponent(pattern);
	}
	public HexagonDesign GetHexagon(int id)
	{
		return _hexagon.GetComponent(id);
	}
	public HexagonPattern GetHexagonPattern(HexagonPattern pattern)
	{
		return _hexagon.GetPattern(pattern);
	}
#endregion

#if UNITY_EDITOR
#region List Getters
	public void AddHexagonsToList(List<VoxelComponent> list)
	{
		_hexagon.AddComponentsToList(list);
	}
	public void AddHexagonsToList(List<HexagonDesign> list)
	{
		_hexagon.AddComponentsToList(list);
	}
	public void AddHexagonPatternsToList(HexagonDesign hexagon, List<HexagonPattern> list)
	{
		_hexagon.AddPatternsToList(hexagon, list);
	}
#endregion
#endif
#endregion

}
