using UnityEngine;
using System.Collections;

public abstract class BaseSpline
{
	public enum UniSplineType
	{
		CatmullRom,
		Hermite,
		KochanekBartels		
	}
	
	public enum SplineWrapMode
	{
		Once,
		Repeat,
		PingPong,
		Loop		
	}
	
	public enum SplinePointType
	{
		Corner,
		Smooth,
		Bezier,
		BezierCorner
	};
	
	public enum SplineSegmentType
	{
		Linear,
		Curve
	};
	
	public enum SplineReparamType
	{
		None,
		Simple,
		RungeKutta
	};
	
	public class SplineIterator
	{
		internal SplineIterator(BaseSpline spline, bool reverse, int startidx, int endidx)
		{
			m_spline = spline;
			m_reverse = reverse;
			m_startidx = Mathf.Min(startidx, endidx);
			m_endidx = Mathf.Max(startidx, endidx);
			Reset();
		}		
		
		public void SetTransform(Transform trnsfrm)
		{
			m_transform = trnsfrm;
		}
		
		public Vector3 GetPosition()
		{
			if(m_transform != null)
			{
				return m_transform.localToWorldMatrix.MultiplyPoint(m_spline.GetPosition(m_segidx, m_segpos));
			}
			else
			{
				return m_spline.GetPosition(m_segidx, m_segpos);
			}
		}
		
		public Vector3 GetTangent()
		{
			if(m_transform != null)
			{
				if(m_reverse)
				{
					return m_transform.localRotation * -m_spline.GetTangent(m_segidx, m_segpos);
				}
				else
				{
					return m_transform.localRotation * m_spline.GetTangent(m_segidx, m_segpos);
				}
			}
			else
			{
				if(m_reverse)
				{
					return -m_spline.GetTangent(m_segidx, m_segpos);
				}
				else
				{
					return m_spline.GetTangent(m_segidx, m_segpos);
				}
			}
		}
		
		public Vector3 GetNormal()
		{
			if(m_transform != null)
			{
				return m_transform.localRotation * m_spline.GetNormal(m_segidx, m_segpos);
			}
			else
			{
				return m_spline.GetNormal(m_segidx, m_segpos);
			}
		}
		
		public bool IsOnceOut()
		{
			return m_onceout;
		}
		
		public void Reset()
		{
			if(m_reverse)
			{
				SetToEnd();
			}
			else
			{
				SetToStart();
			}
			m_back = false;
			m_onceout = false;
		}
		
		public void SetOffset(float offset)
		{
			offset = SplineUtil.WrapPosition(m_spline.WrapMode, offset, m_spline.Length);
			m_segidx = m_spline.FindSegment(offset);
			m_segpos = offset - m_spline.GetSegmentStartLength(m_segidx);
		}
		
		public float GetOffset()
		{
			return m_spline.GetSegmentStartLength(m_segidx) + m_segpos;
		}
		
		public void SetOffsetPercent(float offset)
		{
			offset = SplineUtil.WrapPosition(m_spline.WrapMode, offset, m_spline.Length);
			m_segidx = m_spline.FindSegment(offset * m_spline.Length);
			m_segpos = offset - m_spline.GetSegmentStartLength(m_segidx);
		}
		
		public float GetOffsetPercent()
		{
			if(m_reverse)
			{
				return 1 - (m_spline.GetSegmentStartLength(m_segidx) + m_segpos) / m_spline.Length;
			}
			else
			{
				return (m_spline.GetSegmentStartLength(m_segidx) + m_segpos) / m_spline.Length;
			}
		}
		
		public void Iterate(float length)
		{
			bool stop = false, back = false;
			if(m_reverse)
			{
				length = -length;
			}
			if(length < 0)
			{
				back = !m_back;
				length = -length;
			}
			else
			{
				back = m_back;
			}
			if(back)
			{
				while(m_segpos - length < 0 && !stop)
				{
					length -= m_segpos;
					
					if(m_segidx - 1 < m_startidx)
					{
						switch(m_spline.WrapMode)
						{
						case SplineWrapMode.Loop:
							SetToEnd();
							break;
						case SplineWrapMode.Once:
							SetToStart();
							stop = true;
							m_onceout = true;
							break;
						case SplineWrapMode.Repeat:
							SetToEnd();
							break;
						case SplineWrapMode.PingPong:
							SetToStart();
							m_back = !m_back;
							stop = true;
							break;
						}
					}
					else
					{
						--m_segidx;
						m_segpos = m_spline.GetSegmentLength(m_segidx);
					}
				}
				if(!stop)
				{
					m_segpos -= length;
				}
			}
			else
			{
				while(m_segpos + length > m_spline.GetSegmentLength(m_segidx) && !stop)
				{
					length -= m_spline.GetSegmentLength(m_segidx) - m_segpos;
					
					if(m_segidx + 1 >= m_endidx)
					{
						switch(m_spline.m_wrapmode)
						{
						case SplineWrapMode.Loop:
							SetToStart();
							break;
						case SplineWrapMode.Once:
							SetToEnd();
							stop = true;
							m_onceout = true;
							break;
						case SplineWrapMode.Repeat:
							SetToStart();
							break;
						case SplineWrapMode.PingPong:
							SetToEnd();
							m_back = !m_back;
							stop = true;
							break;
						}
					}
					else
					{
						++m_segidx;
						m_segpos = 0;
					}
				}
				if(!stop)
				{
					m_segpos += length;
				}
			}
		}
		
		private void SetToStart()
		{
			m_segidx = m_startidx;
			m_segpos = 0;
		}
		
		private void SetToEnd()
		{
			m_segidx = m_endidx - 1;
			m_segpos = m_spline.GetSegmentLength(m_segidx);
		}
		
		private Transform m_transform = null;
		private BaseSpline m_spline;
		private int m_segidx = 0, m_startidx = 0, m_endidx = 0;
		private bool m_reverse = false, m_back = false, m_onceout = false;
		private float m_segpos = 0;
	}
	
	[SerializeField]
	protected SplineWrapMode m_wrapmode = SplineWrapMode.Once;
	[SerializeField]
	protected float m_length = 0;
	[SerializeField]
	protected int m_stepcount = 8;
	[SerializeField]
	protected SplineReparamType m_reparam = SplineReparamType.None;
	
	protected int m_buildnum = 0;
	
	public int BuildNum
	{
		get
		{
			return m_buildnum;
		}
	}
	
	public float Length
	{
		get
		{
			return m_length;
		}
	}
	
	public SplineWrapMode WrapMode
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
	
	public int StepCount
	{
		get
		{
			return m_stepcount;
		}
		set
		{
			if(value > 1)
			{
				m_stepcount = value;
			}
		}
	}
	
	public SplineReparamType ReparamType
	{
		get { return m_reparam; }
		set { m_reparam = value; }
	}
	
	public abstract void Build();
	public abstract int GetPointCount();
	public abstract int GetSegmentCount();	
	protected abstract float GetSegmentLength(int segidx);
	protected abstract float GetSegmentStartLength(int segidx);
	protected abstract float GetSegmentEndLength(int segidx);
	protected abstract int FindSegment(float offset);
	protected abstract Vector3 GetDrawPosition(int segidx, float segpos);
	protected abstract Vector3 GetPosition(int segidx, float segpos);
	protected abstract Vector3 GetTangent(int segidx, float segpos);
	protected abstract Vector3 GetNormal(int segidx, float segpos);
	
	public SplineIterator GetIterator()
	{
		return new SplineIterator(this, false, 0, GetSegmentCount());
	}
	
	public SplineIterator GetReverseIterator()
	{
		return new SplineIterator(this, true, 0, GetSegmentCount());
	}
	
	public SplineIterator GetPartialIterator(int startidx, int endidx)
	{
		return new SplineIterator(this, false, startidx, endidx);
	}
	
	public SplineIterator GetPartialReverseIterator(int startidx, int endidx)
	{
		return new SplineIterator(this, true, startidx, endidx);
	}
	
	public Vector3[] GenerateDrawPoints(int divs)
	{
		int segcnt = GetSegmentCount(), ptidx = 0;
		Vector3[] dc = new Vector3[segcnt * divs + 1];
		float dt = 1 / (float)divs;
		
		dc[ptidx] = GetDrawPosition(0, 0);
		++ptidx;
		for(int i = 0; i < segcnt; ++i)
		{
			for(int j = 1; j < divs + 1; ++j)
			{
				dc[ptidx] = GetDrawPosition(i, (float)j * dt);
				++ptidx;
			}
		}
		
		return dc;
	}
}