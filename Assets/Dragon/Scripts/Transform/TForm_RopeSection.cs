using UnityEngine;
using System.Collections;

public class TForm_RopeSection : MonoBehaviour 
{
	// reference to this transform
	Transform 	m_Transform;

	// reference to this position
	Vector3 	m_Position;

	// The distance at which to follow
	public float m_FollowDistance = .5f;

	// Transform to follow
	public Transform m_FollowT;
	public Transform m_AnchorT;

	// Determines if the transform looks toward the transform it is following
	public bool m_LookAtTransform = false;

	// Smoothes the following movement
	public float m_Smoothing = 0;

	public bool m_MaintainLength = false;


	
	// Use this for initialization
	void Start ()
	{
		m_Transform = gameObject.GetComponent< Transform >();

		m_Position = m_Transform.position;
	}
	
	// Update is called once per frame
	public void Update()
	{
		// returns if there is no transform to follow
		if( m_FollowT == null || m_AnchorT == null )
			return;

		Vector3 targetPosition;

		Vector3 vectorToFollowT = m_Transform.position - m_FollowT.position;
		Vector3 vectorToAnchorT = m_Transform.position - m_AnchorT.position;

		Vector3 directionToFollowT = vectorToFollowT.normalized;
		Vector3 directionToAnchorT = vectorToAnchorT.normalized;

		float distanceToFollow = m_FollowDistance;
		float distanceToAnchor = m_FollowDistance;

		if( !m_MaintainLength )
		{
			distanceToFollow = Mathf.Clamp( vectorToFollowT.magnitude, 0, m_FollowDistance );
			distanceToAnchor = Mathf.Clamp( vectorToAnchorT.magnitude, 0, m_FollowDistance );
		}

		Vector3 targetFollowPosition = m_FollowT.position + ( directionToFollowT * distanceToFollow );
		Vector3 targetAnchorPosition = m_AnchorT.position + ( directionToAnchorT * distanceToAnchor );

		targetPosition = ( targetFollowPosition + targetAnchorPosition ) / 2f;

		if( m_Smoothing != 0 )
			m_Position = Vector3.Lerp( m_Position, targetPosition, m_Smoothing * Time.deltaTime );
		else
			m_Position = targetPosition;


		//if( (m_Position - m_FollowT.position).magnitude > m_FollowDistance )
		//	m_Position = m_FollowT.position + ( (m_Position - m_FollowT.position).normalized * m_FollowDistance );
/*
		if( (m_Position - m_AnchorT.position).magnitude > m_FollowDistance )
			m_Position = m_AnchorT.position + ( (m_Position - m_AnchorT.position).normalized * m_FollowDistance );
		*/

		m_Transform.position = m_Position;
	}

	void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere( m_Position, .3f );
	}
}
