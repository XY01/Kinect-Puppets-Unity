using UnityEngine;
using System.Collections;

/// <summary>
/// Ranged float. 
/// - Dynamically handles the scaling of a float to a range and back again
/// </summary>

[System.Serializable]
public class RangedFloat
{
	float 			m_NormalizedValue;
	public float 	NormalizedValue
	{
		set
		{
			// Set normalized value
			m_NormalizedValue = value;
			if( m_ClampToRange ) m_NormalizedValue = Mathf.Clamp01( m_NormalizedValue );

			// Set ranged value from the normalized value
			m_RangedValue = 	m_NormalizedValue.ScaleFrom01( m_RangeMin, m_RangeMax, m_ClampToRange );
		}

		get { return m_NormalizedValue; }
	}

	float 			m_RangedValue;
	public float 	RangedValue
	{
		set
		{
			// Set ranged value
			m_RangedValue = value;
			if( m_ClampToRange ) m_RangedValue = Mathf.Clamp( m_RangedValue, m_RangeMin, m_RangeMax );

			// Set normalized value from the ranged value
			m_NormalizedValue = m_RangedValue.ScaleTo01( m_RangeMin, m_RangeMax, m_ClampToRange );
		}

		get { return m_RangedValue; }
	}

	// Minimum and maximum ranged value for scaling to
	public float m_RangeMin = -1;
	public float m_RangeMax = 1;

	// Determines if the normalized and ranged values should be clamped
	public bool m_ClampToRange = false;
	
	public RangedFloat()
	{
		m_RangeMin = -1;
		m_RangeMax = 1;
		m_ClampToRange = false;
	}

	public RangedFloat( float min, float max, bool clamp )
	{
		m_RangeMin = min;
		m_RangeMax = max;
		m_ClampToRange = clamp;
	}
}
