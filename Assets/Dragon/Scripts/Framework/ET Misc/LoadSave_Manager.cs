using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Load save_ manager.
/// - Handles loading and saveing of any obejct with a aLoad Save fucntion. It stores an array of strings that the settings are stored under.
/// </summary>
public class LoadSave_Manager : ET_ManagerBase
{
	 // Static singleton property
    static LoadSave_Manager m_Instance { get; set; }
 
    // Static singleton property
    public static LoadSave_Manager Instance
    {
        get 
		{					
			if( m_Instance == null )
				m_Instance = new GameObject("Load Save Manager").AddComponent< LoadSave_Manager >();

			return m_Instance; 
		}
    }

	// Target object
	GameObject m_TargetLoadSaveObject;
	
	// Currently loaded strings
	List< string > m_LoadedStrings = new List<string>();
	
	// Currently selcted index
	int m_SelectedIndex = 0;

	public bool m_KBShortcuts = false;
	
	string m_CurrentPrefix;
	string m_NewSaveName = "New Save Name";

	public bool m_AutoStoreDefaults = true;
	
	void Awake()
	{
		m_Instance = this;
	}
	
	protected override void Start ()
	{
		base.Start();
	}

	void Update()
	{
		if( m_KBShortcuts )
		{
			if( Input.GetKeyDown( KeyCode.DownArrow ) )
			{
				if( m_LoadedStrings.Count > 0 )
				{
					m_SelectedIndex++;
					m_SelectedIndex = m_SelectedIndex.WrapIntToRange( 0, m_LoadedStrings.Count - 1 );
					Load();
					print( "Activated preset: " + m_LoadedStrings[ m_SelectedIndex ] );
				}
			}

			if( Input.GetKeyDown( KeyCode.UpArrow ) )
			{
				if( m_LoadedStrings.Count > 0 )
				{
					m_SelectedIndex--;
					m_SelectedIndex = m_SelectedIndex.WrapIntToRange( 0, m_LoadedStrings.Count - 1 );
					Load();

					print( "Activated preset: " + m_LoadedStrings[ m_SelectedIndex ] );
				}
			}
		}
	}

	public void ActivateLoadSave( GameObject go ) // has ot be GO with Load and Save functions
	{
		ActivateLoadSave( go, go.name, m_GUIWindow.m_DrawWindow );		
	}

	public void ActivateLoadSave( GameObject go, bool showWindow ) // has ot be GO with Load and Save functions
	{
		ActivateLoadSave( go, go.name, showWindow );		
	}
	
	public void ActivateLoadSave( GameObject go, string prefix, bool showWindow ) // has ot be GO with Load and Save functions
	{
		m_GUIWindow.m_DrawWindow = showWindow;
		m_GUIWindow.m_WindowName = "Presets - " + go.name;
			
		if( m_TargetLoadSaveObject != null )
			SaveAllStrings();
		
		m_TargetLoadSaveObject = go;
		m_CurrentPrefix = prefix;
		
		int stringCount = PlayerPrefs.GetInt( m_CurrentPrefix + ".SaveStringsCount", 0 );
		m_LoadedStrings = new List<string>();
				
		for( int i = 0; i < stringCount; i++ )
		{
			m_LoadedStrings.Add( PlayerPrefs.GetString( m_CurrentPrefix + ".SaveString" + i ) );
		}
		
		print ( "PRESETS LOADED: " + m_LoadedStrings.Count + " presets for " +  m_CurrentPrefix );

		if( m_AutoStoreDefaults )
		{
			if( !m_LoadedStrings.Contains( "Default" ) )
			{
				CreateNewSave( "Default" );
			}
		}		
	}
	
	void CreateNewSave( string saveString )
	{
		if( !m_LoadedStrings.Contains( saveString ) )
		{			
			m_LoadedStrings.Add( saveString );
			m_SelectedIndex = m_LoadedStrings.Count - 1;				
			
			Save ();
		}
		else
		{
			print ( "Preset already exists");
		}			
	}
	
	void Save() 
	{
		// Save the string
		PlayerPrefs.SetString( m_CurrentPrefix + ".SaveString" + m_SelectedIndex, m_LoadedStrings[ m_SelectedIndex ] );

		// Call save on target object using prefix and index
		m_TargetLoadSaveObject.SendMessage( "Save", m_LoadedStrings[ m_SelectedIndex ] );

		print ( "SAVED: " + m_TargetLoadSaveObject.name + " Index: " + m_SelectedIndex + " String: " + m_LoadedStrings[ m_SelectedIndex ] );
	}
	
	void SaveAllStrings()
	{
		print ( "PRESETS SAVED: " + m_LoadedStrings.Count + " presets for " +  m_CurrentPrefix );

		// Saves the string counts
		PlayerPrefs.SetInt( m_CurrentPrefix + ".SaveStringsCount", m_LoadedStrings.Count );
		
		for( int i = 0; i < m_LoadedStrings.Count; i++ )
		{
			PlayerPrefs.SetString( m_CurrentPrefix + ".SaveString" + i, m_LoadedStrings[ i ] );
		}
	}
	
	void Load()
	{
		if( m_LoadedStrings.Count > m_SelectedIndex &&  m_LoadedStrings.Count != 0 )
		{
			print ( "LOADED: " + m_TargetLoadSaveObject.name + " Index: " + m_SelectedIndex + " String: " + m_LoadedStrings[ m_SelectedIndex ] );

			// Call load on target object using prefix and index
			m_TargetLoadSaveObject.SendMessage( "Load", m_LoadedStrings[ m_SelectedIndex ] );	
		}
		else
		{
			print ( "No preset found" );
		}
	}
	
	void Delete()
	{
		if( m_LoadedStrings[ m_SelectedIndex ] == "Default" )
			return;

		print ( "DELETING: " + m_TargetLoadSaveObject.name + " Index: " + m_SelectedIndex + " String: " + m_LoadedStrings[ m_SelectedIndex ] );
		m_LoadedStrings.RemoveAt( m_SelectedIndex );
		m_SelectedIndex = Mathf.Max( 0, m_LoadedStrings.Count - 1 );
	}
	
	void OnApplicationQuit()
	{
		if( m_TargetLoadSaveObject != null )
			SaveAllStrings();
	}
	
	
	
	/// <summary>
	/// ---------------------------------GUI / WINDOW
	/// </summary>

	protected virtual void DrawGUIWindow()
	{
		GUILayout.BeginVertical( "box" );
		{
			m_GUIScroll = GUILayout.BeginScrollView( m_GUIScroll, "textfield", GUILayout.Height( 160 ) );
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();
					m_SelectedIndex = GUILayout.SelectionGrid( m_SelectedIndex, m_LoadedStrings.ToArray(), 1, GUILayout.Width( 200 ), GUILayout.Height( m_LoadedStrings.Count * 30 ) );
					GUILayout.FlexibleSpace( );
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();
			GUILayout.Space( 5 );
			GUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( "Load", GUILayout.Width( 80 ) ) )
					Load();
				GUILayout.FlexibleSpace( );
				
				if( GUILayout.Button( "Save", GUILayout.Width( 80 ) ) )
					Save();
				GUILayout.FlexibleSpace( );
				
				if( GUILayout.Button( "Delete", GUILayout.Width( 80 ) ) )
					Delete();
			}
			GUILayout.EndHorizontal();
			GUILayout.Space( 5 );
		}
		GUILayout.EndVertical();

		GUILayout.FlexibleSpace( );

		
		GUILayout.BeginHorizontal( "box" );
		{
			m_NewSaveName = GUILayout.TextField( m_NewSaveName );
			if( GUILayout.Button( "Save New" ) )
				CreateNewSave( m_NewSaveName );
		}
		GUILayout.EndHorizontal();

	}	
}
