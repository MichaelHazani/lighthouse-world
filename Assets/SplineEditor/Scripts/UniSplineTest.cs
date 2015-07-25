using UnityEngine;
using System.Collections;

public class UniSplineTest : MonoBehaviour
{
	public UniSplineComponent m_spline;
	public float m_speed = 1, m_curspeed = 0, m_position = 0;
	public Vector3 m_up = Vector3.up;
	public Ease.Easing m_easing;
	private BaseSpline.SplineIterator m_iter;
		
	void Awake()
	{
		m_iter = m_spline.Spline.GetIterator();
		m_iter.SetTransform(m_spline.transform);
	}
	
	void Update()
	{
		m_position += m_speed * Time.deltaTime;
		m_iter.SetOffset(Ease.EaseByType(m_easing, 0, m_spline.Spline.Length, m_position / m_spline.Spline.Length));
	
		Vector3 prevpos = transform.position;
		
		transform.position = m_iter.GetPosition();
		transform.rotation = Quaternion.LookRotation(m_iter.GetTangent(), m_up);
		
		m_curspeed = (transform.position - prevpos).magnitude / Time.deltaTime;
		
		/*
		Debug.DrawLine(transform.position, transform.position + m_iter.GetTangent().normalized, Color.red);
		Debug.DrawLine(transform.position, transform.position + m_iter.GetNormal().normalized, Color.green);		
		Debug.DrawLine(transform.position, transform.position + Vector3.Cross(m_iter.GetTangent().normalized, m_iter.GetNormal().normalized).normalized, Color.blue);
		*/
	}
}
