using UnityEngine;
using System.Collections;

public class BezierSplinePartialTest : MonoBehaviour
{
	public BezierSplineComponent m_spline;
	public float m_speed = 1;
	public Vector3 m_up = Vector3.up;
	private BaseSpline.SplineIterator m_iter;
	
	void Awake()
	{
		//m_iter = m_spline.Spline.GetPartialReverseIterator(0, 1);
		m_iter = m_spline.Spline.GetPartialIterator(0, 1);
		m_iter.SetTransform(m_spline.transform);
	}
	
	void Update()
	{
		m_iter.Iterate(m_speed * Time.deltaTime);
	
		Vector3 prevpos = transform.position;
		
		transform.position = m_iter.GetPosition();
		transform.rotation = Quaternion.LookRotation(m_iter.GetTangent(), m_up);
	}
}
