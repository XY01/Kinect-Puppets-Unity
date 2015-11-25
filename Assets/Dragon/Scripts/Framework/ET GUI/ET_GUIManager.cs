using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ET_ GUI manager.
///  - Handles the GUI skin
///  - Determines which windows draw to screen
///  - Controls mouse drawing to the screen
///  - Handles the offset of the GUI for panning/scrolling through GUI
/// </summary>


public class ET_GUIManager : MonoBehaviour
{
	static ET_GUIManager 			m_Instance { get; set; }	
	public static ET_GUIManager Instance												// Singleton dynamically creates
    {        
        get 
		{	
			GameObject managerParent = GameObject.Find( "* ET Managers" );
			if( managerParent == null )
				managerParent = new GameObject( "* ET Managers" );
			
			if( m_Instance == null )
			{
				m_Instance = new GameObject("GUI Manager").AddComponent< ET_GUIManager >();				
			}		
			
			m_Instance.transform.parent = managerParent.transform;
			
			return m_Instance; 
		}
    }

	public GUISkin 					m_GUISkin;									// GUI skin	
	GUIStyle 						centeredStyle;
	public Vector2 					m_Offset = Vector2.zero;					// The offset of the GUI. Used for panning and scrolling
	public Vector2 					m_TempOffset;								// Temporary offset. Check?
	List< ET_GUIWindow > 			m_AllWindows = new List<ET_GUIWindow>();	// All registered windows
	public List< ET_GUIWindow > 	AllWindows { get{ return m_AllWindows; } }	// Public get accessor for all windows
	public bool 					m_WindowSnapping = false;					// Window snapping flag for snapping windows to one another
	public float 					m_SnappingThreshold = 10;					// Threshold for the window snapping

	bool 							m_SoloMode = false;							// Solo mode flag for displaying a single window for focussing attention
	public bool 					SoloMode{get{ return m_SoloMode; }}			// Solo mode public accessor
	ET_GUIWindow 					m_SoloWindow;								// Window that is selected for solo mode	
	public ET_GUIWindow 			SoloWindow{get{ return m_SoloWindow; }}  	// Public accessor for solo window

	public bool 					m_RadialMenuActive = false;					// Is the radial menu currently active?
	public Texture 					m_RadialBG;									// Radial menu background texture
	public GUI_RadialButton 		m_ActiveRadialGUI;							// The active radial gui	

	public bool 					m_DrawGUI = true;							// Draw GUI flag	
	
	void Awake()
	{
		m_Instance = this;	
	}

	void Start()
	{
		m_GUISkin =  Resources.Load( "ET GUI Skin", typeof( GUISkin ) ) as GUISkin;
	}

	void Update()
	{
		if( Input.GetKey( KeyCode.LeftShift ) )
		{
			if( Input.GetKeyDown( KeyCode.G ) )
			{
				ToggleGUI();
			}
		}
		/*
		if( Input.GetMouseButtonDown( 2 ) )
		{
			m_TempOffset = m_Offset;
		}
		
		else if( Input.GetMouseButton( 2 ) && InputManager.Instance.Alt2Active )
		{
			m_Offset.x = m_TempOffset.x + ( InputManager.Instance.m_NormalizedRelativeMouseDownValue.x * Screen.width );
			m_Offset.y = m_TempOffset.y + ( -InputManager.Instance.m_NormalizedRelativeMouseDownValue.y * Screen.height );
		}
		
		
		if( InputManager.Instance.Alt2Active && Input.GetKeyDown( KeyCode.R) )
		{
			m_Offset = Vector2.zero;
		}

		if( Input.GetKey( KeyCode.LeftShift ) &&  Input.GetKeyDown( KeyCode.G )  )
		{			
			m_DrawGUI = !m_DrawGUI;
		}
		*/
	}
	
	void ToggleGUI()
	{
		m_DrawGUI = !m_DrawGUI;
		Cursor.visible = m_DrawGUI;
		
	}

	public void UseDefaultSkin()
	{
		GUI.skin = ET_GUIManager.Instance.m_GUISkin;
	}
	
	/*
	public void RegisterMenu( GUI_Menu menu )
	{
		m_Menus.Add( menu );
	}
	*/

	public void RegisterWindow( ET_GUIWindow window )
	{
		m_AllWindows.Add( window );
	}

	public void SoloToggle( ET_GUIWindow win )
	{
		if( m_SoloWindow == win && m_SoloMode )	// Incase solo is already active then it deactivates. So the solo 
			DisableSoloMode();
		else
		{
			m_SoloMode = true;
			m_SoloWindow = win;
		}
	}

	public void DisableSoloMode( )
	{
		m_SoloMode = false;
	}

	public void CheckWindowSnapping( ET_GUIWindow window )	// Window snapping
	{
		Vector2 closestSnapPos;

		foreach( ET_GUIWindow win in m_AllWindows )
		{
			//if( win.m_WindowGUI_Rect)
		}
	}
	
	/*
	public void AddToMenu( string menuName, GameObject go )
	{
		for( int i = 0; i < m_Menus.Count; i++ )
		{
			if( m_Menus[i].m_MenuName == menuName )
			{
				m_Menus[i].AddMenuItem( go );
			}
		}
	}
	*/

	float size = 240;
	float contentRadius = 80;
	float contentSize = 80;
	void OnGUI()
	{
		if( !m_DrawGUI ) return;
		ET_GUIManager.Instance.UseDefaultSkin();

		if( centeredStyle == null )
			centeredStyle = GUI.skin.GetStyle("Label");

		
		

	}

}
