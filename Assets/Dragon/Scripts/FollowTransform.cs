using UnityEngine;
using System.Collections;


[AddComponentMenu("Ethno Tekh Framework/Transform/Follow Transform")]
public class FollowTransform: MonoBehaviour
{
	public Transform m_TransformToFollow;
	public float m_FollowDistance = 1;
	public float m_Smoothing = 1;
	public bool m_UpdateFacing = false;
	
	public Space m_Space = Space.Self;
	public Space m_ToSpace = Space.Self;
	
	public Vector3 m_Scaler = Vector3.one;


    public float MasterScaler
    {
        set
        {
            m_Scaler = Vector3.one * value;
        }
    }

    public float YOffset
    {
        set
        {
            m_Offset.y = value;
        }
    }

    public Vector3 m_Offset = Vector3.zero;

	public bool m_OnPress = false;

	public bool m_FollowRot = false;
	public bool m_FollowScale = false;
	
	void Start()
	{
	}
	
	void LateUpdate ()
	{
		if( m_TransformToFollow == null )
			return;

		if( m_OnPress )
			if( !Input.GetMouseButton( 2 ) )
				return;

		if( m_Space == Space.Self )
		{
			Vector3 scaledPosition = m_TransformToFollow.localPosition + m_Offset;
			scaledPosition.Scale( m_Scaler );
			Vector3 directionToObject = scaledPosition - transform.localPosition;
		
			float distanceToObject = directionToObject.magnitude;
			
			Vector3 positionToMoveTo;
			if( m_FollowDistance == 0 )
			{
				positionToMoveTo = scaledPosition;

                if (m_Smoothing == 0)
                {
                    if (m_ToSpace == Space.World)
                        transform.position = positionToMoveTo;
                    else if (m_ToSpace == Space.Self)
                        transform.localPosition = positionToMoveTo;
                }
                else
                {
                    if (m_ToSpace == Space.World)
                        transform.position = Vector3.Lerp(transform.position, positionToMoveTo, m_Smoothing * Time.deltaTime);
                    else if (m_ToSpace == Space.Self)
                        transform.localPosition = Vector3.Lerp(transform.localPosition, positionToMoveTo, m_Smoothing * Time.deltaTime);
                }
			}
			else
			{
				positionToMoveTo = scaledPosition - ( directionToObject.normalized * m_FollowDistance );

                if (m_ToSpace == Space.World)
                    transform.position = positionToMoveTo;
                else if (m_ToSpace == Space.Self)
                    transform.localPosition = positionToMoveTo;
			}
			
			if( m_UpdateFacing ) transform.LookAt( positionToMoveTo );
			
			if( distanceToObject > m_FollowDistance )
			{		
				if( m_Smoothing == 0 )
				{
					if( m_UpdateFacing ) transform.LookAt( positionToMoveTo );					
					
					if( m_ToSpace == Space.World )					
						transform.position = positionToMoveTo;
					else if( m_ToSpace == Space.Self )	
						transform.localPosition = positionToMoveTo;
				}
				else 	
				{				
					float smoothing = distanceToObject / m_FollowDistance;
					
					if( m_ToSpace == Space.World )					
						transform.position = Vector3.Lerp( transform.position, positionToMoveTo, m_Smoothing * Time.deltaTime );
					else if( m_ToSpace == Space.Self )	
						transform.localPosition = Vector3.Lerp( transform.localPosition, positionToMoveTo, m_Smoothing * Time.deltaTime );
				}
			}
		}
		else
		{
            Vector3 scaledPosition = m_TransformToFollow.position + m_Offset;
            scaledPosition.Scale(m_Scaler);
            Vector3 directionToObject = scaledPosition - transform.position;

            float distanceToObject = directionToObject.magnitude;

            Vector3 positionToMoveTo;
            if (m_FollowDistance == 0)
            {
                positionToMoveTo = scaledPosition;

                if (m_ToSpace == Space.World)
                    transform.position = positionToMoveTo;
                else if (m_ToSpace == Space.Self)
                    transform.localPosition = positionToMoveTo;
            }
            else
            {
                positionToMoveTo = scaledPosition - (directionToObject.normalized * m_FollowDistance);

                if (m_ToSpace == Space.World)
                    transform.position = positionToMoveTo;
                else if (m_ToSpace == Space.Self)
                    transform.localPosition = positionToMoveTo;
            }
            		
			
			
			if( m_UpdateFacing ) transform.LookAt( positionToMoveTo );
			
			if( distanceToObject > m_FollowDistance )
			{		
				if( m_Smoothing == 0 )
				{
                    if (m_UpdateFacing) transform.LookAt(m_TransformToFollow.position + m_Offset);
					transform.position = positionToMoveTo;
				}
				else 	
				{				
					float smoothing = distanceToObject / m_FollowDistance;
					transform.position = Vector3.Lerp( transform.position, positionToMoveTo, m_Smoothing * Time.deltaTime );
				}
			}
		}

		if( m_FollowRot )
			transform.rotation = m_TransformToFollow.rotation;

		if( m_FollowScale )
			transform.localScale = m_TransformToFollow.localScale;

		
	}
}
