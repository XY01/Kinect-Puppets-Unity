using UnityEngine;
using System.Collections;

/// <summary>
/// ET debug manager.
///  - Shows an ingame debug console
///  - Shows FPS
///  - Adding more...
///  -- Run duration
///  -- Runtime memory usage 	
/// </summary>

public class ET_DebugManager : ET_ManagerBase
{
	static 			ET_DebugManager m_Instance { get; set; }
	public static 	ET_DebugManager Instance{ get { return m_Instance; } }


	float 			m_FPS;
	public float 	FPS { get{ return m_FPS; }  }
	string 			m_DebugText = "";
	bool 			m_UpdateDebugInfo = true;

	void Awake ()
	{
		m_Instance = this;
	}
	
	void Start ()
	{
		base.Start();
	}

	void Update ()
	{
		float prevFPS = m_FPS;
		m_FPS = 1f / Time.deltaTime;
		m_FPS = Mathf.Lerp( prevFPS, m_FPS, Time.deltaTime * 8 );
	}

	public void Print( string debugtext )
	{
		if( m_UpdateDebugInfo )
		{
			Debug.Log( debugtext );
			m_DebugText += debugtext + "\n";
		}
	}

	void DrawGUIWindow()
	{		
		m_GUIScroll = GUILayout.BeginScrollView( m_GUIScroll );
		{
			GUILayout.TextArea( m_DebugText, GUILayout.Height( 300 ) );
		}
		GUILayout.EndScrollView();

		GUILayout.BeginHorizontal();
		{
			m_UpdateDebugInfo = GUILayout.Toggle( m_UpdateDebugInfo, "Update Debug" );

			if( GUILayout.Button( "Clear", GUILayout.Width(80) ))
				m_DebugText = "";
			
			if( GUILayout.Button( "Save", GUILayout.Width(80) ) )
				PlayerPrefs.SetString( "DebugText", m_DebugText );

			GUILayout.FlexibleSpace();

			GUILayout.Label( "FPS: " + m_FPS.ToDoubleDecimalString() );
		}
		GUILayout.EndHorizontal();
	}
}
