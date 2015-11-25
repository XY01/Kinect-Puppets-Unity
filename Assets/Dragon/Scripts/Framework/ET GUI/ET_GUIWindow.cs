using UnityEngine;
using System.Collections;

public class ET_GUIWindow : MonoBehaviour
{
	public enum ScreenXAnchor
	{
		None,
		Left,
		Middle,
		Right,
	}

	public enum ScreenYAnchor
	{		
		None,
		Top,
		Middle,
		Bottom,
	}

	int m_WindowIndex;
	public ScreenXAnchor m_XAnchor = ScreenXAnchor.Middle;
	public ScreenYAnchor m_YAnchor = ScreenYAnchor.Middle;

	public bool 	m_DrawWindow = false;
	public Rect 	m_WindowGUI_Rect = new Rect( Screen.width / 3 , Screen.height/2, 250, 400 );
	Rect 			m_OffsetWindowGUI_Rect;
	public string 	m_WindowName = "";
	
	public GameObject m_ObjectToCallFrom;

	public KeyCode m_ToggleKey = KeyCode.Insert;

	public bool m_Horizontal = false;

	public bool m_AdjustForHeader = true;
	public bool m_DisplaySolo = true;
	public bool m_DisplayClose = true;
	public bool m_Draggable = true;
		
	void Awake()
	{
		m_WindowIndex = Utils.GetWindowIndex();
        m_OffsetWindowGUI_Rect = m_WindowGUI_Rect;
	}

	void Start()
	{
		ET_GUIManager.Instance.RegisterWindow( this );
	}

	void Update()
	{
		//Anchor(); // hax

		m_WindowGUI_Rect.x = Mathf.Min( m_WindowGUI_Rect.x, ( Screen.width / WindowMod.m_ScreenCount ) - m_WindowGUI_Rect.width );

		if( Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown( m_ToggleKey ) )
		{
			m_DrawWindow = !m_DrawWindow;
		}

		if( Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown( KeyCode.E) )
		{
			Anchor();
		}
	}
	
	public void Init( string name, GameObject gO )
	{
		m_ObjectToCallFrom = gO;
		m_WindowName = name;

		Anchor();
	}

	void onSetScreenCount( int screens )
	{
		Invoke( "Anchor", .5f );
	}

	public void Anchor()
	{
		float adjustedWidth = Screen.width / WindowMod.m_ScreenCount;
		
		if( m_XAnchor == ScreenXAnchor.Left )
		{
			m_WindowGUI_Rect.x = 0;
		}
		else if( m_XAnchor == ScreenXAnchor.Right )
		{
			m_WindowGUI_Rect.x = adjustedWidth - m_WindowGUI_Rect.width;
		}
		else if( m_XAnchor == ScreenXAnchor.Middle )
		{
			m_WindowGUI_Rect.x = (adjustedWidth/2) - ( m_WindowGUI_Rect.width / 2 );
		}
		
		if( m_YAnchor == ScreenYAnchor.Top )
		{
			if( m_AdjustForHeader )
				m_WindowGUI_Rect.y = 45;
			else
				m_WindowGUI_Rect.y = 0;
		}
		else if( m_YAnchor == ScreenYAnchor.Bottom )
		{
			m_WindowGUI_Rect.y = Screen.height - m_WindowGUI_Rect.height;
		}
		else if( m_YAnchor == ScreenYAnchor.Middle )
		{
			m_WindowGUI_Rect.y = ( Screen.height / 2 ) - ( m_WindowGUI_Rect.height / 2 );
		}
	}

	public void Init( string name, GameObject gO, KeyCode key )
	{
		Init( name, gO );
		m_ToggleKey = key;
	}

	public void ToggleWindow()
	{
		m_DrawWindow = !m_DrawWindow;
	}

	string blankWindowName = "";
	public void BeginWindow( )
	{
		if( ET_GUIManager.Instance.SoloMode && ET_GUIManager.Instance.SoloWindow != this )	// If in solo mode and this isnt the solo window, return
			return;

		if( m_DrawWindow && ET_GUIManager.Instance.m_DrawGUI )
		{
			m_OffsetWindowGUI_Rect.x = ET_GUIManager.Instance.m_Offset.x + m_WindowGUI_Rect.x;
			m_OffsetWindowGUI_Rect.y = ET_GUIManager.Instance.m_Offset.y + m_WindowGUI_Rect.y;
			m_OffsetWindowGUI_Rect.width = m_WindowGUI_Rect.width;
			m_OffsetWindowGUI_Rect.height = m_WindowGUI_Rect.height;

			ET_GUIManager.Instance.UseDefaultSkin();
			m_WindowGUI_Rect = GUILayout.Window( m_WindowIndex, m_OffsetWindowGUI_Rect, DrawWindow_GUI, blankWindowName );

			m_WindowGUI_Rect.x -= ET_GUIManager.Instance.m_Offset.x;
			m_WindowGUI_Rect.y -=  ET_GUIManager.Instance.m_Offset.y;
		}
	}

	public void DrawWindowButton()
	{
		if( GUILayout.Button( m_WindowName, GUILayout.Width( 150 )) )
			ToggleWindow();
	}
	
	public void DrawWindow_GUI( int windowID )
	{
		GUILayout.BeginHorizontal("box");
		{
			GUILayout.Label(m_WindowName);
			GUILayout.FlexibleSpace();
			if( m_DisplaySolo )
			{
				if( GUILayout.Button( "S", GUILayout.Width( 25 )) )
				{
					ET_GUIManager.Instance.SoloToggle( this );
				}
			}

			if( m_DisplayClose )
			{
				if( GUILayout.Button( "X", GUILayout.Width( 25 )) )
					m_DrawWindow = false;
			}
		}
		GUILayout.EndHorizontal();

		if( m_Horizontal )
		{
			GUILayout.BeginHorizontal( "box" );
			{
				m_ObjectToCallFrom.SendMessage( "DrawGUIWindow" );
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginVertical("box" );
			{
				m_ObjectToCallFrom.SendMessage( "DrawGUIWindow" );
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndVertical();
		}

		if( m_Draggable )
			GUI.DragWindow( new Rect(0, 0, Screen.width, Screen.height ) );	
	}
	
}
