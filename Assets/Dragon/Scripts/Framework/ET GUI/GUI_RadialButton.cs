using UnityEngine;
using System.Collections;

public class GUI_RadialButton 
{
	public bool 		m_RadialDown = false;
	public float 		m_NormalizedSelection;
	public int 			m_SelectedIndex = 0;
	public string[] 	m_Options;

	public Vector2 m_MouseDown;


	public GUI_RadialButton( string[] options, int currentlySelected )
	{
		m_SelectedIndex = currentlySelected;
		m_Options = options;
	}

	public float radius = 40;
	// returns 999 if not selected
	public bool Draw( float size )
	{
		//m_SelectedIndex = m_SelectedIndex.WrapIntToRange( 0, m_Options.Length - 1 );
		if( GUILayout.RepeatButton( m_Options[ m_SelectedIndex ], GUILayout.Width( size ), GUILayout.Height( size ) ) )
		{
			if( !m_RadialDown )
			{
				m_RadialDown = true;
				m_MouseDown = Event.current.mousePosition;
				ET_GUIManager.Instance.m_ActiveRadialGUI = this;
			}
		}
		
		if( m_RadialDown && Input.GetMouseButton( 0 ) )
		{
			/*
			GUI.Box( new Rect( m_MouseDown.x, m_MouseDown.y, size*2, size*2 ), "" );
			float divisionAngle = 360 / m_Options.Length; //Angle between menu items
		
			// Place items in correct spot
			for(int i = 0; i < m_Options.Length; i++)
			{
				Vector2 localPos = Vector2.zero;
				localPos.x = Mathf.Sin( i * divisionAngle * Mathf.Deg2Rad ) * radius;
				localPos.x += m_MouseDown.x;
				localPos.y = Mathf.Cos( i * divisionAngle * Mathf.Deg2Rad ) * radius;
				localPos.y += m_MouseDown.y;

				GUI.Label( new Rect( localPos.x, localPos.y, 100, 100 ), m_Options[ i ] );
			}

			float m_NormalizedSelection = 1 - (  ( Utils.SignedAngleBetweenVectors( -Vector3.up, InputManager.Instance.m_NormalizedRelativeMouseDownValue.normalized  ) + 180 ) / 360 );
			m_SelectedIndex = (int)( m_NormalizedSelection * m_Options.Length);
			//Debug.Log( m_NormalizedSelection );
			*/
		}
		else if( m_RadialDown && !Input.GetMouseButton( 0 ) )
		{
			//Utils.SignedAngleBetweenVectors( Vector3.up, InputManager.Instance.m_NormalizedRelativeMouseDownValue.normalized  );
			m_RadialDown = false;

			return true;
		}

		return false;
	}

}
