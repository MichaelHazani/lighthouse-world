using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

// Deprecated
[CustomEditor(typeof(SplineComponent))]
public class SplineEditor : Editor
{
	public float m_point_size = 0.05f;
	protected SplineComponent m_spl = null;
	protected int m_selidx = -1, m_subselidx = -1, m_drawidx = 0;
	protected bool m_drawselected = false, m_drawsubselected = false;
	
	void OnEnable()
    {
		m_spl = (SplineComponent)target;
    }
	
    void OnDisable()
    {
		ToolsUtil.Hidden = false;
    }
	
	void OnSceneGUI()
	{
		SplineComponent.SplineComponentPoint pt = null;
		bool selected = false, drwcp1 = false, drwcp2 = false;
		float hsize, hcsize1, hcsize2;
		
		Undo.SetSnapshotTarget(m_spl, "SplineComponent Modify");
		
		Handles.matrix = m_spl.transform.localToWorldMatrix;
		
		ToolsUtil.Hidden = (m_selidx != -1);
		
		int idx = 0;
		while(idx < m_spl.m_points.Count)
		{
			pt = m_spl.m_points[idx];
			//
			hsize = HandleUtility.GetHandleSize(pt.m_point);
			if(m_selidx == idx)
			{
				if(m_subselidx >= 0)
				{
					m_drawselected = false;
				}
				else
				{
					m_drawselected = true;
				}
			}
			else
			{
				m_drawselected = false;
			}
			m_drawidx = idx;
			
			if(Handles.Button(pt.m_point, Quaternion.identity, hsize * m_point_size, hsize * m_point_size, DrawPoint))
			{
				m_selidx = idx;
				m_subselidx = -1;
				selected = true;
			}
			if(m_selidx == idx)
			{
				drwcp1 = CanDrawControlPoint1(idx);
				drwcp2 = CanDrawControlPoint2(idx);
				Handles.color = new Color(1, 1, 0, 1);
				if(drwcp1)
				{
					Handles.DrawLine(pt.m_point, pt.m_point + pt.m_control1);
				}
				if(drwcp2)
				{
					Handles.DrawLine(pt.m_point, pt.m_point + pt.m_control2);
				}
				Handles.color = new Color(1, 1, 1, 1);
				
				if(drwcp1)
				{
					if(m_subselidx == 0)
					{
						m_drawsubselected = true;
					}
					else
					{
						m_drawsubselected = false;
					}
					hcsize1 = HandleUtility.GetHandleSize(pt.m_control1);
					m_drawidx = 1;
					if(Handles.Button(pt.m_point + pt.m_control1, Quaternion.identity, hcsize1 * m_point_size, hcsize1 * m_point_size, DrawControlPoint))
					{
						m_subselidx = 0;
						selected = true;
					}
				}
				if(drwcp2)
				{
					if(m_subselidx == 1)
					{
						m_drawsubselected = true;
					}
					else
					{
						m_drawsubselected = false;
					}
					hcsize2 = HandleUtility.GetHandleSize(pt.m_control2);
					m_drawidx = 2;
					if(Handles.Button(pt.m_point + pt.m_control2, Quaternion.identity, hcsize2 * m_point_size, hcsize2 * m_point_size, DrawControlPoint))
					{
						m_subselidx = 1;
						selected = true;
					}
				}
			}
			if(idx == m_selidx)
			{
				if(m_subselidx == -1)
				{
					if(Tools.current == Tool.Move)
					{
						pt.m_point = Handles.PositionHandle(pt.m_point, Quaternion.identity);
					}
				}
				else if(m_subselidx == 0)
				{
					if(Tools.current == Tool.Move)
					{
						pt.m_control1 = Handles.PositionHandle(pt.m_control1 + pt.m_point, Quaternion.identity) - pt.m_point;
					}
				}
				else if(m_subselidx == 1)
				{
					if(Tools.current == Tool.Move)
					{
						if(m_spl.m_points[m_selidx].m_type == BaseSpline.SplinePointType.Bezier)
						{
							pt.m_control1 = -(Handles.PositionHandle(pt.m_control2 + pt.m_point, Quaternion.identity) - pt.m_point);
						}
						else
						{
							pt.m_control2 = Handles.PositionHandle(pt.m_control2 + pt.m_point, Quaternion.identity) - pt.m_point;
						}
					}
				}
			}
			++idx;
		}
		if(m_spl.WrapMode == BaseSpline.SplineWrapMode.Loop)
		{
			
		}
		//
		if(GUI.changed)
		{
			m_spl.Build();
			EditorUtility.SetDirty(m_spl);
			Repaint();
			
			Event e = Event.current;
			if(e.isMouse && e.button == 0 && e.type == EventType.MouseUp)
			{
				Undo.CreateSnapshot();
				Undo.RegisterSnapshot();
			}
		}
		else if(selected)
		{
			Repaint();
			SceneView.RepaintAll();
		}
	}
		
	void DrawPoint(int controlID, Vector3 position, Quaternion rotation, float size)
	{
		if(m_drawselected)
		{
			Handles.color = new Color(1, 0, 0, 1);
		}
		else
		{
			Handles.color = new Color(1, 1, 1, 1);
		}
		Handles.DotCap(controlID, position, rotation, size);
		Handles.Label(position, m_drawidx.ToString());
	}
	
	void DrawControlPoint(int controlID, Vector3 position, Quaternion rotation, float size)
	{
		if(m_drawsubselected)
		{
			Handles.color = new Color(1, 0, 0, 1);
		}
		else
		{
			Handles.color = new Color(0, 1, 0, 1);
		}
		Handles.DotCap(controlID, position, rotation, size);
		if(m_drawidx == 1)
		{
			Handles.Label(position, "-");
		}
		else
		{
			Handles.Label(position, "+");
		}
	}
	
	bool CanDrawControlPoint1(int idx)
	{
		bool retval = false;
		if(m_spl.WrapMode == BaseSpline.SplineWrapMode.Loop)
		{
			switch(m_spl.m_points[idx].m_type)
			{
			case BaseSpline.SplinePointType.Bezier:
			case BaseSpline.SplinePointType.BezierCorner:
				retval = true;
				break;
			default:
				retval = false;
				break;
			}
		}
		else
		{
			switch(m_spl.m_points[idx].m_type)
			{
			case BaseSpline.SplinePointType.Bezier:
			case BaseSpline.SplinePointType.BezierCorner:
				if(idx == 0)
				{
					retval = false;
				}
				else
				{
					retval = true;
				}
				break;
			default:
				retval = false;
				break;
			}
		}
		return retval;
	}
	
	bool CanDrawControlPoint2(int idx)
	{
		bool retval = false;
		if(m_spl.WrapMode == BaseSpline.SplineWrapMode.Loop)
		{
			switch(m_spl.m_points[idx].m_type)
			{
			case BaseSpline.SplinePointType.Bezier:
			case BaseSpline.SplinePointType.BezierCorner:
				retval = true;
				break;
			default:
				retval = false;
				break;
			}
		}
		else
		{
			switch(m_spl.m_points[idx].m_type)
			{
			case BaseSpline.SplinePointType.Bezier:
			case BaseSpline.SplinePointType.BezierCorner:
				if(idx == m_spl.m_points.Count - 1)
				{
					retval = false;
				}
				else
				{
					retval = true;
				}
				break;
			default:
				retval = false;
				break;
			}
		}
		return retval;
	}
	
	override public void OnInspectorGUI()
    {
		bool addremove = false, selected = false, drwcp1 = false, drwcp2 = false;
		
		Undo.SetSnapshotTarget(m_spl, "SplineComponent Modify");
		
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField("Point count", m_spl.m_points.Count.ToString());
		EditorGUILayout.LabelField("Length", m_spl.Length.ToString());
		m_spl.WrapMode = (BaseSpline.SplineWrapMode)EditorGUILayout.EnumPopup("Wrap mode", m_spl.WrapMode);
		
		m_spl.InterpolationSteps = EditorGUILayout.IntSlider("Steps", m_spl.InterpolationSteps, 1, 64);
				
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.BeginHorizontal();
		if(GUILayout.Button("Append point"))
		{
			if(m_spl.m_points.Count == 0)
			{
				m_spl.AppendPoint(Vector3.zero, BaseSpline.SplinePointType.Corner, Vector3.zero, Vector3.zero);
			}
			else
			{
				m_spl.AppendPoint(m_spl.m_points[m_spl.m_points.Count - 1].m_point + Vector3.right, BaseSpline.SplinePointType.Corner, Vector3.zero, Vector3.zero);
			}
			m_selidx = m_spl.m_points.Count - 1;
			addremove = true;
		}
		if(GUILayout.Button("Remove last"))
		{
			if(m_spl.m_points.Count > 0)
			{
				m_spl.RemoveLastPoint();
			}
			if(m_selidx >= m_spl.m_points.Count)
			{
				m_selidx = m_spl.m_points.Count - 1;
			}
			addremove = true;
		}
		if(GUILayout.Button("Reverse points"))
		{
			m_spl.ReversePoints();
			addremove = true;
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.BeginHorizontal();
		if(m_spl.m_points.Count > 0)
		{
			if(GUILayout.Button("Select first"))
			{
				m_selidx = 0;
				selected = true;
			}
			if(m_selidx != -1)
			{
				if(GUILayout.Button("Select next"))
				{
					++m_selidx;
					if(m_selidx >= m_spl.m_points.Count)
					{
						m_selidx = 0;
					}
					selected = true;
				}
				if(GUILayout.Button("Select previous"))
				{
					--m_selidx;
					if(m_selidx < 0)
					{
						m_selidx = m_spl.m_points.Count - 1;
					}
					selected = true;
				}
			}
		}
		EditorGUILayout.EndHorizontal();

		if(m_selidx != -1)
		{
			drwcp1 = CanDrawControlPoint1(m_selidx);
			drwcp2 = CanDrawControlPoint2(m_selidx);
			
			m_spl.m_points[m_selidx].m_type = (BaseSpline.SplinePointType)EditorGUILayout.EnumPopup("Point type", m_spl.m_points[m_selidx].m_type);
			m_spl.m_points[m_selidx].m_point = EditorGUILayout.Vector3Field("Position", m_spl.m_points[m_selidx].m_point);
			if(drwcp1)
			{
				m_spl.m_points[m_selidx].m_control1 = EditorGUILayout.Vector3Field("Control -", m_spl.m_points[m_selidx].m_control1);
			}
			if(drwcp2)
			{
				m_spl.m_points[m_selidx].m_control2 = EditorGUILayout.Vector3Field("Control +", m_spl.m_points[m_selidx].m_control2);
			}
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Insert before"))
			{
				if(m_spl.m_points.Count == 1)
				{
					m_spl.InsertPoint(0, m_spl.m_points[m_spl.m_points.Count - 1].m_point + Vector3.right, BaseSpline.SplinePointType.Corner, Vector3.zero, Vector3.zero);
				}
				else
				{
					int previdx = m_selidx;
					--m_selidx;
					if(m_selidx < 0)
					{
						m_selidx = m_spl.m_points.Count - 1;
					}
					m_spl.InsertPoint(previdx, (m_spl.m_points[m_selidx].m_point + m_spl.m_points[previdx].m_point) * 0.5f, BaseSpline.SplinePointType.Corner, Vector3.zero, Vector3.zero);
					m_selidx = previdx;
				}
				addremove = true;
			}
			if(GUILayout.Button("Insert after"))
			{
				if(m_spl.m_points.Count == 1)
				{
					m_spl.InsertPoint(0, m_spl.m_points[m_spl.m_points.Count - 1].m_point + Vector3.right, BaseSpline.SplinePointType.Corner, Vector3.zero, Vector3.zero);
				}
				else
				{
					int previdx = m_selidx;
					++m_selidx;
					if(m_selidx == m_spl.m_points.Count)
					{
						m_selidx = m_spl.m_points.Count - 1;
					}
					m_spl.InsertPoint(m_selidx, (m_spl.m_points[m_selidx].m_point + m_spl.m_points[previdx].m_point) * 0.5f, BaseSpline.SplinePointType.Corner, Vector3.zero, Vector3.zero);
				}
				addremove = true;
			}
			if(GUILayout.Button("Remove"))
			{
				m_spl.m_points.RemoveAt(m_selidx);
				if(m_selidx >= m_spl.m_points.Count)
				{
					m_selidx = m_spl.m_points.Count - 1;
				}
				addremove = true;
			}
			EditorGUILayout.EndHorizontal();
		}

		if(GUI.changed || addremove)
		{
			m_spl.Build();
			EditorUtility.SetDirty(m_spl);
			Repaint();
			
			Event e = Event.current;
			if(e.isMouse && e.button == 0 && e.type == EventType.MouseUp)
			{
				Undo.CreateSnapshot();
				Undo.RegisterSnapshot();
			}
		}
		if(selected)
		{
			Repaint();
			SceneView.RepaintAll();
			GUIUtility.keyboardControl = 0;
		}
    }
	
	[MenuItem("GameObject/Create Other/SplineComponent")]
	public static void CreateUniSplineComponent()
	{
		GameObject go = new GameObject("Spline");
		go.AddComponent(typeof(SplineComponent));
	}
}
