using UnityEngine;
using System.Collections;

/// <summary>
/// ET_ manager base.
///  - Base class for all managers, handles the windows assignment and drawing
/// </summary>
[RequireComponent (typeof ( ET_GUIWindow ))]
public class ET_ManagerBase : MonoBehaviour
{	
	public string			m_GUIWindowName = "Manager";
	protected ET_GUIWindow 	m_GUIWindow;

	public ET_GUIWindow GUIWindow
	{
		get
		{
			return m_GUIWindow;
		}
	}		
	
	protected Vector2 		m_GUIScroll = Vector2.zero;	
	public bool 			m_Debug = false;
	
	
	protected virtual void Start ()
	{
		m_GUIWindow = gameObject.GetComponent< ET_GUIWindow >();
		
		if( m_GUIWindow == null )
			m_GUIWindow = gameObject.AddComponent< ET_GUIWindow >() as ET_GUIWindow;
			
		m_GUIWindow.Init( m_GUIWindowName, gameObject );
	}

	protected virtual void OnGUI()
	{
		m_GUIWindow.BeginWindow();
	}	
	
	protected virtual void DrawGUIWindow()
	{
		GUILayout.Label( "Impliment GUI for this manager: " + name );
	}
}
