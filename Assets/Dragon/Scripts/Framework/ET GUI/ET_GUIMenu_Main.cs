 using UnityEngine;
using System.Collections;

public class ET_GUIMenu_Main: MonoBehaviour 
{
	static 			ET_GUIMenu_Main m_Instance { get; set; }
	public static 	ET_GUIMenu_Main Instance{ get { return m_Instance; } }

	public ET_GUIWindow[] 	m_OtherWindows;

	Rect m_GUIRect;

	
	void Awake ()
	{
		m_Instance = this;
		m_GUIRect = new Rect( 0, 0, Screen.width, 35 );
	}
	
	void Start ()
	{
	}
	
	void Update ()
	{
		m_GUIRect.width = Screen.width / WindowMod.m_ScreenCount;

		if( Input.GetKeyDown( KeyCode.Escape ) )
			Application.Quit();
	}	

	void OnGUI()
	{
		if( !ET_GUIManager.Instance.m_DrawGUI )
			return;

		ET_GUIManager.Instance.UseDefaultSkin();
		GUILayout.BeginArea( m_GUIRect );

		GUILayout.BeginHorizontal("box");
		{
			GUILayout.Label( ET_ProjectManager.Instance.m_ProjectName );

			for( int i = 0; i < m_OtherWindows.Length; i++ )
			{
				m_OtherWindows[ i ].DrawWindowButton();
			}
			GUILayout.FlexibleSpace();

			ET_GUIMenu_Options.Instance.m_GUIWindow.DrawWindowButton();
			ET_DebugManager.Instance.GUIWindow.DrawWindowButton();
			ET_GUIMenu_Help.Instance.m_GUIWindow.DrawWindowButton();

			GUILayout.FlexibleSpace();



		}
		GUILayout.EndHorizontal();

		GUILayout.EndArea();
	}
}
