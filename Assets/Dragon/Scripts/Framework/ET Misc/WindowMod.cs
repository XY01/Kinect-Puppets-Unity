/* 
* Custom fullscreen and Borderless window script by Martijn Dekker (Pixelstudio) 
* For questions pls contact met at martijn.pixelstudio@gmail.com 
* version 0.1 
* 
*/  
  
using System;  
using System.Collections;  
using System.Runtime.InteropServices;  
using System.Diagnostics;  
using UnityEngine;  
  
[ RequireComponent( typeof( ET_GUIWindow ))]
public class WindowMod : MonoBehaviour  
{  	
	static WindowMod 			m_Instance { get; set; }	 // Static singleton property 
	public static WindowMod 	Instance { get{ return m_Instance; } }	// Static singleton property

	public delegate void 	SetScreenCount( int screenCount );
	public static event 	SetScreenCount onSetScreenCount;	
	
	//public Rect screenPosition;  
	  
	[DllImport("user32.dll")]  
	static extern IntPtr SetWindowLong (IntPtr hwnd,int  _nIndex ,int  dwNewLong);  
	[DllImport("user32.dll")]  
	static extern bool SetWindowPos (IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);  
	[DllImport("user32.dll")]  
	static extern IntPtr GetForegroundWindow ();  
	  
	Vector2[] 				m_Resolutions;
	public int 				m_ResolutionIndex;
	public static int 		m_ScreenCount = 1;
	
	// not used rigth now  
	//const uint SWP_NOMOVE = 0x2;  
	//const uint SWP_NOSIZE = 1;  
	//const uint SWP_NOZORDER = 0x4;  
	//const uint SWP_HIDEWINDOW = 0x0080;  
	   
	const uint 	SWP_SHOWWINDOW = 0x0040;
	const int 	GWL_STYLE = -16;  
	const int 	WS_BORDER = 1;  
	
	int m_SnapToMonitor = 0;

	public ET_GUIWindow m_GUIWindow;
	
	public bool m_ResizeWindowOnStartup = false;
	
	// GUI
	int m_WindowIndex;
	  
	void Awake ()  
	{  
		m_Instance = this;

		m_Resolutions = new Vector2[ 7 ];
		m_Resolutions[0] = new Vector2( 800, 600 );
		m_Resolutions[1] = new Vector2( 1024, 768 );
		m_Resolutions[2] = new Vector2( 1280, 720 );
		m_Resolutions[3] = new Vector2( 1280, 800 );
		m_Resolutions[4] = new Vector2( 1600, 900 );
		m_Resolutions[5] = new Vector2( 1920, 1080 );
		m_Resolutions[6] = new Vector2( 3840, 1200 );

		Load();
		if( m_ResizeWindowOnStartup && !Screen.fullScreen )
			ResizeWindow( );		
	}
	
	void Start()
	{
		m_GUIWindow = gameObject.GetComponent<ET_GUIWindow>() as ET_GUIWindow;
		m_GUIWindow.Init( "Resolution Settings", gameObject, KeyCode.W );

		// Get unique window index
		m_WindowIndex = Utils.GetWindowIndex();		
		

	}
	
	void Update()
	{
		if( Input.GetKey( KeyCode.LeftShift ) )
		{
			if( Input.GetKeyDown( KeyCode.LeftArrow ) )
			{
				m_SnapToMonitor--;				
				m_SnapToMonitor = Mathf.Clamp( m_SnapToMonitor, 0, 2 );
				ResizeWindow();				
			}
			else if( Input.GetKeyDown( KeyCode.RightArrow ) )
			{
				m_SnapToMonitor++;				
				m_SnapToMonitor = Mathf.Clamp( m_SnapToMonitor, 0, 2 );
				ResizeWindow();				
			}
		}
		//if( Input.GetKeyDown("7"))	ResizeWindow(0);
		//if( Input.GetKeyDown("8"))	ResizeWindow(1);
		//if( Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown( KeyCode.R ) )	m_DrawGUIWindow = !m_DrawGUIWindow;
	}

	void Save()
	{
		string prefix = ET_ProjectManager.Instance.m_ProjectName;
		PlayerPrefs.SetInt( 		prefix + "m_ScreenCount", 			m_ScreenCount );
		PlayerPrefs.SetInt( 		prefix + "m_SnapToMonitor", 		m_SnapToMonitor );
		PlayerPrefs.SetInt( 		prefix + "m_ResolutionIndex", 		m_ResolutionIndex );
		PlayerPrefsPlus.SetBool( 	prefix + "m_ResizeWindowOnStartup", m_ResizeWindowOnStartup );
	}

	void Load()
	{
		string prefix = ET_ProjectManager.Instance.m_ProjectName;
		m_ScreenCount = PlayerPrefs.GetInt( 				prefix + "m_ScreenCount"			, 1 );
		//m_SnapToMonitor = PlayerPrefs.GetInt( 				prefix + "m_SnapToMonitor"			, 0 );
		m_SnapToMonitor = 0;
		m_ResolutionIndex = PlayerPrefs.GetInt( 			prefix + "m_ResolutionIndex"		, 3 );
		m_ResizeWindowOnStartup = PlayerPrefsPlus.GetBool( 	prefix + "m_ResizeWindowOnStartup"	, true );
	}

	void OnApplicationQuit()
	{
		Save();
	}

	void ResizeWindow()
	{
		if( Application.isEditor ) 
			return;
		ET_DebugManager.Instance.Print( "LOL" );
		if( onSetScreenCount != null )
		{
			ET_DebugManager.Instance.Print( "Snapping" );
			onSetScreenCount( m_ScreenCount );
		}

		Screen.SetResolution( (int)m_Resolutions[ m_ResolutionIndex ].x * m_ScreenCount, (int)m_Resolutions[ m_ResolutionIndex ].y, false );			
		StartCoroutine( WaitAndSetScreenSize(.5F) );
	}
	
    IEnumerator WaitAndSetScreenSize(float waitTime)
	{
        yield return new WaitForSeconds(waitTime);
		
        SetWindowLong( GetForegroundWindow (), GWL_STYLE, WS_BORDER );  
		bool result = SetWindowPos( GetForegroundWindow (), 0, m_SnapToMonitor * (int)m_Resolutions[ m_ResolutionIndex ].x , 0, (int)m_Resolutions[ m_ResolutionIndex ].x * m_ScreenCount, (int)m_Resolutions[ m_ResolutionIndex ].y, SWP_SHOWWINDOW );
    }

	void OnGUI()
	{
		m_GUIWindow.BeginWindow();
	}	
	 
	public void DrawGUIWindow()
	{
		GUILayout.BeginVertical();

		m_ResizeWindowOnStartup = GUILayout.Toggle( m_ResizeWindowOnStartup, "Resize at startup: " );

		GUILayout.Label( "Number of screens: " + m_ScreenCount );
		
		GUILayout.BeginHorizontal();
		{
			if( GUILayout.Button( "1" ) )
			{
				m_ScreenCount = 1;
				ResizeWindow();
			}
			if( GUILayout.Button( "2" ) )
			{
				m_ScreenCount = 2;
				ResizeWindow();
			}
			if( GUILayout.Button( "3" ) )
			{
				m_ScreenCount = 3;
				ResizeWindow();
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		{
			if( GUILayout.Button( "Snap to monitor 1" ) )
			{
				m_SnapToMonitor = 0;
				ResizeWindow();
			}
			if( GUILayout.Button( "Snap to monitor 2" ) )
			{
				m_SnapToMonitor = 1;
				ResizeWindow();
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.Space( 10 );
		
		for( int i = 0; i < m_Resolutions.Length; i++ )
		{
			if( GUILayout.Button( m_Resolutions[i].x + " x " + m_Resolutions[i].y ) )
			{
				m_ResolutionIndex = i;
				ResizeWindow();
			}
		}
		
		GUILayout.Space( 10 );
		
		GUILayout.EndVertical();
	}
}  