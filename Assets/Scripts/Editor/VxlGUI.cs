using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public static class VxlGUI
{
	public const int MINWIDTH_WINDOW = 300;
	public const int MAXWIDTH_WINDOW = 500;

	public const int LRG_BAR = 30;
	public const int MED_BAR = 20;
	public const int SM_BAR = 16;

	public const int SM_SPACE = 2;
	public const int MED_SPACE = 6;
	public const int LRG_SPACE = 10;

	public const int SM_PAD = 4;
	public const int MED_PAD = 12;
	public const int LRG_PAD = 20;

	public const int MODE = 30;
	public const int PANEL_TITLE = 24;
	public const int HEADER_TITLE = 12;

	public readonly static Mesh VertexMesh;

	static VxlGUI()
	{
		GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		Mesh mesh = sphere.GetComponent<MeshFilter>().sharedMesh;
		VertexMesh = new Mesh();
		VertexMesh.vertices = (Vector3[]) mesh.vertices.Clone();
		VertexMesh.triangles = (int[]) mesh.triangles.Clone();
		VertexMesh.normals = (Vector3[]) mesh.normals.Clone();
		VertexMesh.uv = (Vector2[]) mesh.uv.Clone();
		VertexMesh.RecalculateBounds();
		VertexMesh.RecalculateTangents();
		VertexMesh.UploadMeshData(false);

		GameObject.DestroyImmediate(sphere);
	}

	public static Rect GetAboveRow(Rect rect, int space, float factor)
	{
		return new Rect(
			rect.x,
			rect.y,
			rect.width,
			factor * (rect.height - space)
		);
	}
	public static Rect GetBelowRow(Rect rect, int space, float factor)
	{
		float height = factor * (rect.height - space);
		return new Rect(
			rect.x,
			rect.yMax - height,
			rect.width,
			height
		);
	}
	public static Rect GetLeftColumn(Rect rect, int space, float factor)
	{
		return new Rect(
			rect.x,
			rect.y,
			factor * (rect.width - space),
			rect.height
		);
	}
	public static Rect GetRightColumn(Rect rect, int space, float factor)
	{
		float width = factor * (rect.width - space);
		return new Rect(
			rect.xMax - width,
			rect.y,
			width,
			rect.height
		);
	}
	public static Rect GetAboveElement(Rect rect, int index, float element_height)
	{
		return GetAboveElement(rect, index, element_height, 0, 0);
	}
	public static Rect GetAboveElement(Rect rect, int index, float element_height, float offset)
	{
		return GetAboveElement(rect, index, element_height, 0, offset);
	}
	public static Rect GetAboveElement(Rect rect, int index, float element_height, int space, float offset)
	{
		return new Rect(
			rect.x,
			rect.y + offset + (index * (element_height + space)),
			rect.width,
			element_height
		);
	}
	public static Rect GetBelowElement(Rect rect, int index, float element_height)
	{
		return GetBelowElement(rect, index, element_height, 0, 0);
	}
	public static Rect GetBelowElement(Rect rect, int index, float element_height, float offset)
	{
		return GetBelowElement(rect, index, element_height, 0, offset);
	}
	public static Rect GetBelowElement(Rect rect, int index, float element_height, int space, float offset)
	{
		return new Rect(
			rect.x,
			rect.yMax + space - offset - ((index + 1) * (element_height + space)),
			rect.width,
			element_height
		);
	}
	public static Rect GetLeftElement(Rect rect, int index, float element_width)
	{
		return GetLeftElement(rect, index, element_width, 0, 0);
	}
	public static Rect GetLeftElement(Rect rect, int index, float element_width, float offset)
	{
		return GetLeftElement(rect, index, element_width, 0, offset);
	}
	public static Rect GetLeftElement(Rect rect, int index, float element_width, int space, float offset)
	{
		return new Rect(
			rect.x + offset + (index * (element_width + space)),
			rect.y,
			element_width,
			rect.height
		);
	}
	public static Rect GetRightElement(Rect rect, int index, float element_width)
	{
		return GetRightElement(rect, index, element_width, 0, 0);
	}
	public static Rect GetRightElement(Rect rect, int index, float element_width, float offset)
	{
		return GetRightElement(rect, index, element_width, 0, offset);
	}
	public static Rect GetRightElement(Rect rect, int index, float element_width, int space, float offset)
	{
		return new Rect(
			rect.xMax + space - offset - ((index + 1) * (element_width + space)),
			rect.y,
			element_width,
			rect.height
		);
	}

	public static Rect GetAboveLeftElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.x + xoffset,
			rect.y + yoffset,
			element_width,
			element_height
		);
	}
	public static Rect GetAboveRightElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.xMax - xoffset - element_width,
			rect.y + yoffset,
			element_width,
			element_height
		);
	}
	public static Rect GetBelowLeftElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.x + xoffset,
			rect.yMax - yoffset - element_height,
			element_width,
			element_height
		);
	}
	public static Rect GetBelowRightElement(Rect rect, float xoffset, float element_width, float yoffset, float element_height)
	{
		return new Rect(
			rect.xMax - xoffset - element_width,
			rect.yMax - yoffset - element_height,
			element_width,
			element_height
		);
	}

	public static Rect GetPaddedRect(Rect rect, int padding)
	{
		return new Rect(
			rect.x + padding,
			rect.y + padding,
			rect.width - padding - padding,
			rect.height - padding - padding
		);
	}

	public static Rect GetPaddedRect(Rect rect, int vertical, int horizontal)
	{
		return new Rect(
			rect.x + horizontal,
			rect.y + vertical,
			rect.width - horizontal - horizontal,
			rect.height - vertical - vertical
		);
	}

	public static Rect GetPaddedRect(Rect rect, int above, int below, int left, int right)
	{
		return new Rect(
			rect.x + left,
			rect.y + above,
			rect.width - left - right,
			rect.height - above - below
		);
	}

	public static Rect GetSandwichedRectX(Rect rect, float before, float after)
	{
		return new Rect(
			rect.x + before,
			rect.y,
			rect.width - before - after,
			rect.height
		);
	}

	public static Rect GetSandwichedRectY(Rect rect, float before, float after)
	{
		return new Rect(
			rect.x,
			rect.y + before,
			rect.width,
			rect.height - before - after
		);
	}

	public static Rect GetMiddleX(Rect rect, float width)
	{
		return new Rect(rect.center.x - (width / 2f), rect.y, width, rect.height);
	}
	public static Rect GetMiddleY(Rect rect, float height)
	{
		return new Rect(rect.x, rect.center.y - (height / 2f), rect.width, height);
	}

	public static Rect CenterInvertRect(Rect rect)
	{
		float xoffset = rect.xMin - rect.center.x;
		float yoffset = rect.yMin - rect.center.y;
		return new Rect(rect.center.x + yoffset, rect.center.y + xoffset, rect.height, rect.width);
	}

	public static void DrawRect(Rect rect, string name)
	{
		DrawRect(rect, name, false, false, false, false);
	}
	public static void DrawRect(Rect rect, string name, bool hover, bool active, bool on, bool focus)
	{
		if (Event.current.type == EventType.Repaint) {
			GUIStyle style = GUI.skin.GetStyle(name);
			if (style != null) {
				style.Draw(rect, hover, active, on, focus);
			}
		}
	}
	public static Rect GetScrollViewRect(ReorderableList list, float width, float scrollheight)
	{
		float height = 0;
		if (list != null) {
			height += list.GetHeight();
		}
		GUIStyle vscroll_style = GUI.skin.verticalScrollbar;
		if (vscroll_style != null && height > scrollheight) {
			width -= vscroll_style.fixedWidth;
		}
		return new Rect(0, 0, width, height);
	}
}
