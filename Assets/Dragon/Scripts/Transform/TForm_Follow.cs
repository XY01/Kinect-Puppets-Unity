using UnityEngine;
using System.Collections;

public class TForm_Follow : MonoBehaviour 
{
	// reference to this transform
	Transform 	m_Transform;

	// reference to this position
	Vector3 	m_Position;

	// The space in which it follows
	Space 		m_Space = Space.World;

	// The distance at which to follow
	public float m_FollowDistance = 0;

	// Transform to follow
	public Transform m_FollowT;

	// Flags weather to follow all axis
	public bool m_FollowXPos = true;
	public bool m_FollowYPos = true;
	public bool m_FollowZPos = true;

	// Determines if the transform looks toward the transform it is following
	public bool m_LookAtTransform = false;

	// Smoothes the following movement
	public float m_Smoothing = 0;
	
	// Use this for initialization
	void Start ()
	{
		m_Transform = gameObject.GetComponent< Transform >();

		if( m_Space == Space.Self )	m_Position = m_Transform.localPosition;
		else 						m_Position = m_Transform.position;
	}

    public Vector3 m_RotationOffset;

	// Update is called once per frame
	void LateUpdate ()
	{
		// returns if there is no transform to follow
		if( m_FollowT == null )
			return;

		Vector3 targetPosition;
		Vector3 directionToFollowT = ( m_Transform.position - m_FollowT.position ).normalized;

		if( m_FollowDistance != 0 )		
			targetPosition = m_FollowT.position + ( directionToFollowT * m_FollowDistance );
		else
			targetPosition = m_FollowT.position;
		
		if( m_FollowXPos )
		{
			if( m_Smoothing == 0 ) 	m_Position.x = targetPosition.x;
			else 					m_Position.x = Mathf.Lerp( m_Position.x, targetPosition.x, m_Smoothing * Time.deltaTime );
		}
		
		if( m_FollowYPos )
		{
			if( m_Smoothing == 0 ) 	m_Position.y = targetPosition.y;
			else 					m_Position.y = Mathf.Lerp( m_Position.y, targetPosition.y, m_Smoothing * Time.deltaTime );
		}
		
		if( m_FollowZPos )
		{
			if( m_Smoothing == 0 ) 	m_Position.z = targetPosition.z;
			else 					m_Position.z = Mathf.Lerp( m_Position.z, targetPosition.z, m_Smoothing * Time.deltaTime );
		}

		if( m_Space == Space.Self )	m_Transform.localPosition = m_Position;
		else 						m_Transform.position = m_Position;

		if( m_LookAtTransform )
		{
            if (m_Smoothing != 0)
            {
                Quaternion targetLookAtRotation = Quaternion.identity;
                if (directionToFollowT != Vector3.zero) targetLookAtRotation.SetLookRotation(directionToFollowT);

                targetLookAtRotation *= Quaternion.Euler(m_RotationOffset);

                m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetLookAtRotation, m_Smoothing * Time.deltaTime);
            }
            else
            {
                Quaternion targetLookAtRotation = Quaternion.identity;
                if (directionToFollowT != Vector3.zero) targetLookAtRotation.SetLookRotation(directionToFollowT);

                targetLookAtRotation *= Quaternion.Euler(m_RotationOffset);

                m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, targetLookAtRotation, 1);
                
            }
		}
	}
}
