using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

[CustomEditor(typeof(UniSplineComponent))]
public class UniSplineEditor : Editor
{
	private const float m_point_size = 0.05f;
	private UniSplineComponent m_splinecomp = null;
	private UniSpline m_spline = null;
	private UniSpline.EditHelper m_edithelper;
	
	void OnEnable()
    {
		if(AssetDatabase.Contains(target))
		{
			m_splinecomp = null;
			m_spline = null;
			m_edithelper = null;
		}
		else
		{
			m_splinecomp = (UniSplineComponent)target;
			m_spline = m_splinecomp.Spline;
			m_edithelper = m_spline.GetEditHelper();
		}
    }
	
    void OnDisable()
    {
		ToolsUtil.Hidden = false;
    }
	
	void OnSceneGUI()
	{
		if(m_splinecomp != null)
		{
			bool selected = false;
			float hsize;
			
			Undo.SetSnapshotTarget(m_splinecomp, "UniSpline Modify");
			
			Handles.matrix = m_splinecomp.transform.localToWorldMatrix;
			
			ToolsUtil.Hidden = m_edithelper.SomethingSelected;
			
			m_edithelper.Reset();
			while(m_edithelper.MoveNext())
			{
				hsize = HandleUtility.GetHandleSize(m_edithelper.Point);
				if(Handles.Button(m_edithelper.Point, Quaternion.identity, hsize * m_point_size, hsize * m_point_size, DrawPoint))
				{
					m_edithelper.Selected = true;
					selected = true;
				}
				if(m_edithelper.Selected)
				{
					if(Tools.current == Tool.Move)
					{
						m_edithelper.Point = Handles.PositionHandle(m_edithelper.Point, Quaternion.identity);
					}
				}
			}

			if(GUI.changed)
			{
				m_spline.Build();
				EditorUtility.SetDirty(m_splinecomp);
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
	}
		
	void DrawPoint(int controlID, Vector3 position, Quaternion rotation, float size)
	{
		if(m_edithelper.Selected)
		{
			Handles.color = new Color(1, 0, 0, 1);
		}
		else
		{
			Handles.color = new Color(1, 1, 1, 1);
		}
		Handles.DotCap(controlID, position, rotation, size);
		Handles.Label(position, m_edithelper.Index.ToString());
	}
	
	override public void OnInspectorGUI()
    {
		if(m_splinecomp != null)
		{
			bool addremove = false, selected = false;
			
			Undo.SetSnapshotTarget(m_splinecomp, "UniSpline Modify");
			
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Point count", m_spline.GetPointCount().ToString());
			EditorGUILayout.LabelField("Length", m_spline.Length.ToString());
			m_spline.SplineType = (BaseSpline.UniSplineType)EditorGUILayout.EnumPopup("Type", m_spline.SplineType);
			m_spline.WrapMode = (BaseSpline.SplineWrapMode)EditorGUILayout.EnumPopup("Wrap mode", m_spline.WrapMode);
			m_spline.ReparamType = (BaseSpline.SplineReparamType)EditorGUILayout.EnumPopup("Reparameterization", m_spline.ReparamType);
			if(m_spline.ReparamType != BaseSpline.SplineReparamType.None)
			{
				m_spline.StepCount = EditorGUILayout.IntSlider("Step count", m_spline.StepCount, 1, 64);
			}
			
			switch(m_spline.SplineType)
			{
			case BaseSpline.UniSplineType.CatmullRom: break;
			case BaseSpline.UniSplineType.Hermite:
				m_spline.Tension = EditorGUILayout.FloatField("Tension", m_spline.Tension);
				m_spline.Bias = EditorGUILayout.FloatField("Bias", m_spline.Bias);
				break;
			case BaseSpline.UniSplineType.KochanekBartels:
				m_spline.Tension = EditorGUILayout.FloatField("Tension", m_spline.Tension);
				m_spline.Continuity = EditorGUILayout.FloatField("Continuity", m_spline.Continuity);
				m_spline.Bias = EditorGUILayout.FloatField("Bias", m_spline.Bias);
				break;
			}
			
			EditorGUILayout.EndVertical();
			
			EditorGUILayout.BeginHorizontal();
			if(GUILayout.Button("Append point"))
			{
				m_edithelper.AppendPoint();
				addremove = true;
			}
			if(GUILayout.Button("Remove last"))
			{
				m_edithelper.RemoveLast();
				addremove = true;
			}
			if(GUILayout.Button("Reverse points"))
			{
				m_spline.ReversePoints();
				addremove = true;
			}
			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.BeginHorizontal();
			if(m_spline.GetPointCount() > 0)
			{
				if(GUILayout.Button("Select first"))
				{
					m_edithelper.SelectFirst();
					selected = true;
				}
				if(m_edithelper.SomethingSelected)
				{
					if(GUILayout.Button("Select next"))
					{
						m_edithelper.SelectNext();
						selected = true;
					}
					if(GUILayout.Button("Select previous"))
					{
						m_edithelper.SelectPrev();
						selected = true;
					}
				}
			}
			EditorGUILayout.EndHorizontal();
	
			if(m_edithelper.SomethingSelected)
			{
				m_edithelper.SelectedPoint = EditorGUILayout.Vector3Field("Position", m_edithelper.SelectedPoint);
				
				EditorGUILayout.BeginHorizontal();
				if(GUILayout.Button("Insert before"))
				{
					m_edithelper.InsertBefore();
					addremove = true;
				}
				if(GUILayout.Button("Insert after"))
				{
					m_edithelper.InsertAfter();
					addremove = true;
				}
				if(GUILayout.Button("Remove"))
				{
					m_edithelper.Remove();
					addremove = true;
				}
				EditorGUILayout.EndHorizontal();
			}
	
			if(GUI.changed || addremove)
			{
				m_spline.Build();
				EditorUtility.SetDirty(m_splinecomp);
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
    }
	
	[MenuItem("GameObject/Create Other/UniSpline")]
	public static void CreateUniSpline()
	{
		GameObject go = new GameObject("UniSpline");
		go.AddComponent(typeof(UniSplineComponent));
	}
}