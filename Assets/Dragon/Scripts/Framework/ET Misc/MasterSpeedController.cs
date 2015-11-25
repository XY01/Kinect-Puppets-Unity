using UnityEngine;
using System.Collections;


public class RhythmTimer
{
	public delegate void 	RhythmTrigger( MasterSpeedController.Rhythm rhythmType, int index );
	public static event 	RhythmTrigger onRhythmTrigger;	
	
	MasterSpeedController.Rhythm m_RhythmType;
	
	float 			m_Modulo;
	public float 			Freq{ get { return m_Modulo;}}
	int 			m_Index;	
	float			m_Timer;
	public float 	m_NormalizedTimer { get{ return m_Timer/m_Modulo;  } }
	public bool 	m_TriggeredThisFrame = false;

	OSCListener 	m_TransportInOSC;
	
	public RhythmTimer( MasterSpeedController.Rhythm rhythmType )
	{
		m_RhythmType = rhythmType;
		
		if( m_RhythmType == MasterSpeedController.Rhythm.Phrase )
		{
			m_Modulo = 16;
		}
		else if( m_RhythmType == MasterSpeedController.Rhythm.Bar )
		{
			m_Modulo = 4;
		}
		else if( m_RhythmType == MasterSpeedController.Rhythm.Beat )
		{
			m_Modulo = 1;
		}
		else if( m_RhythmType == MasterSpeedController.Rhythm.Eighth )
		{
			m_Modulo = .25f;
		}
		else if( m_RhythmType == MasterSpeedController.Rhythm.Sixteenth)
		{
			m_Modulo = .0125f;
		}
		
	}

	public bool m_OutputOSC = false;
	public void Update( float masterTimer )
	{
		m_TriggeredThisFrame = false;
		
		float prevTime = 	m_Timer;
		m_Timer = 			masterTimer % m_Modulo;
		
		if( m_Timer < prevTime )
		{
			m_Index = (int) ( masterTimer / m_Modulo );
			m_Index = m_Index % 4;
			
		//	Debug.Log( m_Index );
			
			m_TriggeredThisFrame = true;	
			if( onRhythmTrigger != null )
			{ 
				if( m_OutputOSC )
				{
					if( m_RhythmType == MasterSpeedController.Rhythm.Beat )
					{
						for( int i = 0; i < 4; i++ )
						{
							if( i == m_Index )
								OSCHandler.Instance.SendOSCMessage( "/rhythm/beat/" + i, 1 );	// Update bpm label
							else 
								OSCHandler.Instance.SendOSCMessage( "/rhythm/beat/" + i, 0 );	// Update bpm label
						}
						OSCHandler.Instance.SendOSCMessage( "/rhythm/beat/" + m_Index, 1 );	// Update bpm label
						OSCHandler.Instance.SendOSCMessage( "/rhythm/beat/" + m_Index, 1 );	// Update bpm label
					}
				}

				onRhythmTrigger( m_RhythmType, m_Index );		
			}
		}	
	}
}

[RequireComponent (typeof ( ET_GUIWindow ))]
public class MasterSpeedController : MonoBehaviour
{
	static MasterSpeedController m_Instance { get; set; }
 	
	public static MasterSpeedController Instance
    {
        // Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get 
		{
			return m_Instance; 
		}
    }
	
	public enum Rhythm
	{
		Phrase,
		Bar,//1
		Beat, //4
		Eighth,//8
		Sixteenth,//16
	}

	public static string[] m_TempoStrings = new string[]
	{
		"Phrase",
		"Bar",
		"Beat",	
		"Eighth",
		"Sixteenth",
	};

		
	// Master speec values	
	public float m_MasterSpeedScaler = 1;
	public float m_MasterPositionInCycle = 0;
	public float m_MasterContinuousValue = 0;


	public float m_TimeBetweenBeats { get{ return 1f / m_Frequency; } } 
	
	// Speed readouts
	public float m_Frequency { get{ return m_BPM/60; } }	
	public float BPM { get{ return m_BPM; } }	
	float m_BPM = 60;
	
	// Rhythm Timers
	RhythmTimer m_PhraseTimer;
	public RhythmTimer PhraseTimer { get{ return m_PhraseTimer; } }
	RhythmTimer m_BarTimer;
	public RhythmTimer BarTimer { get{ return m_BarTimer; } }
	RhythmTimer m_BeatTimer;
	public RhythmTimer BeatTimer { get{ return m_BeatTimer; } }
	RhythmTimer m_SixteenthTimer;
	public RhythmTimer SixteenthTimer { get{ return m_SixteenthTimer; } }
	RhythmTimer m_ThirtySecondthTimer;
	public RhythmTimer ThirtySecondthTimer { get{ return m_ThirtySecondthTimer; } }

	public AnimationCurve m_BeatCurve;


	OSCListener m_BackSetOSC;
	OSCListener m_BackFastOSC;
	OSCListener m_BackOSC;
	OSCListener m_PauseOSC;
	OSCListener m_FwdOSC;
	OSCListener m_FwdFastOSC;
	OSCListener m_FwdSetOSC;

	ET_GUIWindow m_Window;
	
	void Awake()
	{
		m_Instance = this;
	}
	
	// Use this for initialization
	void Start () 
	{
		BPMCounter.onSetBPM 	+= OnSetBPM;
		BPMCounter.onFirstBeat 	+= OnFirstBeat;
		
		m_PhraseTimer 			= new RhythmTimer( Rhythm.Phrase );
		m_BarTimer 				= new RhythmTimer( Rhythm.Bar );
		m_BeatTimer 			= new RhythmTimer( Rhythm.Beat );
		m_SixteenthTimer 		= new RhythmTimer( Rhythm.Eighth);
		m_ThirtySecondthTimer 	= new RhythmTimer( Rhythm.Sixteenth );	

		m_BackSetOSC = 	new OSCListener( "/tempo/back/set" );
		m_BackFastOSC = new OSCListener( "/tempo/back/fast" );
		m_BackOSC = 	new OSCListener( "/tempo/back" );
		m_PauseOSC = 	new OSCListener( "/tempo/pause" );
		m_FwdOSC = 		new OSCListener( "/tempo/fwd" );
		m_FwdFastOSC = 	new OSCListener( "/tempo/fwd/fast" );
		m_FwdSetOSC = 	new OSCListener( "/tempo/fwd/set" );

		m_Window = gameObject.GetComponent< ET_GUIWindow >();
		m_Window.Init( "Speed Controller", gameObject );
	}
	
	
	public float ScaledDeltaTime()
	{
		float scaledTime = Time.deltaTime;	// Get scaledTime since last frame
		scaledTime *= m_MasterSpeedScaler;	// Adjust scaledTime by speed multiplyer. Usefull for fast forwarding and reversing
		scaledTime *= m_Frequency; 			// Adjust scaledTime by frequency
		
		return scaledTime;
	}

	public bool m_OutputOSC = false;
	float previousMasterScaler = 1;
	// Update is called once per frame
	void Update () 
	{
		if( snap )
			m_BPM = Mathf.Round( m_BPM );

		if( m_OutputOSC )
		{
			OSCHandler.Instance.SendOSCMessage( "/bpm/label", m_BPM.ToString() );	// Update bpm label
			OSCHandler.Instance.SendOSCMessage( "/rhythm/phrase", GetPositionInCycle( Rhythm.Phrase ) );	// Update bpm label
			OSCHandler.Instance.SendOSCMessage( "/rhythm/bar", GetPositionInCycle( Rhythm.Bar ) );	// Update bpm label
		}


		// Inputs to control the direction of the timer
		if( Input.GetKey( KeyCode.LeftArrow ) ) 
		{
			m_MasterSpeedScaler = -1;
		}
		else if( Input.GetKey( KeyCode.RightArrow ) ) 
		{
			m_MasterSpeedScaler = 1;
		}

		if( m_BackSetOSC.Updated )
		{

			if( m_BackSetOSC.GetDataAsFloat(0) > 0 )
			{
				m_MasterSpeedScaler = -1;
			}
		}
		else if( m_BackFastOSC.Updated )
	   	{
			if( m_BackFastOSC.GetDataAsFloat(0) > 0 )
			{
				previousMasterScaler = m_MasterSpeedScaler;
				m_MasterSpeedScaler = -4;
			}
			else
			{
				m_MasterSpeedScaler = previousMasterScaler;
			}
		}
		else if( m_BackOSC.Updated )
		{
			if( m_BackOSC.GetDataAsFloat(0) > 0 )
			{
				previousMasterScaler = m_MasterSpeedScaler;
				m_MasterSpeedScaler = -1;
			}
			else
			{
				m_MasterSpeedScaler = previousMasterScaler;
			}
		}
		else if( m_PauseOSC.Updated )
		{
			if( m_PauseOSC.GetDataAsFloat(0) > 0 )
			{
				previousMasterScaler = m_MasterSpeedScaler;
				m_MasterSpeedScaler = 0;
			}
			else
			{
				m_MasterSpeedScaler = previousMasterScaler;
			}
		}
		else if( m_FwdOSC.Updated )
		{
			print( "FWD" );
			if( m_FwdOSC.GetDataAsFloat(0) > 0 )
			{
				previousMasterScaler = m_MasterSpeedScaler;
				m_MasterSpeedScaler = 1;
			}
			else
			{
				m_MasterSpeedScaler = previousMasterScaler;
			}
		}
		else if( m_FwdFastOSC.Updated )
		{
			print( "FAST FWD" );
			if( m_FwdFastOSC.GetDataAsFloat(0) > 0 )
			{
				previousMasterScaler = m_MasterSpeedScaler;
				m_MasterSpeedScaler = 4;
			}
			else
			{
				m_MasterSpeedScaler = previousMasterScaler;
			}
		}
		else if( m_FwdSetOSC.Updated )
		{
			print( "FAST SET" );
			if( m_FwdSetOSC.GetDataAsFloat(0) > 0 )
			{
				m_MasterSpeedScaler = 1;
			}
		}
		
		
		float scaledTime = Time.deltaTime;	// Get scaledTime since last frame
		scaledTime *= m_MasterSpeedScaler;	// Adjust scaledTime by speed multiplyer. Usefull for fast forwarding and reversing
		if( m_DoubleTime)
			scaledTime *= 4;

		scaledTime *= m_Frequency; 			// Adjust scaledTime by frequency
		
		m_MasterPositionInCycle += scaledTime;												// Update position in cycle
		m_MasterPositionInCycle = m_MasterPositionInCycle.WrapFloatToRange( 0, 1 );		// Wrap it to 0-1 		
		m_MasterContinuousValue += scaledTime;												// Update continuous value
		
		m_PhraseTimer.Update( m_MasterContinuousValue );
		m_BarTimer.Update( m_MasterContinuousValue );
		m_BeatTimer.Update( m_MasterContinuousValue );
		m_SixteenthTimer.Update( m_MasterContinuousValue );
		m_ThirtySecondthTimer.Update( m_MasterContinuousValue );
	}

	bool snap = true;
	public void OnSetBPM( float bpm )
	{
		print( "Setting BPM: " +  bpm );
		if( snap )
			m_BPM = Mathf.Round( bpm );
		else
			m_BPM = bpm;

		print( "BPM: " +  m_BPM + " Freq: " + m_Frequency );
		//m_Frequency = ( bpm/60 );
	}
	
	public float GetPositionInCycle( Rhythm rhythm )
	{
		if( rhythm == Rhythm.Phrase )
		{
			return m_PhraseTimer.m_NormalizedTimer;
		}
		else if( rhythm == Rhythm.Bar )
		{
			return m_BarTimer.m_NormalizedTimer;
		}
		else if( rhythm == Rhythm.Beat )
		{
			return m_BeatTimer.m_NormalizedTimer;
		}
		else if( rhythm == Rhythm.Eighth )
		{
			return m_SixteenthTimer.m_NormalizedTimer;
		}
		else if( rhythm == Rhythm.Sixteenth )
		{
			return m_ThirtySecondthTimer.m_NormalizedTimer;
		}
		
		return 0;		
	}

	public RhythmTimer GetRhythmTimer( Rhythm rhythm )
	{
		if( rhythm == Rhythm.Phrase )
		{
			return m_PhraseTimer;
		}
		else if( rhythm == Rhythm.Bar )
		{
			return m_BarTimer;
		}
		else if( rhythm == Rhythm.Beat )
		{
			return m_BeatTimer;
		}
		else if( rhythm == Rhythm.Eighth )
		{
			return m_SixteenthTimer;
		}
		else if( rhythm == Rhythm.Sixteenth )
		{
			return m_ThirtySecondthTimer;
		}

		return null;
	}

	public float GetCurveAdjustedPositionInCycle( Rhythm rhythm )
	{
		return m_BeatCurve.Evaluate( GetPositionInCycle( rhythm) );  		
	}

	public void Reset()
	{
		m_MasterContinuousValue = 0;
	}

	void OnFirstBeat()
	{
		m_MasterContinuousValue = 0;
	}

	void OnGUI()
	{
		m_Window.BeginWindow();
	}

	bool m_DoubleTime = false;

	void DrawGUIWindow()
	{ 
		GUILayout.BeginVertical();
		{
			GUILayout.Label( "BPM: " + 		m_BPM.ToDoubleDecimalString() );
			m_BPM = GUILayout.HorizontalSlider( m_BPM, 0, 200, GUILayout.Width( 160 ) );

			GUILayout.Space(10);

			//GUILayout.Label( "Scaler: " + 	m_MasterSpeedScaler.ToDoubleDecimalString() );

			GUILayout.BeginHorizontal();
			{
				m_DoubleTime = false;

				if( GUILayout.RepeatButton( "<<", GUILayout.Width( 40 ), GUILayout.Height( 40 ) ) )
				{
					m_MasterSpeedScaler = -1;
					m_DoubleTime = true;
				}

				if( GUILayout.RepeatButton( "<", GUILayout.Width( 40 ), GUILayout.Height( 40 ) ) )
				{
					m_MasterSpeedScaler = -1;
				}

				if( GUILayout.RepeatButton( ">", GUILayout.Width( 40 ), GUILayout.Height( 40 ) ) )
				{
					m_MasterSpeedScaler = 1;
				}

				if( GUILayout.RepeatButton( ">>", GUILayout.Width( 40 ), GUILayout.Height( 40 ) ) )
				{
					m_MasterSpeedScaler = 1;
					m_DoubleTime = true;
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.Space(10);

			GUILayout.HorizontalSlider( PhraseTimer.m_NormalizedTimer, 0, 1, GUILayout.Width( 160 ) );
			GUILayout.HorizontalSlider( BarTimer.m_NormalizedTimer,  0, 1, GUILayout.Width( 160 )  );
			GUILayout.HorizontalSlider( BeatTimer.m_NormalizedTimer,  0, 1, GUILayout.Width( 160 )  );
			GUILayout.HorizontalSlider( SixteenthTimer.m_NormalizedTimer,  0, 1, GUILayout.Width( 160 )  );

		}
		GUILayout.EndVertical();
	}
	
	void OnDrawGizmos ()
	{
		if( m_BeatTimer == null ) return;
		
		//if( m_PhraseTimer.m_TriggeredThisFrame )
			Gizmos.DrawSphere( transform.position, .5f * ( 1 - m_PhraseTimer.m_NormalizedTimer ) );	
		
		//if( m_BarTimer.m_TriggeredThisFrame )
			Gizmos.DrawSphere( transform.position + Vector3.right , .5f * ( 1 - m_BarTimer.m_NormalizedTimer )  );	
		
		//if( m_BeatTimer.m_TriggeredThisFrame )
			Gizmos.DrawSphere( transform.position + ( Vector3.right * 2 ) , .5f * ( 1 - m_BeatTimer.m_NormalizedTimer )  );	
		
		//if( m_SixteenthTimer.m_TriggeredThisFrame )
			Gizmos.DrawSphere( transform.position + ( Vector3.right * 3 ) , .5f * ( 1 - m_SixteenthTimer.m_NormalizedTimer )  );
		
		//if( m_ThirtySecondthTimer.m_TriggeredThisFrame )
			Gizmos.DrawSphere( transform.position + ( Vector3.right * 4 ) , .5f * ( 1 - m_ThirtySecondthTimer.m_NormalizedTimer )  );
	}
}
