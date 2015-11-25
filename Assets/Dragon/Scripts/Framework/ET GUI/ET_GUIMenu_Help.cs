using UnityEngine;
using System.Collections;

[RequireComponent (typeof ( ET_GUIWindow ))]
public class ET_GUIMenu_Help : MonoBehaviour 
{
	static 			ET_GUIMenu_Help m_Instance { get; set; }
	public static 	ET_GUIMenu_Help Instance{ get { return m_Instance; } }
	
	string 			m_HelpContent;

	public ET_GUIWindow 	m_GUIWindow;
	public string 	m_ReadMeName;
	
	void Awake()
	{
		m_Instance = this;
	}
	
	void Start ()
	{
		m_GUIWindow = gameObject.GetComponent< ET_GUIWindow >();
		m_GUIWindow.Init( "About", gameObject );

		TextAsset txt = (TextAsset)Resources.Load(m_ReadMeName, typeof(TextAsset));
		
		if( txt == null )
		{
			print( "No readme file found" );
			m_HelpContent = "No readme file found";
		}
		else
		{
			m_HelpContent = txt.text;
		}
	}

	void OnGUI()
	{
		m_GUIWindow.BeginWindow();
	}

	void DrawGUIWindow()
	{
		GUILayout.TextArea( m_HelpContent );
	}
}
