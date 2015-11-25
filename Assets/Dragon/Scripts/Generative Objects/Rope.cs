using UnityEngine;
using System.Collections;

public class Rope : MonoBehaviour 
{
	public int 			m_SegmentCount = 10;

	TForm_RopeSection[] m_Segments;

	public float 		m_FollowDistance = .1f;

	public float 		m_Smoothing = 4;


	public Transform 	m_StartPoint;
	public Transform 	m_EndPoint;
	
	public float 		m_MaxLength = 5;

	LineRenderer 		m_LineRenderer;



	// Use this for initialization
	void Start () 
	{
		m_StartPoint.transform.parent = transform;
		m_EndPoint.transform.parent = transform;



		m_Segments = new TForm_RopeSection[m_SegmentCount];

		m_FollowDistance = m_MaxLength / (float)m_SegmentCount;

		for (int i = 0; i < m_SegmentCount; i++)
		{
			TForm_RopeSection newFollowSegment = (TForm_RopeSection)new GameObject( "Segment " + i ).AddComponent< TForm_RopeSection >();
	
			newFollowSegment.transform.parent = transform;

			float normValue = (float)i/(float)(m_SegmentCount - 1);

			if( i == 0 )
			{
				newFollowSegment.m_AnchorT = m_StartPoint;
			}
			else 			
			{
				newFollowSegment.m_AnchorT = m_Segments[ i - 1 ].transform;
				m_Segments[ i - 1 ].m_FollowT = newFollowSegment.transform;
			}

			newFollowSegment.m_FollowDistance = m_FollowDistance;

			m_Segments[ i ] = newFollowSegment;
		}

		m_Segments[ m_Segments.Length - 1 ].m_FollowT = m_EndPoint;

		// Set up renderer
		m_LineRenderer = gameObject.GetComponent< LineRenderer >();
		m_LineRenderer.SetVertexCount( m_Segments.Length + 2 );


	}

	void Update () 
	{
		m_FollowDistance = m_MaxLength / (float)m_SegmentCount;
		m_LineRenderer.SetPosition( 0, m_StartPoint.transform.position );
		for (int i = 0; i < m_SegmentCount; i++)
		{

			m_Segments[ i ].m_Smoothing = m_Smoothing;
			m_Segments[ i ].m_FollowDistance = m_FollowDistance;

			m_LineRenderer.SetPosition( i + 1, m_Segments[ i ].transform.position );
		}
		m_LineRenderer.SetPosition( m_SegmentCount + 1, m_EndPoint.transform.position );
	}


}
