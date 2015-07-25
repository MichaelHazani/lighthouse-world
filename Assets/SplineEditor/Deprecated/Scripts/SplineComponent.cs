using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// Deprecated
public class SplineComponent : MonoBehaviour
{
	[Serializable]
	public class SplineComponentSegment
	{
		public Vector3 m_startpos, m_endpos;
		public float m_startlen, m_endlen;
		public SplineComponentSegment[] m_subsegments = null;
	}
	
	[Serializable]
	public class SplineComponentPoint
	{
		public SplineComponentPoint(Vector3 p, Vector3 c1, Vector3 c2, BaseSpline.SplinePointType t)
		{
			m_point = p;
			m_control1 = c1;
			m_control2 = c2;
			m_type = t;
		}
			
		public Vector3 m_point, m_control1, m_control2;
		public BaseSpline.SplinePointType m_type;
	}
	
	public List<SplineComponentPoint> m_points = new List<SplineComponentPoint>();
	[SerializeField]
	private int m_numsteps = 4;
	[SerializeField]
	private BaseSpline.SplineWrapMode m_wrapmode = BaseSpline.SplineWrapMode.Once;
	[SerializeField]
	private float m_length = 0;
	[SerializeField]
	private SplineComponentSegment[] m_segments = null;
	
	public int InterpolationSteps
	{
		get
		{
			return m_numsteps;
		}
		set
		{
			if(m_numsteps <= 0)
			{
				throw(new ArgumentOutOfRangeException());
			}
			m_numsteps = value;
		}
	}
	
	public SplineComponentSegment[] Segments
	{
		get
		{
			return m_segments;
		}
	}
	
	public float Length
	{
		get
		{
			return m_length;
		}
	}
	
	public BaseSpline.SplineWrapMode WrapMode
	{
		get
		{
			return m_wrapmode;
		}
		set
		{
			m_wrapmode = value;
		}
	}
	
	public void AppendPoint(Vector3 pos, BaseSpline.SplinePointType type, Vector3 cp1, Vector3 cp2)
	{
		m_points.Add(new SplineComponent.SplineComponentPoint(pos, cp1, cp2, type));
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
		Vector3 swp;
		for(int i = 0; i < m_points.Count; ++i)
		{
			swp = m_points[i].m_control1;
			m_points[i].m_control1 = m_points[i].m_control2;
			m_points[i].m_control2 = swp;
		}
	}
	
	public void InsertPoint(int idx, Vector3 pos, BaseSpline.SplinePointType type, Vector3 cp1, Vector3 cp2)
	{
		if(idx < 0 || idx > m_points.Count)
		{
			throw(new IndexOutOfRangeException());
		}
		m_points.Insert(idx, new SplineComponent.SplineComponentPoint(pos, cp1, cp2, type));
	}
	
	public void Build()
	{
		int idx, previdx, nextidx, segidx, count;
		SplineComponentPoint spt1 = null, spt2 = null, spt3 = null;
		//
		if(m_points.Count < 2)
		{
			m_segments = null;
			m_length = 0;
			return;
		}
		if(m_wrapmode == BaseSpline.SplineWrapMode.Loop)
		{
			count = m_points.Count;
		}
		else
		{
			count = m_points.Count - 1;
		}
		
		// Prepare
		idx = 0;
		while(idx < m_points.Count)
		{
			spt1 = m_points[idx];
			
			switch(spt1.m_type)
			{
			case BaseSpline.SplinePointType.Corner:
				spt1.m_control1 = Vector3.zero;
				spt1.m_control2 = Vector3.zero;
				break;
				
			case BaseSpline.SplinePointType.Bezier:
				spt1.m_control2 = -spt1.m_control1;
				break;
				
			case BaseSpline.SplinePointType.Smooth:
				if(m_wrapmode == BaseSpline.SplineWrapMode.Loop)
				{
					if(idx == 0)
					{
						nextidx = idx + 1;
						previdx = m_points.Count - 1;
					}
					else if(idx == m_points.Count - 1)
					{
						nextidx = 0;
						previdx = idx - 1;
					}
					else
					{
						nextidx = idx + 1;
						previdx = idx - 1;
					}
					
					spt2 = m_points[nextidx];
					spt3 = m_points[previdx];
					spt1.m_control2 = -(spt3.m_point - spt2.m_point) * 0.25f;
					spt1.m_control1 = -spt1.m_control2;
				}
				else
				{
					if(idx == 0)
					{
						spt1.m_control1 = Vector3.zero;
						spt1.m_control2 = Vector3.zero;
					}
					else if(idx == m_points.Count - 1)
					{
						spt1.m_control1 = Vector3.zero;
						spt1.m_control2 = Vector3.zero;
					}
					else
					{
						nextidx = idx + 1;
						previdx = idx - 1;
						
						spt2 = m_points[nextidx];
						spt3 = m_points[previdx];
						spt1.m_control2 = -(spt3.m_point - spt2.m_point) * 0.25f;
						spt1.m_control1 = -spt1.m_control2;
					}
				}
				break;
			}
			++idx;
		}
		
		//
		m_segments = new SplineComponentSegment[count];
		m_length = 0;
		
		idx = 0; segidx = 0;
		while(idx < m_points.Count)
		{
			spt2 = m_points[idx];
			if(idx > 0)
			{
				m_segments[segidx] = new SplineComponentSegment();
				BuildSegment(m_segments[segidx], spt1, spt2);
				++segidx;
			}

			spt1 = spt2;
			++idx;
		}
		if(m_wrapmode == BaseSpline.SplineWrapMode.Loop)
		{
			spt2 = m_points[0];
			m_segments[segidx] = new SplineComponentSegment();
			BuildSegment(m_segments[segidx], spt1, spt2);
		}
	}
	
	private void BuildSegment(SplineComponentSegment ss, SplineComponentPoint spt1, SplineComponentPoint spt2)
	{
		int idx;
		SplineComponentSegment newss;
		
		ss.m_startpos = spt1.m_point;
		ss.m_endpos = spt2.m_point;
		ss.m_startlen = m_length;
		
		if(spt1.m_type == BaseSpline.SplinePointType.Corner && spt2.m_type == BaseSpline.SplinePointType.Corner)
		{
			ss.m_subsegments = null;
			m_length += (ss.m_endpos - ss.m_startpos).magnitude;
		}
		else
		{
			ss.m_subsegments = new SplineComponentSegment[m_numsteps];
			
			idx = 0;
			while(idx < m_numsteps)
			{
				newss = new SplineComponentSegment();
				newss.m_subsegments = null;
				
				newss.m_startlen = m_length;
				newss.m_startpos = GetBezierPoint(spt1, spt2, idx);
				newss.m_endpos = GetBezierPoint(spt1, spt2, idx + 1);
				m_length += (newss.m_endpos - newss.m_startpos).magnitude;
				newss.m_endlen = m_length;
				
				ss.m_subsegments[idx] = newss;
				++idx;
			}
		}
		
		ss.m_endlen = m_length;
	}
	
	private Vector3 GetBezierPoint(SplineComponentPoint spt1, SplineComponentPoint spt2, int segidx)
	{
		Vector3 retval = Vector3.zero;
		if(segidx == 0)
		{
			retval = spt1.m_point;
		}
		else if(segidx == m_numsteps)
		{
			retval = spt2.m_point;
		}
		else
		{
			Vector3 ctl1 = spt1.m_point + spt1.m_control2, ctl2 = spt2.m_point + spt2.m_control1;
			float t = (float)segidx / (float)m_numsteps, _1mt = 1 - t;
			retval = _1mt * _1mt * _1mt * spt1.m_point + 3 * _1mt * _1mt * t * ctl1 +
						3 * _1mt * t * t * ctl2 + t * t * t * spt2.m_point;
		}
		return retval;
	}
	
	void OnDrawGizmosSelected()
	{
		DrawGizmos(Color.red, new Color(1.0f, 0.5f, 0.5f, 1.0f));
	}
	
	void OnDrawGizmos()
	{
		DrawGizmos(Color.white, Color.white);
	}
	
	public void DrawGizmos(Color color1, Color color2)
	{
		Gizmos.matrix = transform.localToWorldMatrix;
		if(m_segments != null)
		{
			SplineComponentSegment curseg = null, cursubseg = null;
			int idx = 0, subidx;
			while(idx < m_segments.Length)
			{
				curseg = m_segments[idx];
				if(curseg.m_subsegments == null || curseg.m_subsegments.Length == 0)
				{
					Gizmos.color = color1;
					Gizmos.DrawLine(curseg.m_startpos, curseg.m_endpos);
				}
				else
				{
					subidx = 0;
					while(subidx < curseg.m_subsegments.Length)
					{
						if(subidx % 2 == 0)
						{
							Gizmos.color = color1;
						}
						else
						{
							Gizmos.color = color2;
						}
						cursubseg = curseg.m_subsegments[subidx];
						Gizmos.DrawLine(cursubseg.m_startpos, cursubseg.m_endpos);
						++subidx;
					}
				}
				++idx;
			}
		}
	}
	
	public Vector3 GetPosition(float pos)
	{
		Vector3 retval = Vector3.zero;
		SplineComponentSegment ss = null;
		bool found = false;
		float weight, seglen;
		SplineComponentSegment[] ssarr = m_segments;
		if(m_segments != null)
		{
			pos = SplineUtil.WrapPosition(m_wrapmode, pos, m_length);
			
			if(pos <= 0)
			{
				retval = m_segments[0].m_startpos;
			}
			else if(pos >= m_length)
			{
				retval = m_segments[m_segments.Length - 1].m_endpos;
			}
			else
			{
				while(!found)
				{
					ss = FindSegment(ssarr, pos);
					if(ss.m_subsegments == null || ss.m_subsegments.Length == 0)
					{
						found = true;
					}
					else
					{
						ssarr = ss.m_subsegments;
					}
				}
				seglen = ss.m_endlen - ss.m_startlen;
				if(seglen == 0)
				{
					retval = ss.m_endpos;
				}
				else
				{
					weight = (pos - ss.m_startlen) / seglen;
					retval = ss.m_startpos + (ss.m_endpos - ss.m_startpos) * weight;
				}
			}
		}
		return transform.localToWorldMatrix.MultiplyPoint(retval);
	}
		
	private SplineComponentSegment FindSegment(SplineComponentSegment[] ssarr, float pos)
	{
		SplineComponentSegment retval = null;
		int idx = 0;
		while(idx < ssarr.Length)
		{
			if(pos > ssarr[idx].m_startlen && pos <= ssarr[idx].m_endlen)
			{
				retval = ssarr[idx];
				break;
			}
			++idx;
		}
		return retval;
	}
}
