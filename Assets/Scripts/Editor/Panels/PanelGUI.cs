using System.Collections.Generic;
using UnityEngine;

public abstract class PanelGUI
{
	protected bool _repaint_menu;
	protected bool _update_mesh;
	protected bool _render_mesh;
	
	protected string _title;

	public abstract void Enable();
	public abstract void Disable();
	public abstract void DrawGUI(Rect rect);
	public virtual void DrawPreview(Rect rect) { }

	public virtual bool repaintMenu {
		get => _repaint_menu;
		set => _repaint_menu = value;
	}

	public virtual bool updateMesh {
		get => _update_mesh;
		set => _update_mesh = value;
	}

	public virtual bool renderMesh {
		get => _render_mesh;
		set => _render_mesh = value;
	}

	protected void ReorderMaintainSelection(int old_index, int new_index, List<int> select_list, HashSet<int> select_set)
	{
		if (old_index > new_index) {
			bool is_oldselected = select_set.Remove(old_index);
			for (int k = old_index; k >= new_index + 1; k--) {
				if (select_set.Remove(k - 1)) {
					select_set.Add(k);
					int listindex = select_list.IndexOf(k - 1);
					if (listindex >= 0) {
						select_list[listindex] = k;
					}
				}
			}
			if (is_oldselected) {
				select_set.Add(new_index);
				int listindex = select_list.IndexOf(old_index);
				if (listindex >= 0) {
					select_list[listindex] = new_index;
				}
			}
		}
		else {
			bool is_oldselected = select_set.Remove(old_index);
			for (int k = new_index - 1; k >= old_index; k--) {
				if (select_set.Remove(k + 1)) {
					select_set.Add(k);
					int listindex = select_list.IndexOf(k + 1);
					if (listindex >= 0) {
						select_list[listindex] = k;
					}
				}
			}
			if (is_oldselected) {
				select_set.Add(new_index);
				int listindex = select_list.IndexOf(old_index);
				if (listindex >= 0) {
					select_list[listindex] = new_index;
				}
			}
		}
	}

	protected void SelectListElement(int index, List<int> selectlist, HashSet<int> selectset)
	{
		if (Event.current.control) {
			int select_cnt = selectset.Count;
			if (select_cnt <= 0) {
				selectset.Add(index);
				selectlist.Add(index);
			}
			else {
				int last_index = selectlist[select_cnt - 1];
				int sm_index = Mathf.Min(index, last_index);
				int bg_index = Mathf.Max(index, last_index);
				for (int k = sm_index; k <= bg_index; k++) {
					if (selectset.Add(k)) {
						selectlist.Add(k);
					}
				}
			}
		}
		else if (Event.current.shift) {
			if (selectset.Add(index)) {
				selectlist.Add(index);
			}
			else {
				selectset.Remove(index);
				selectlist.Remove(index);
			}
		}
		else {
			selectset.Clear();
			selectset.Add(index);
			selectlist.Clear();
			selectlist.Add(index);
		}
	}
}
