using UnityEngine;
using System.Collections;

/// <summary>
/// ET_ project manager.
///  - Stores information about the project
///  - Water marking / logos
///  - Handle application quitting
///  - TO DO:
///  - Handles timeout / usage incase of demo
///  - 
/// </summary>

[ AddComponentMenu( "Ethno Tekh/Project Manager" ) ]
public class ET_ProjectManager : ET_ManagerBase
{
	public enum ProjectState
	{
		Alpha,
		Beta,
		Final,
	}

	static ET_ProjectManager 			m_Instance { get; set; }
	public static ET_ProjectManager 	Instance{ get{ return m_Instance; } }

	public string 			m_ProjectName = "Ethno Tekh" ;
	public string 			m_Version 	= "v001001" ;
	public ProjectState 	m_State = ProjectState.Alpha;

	// Logo
	public bool 			m_DrawLogo = false;
	public Texture 			m_Logo;
	public Vector2 			m_LogoGUITexSize = new Vector2( 120, 120 );

	public int m_TargetFrameRate = 0;

	public bool m_DateLocked = false;
	public int m_LockDate = 1;
	public int m_LockMonth = 1;
	public int m_LockYear = 2014;


	void Awake()
	{
		m_Instance = this;
	}

	protected override void Start () 
	{

		base.Start();

		if( m_TargetFrameRate != 0 )
			Application.targetFrameRate = m_TargetFrameRate;

		CheckLock();
	}

	void Update()
	{
		if( Input.GetKeyDown( KeyCode.Escape ) ) 
			Application.Quit();
	}

	void CheckLock()
	{
		if( m_DateLocked )
		{
			System.DateTime date = System.DateTime.Now;
			bool locked = false;
			
			
			if( date.Year > m_LockYear )
			{
				locked = true;
			}
			else if( date.Month > m_LockMonth )
			{
				locked = true;
			}
			else if( date.Month == m_LockMonth && date.Day > m_LockDate )
			{
				locked = true;
			}

			print( "************** DATE LOCK *********************");
			print( m_LockDate + "/" + m_LockMonth + "/" + m_LockYear + "    " + date + "     Locked: " + locked );
			
			if( locked )
				Application.Quit();

		
		}
	}
	
	protected override void OnGUI()
	{
		base.OnGUI();
		
		if( m_DrawLogo )
			DrawLogo();
	}
	
	void DrawLogo()
	{
		GUI.DrawTexture( new Rect( Screen.width - m_LogoGUITexSize.x - 10 , Screen.height - m_LogoGUITexSize.y - 10, m_LogoGUITexSize.x, m_LogoGUITexSize.y),  m_Logo, ScaleMode.ScaleToFit, true );
	}
	
	void DrawGUIWindow()
	{
		
		// GUI for selecting scenes and reloading current		
		GUILayout.Label( "Scene loader going here" );
	}
}
