using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Presets
{
	/*
	string 			m_PresetCollectionName;		// I.e. Layers, FSFX, Composition
	List<string>	m_Presets = new List< string >();

	int 			m_SelectedPreset = 0; 
	
	void SaveProjectPresets( string projectName, string sceneName )
	{
		string savePath = projectName + sceneName + m_PresetCollectionName;
		PlayerPrefs.SetInt( savePath + "m_NumberOfPresets",  m_Presets.Count );	// Saves the number of presets
		
		for( int i = 0; i < m_Presets.Count; i++ )
		{
			PlayerPrefs.SetString(  savePath + i, m_Presets[i] );
		}
	}
	
	void SaveCurrentPreset()
	{
		SavePresetAtIndex( m_SelectedPreset );
	}
	
	void SavePresetAtIndex( string projectName, string sceneName, int index )
	{
		if( m_Presets.Count > index )
		{
			Debug_Manager.Instance.print( "Saving: " + m_Presets[ m_SelectedPreset ] );
			PlayerPrefs.SetString( projectName + sceneName + index, m_Presets[index] );
			if( onSavePreset != null ) onSavePreset( m_Presets[ index ] );
		}
		else
		{
			AddNewPreset( "TouchOSCPreset" + m_Presets.Count );
		}
	}
	
	void LoadPresetAtIndex( int index )
	{
		
		print( "Loading: Trying to load index " + index );
		
		if( index == m_SelectedPreset ) return;
		
		m_SelectedPreset = index;
		
		if( m_Presets.Count > index )
		{
			print( "Loading: " + m_Presets[ index] + " At index: " + index );
			if( onLoadPreset != null ) onLoadPreset( m_Presets[ index ] );
		}
		
		RefreshPresetLabels();
	}
	
	void DeleteCurrentPreset()
	{
		print( "Deleting : " + m_Presets[m_SelectedPreset] );
		
		m_Presets.Remove( m_Presets[m_SelectedPreset] );
		m_SelectedPreset = Mathf.Max( m_SelectedPreset = 1, 0 );
		
		RefreshPresetLabels();
	}
	
	void AddNewPreset( string newPresetString )
	{
		m_Presets.Add( newPresetString );
		m_SelectedPreset = m_Presets.Count - 1;
		print( "New preset added at " + m_SelectedPreset + " Preset count: " + m_Presets.Count );
		SaveCurrentPreset();
		
		RefreshPresetLabels();
	}
	*/
}

[RequireComponent (typeof ( ET_GUIWindow ))]
public class Presets_Manager : MonoBehaviour
{
	static 			Presets_Manager m_Instance { get; set; }
	public static 	Presets_Manager Instance{ get { return m_Instance; } }

	ET_GUIWindow 		m_Window;

	public delegate void 	LoadPreset( string preset );
	public static event 	LoadPreset onLoadPreset;	

	public delegate void 	SavePreset( string preset );
	public static event 	SavePreset onSavePreset;

	OSCListener m_LoadPresetOSC;
	OSCListener m_SavePresetOSC;
	OSCListener m_SaveToggleOSC;

	OSCListener m_QuickSaveOSC;
	OSCListener m_QuickLoadOSC;

	OSCListener m_RefreshOSC;
	OSCListener m_SwitchOSC;

	bool m_OSCSaveEnabled = false;


	enum State
	{
		Load,
		Save,
	}
	

	State 			m_State = State.Load;	
	public int 			m_PrevSelectedPreset;
	public int 			m_SelectedPreset = 0;
	int 			m_NumberOfPresets = 0;
	string 			m_ProjectPrefix;
	string 			m_ScenePrefix;
	List<string>	m_Presets = new List<string>();

	// /preset/switch
	// /preset/refresh
	// /preset/quickload
	// /preset/quicksave


	void Awake ()
	{
		m_Instance = this;
	}
	
	void Start ()
	{
		m_Window = gameObject.GetComponent< ET_GUIWindow >();
		m_Window.Init( "PRESETS", gameObject );

		m_LoadPresetOSC = new OSCListener( "/preset/load" );
		m_SavePresetOSC = new OSCListener( "/preset/save" );
		m_SaveToggleOSC = new OSCListener( "/preset/save/toggle" );

		m_QuickSaveOSC = new OSCListener( "/preset/quicksave" );
		m_QuickLoadOSC = new OSCListener( "/preset/quickload" );
		
		m_RefreshOSC = new OSCListener( "/preset/refresh" );
		m_SwitchOSC = new OSCListener( "/preset/switch" );
	}

	int prevSelectedIndex;
	void Update()
	{
		if( Input.GetKeyDown( KeyCode.L ) )
			RefreshPresetLabels();

		if(   Input.GetKey(KeyCode.LeftShift ) && Input.GetKeyDown( KeyCode.Alpha1 )  )
		{
			LoadPresetAtIndex( 0 );
		}
		else if(  Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown( KeyCode.Alpha2 )  )
		{
			LoadPresetAtIndex( 1 );
		}
		else if(   Input.GetKey( KeyCode.LeftShift ) && Input.GetKeyDown( KeyCode.Alpha3 )  )
		{
			LoadPresetAtIndex( 2 );
		}
		else if( Input.GetKey( KeyCode.LeftShift )  && Input.GetKeyDown( KeyCode.Alpha4 )  )
		{
			LoadPresetAtIndex( 3 );
		}
		else if( Input.GetKey( KeyCode.LeftShift )  && Input.GetKeyDown( KeyCode.Alpha5 )  )
		{
			LoadPresetAtIndex( 4 );
		}
		else if( Input.GetKey( KeyCode.LeftShift )  && Input.GetKeyDown( KeyCode.Alpha6 )  )
		{
			LoadPresetAtIndex( 5 );
		}
		
		if( Input.GetKey( KeyCode.RightShift ) &&  Input.GetKeyDown( KeyCode.Alpha1 )  )
		{
			SavePresetAtIndex( 0 );
		}
		else if( Input.GetKey( KeyCode.RightShift ) &&  Input.GetKeyDown( KeyCode.Alpha2 )  )
		{
			SavePresetAtIndex( 1 );
		}
		else if( Input.GetKey( KeyCode.RightShift ) && Input.GetKeyDown( KeyCode.Alpha3 )  )
		{
			SavePresetAtIndex( 2 );
		}
		else if( Input.GetKey( KeyCode.RightShift ) &&   Input.GetKeyDown( KeyCode.Alpha4 )  )
		{
			SavePresetAtIndex( 3 );
		}
		else if( Input.GetKey( KeyCode.RightShift ) &&  Input.GetKeyDown( KeyCode.Alpha5 )  )
		{
			SavePresetAtIndex( 4 );
		}
		else if( Input.GetKey( KeyCode.RightShift ) &&  Input.GetKeyDown( KeyCode.Alpha6 )  )
		{
			SavePresetAtIndex( 5 );
		}

		if( Input.GetKey( KeyCode.Alpha9 ) )
		{
			m_AutoSwap = true;
			StartCoroutine( "AutoSwap" );
		}

		if( Input.GetKey( KeyCode.Alpha8 ) )
		{
			m_AutoSwap = false;
			StopAllCoroutines();
		}


		if( m_QuickSaveOSC.Updated )
		{
			if( m_QuickSaveOSC.GetDataAsFloat( 0 ) > 0 )
				SavePresetByName( "QuickSave" + m_QuickSaveOSC.GetDataAsFloat( 0 ).ToString() );
		}

		if( m_QuickLoadOSC.Updated )
		{
			if( m_QuickSaveOSC.GetDataAsFloat( 0 ) > 0 )
				LoadPresetByName( "QuickSave" + m_QuickLoadOSC.GetDataAsFloat( 0 ).ToString() );
		}

		if( m_SwitchOSC.Updated )
		{
			print( "Switching presets");
			LoadPresetAtIndex( m_PrevSelectedPreset );
		}

		if( m_SaveToggleOSC.Updated )
		{
			if( m_SaveToggleOSC.GetDataAsFloat(0) > 0 )
				m_OSCSaveEnabled = true;
			else
				m_OSCSaveEnabled = false;
		}

		if( m_RefreshOSC.Updated )
		{
			RefreshPresetLabels();
		}



		if( m_State == State.Load )
		{
			if( m_SelectedPreset != prevSelectedIndex )
			{
				LoadPresetAtIndex( m_SelectedPreset );
			}

			prevSelectedIndex = m_SelectedPreset;
		}

		if( m_SavePresetOSC.Updated )
		{
			int presetToSave = (int)m_SavePresetOSC.GetDataAsFloat(0);
			if( presetToSave > 0 && m_OSCSaveEnabled )
				SavePresetAtIndex( presetToSave - 1 );
		}

		if( m_LoadPresetOSC.Updated )
		{
			int presetToLoad = (int)m_LoadPresetOSC.GetDataAsFloat(0);
			if( presetToLoad > 0 )
				LoadPresetAtIndex( presetToLoad - 1 );
		}
	}

	int m_LabelCount = 20;
	void RefreshPresetLabels()
	{
		for( int i = 0; i < m_LabelCount; i++ )
		{
			if( i < m_Presets.Count )
			{
				string address = "/preset/label/"+(i+1).ToString();

				string name;
				if( i == m_SelectedPreset )
					name =  "*" + m_Presets[ i ] + "*";
				else
					name =  m_Presets[ i ];

				OSCHandler.Instance.SendOSCMessage( address, name );
			}
			else
			{
				string address =  "/preset/label/"+(i+1).ToString();
				OSCHandler.Instance.SendOSCMessage( address, "Empty" );
			}
		}
	}

	public void LoadProjectPresets( string projectPrefix, string scenePrefix )
	{
		m_ProjectPrefix = projectPrefix;
		m_ScenePrefix = scenePrefix;

		m_NumberOfPresets = PlayerPrefs.GetInt( m_ProjectPrefix + m_ScenePrefix + "m_NumberOfPresets", 0 );

		for( int i = 0; i < m_NumberOfPresets; i++ )
		{
			string preset = PlayerPrefs.GetString( m_ProjectPrefix + m_ScenePrefix + i, "Default Name" );
			m_Presets.Add( preset );
		}

	
		RefreshPresetLabels();
	}

	void SaveProjectPresets()
	{
		PlayerPrefs.SetInt( m_ProjectPrefix + m_ScenePrefix + "m_NumberOfPresets",  m_Presets.Count );

		for( int i = 0; i < m_Presets.Count; i++ )
		{
			PlayerPrefs.SetString( m_ProjectPrefix + m_ScenePrefix + i, m_Presets[i] );
		}
	}

	void SavePresetByName( string name )
	{
		for( int i = 0; i < m_Presets.Count; i++ )
		{
			if( m_Presets[ i ] == name )
			{
				SavePresetAtIndex( i );
				return;
			}
		}

		AddNewPreset( name );
	}

	void LoadPresetByName( string name )
	{
		for( int i = 0; i < m_Presets.Count; i++ )
		{
			if( m_Presets[ i ] == name )
			{
				LoadPresetAtIndex( i );
				return;
			}
		}
	}

	void SaveCurrentPreset()
	{
		SavePresetAtIndex( m_SelectedPreset );
	}

	void SavePresetAtIndex( int index )
	{
		if( m_Presets.Count > index )
		{
			print( "Saving: " + m_Presets[ m_SelectedPreset ] );
			PlayerPrefs.SetString( m_ProjectPrefix + m_ScenePrefix + index, m_Presets[index] );
			if( onSavePreset != null ) onSavePreset( m_Presets[ index ] );
		}
		else
		{
			AddNewPreset( "TouchOSCPreset" + m_Presets.Count );
		}
	}

	public void LoadPresetAtIndex( int index )
	{
		print( "Loading: Trying to load index " + index );

		m_SelectedPreset = index;

		if( m_Presets.Count > index )
		{
			print( "Loading: " + m_Presets[ index] + " At index: " + index );
			if( onLoadPreset != null ) onLoadPreset( m_Presets[ index ] );
		}

		RefreshPresetLabels();
	}

	void DeleteCurrentPreset()
	{
		print( "Deleting : " + m_Presets[m_SelectedPreset] );

		m_Presets.Remove( m_Presets[m_SelectedPreset] );
		m_SelectedPreset = Mathf.Max( m_SelectedPreset = 1, 0 );

		RefreshPresetLabels();
	}

	void AddNewPreset( string newPresetString )
	{
		m_Presets.Add( newPresetString );
		m_SelectedPreset = m_Presets.Count - 1;
		print( "New preset added at " + m_SelectedPreset + " Preset count: " + m_Presets.Count );
		SaveCurrentPreset();

		RefreshPresetLabels();
	}

	void OnApplicationQuit()
	{
		SaveProjectPresets();
	}

	public int m_AutoSwapPresetStart = 0;
	public int m_AutoSwapPresetEnd = 3;

	bool m_AutoSwap = false;
	IEnumerator AutoSwap()
	{
		yield return new WaitForSeconds( 2 );
		m_SelectedPreset++ ;
		m_SelectedPreset = m_SelectedPreset.WrapIntToRange( m_AutoSwapPresetStart, m_AutoSwapPresetEnd);
		LoadPresetAtIndex( m_SelectedPreset++ );

		if( m_AutoSwap )
		{
			StartCoroutine( "AutoSwap" );
		}

	}
	
	void OnGUI()
	{
		m_Window.BeginWindow();
	}

	string newPresetName = "New Preset";
	string renameValue = "";
	Vector2 scroll = Vector2.zero;
	void DrawGUIWindow()
	{
		GUILayout.BeginHorizontal("box");
		{
			if( GUILayout.Button( "LOAD", GUILayout.Width( 150 ) ) )
				m_State = State.Load;
			
			if( GUILayout.Button( "SAVE", GUILayout.Width( 150 ) ) )
				m_State = State.Save;
		}
		GUILayout.EndHorizontal();

		if( m_State == State.Load )
		{
			scroll = GUILayout.BeginScrollView( scroll );
			{
				m_SelectedPreset = GUILayout.SelectionGrid( m_SelectedPreset, m_Presets.ToArray(), 6, GUILayout.Height( 300 ) );
			}
			GUILayout.EndScrollView();

			if( GUILayout.Button("Load") )
			{
				LoadPresetAtIndex( m_SelectedPreset );
			}
		}
		else
		{
			scroll = GUILayout.BeginScrollView( scroll );
			{
				m_SelectedPreset = GUILayout.SelectionGrid( m_SelectedPreset, m_Presets.ToArray(), 6, GUILayout.Height( 300 ) );
			}
			GUILayout.EndScrollView();

			GUILayout.BeginVertical("box");
			{
				if( m_Presets.Count > m_SelectedPreset  )
					GUILayout.Label( "Selected save: " + m_Presets[ m_SelectedPreset ] );

				/*
				GUILayout.BeginHorizontal();
				{
					renameValue = GUILayout.TextField( renameValue );
					if( GUILayout.Button("Rename") )
					{
						m_Presets[ m_SelectedPreset ] = renameValue;
						SaveCurrentPreset();
					}
				}
				GUILayout.EndHorizontal();
				*/

				if( GUILayout.Button("Save") )
				{
					SaveCurrentPreset();
				}

				if( GUILayout.Button("Delete") )
				{
					DeleteCurrentPreset();
				}

				if( GUILayout.Button("Move to start") )
				{
					string currentPreset = m_Presets[ m_SelectedPreset ];
					m_Presets.Remove( currentPreset );
					m_Presets.Insert( 0, currentPreset );
				}

			}
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			{
				GUILayout.Label( "Add new" );
				newPresetName = GUILayout.TextField( newPresetName );

				if( GUILayout.Button("Add new") )
				{
					AddNewPreset( newPresetName );
					newPresetName = "New preset";
				}
			}
			GUILayout.EndVertical();
		}

		// Selected Preset name
		// Save as current preset
		// Save as new preset
	}
}

