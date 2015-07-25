using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class UniSpline : BaseSpline
{
	public class EditHelper
	{
		internal EditHelper(UniSpline spline)
		{
			m_spline = spline;
		}
		
		public bool MoveNext()
		{
			++m_idx;
			if(m_idx < m_spline.m_points.Count)
			{
				return true;
			}
			return false;
		}
		
		public void Reset()
		{
			m_idx = -1;
		}
		
		public void AppendPoint()
		{
			if(m_spline.m_points.Count == 0)
			{
				m_spline.AppendPoint(Vector3.zero);
			}
			else
			{
				m_spline.AppendPoint(m_spline.m_points[m_spline.m_points.Count - 1] + Vector3.right);
			}
			m_selidx = m_spline.m_points.Count - 1;			
		}
		
		public void InsertBefore()
		{
			if(m_spline.m_points.Count == 1)
			{
				m_spline.InsertPoint(0, m_spline.m_points[m_spline.m_points.Count - 1] + Vector3.right);
			}
			else
			{
				int previdx = m_selidx;
				--m_selidx;
				if(m_selidx < 0)
				{
					m_selidx = m_spline.m_points.Count - 1;
				}
				m_spline.InsertPoint(previdx, (m_spline.m_points[m_selidx] + m_spline.m_points[previdx]) * 0.5f);
				m_selidx = previdx;
			}
		}
		
		public void InsertAfter()
		{
			if(m_spline.m_points.Count == 1)
			{
				m_spline.InsertPoint(0, m_spline.m_points[m_spline.m_points.Count - 1] + Vector3.right);
			}
			else
			{
				int previdx = m_selidx;
				++m_selidx;
				if(m_selidx == m_spline.m_points.Count)
				{
					m_selidx = m_spline.m_points.Count - 1;
				}
				m_spline.InsertPoint(m_selidx, (m_spline.m_points[m_selidx] + m_spline.m_points[previdx]) * 0.5f);
			}
		}
		
		public void Remove()
		{
			m_spline.m_points.RemoveAt(m_selidx);
			if(m_selidx >= m_spline.m_points.Count)
			{
				m_selidx = m_spline.m_points.Count - 1;
			}
		}
		
		public void RemoveLast()
		{
			if(m_spline.m_points.Count > 0)
			{
				m_spline.RemoveLastPoint();
			}
			if(m_selidx >= m_spline.m_points.Count)
			{
				m_selidx = m_spline.m_points.Count - 1;
			}
		}
		
		public void SelectFirst()
		{
			if(m_spline.m_points.Count > 0)
			{
				m_selidx = 0;
			}
			else
			{
				m_selidx = -1;
			}
		}
		
		public void SelectNext()
		{
			if(m_selidx < m_spline.m_points.Count - 1)
			{
				++m_selidx;
			}
			else
			{
				m_selidx = 0;
			}
		}
		
		public void SelectPrev()
		{
			if(m_selidx > 0)
			{
				--m_selidx;
			}
			else
			{
				m_selidx = m_spline.m_points.Count - 1;
			}
		}
		
		private UniSpline m_spline;
		private int m_idx = -1, m_selidx = -1;
		
		public Vector3 Point
		{
			get { return m_spline.m_points[m_idx]; }
			set { m_spline.m_points[m_idx] = value; }
		}
		
		public Vector3 SelectedPoint
		{
			get { return m_spline.m_points[m_selidx]; }
			set { m_spline.m_points[m_selidx] = value; }
		}
				
		public bool Selected
		{
			get { return m_idx == m_selidx; }
			set { if(value) { m_selidx = m_idx; } else { m_selidx = -1; } }
		}
		
		public bool SomethingSelected
		{
			get { return m_selidx != -1; }
		}
		
		public int Index
		{
			get { return m_idx; }
		}
		
		public int SelectedIndex
		{
			get { return m_selidx; }
		}		
	}
	
	[Serializable]
	public class UniSplineSegment
	{
		public Vector3 m_startpos, m_startctrl, m_endctrl, m_endpos;
		public float m_startlen, m_endlen, m_length;
		public float[] m_params, m_precomps;
	}
	
	[SerializeField]
	private UniSplineType m_type = BaseSpline.UniSplineType.CatmullRom;
	[SerializeField]
	private List<Vector3> m_points = new List<Vector3>();
	[SerializeField]
	private UniSplineSegment[] m_segments = null;
	[SerializeField]
	private float m_bias = 0, m_tension = 0, m_continuity = 0;
	[SerializeField]
	private float m_precompdiv = 1;
	
	public UniSplineType SplineType
	{
		get { return m_type; }
		set { m_type = value; }
	}
	
	public float Bias
	{
		get { return m_bias; }
		set
		{
			if(value < -1)
			{
				m_bias = -1;
			}
			else if(value > 1)
			{
				m_bias = 1;
			}
			else
			{
				m_bias = value;
			}
		}
	}
	
	public float Continuity
	{
		get { return m_continuity; }
		set
		{
			if(value < -1)
			{
				m_continuity = -1;
			}
			else if(value > 1)
			{
				m_continuity = 1;
			}
			else
			{
				m_continuity = value;
			}
		}
	} 
	
	public float Tension
	{
		get { return m_tension; }
		
		set
		{
			if(value < -1)
			{
				m_tension = -1;
			}
			else if(value > 1)
			{
				m_tension = 1;
			}
			else
			{
				m_tension = value;
			}
		}
	}
	
	public void AppendPoint(Vector3 pos)
	{
		m_points.Add(pos);
	}
	
	public void RemoveLastPoint()
	{
		m_points.RemoveAt(m_points.Count - 1);
	}
	
	public void RemoveAllPoints()
	{
		m_points.Clear();
	}
	
	public void ReversePoints()
	{
		m_points.Reverse();
	}
	
	public void InsertPoint(int idx, Vector3 pos)
	{
		if(idx < 0 || idx > m_points.Count)
		{
			throw(new IndexOutOfRangeException());
		}
		m_points.Insert(idx, pos);
	}
	
	public override void Build()
	{
		int idx, count;
		Vector3 spt1 = Vector3.zero, spt2 = Vector3.zero, spt3 = Vector3.zero, spt4 = Vector3.zero;
		//
		if(m_points.Count < 2)
		{
			m_segments = null;
			m_length = 0;
			return;
		}
		if(m_wrapmode == SplineWrapMode.Loop)
		{
			count = m_points.Count;
		}
		else
		{
			count = m_points.Count - 1;
		}
		//
		m_segments = new UniSplineSegment[count];
		m_length = 0;
		idx = 0;
		if(m_wrapmode == SplineWrapMode.Loop)
		{
			while(idx < count)
			{
				spt1 = m_points[SplineUtil.WrapIndex(idx, m_points.Count)];
				spt2 = m_points[SplineUtil.WrapIndex(idx - 1, m_points.Count)];
				spt3 = m_points[SplineUtil.WrapIndex(idx + 2, m_points.Count)];
				spt4 = m_points[SplineUtil.WrapIndex(idx + 1, m_points.Count)];
				
				m_segments[idx] = new UniSplineSegment();
				BuildSegment(m_segments[idx], spt1, spt2, spt3, spt4);
				++idx;
			}
		}
		else
		{
			while(idx < count)
			{
				spt1 = m_points[SplineUtil.ClampIndex(idx, m_points.Count)];
				spt2 = m_points[SplineUtil.ClampIndex(idx - 1, m_points.Count)];
				spt3 = m_points[SplineUtil.ClampIndex(idx + 2, m_points.Count)];
				spt4 = m_points[SplineUtil.ClampIndex(idx + 1, m_points.Count)];
				
				m_segments[idx] = new UniSplineSegment();
				BuildSegment(m_segments[idx], spt1, spt2, spt3, spt4);
				++idx;
			}
		}
		
		++m_buildnum;
	}
	
	private void BuildSegment(UniSplineSegment ss, Vector3 sp, Vector3 sc, Vector3 ec, Vector3 ep)
	{
		ss.m_startpos = sp;
		ss.m_endpos = ep;
		
		switch(m_type)
		{
		case UniSplineType.CatmullRom:
			
			ss.m_startctrl = sc;
			ss.m_endctrl = ec;
			break;
		
		case UniSplineType.Hermite:
			{
				float	tens = (1 - m_tension) * 0.5f,
						coef1 = (1 + m_bias) * tens,
						coef2 = (1 - m_bias) * tens;
				ss.m_startctrl = (sp - sc) * coef1 + (ep - sp) * coef2;
				ss.m_endctrl = (ep - sp) * coef1 + (ec - ep) * coef2;
			}
			break;
			
		case UniSplineType.KochanekBartels:
			{
				float 	c1 = (1 - m_tension) * (1 + m_bias) * (1 + m_continuity) * 0.5f,
						c2 = (1 - m_tension) * (1 - m_bias) * (1 - m_continuity) * 0.5f,
						c3 = (1 - m_tension) * (1 + m_bias) * (1 - m_continuity) * 0.5f,
						c4 = (1 - m_tension) * (1 - m_bias) * (1 + m_continuity) * 0.5f;
				ss.m_startctrl = c1 * (sp - sc) + c2 * (ep - sp);
				ss.m_endctrl = c3 * (ep - sp) + c4 * (ec - ep);
			}
			break;
		}
			
		ss.m_startlen = m_length;
		float seglen = GetLength(ss);
		m_length += seglen;
		ss.m_length = seglen;
		ss.m_endlen = m_length;
		
		switch(m_reparam)
		{
		case SplineReparamType.None:			
			ss.m_params = null;
			ss.m_precomps = null;
			break;
			
		case SplineReparamType.Simple:
			{
				m_precompdiv = 1 / (float)m_stepcount;
				float param = 0, length = 0;
			
				Vector3 prev, next;
				
				ss.m_params = new float[m_stepcount + 1];
				ss.m_precomps = new float[m_stepcount + 1];
				for(int i = 1; i < m_stepcount + 1; ++i)
				{
					prev = GetPosition(ss, param);
					param += m_precompdiv;
					next = GetPosition(ss, param);
					length += (next - prev).magnitude;
					ss.m_precomps[i] = length / seglen;
					ss.m_params[i] = param;
				}
				ss.m_params[0] = 0;
				ss.m_params[m_stepcount] = 1;
				ss.m_precomps[0] = 0;
				ss.m_precomps[m_stepcount] = 1;
				m_precompdiv = 1 / (float)m_stepcount;
			}
			break;
			
		case SplineReparamType.RungeKutta:
			float dlen = seglen / (float)m_stepcount, lparam = 0;
			
			ss.m_params = new float[m_stepcount + 1];
			ss.m_precomps = new float[m_stepcount + 1];
			for(int i = 0; i < m_stepcount + 1; ++i)
			{
				ss.m_params[i] = GetReparamRungeKutta(ss, lparam);
				ss.m_precomps[i] = lparam / seglen;
				lparam += dlen;
			}
			ss.m_params[0] = 0;
			ss.m_params[m_stepcount] = 1;
			ss.m_precomps[0] = 0;
			ss.m_precomps[m_stepcount] = 1;
			m_precompdiv = 1 / (float)m_stepcount;
			break;
		}
	}
	
	private float GetLength(UniSplineSegment ss)
	{
		float len = 0;
		Vector3 start, end;
		float t = 0, dt = 1 / (float)m_stepcount;
		int idx = 0;
		start = ss.m_startpos;
		while(idx < m_stepcount)
		{
			t += dt;
			end = GetPosition(ss, t);
			len += (end - start).magnitude;
			start = end;
			++idx;
		}
		return len;
	}
	
	private Vector3 GetPosition(UniSplineSegment ss, float t)
	{
		switch(m_type)
		{
		case UniSplineType.CatmullRom:
			{
				float	t2 = t * t,
						t3 = t2 * t;
		        return	ss.m_startpos * ( 1.5f * t3 - 2.5f * t2 + 1.0f) +
						ss.m_startctrl * (-0.5f * t3 + t2 - 0.5f * t) +
		            	ss.m_endctrl * ( 0.5f * t3 - 0.5f * t2) +
						ss.m_endpos * (-1.5f * t3 + 2.0f * t2 + 0.5f * t);
			}
			
		case UniSplineType.Hermite:
			{
				float	t2 = t * t,
						t3 = t2 * t;
				
		        return	ss.m_startpos * (2 * t3 - 3 * t2 + 1) +
						ss.m_startctrl * (t3 - 2 * t2 + t) +
						ss.m_endctrl * (t3 - t2) +
						ss.m_endpos * (-2 * t3 + 3 * t2);
			}
			
		case UniSplineType.KochanekBartels:
			{				
				float 	t2 = t * t,
						t3 = t2 * t;
		        return	ss.m_startpos * (2 * t3 - 3 * t2 + 1) +
						ss.m_startctrl * (t3 - 2 * t2 + t) +
						ss.m_endctrl * (t3 - t2) +
						ss.m_endpos * (-2 * t3 + 3 * t2);
			}
			
		default: return Vector3.zero;
		}
	}
	
	private Vector3 GetTangent(UniSplineSegment ss, float t)
	{
		switch(m_type)
		{
		case UniSplineType.CatmullRom:
			{
				float	t2 = t * t;
		        return	ss.m_startpos * (4.5f * t - 5.0f) * t +
						ss.m_startctrl * (-1.5f * t2 + 2.0f * t - 0.5f) +
						ss.m_endctrl * (1.5f * t - 1.0f) * t +
						ss.m_endpos * (-4.5f * t2 + 4.0f * t + 0.5f);
			}
			
		case UniSplineType.Hermite:
			{
				float t2 = t * t;
				return	ss.m_startpos * (6 * t2 - 6 * t) +
						ss.m_startctrl * (3 * t2 - 4 * t + 1) +
						ss.m_endctrl * (3 * t2 - 2 * t) +
						ss.m_endpos * (-6 * t2 + 6 * t);
			}
			
		case UniSplineType.KochanekBartels:
			{
				float	t2 = t * t;
				return	ss.m_startpos * (6 * t2 - 6 * t) +
						ss.m_startctrl * (3 * t2 - 4 * t + 1) +
						ss.m_endctrl * (3 * t2 - 2 * t) +
						ss.m_endpos * (-6 * t2 + 6 * t);
			}
			
		default: return Vector3.zero;
		}
	}
	
	private Vector3 GetNormal(UniSplineSegment ss, float t)
	{
		switch(m_type)
		{
		case UniSplineType.CatmullRom:
			{
				return	ss.m_startpos * (9.0f * t - 5.0f) -
						ss.m_startctrl * (2.0f - 3.0f * t) +
						9.0f * ss.m_endpos * t +
						3.0f * ss.m_endctrl * t +
						4.0f * ss.m_endpos -
						ss.m_endctrl;
			}
		
		case UniSplineType.Hermite:
			{
				return	ss.m_startpos * (12 * t - 6) +
						ss.m_startctrl * (6 * t - 4) +
						ss.m_endctrl * (6 * t - 2) +
						ss.m_endpos * (-12 * t + 6);
			}
			
		case UniSplineType.KochanekBartels:
			{
				return	ss.m_startpos * (12 * t - 6) +
						ss.m_startctrl * (6 * t - 4) +
						ss.m_endctrl * (6 * t - 2) +
						ss.m_endpos * (-12 * t + 6);
			}
		default: return Vector3.zero;
		}
	}
	
	private float GetReparamRungeKutta(UniSplineSegment ss, float u)
	{
		float t = 0, k1, k2, k3, k4, h = u / (float)m_stepcount, mag;
		for (int i = 1; i <= m_stepcount; i++)
		{
			mag = GetTangent(ss, t).magnitude;
			if(mag == 0)
			{
				k1 = 0;
				k2 = 0;
				k3 = 0;
				k4 = 0;
			}
			else
			{
				k1 = h / GetTangent(ss, t).magnitude;
				k2 = h / GetTangent(ss, t + k1 * 0.5f).magnitude;
				k3 = h / GetTangent(ss, t + k2  * 0.5f).magnitude;
				k4 = h / GetTangent(ss, t + k3).magnitude;
			}
			t += (k1 + 2 * (k2 + k3) + k4) * 0.16666666666666666666666666666667f;
		}
		return t;
	}
	
	private float GetReparam(UniSplineSegment ss, float u)
	{
		if(u <= 0)
		{
			return 0;
		}
		else if(u >= 1)
		{
			return 1;
		}
		
		switch(m_reparam)
		{
		case SplineReparamType.RungeKutta:
			{
				int ridx = (int)(u * (float)m_stepcount);
				float uc = (u - ss.m_precomps[ridx]) / m_precompdiv;
				return Mathf.Lerp(ss.m_params[ridx], ss.m_params[ridx + 1], uc);
			}
			
		case SplineReparamType.Simple:
			{
				int ridx = 0;
				for(int i = 1; i < m_stepcount + 1; ++i)
				{
					if(ss.m_precomps[i] > u)
					{
						ridx = i - 1;
						break;
					}
				}
				float uc = (u - ss.m_precomps[ridx]) / (ss.m_precomps[ridx + 1] - ss.m_precomps[ridx]);
				return Mathf.Lerp(ss.m_params[ridx], ss.m_params[ridx + 1], uc);
			}
			
		default:
			return 0;
		}
	}
	
	public override int GetPointCount()
	{
		return m_points.Count;
	}
	
	public override int GetSegmentCount()
	{
		if(m_segments != null)
		{
			return m_segments.Length;
		}
		return 0;
	}
	
	protected override float GetSegmentLength(int segidx)
	{
		return m_segments[segidx].m_length;
	}
	
	protected override float GetSegmentStartLength(int segidx)
	{
		return m_segments[segidx].m_startlen;
	}
	
	protected override float GetSegmentEndLength(int segidx)
	{
		return m_segments[segidx].m_endlen;
	}
	
	protected override int FindSegment(float offset)
	{
		for(int i = 0; i < m_segments.Length; ++i)
		{
			if(m_segments[i].m_startlen <= offset && m_segments[i].m_endlen > offset)
			{
				return i;
			}
		}
		return m_segments.Length - 1;
	}
	
	protected override Vector3 GetDrawPosition(int segidx, float t)
	{
		UniSplineSegment ss = m_segments[segidx];
		return GetPosition(ss, t);
	}
	
	protected override Vector3 GetPosition(int segidx, float segpos)
	{
		UniSplineSegment ss = m_segments[segidx];
		if(m_reparam == BaseSpline.SplineReparamType.None)
		{
			return GetPosition(ss, segpos / ss.m_length);
		}
		else
		{
			return GetPosition(ss, GetReparam(ss, segpos / ss.m_length));
		}
	}
	
	protected override Vector3 GetTangent(int segidx, float segpos)
	{
		UniSplineSegment ss = m_segments[segidx];
		if(m_reparam == BaseSpline.SplineReparamType.None)
		{
			return GetTangent(ss, segpos / ss.m_length);
		}
		else
		{
			return GetTangent(ss, GetReparam(ss, segpos / ss.m_length));
		}
	}
	
	protected override Vector3 GetNormal(int segidx, float segpos)
	{
		UniSplineSegment ss = m_segments[segidx];
		if(m_reparam == BaseSpline.SplineReparamType.None)
		{
			return GetNormal(ss, segpos / ss.m_length);
		}
		else
		{
			return GetNormal(ss, GetReparam(ss, segpos / ss.m_length));
		}
	}
	
	public EditHelper GetEditHelper()
	{
		return new EditHelper(this);
	}
}

public class UniSplineComponent : MonoBehaviour
{
	[SerializeField]
	private UniSpline m_spline = new UniSpline();
	
	private int m_buildnum = -1;
	private Vector3[] m_drawcache;
	
	public UniSpline Spline
	{
		get { return m_spline; }
	}
	
	void OnDrawGizmosSelected()
	{
		DrawGizmos(Color.red);
	}
	
	void OnDrawGizmos()
	{
		DrawGizmos(Color.white);
	}
	
	public void DrawGizmos(Color color)
	{
		int curbuild = m_spline.BuildNum;
		if(m_spline.GetSegmentCount() > 0)
		{
			if(m_buildnum != curbuild)
			{
				m_drawcache = m_spline.GenerateDrawPoints(16);
				m_buildnum = curbuild;
			}

			if(m_drawcache != null)
			{
				Gizmos.matrix = transform.localToWorldMatrix;
				Gizmos.color = color;
			
				Vector3 startpos = Vector3.zero, endpos = Vector3.zero;
		
				for(int i = 0; i < m_drawcache.Length; ++i)
				{
					endpos = m_drawcache[i];
					if(i != 0)
					{
						Gizmos.DrawLine(startpos, endpos);
					}
					startpos = endpos;
				}
			}
		}
	}
}
