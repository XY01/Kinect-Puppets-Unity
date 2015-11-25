using UnityEngine;
using System.Collections;

/// <summary>
/// Normalized object value.
///  - Used to store a normalized object value which is reference by objects scripts, i.e. to give an offset for perlin sequence, an fft value......
/// </summary>
public class ET_NormalizedValue : MonoBehaviour
{
	public float m_NormalizedObjectValue;	
	public float m_CycleSpeed = 0;

	public bool m_RandomizeValue = true;
	
	public void RandomizeValue()
	{
		m_NormalizedObjectValue = Random.Range( 0.05f, 1f );
	}
	
	void Start()
	{
		if( m_RandomizeValue )
			RandomizeValue();
	}
	
	void Update()
	{
		if( m_CycleSpeed != 0 )
			m_NormalizedObjectValue =  m_NormalizedObjectValue + m_CycleSpeed * Time.deltaTime;

		m_NormalizedObjectValue = m_NormalizedObjectValue.WrapFloatTo01() ;
		
	}
	
}
