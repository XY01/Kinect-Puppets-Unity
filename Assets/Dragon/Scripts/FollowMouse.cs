using UnityEngine;
using System.Collections;

/// <summary>
/// Follow mouse point in world.
/// - Positions the object based on the mouse position in camera space, with a z offset
/// 
/// TODO: 
///  - Extend to incorporate different spaces, world or screen.
/// </summary>

public class FollowMouse : MonoBehaviour
{
	public Camera m_Camera;
	public float m_ZOffset = 0;
	
	public float m_Smoothing = 5; 
	public bool  m_Paused = false;
	public bool  Paused { get{ return m_Paused; } set{ m_Paused = value; } }

	public bool m_OnLeftClick = false;
	public bool m_OnRightClick = false;
	
	void Start ()
	{
		if( m_Camera == null )
		{
			m_Camera = Camera.main;
		}

		//InputMonitor.onInputTimedOut += onInputTimedOut;
		//InputMonitor.onInputActive += onInputActive;
	}
	
	void Update ()
	{
		if( m_Paused ) return;
		//transform.position = Camera.main.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z) );
		//Vector3 pos = 


		if( m_OnRightClick )
		{
			if( Input.GetMouseButton( 1 ) )
			{
				if( Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height )
				{
					Vector3 mousePosInWorld = m_Camera.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, m_ZOffset) );
					//transform.position =  m_Camera.ViewportToWorldPoint( new Vector3( .5f, .5f, -m_Camera.transform.position.z) );
					if( m_Smoothing > 0 )
						transform.position =  Vector3.Lerp( transform.position, mousePosInWorld, Time.deltaTime * m_Smoothing);
					else
						transform.position = mousePosInWorld;
				}
			}
		}
		else
		{
			if( Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width && Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height )
			{
				Vector3 mousePosInWorld = m_Camera.ScreenToWorldPoint( new Vector3( Input.mousePosition.x, Input.mousePosition.y, m_ZOffset) );
				//transform.position =  m_Camera.ViewportToWorldPoint( new Vector3( .5f, .5f, -m_Camera.transform.position.z) );
				if( m_Smoothing > 0 )
					transform.position =  Vector3.Lerp( transform.position, mousePosInWorld, Time.deltaTime * m_Smoothing);
				else
					transform.position = mousePosInWorld;
			}
		}


	}
	
	void onInputTimedOut()
	{
		//collider.enabled = false;
	}
	
	void onInputActive()
	{
		//collider.enabled = true;
	}
}
