using UnityEngine;
using System.Collections;

public class Follow_Rope : MonoBehaviour 
{
	public Transform 	m_FollowTransform;

	public TForm_Follow	m_SegmentPrefab;

	public int 			m_SegmentCount = 10;

	TForm_Follow[] 		m_Segments;

	public float 		m_FollowDistance = .1f;

	public float 		m_Smoothing = 4;

	public AnimationCurve m_ScaleCurve;



	public Transform 	m_AnchorPoint;
	
	public float 		m_MaxLength = 5;





	// Use this for initialization
	void Start () 
	{
		transform.position = Vector3.zero;

		//m_FollowTransform.parent = transform;

		m_Segments = new TForm_Follow[m_SegmentCount];

		for (int i = 0; i < m_SegmentCount; i++)
		{
			TForm_Follow newFollowSegment = (TForm_Follow)Instantiate( m_SegmentPrefab );
			newFollowSegment.name = "Segment " + i;
			newFollowSegment.transform.parent = transform;

			float normValue = (float)i/(float)(m_SegmentCount - 1);

			newFollowSegment.transform.localScale = Vector3.one * m_ScaleCurve.Evaluate( normValue );

			if( i == 0 )	newFollowSegment.m_FollowT = m_FollowTransform;
			else 			newFollowSegment.m_FollowT = m_Segments[ i - 1 ].transform;

			newFollowSegment.m_FollowDistance = m_FollowDistance;

			m_Segments[ i ] = newFollowSegment;
		}
	}

	void Update () 
	{
		for (int i = 0; i < m_SegmentCount; i++)
		{
			float normValue = (float)i/(float)(m_SegmentCount - 1);

			float distanceFromAnchor = ( m_Segments[ i ].transform.position - m_AnchorPoint.position ).magnitude;
			float allowedDistanceFromAnchor = normValue * m_MaxLength;

			if( distanceFromAnchor > allowedDistanceFromAnchor )
			{
				Vector3 directionToAnchor = ( m_Segments[ i ].transform.position - m_AnchorPoint.position ).normalized;

			}

			m_Segments[ i ].m_Smoothing = m_Smoothing;
			m_Segments[ i ].m_FollowDistance = m_FollowDistance * m_ScaleCurve.Evaluate( normValue );
		}
	}
}
