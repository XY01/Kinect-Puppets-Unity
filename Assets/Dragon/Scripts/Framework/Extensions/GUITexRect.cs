using UnityEngine;
using System.Collections;


[ System.Serializable ]
public class GUITexRect
{
	public enum Anchor
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
	}
	
	public Anchor m_Anchor = Anchor.BottomRight;
	
	public Vector2 m_Size = new Vector2( 160, 120 );
	public Vector2 m_Offset = new Vector2( 0, 0 );
	
	public Rect GetRect()
	{
		Rect newRect = new Rect( 0, 0, 0, 0 );
		
		if( m_Anchor == Anchor.TopLeft )
		{
			newRect = new Rect( 0 + m_Offset.x, m_Offset.y, m_Size.x , m_Size.y );
		}
		else if( m_Anchor == Anchor.TopRight )
		{
			newRect = new Rect( Screen.width - m_Size.x + m_Offset.x, m_Offset.y, m_Size.x , m_Size.y );
		}
		else if( m_Anchor == Anchor.BottomRight )
		{
			newRect = new Rect( Screen.width - m_Size.x + m_Offset.x, Screen.height - m_Size.y + m_Offset.y, m_Size.x , m_Size.y );
		}
		else if( m_Anchor == Anchor.BottomLeft )
		{
			newRect = new Rect( 0 + m_Offset.x, Screen.height - m_Size.y + m_Offset.y, m_Size.x , m_Size.y );
		}
		
		return newRect;
	}
}