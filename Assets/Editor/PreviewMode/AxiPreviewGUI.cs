using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AxiPreviewGUI : TogglePreviewGUI
{
	private const float SCALE = 0.01f;
	private const float BOUNDSLENGTH = 4f;
	private readonly static Color COLOUR_ORIGIN = new Color(1, 1, 1, 0.8f);
	private readonly static Color COLOUR_XAXIS = new Color(1, 102 / 255f, 0, 0.8f);
	private readonly static Color COLOUR_YAXIS = new Color(0, 204 / 255f, 102 / 255f, 0.8f);
	private readonly static Color COLOUR_ZAXIS = new Color(0, 153 / 255f, 1, 0.8f);
	private readonly static Color[] COLOUR_AXI = new Color[] {
		new Color(0, 224 / 255f, 245 / 255f),//above
		new Color(0, 167 / 255f, 250 / 255f),//below
		new Color(153 / 255f, 204 / 255f, 1, 0.8f),//d0
		new Color(102 / 255f, 153 / 255f, 1, 0.8f),//d1
		new Color(102 / 255f, 102 / 255f, 1, 0.8f),//d2
		new Color(153 / 255f, 102 / 255f, 1, 0.8f),//d3
		new Color(153 / 255f, 51 / 255f, 1, 0.8f),//d4
		new Color(102 / 255f, 0, 204 / 255f, 0.8f)//d5
	};

	public Mesh origin_mesh;
	public Mesh axi_mesh;
	public Material material;

	private MaterialPropertyBlock _propblock;

	public AxiPreviewGUI()
	{
		_title = "Display";
		_row = 0;
		_column = 0;
		_options = new List<ToggleOption>() {
			new ToggleOption(0, "Origin", true, true, false, true),
			new ToggleOption(0, "XYZ", false, true, false, true),
			new ToggleOption(0, "Hexagon Axi", true, true, false, true)
		};
		_propblock = new MaterialPropertyBlock();
	}

	public override void Enable()
	{
		_propblock.Clear();
		_scroll = Vector2.zero;
		_expanded = false;
	}

	public override void Disable()
	{
		_propblock.Clear();
		_scroll = Vector2.zero;
		_expanded = false;
	}

	public override void DrawDisplayObjects(PreviewRenderUtility prevutility)
	{
		if (_options[0].value) {
			DrawOriginDisplayObject(prevutility);
		}
		if (_options[1].value) {
			DrawGlobalAxisDisplayObject(prevutility);
		}
		if (_options[2].value) {
			DrawAxiDisplayObject(prevutility);
		}
	}

	private void DrawOriginDisplayObject(PreviewRenderUtility prevutility)
	{
		float length = prevutility.camera.transform.position.magnitude;
		float distance_factor = Mathf.Max(0.001f, Mathf.Clamp01(length / BOUNDSLENGTH));
		Vector3 scale = SCALE * distance_factor * Vector3.one;
		_propblock.SetColor("_Color", COLOUR_ORIGIN);
		DrawDisplayObject(prevutility, origin_mesh, Vector3.zero, Quaternion.identity, scale);
	}

	private void DrawGlobalAxisDisplayObject(PreviewRenderUtility preview)
	{
		float length = preview.camera.transform.position.magnitude;
		float distance_factor = Mathf.Max(0.001f, Mathf.Clamp01(length / BOUNDSLENGTH));
		//y axis
		Vector3 pos = 2 * SCALE * distance_factor * Vector3.up;
		Quaternion rot = Quaternion.identity;
		Vector3 scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_YAXIS);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//x axis
		pos = 2 * SCALE * distance_factor * Vector3.right;
		rot = Quaternion.Euler(0, 0, -90);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_XAXIS);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//z axis
		pos = 2 * SCALE * distance_factor * Vector3.forward;
		rot = Quaternion.Euler(90, 0, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_ZAXIS);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
	}

	private void DrawAxiDisplayObject(PreviewRenderUtility preview)
	{
		float length = preview.camera.transform.position.magnitude;
		float distance_factor = Mathf.Max(0.001f, Mathf.Clamp01(length / BOUNDSLENGTH));
		//above
		Vector3 pos = (2 * SCALE * distance_factor * Vector3.up);
		Vector3 scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[0]);
		DrawDisplayObject(preview, axi_mesh, pos, Quaternion.identity, scale);
		//below
		pos = (2 * SCALE * distance_factor * Vector3.down);
		Quaternion rot = Quaternion.Euler(180, 0, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[1]);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//d0
		pos = Quaternion.Euler(0, 60, 0) * (2 * SCALE * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 60, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[2]);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//d1
		pos = Quaternion.Euler(0, 120, 0) * (2 * SCALE * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 120, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[3]);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//d2
		pos = Quaternion.Euler(0, 180, 0) * (2 * SCALE * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 180, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[4]);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//d3
		pos = Quaternion.Euler(0, 240, 0) * (2 * SCALE * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 240, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[5]);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//d4
		pos = Quaternion.Euler(0, 300, 0) * (2 * SCALE * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 300, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[6]);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
		//d5
		pos = (2 * SCALE * distance_factor * Vector3.forward);
		rot = Quaternion.Euler(90, 0, 0);
		scale = new Vector3(SCALE * distance_factor, 100f, SCALE * distance_factor);
		_propblock.SetColor("_Color", COLOUR_AXI[7]);
		DrawDisplayObject(preview, axi_mesh, pos, rot, scale);
	}

	private void DrawDisplayObject(PreviewRenderUtility preview, Mesh mesh, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		if (preview == null || mesh == null || scale == Vector3.zero) {
			return;
		}
		preview.DrawMesh(mesh, pos, scale, rot, material, 0, _propblock, null, false);
	}
}
