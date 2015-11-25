using UnityEngine;
using System.Collections;

[RequireComponent (typeof ( ET_GUIWindow ))]
public class ET_GUIMenu_Options : MonoBehaviour 
{
	static 			ET_GUIMenu_Options m_Instance { get; set; }
	public static 	ET_GUIMenu_Options Instance{ get { return m_Instance; } }
	
	public GameObject[] m_Options;
	public ET_GUIWindow m_GUIWindow;
	
	
	void Awake()
	{
		m_Instance = this;
	}
	
	void Start ()
	{
		m_GUIWindow = gameObject.GetComponent< ET_GUIWindow >();
		m_GUIWindow.Init( "OPTIONS", gameObject );
	}

	void OnGUI()
	{
		m_GUIWindow.BeginWindow();
	}

	Vector2 scroll = Vector2.zero;
	Vector2 scroll2 = Vector2.zero;
	void DrawGUIWindow()
	{
		scroll = GUILayout.BeginScrollView( scroll );
		for( int i = 0; i < m_Options.Length; i++ )
		{
			if( m_Options[i].activeSelf )
				m_Options[i].SendMessage( "DrawWindowButton" );
		}
		GUILayout.EndScrollView();

		GUILayout.Space( 20 );
		scroll2 = GUILayout.BeginScrollView( scroll2 );
		for( int i = 0; i < ET_GUIManager.Instance.AllWindows.Count; i++ )
		{
			if( ET_GUIManager.Instance.AllWindows[i].gameObject.activeSelf &&  ET_GUIManager.Instance.AllWindows[i].gameObject != gameObject )
				ET_GUIManager.Instance.AllWindows[i].gameObject.SendMessage( "DrawWindowButton" );
		}
		GUILayout.EndScrollView();

	}
}
