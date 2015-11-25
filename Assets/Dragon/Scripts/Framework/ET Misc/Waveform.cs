using UnityEngine;
using System.Collections;

public class Waveform : MonoBehaviour
{
	public float 	m_Frequency;
	public float 	m_Amplitude = 1;

	public bool 	m_DynamicRange = true;
	public float 	m_DynamicRange_Min = -1;
	public float 	m_DynamicRange_Max = 1;

	#region View variables
	public int 		m_Samples = 200;
	public float 	m_DrawLenghth = 4;
	#endregion

	public Graph m_Graph;

	protected void Start()
	{
		if( m_DynamicRange )
			NormalizeToRange();

		m_Graph = gameObject.GetComponent< Graph >() as Graph;

		if( m_Graph )
			m_Graph.m_Values = new float[ m_Samples ];
	}

	protected void Update()
	{
		if( m_DynamicRange )
			NormalizeToRange();

		if( m_Graph )
		{
			for (int i = 0; i < m_Samples; i++)
			{
				float sample = i / (float)m_Samples;
				m_Graph.m_Values[ i ] = GetValueAtNormalizedPos( sample + MasterSpeedController.Instance.m_MasterContinuousValue );
			}
		}
	}

	public virtual float GetValueAtNormalizedPos( float norm )
	{
		return 5;
	}

	[ContextMenu ("Noramlize")]
	protected virtual void NormalizeToRange()
	{
		bool storedDynamicRange = m_DynamicRange;
		m_DynamicRange = false;
		m_DynamicRange_Min = -1;
		m_DynamicRange_Max = 1;
		
		for (int i = 0; i < m_Samples; i++)
		{
			float sample = i / (float)m_Samples;
			float sampleValue = GetValueAtNormalizedPos( sample );
		//	print( sampleValue );
			
			if( sampleValue < m_DynamicRange_Min )
				m_DynamicRange_Min = sampleValue;
			else if( sampleValue > m_DynamicRange_Max )
				m_DynamicRange_Max = sampleValue;
		}

		m_DynamicRange = storedDynamicRange;
	}

	
	public virtual void DrawGUI()
	{
		
	}

	public bool m_DrawGUI = true;
	protected virtual void OnDrawGizmos()
	{
		if( !m_DrawGUI )
			return;

		if( MasterSpeedController.Instance != null )
		{
			Gizmos.color = Color.gray;
			Gizmos.DrawLine( transform.position + new Vector3( 0, 1, .2f ), transform.position + new Vector3( m_DrawLenghth, 1, .2f ) );
			Gizmos.DrawLine( transform.position + new Vector3( 0, -1, .2f ), transform.position + new Vector3( m_DrawLenghth, -1, .2f ) );

			Gizmos.color = Color.white;

			Vector3 posStart = transform.position;
			posStart.y += GetValueAtNormalizedPos( 0 + MasterSpeedController.Instance.m_MasterContinuousValue );
			Vector3 posEnd = transform.position;

			for (int i = 0; i < m_Samples; i++)
			{
				float sample = i / (float)m_Samples;

				posEnd.x = transform.position.x + ( sample * m_DrawLenghth );
				posEnd.y = transform.position.y + GetValueAtNormalizedPos( sample + MasterSpeedController.Instance.m_MasterContinuousValue );
				Gizmos.DrawLine( posStart, posEnd );
				posStart = posEnd;
			}
		}

	}
}
