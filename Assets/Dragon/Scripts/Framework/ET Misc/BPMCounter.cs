using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BPMCounter : MonoBehaviour
{	
	public delegate void 	FirstBeatHandler( );
	public static event 	FirstBeatHandler onFirstBeat;
	
	public delegate void 	BeatHandler( int beatNumber );
	public static event 	BeatHandler onBeat;
	
	public delegate void 	SetBPMHandler( float bpm );
	public static event 	SetBPMHandler onSetBPM;
		
	public List<float> 	m_BPMTaps;
 	public float m_BPM = 80;
	public float BPM 
	{
		get { return m_BPM; }
		set
		{
			m_BPM = value;
			/*
			if( m_BPMLabel != null )
			{
				m_BPMLabel.text = m_BPM.ToString("##");
				if(m_BPM == 0 ) m_BPMLabel.text = "0";
			*/
		}
	}

 	public bool 		m_ShouldAddBPMTap = false;
 	public float 		m_ElapsedTime;
  	public float 		m_AverageTimeBetweenBeats;
	public int 			m_MaxBPM = 200;
	
	public bool 		m_SendOnBeat = true;
	
	float 				m_NextBeat;
	public static bool 	m_BeatThisFrame = false;
	
	string[] m_BeatDisplay = new string[4] {"O", "O", "O", "O"};
	int m_CurrentBeatIndex = 0;
	
	int m_BeatOSCIndex = 34; // rewind button on korg
	int m_ClearBPMOSCIndex = 35; // loop button on korg 
	
	int m_BarCount = 0;
	
//	public UISlider m_BPMSlider;
//	public UILabel 	m_BPMLabel;

	OSCListener m_OSCBeat;
	OSCListener m_OSCSetBPM;
	OSCListener m_OSCSetFirst;

	bool m_SetFirst = false;
	
	
	// OSC Interface
	//public OSCMessageHandler m_BPMRate;
	
	//public OSCMessageHandler m_BPMTap;
	//public OSCMessageHandler m_BPMSendToggle;
	
	
	
	
	void OnEnable()
	{
	}
	
	void OnDisable()
	{
	}
	
	
 	void Start() 
	{		
		m_OSCBeat = 	new OSCListener( "/bpm/beat" );
		m_OSCSetBPM = 	new OSCListener( "/bpm/set" );
		m_OSCSetFirst = new OSCListener( "/bpm/setfirst" );
		// OSC Intercace
	//	OSC_Manager.RegisterOSCAddress( m_BPMTap, gameObject );
	//	OSC_Manager.RegisterOSCAddress( m_BPMSendToggle, gameObject );
	//	OSC_Manager.RegisterOSCAddress( m_BPMRate, gameObject );
 	}
	
  	void Update()
	{
		if( m_OSCBeat.Updated )
		{
			if( m_OSCBeat.GetDataAsFloat( 0 ) > 0 )
				AddBeat();
		}

		if( m_OSCSetBPM.Updated )
		{
			SetBPM( m_OSCSetBPM.GetDataAsFloat( 0 ) * m_MaxBPM );
			//m_BPMSlider.Set( m_BPM/m_MaxBPM, false );

		}

		if( m_OSCSetFirst.Updated )
		{
			if( m_OSCSetFirst.GetDataAsFloat( 0 ) > 0 )
			{
				m_SetFirst = true;
				AddBeat();
			}
			else
				m_SetFirst = false;
		}

		if( Input.GetKeyDown( KeyCode.B ) )
		{
			if( Input.GetKey( KeyCode.LeftShift ) )
			{
				m_SetFirst = true;
			}

			AddBeat();
		}



		//print( m_SetFirst );


		// Update OSC inputs
	//	if( m_BPMTap.Initialized && m_BPMTap.Triggered ) AddBeat();
	//	if( m_BPMSendToggle.Initialized && m_BPMSendToggle.Triggered ) ToggleSendOnBeat();
		
		//if( m_BPMRate.Initialized && m_BPMRate.UpdatedThisFrame ) SetBPM( (int)m_BPMRate.Values[0] );
		//else if( m_BPMRate.Initialized )  m_BPMRate.Values[0] = m_BPM;
		
		m_BeatThisFrame = false;		
		
		if(m_AverageTimeBetweenBeats != 0)
		{
			if(Time.time > m_NextBeat)
			{
				m_BeatThisFrame = true;
				m_NextBeat += m_AverageTimeBetweenBeats;
				m_CurrentBeatIndex++;
			
				if(m_CurrentBeatIndex == 4)
				{
					m_CurrentBeatIndex = 0;
					m_BarCount++;
					
					if(m_BarCount == 4)
					{
						// Fires first beat event which can be used to sync LFO's
						//if(onFirstBeat != null) onFirstBeat();
						m_BarCount = 0;
						//print("First Bar");
					}
				}
				
				if( onBeat != null && m_SendOnBeat )
				{
					onBeat( m_CurrentBeatIndex );
				}
			}
		}
		
		//if(Input.GetKeyDown(KeyCode.B)) onBPMButtonPress();
		
		//if( Input.GetKeyDown(KeyCode.X))		
		//	ClearBPM();		
		
    	//if(m_ShouldAddBPMTap == true) onBPMButtonPress();
  	}
	
	public void ToggleSendOnBeat()
	{
		m_SendOnBeat = !m_SendOnBeat;
		
	}
	
	void onSendBeat( bool state, string function )
	{
		if( function == "onSendBeat" )
			m_SendOnBeat = state;
	}

	void AddFirstBeat()
	{
		m_SetFirst = true;
		AddBeat();
		m_SetFirst = false;
	}

	public void AddBeat()
	{
		if( m_SetFirst )
			if( onFirstBeat != null ) onFirstBeat();

		if( m_BPMTaps.Count > 0 )
			if( Time.time - m_BPMTaps[ m_BPMTaps.Count - 1 ] > 2 ) m_BPMTaps.Clear();
		
  		m_BPMTaps.Add( Time.time );      		
  		//m_ShouldAddBPMTap = false;
		
  		if( m_BPMTaps.Count > 2 ) CalcBPM();
		
		m_BeatThisFrame = true;
		
		// Set time of the next beat based on the average between beats
		if(m_AverageTimeBetweenBeats > 0)	m_NextBeat = Time.time + m_AverageTimeBetweenBeats;
		
		if(onBeat != null) onBeat( m_CurrentBeatIndex );
	}


  	public void CalcBPM() 
	{
    	m_ElapsedTime = m_BPMTaps[m_BPMTaps.Count-1]-m_BPMTaps[0];
    	m_AverageTimeBetweenBeats = m_ElapsedTime/(m_BPMTaps.Count-1);
    	BPM = 60/m_AverageTimeBetweenBeats;
		
		m_NextBeat = Time.time + m_AverageTimeBetweenBeats;

		//if( m_BPMSlider != null )
		//	m_BPMSlider.Set( m_BPM/m_MaxBPM, false );
		
		
		if(onSetBPM != null) onSetBPM( m_BPM );
  	}
	
	public void SetBPM( float bpm )
	{
		BPM = bpm;
		OSCHandler.Instance.SendOSCMessage( "/3/label9", BPM.ToString() ); // Send bpm as string to label
		m_AverageTimeBetweenBeats = 60/m_BPM;
		if(onSetBPM != null) onSetBPM( m_BPM );		
	}
	
	public void SetBPMFromSlider( float bpm )
	{
		BPM = bpm * m_MaxBPM;
		m_AverageTimeBetweenBeats = 60/m_BPM;
		if(onSetBPM != null) onSetBPM( m_BPM );	
	}

  	public void ClearBPM() 
	{
		m_NextBeat = Time.time + 99999999999;
    	m_BPMTaps.Clear();
    	//m_BPM=0;
		m_CurrentBeatIndex = 0;
		m_BarCount = 0;
		
		if(onSetBPM != null) onSetBPM( m_BPM );
  	}

	void DrawGUIWindow()
	{
		GUILayout.BeginVertical(  );
		{
			if( GUILayout.Button( "BPM tap", GUILayout.Width( 60 ), GUILayout.Height( 60 )  ) )
				AddBeat();

			if( GUILayout.Button( "First Beat", GUILayout.Width( 60 ), GUILayout.Height( 60 ) ) )
				AddFirstBeat();
		}
		GUILayout.EndVertical();
	}
	
}
