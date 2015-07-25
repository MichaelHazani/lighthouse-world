using UnityEngine;
using System.Collections;

public class SplineTest : MonoBehaviour
{
	public SplineComponent m_spline;
	public float m_position = 0, m_speed = 1;
	
	void Update()
	{
		m_position += m_speed * Time.deltaTime;
		
		transform.position = m_spline.GetPosition(m_position);
	}
}
