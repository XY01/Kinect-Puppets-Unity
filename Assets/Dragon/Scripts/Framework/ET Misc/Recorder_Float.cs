// Delegate Event Framework
// Copyright: Cratesmith (Kieran Lord)
// Created: 2010
//
// No warranty or garuntee whatsoever with this code. 
// 

using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Recorder_Float
{	
	public enum State
	{
		Recording,
		Playing,
		Paused,
	}
	
	public State 	m_State = State.Paused;
	int 			m_Framecounter;	
	List< float > 	m_RecordedFloats = new List< float >();
	List< float > 	m_RecordedFloatTimes = new List< float >();
	public MasterSpeedController.Rhythm m_Rhythm = MasterSpeedController.Rhythm.Bar;
	
	public float 	m_RecordedFloat;
	float m_StartRecordTime;
	float m_RecordingDuration;

	string name;

	float debugInitFloat;
	
	public Recorder_Float( string name )
	{
		Load();
	}
	
	public void Update()
	{
		/*
		if( Input.GetKeyDown( KeyCode.R )  )
		{
			Recording();
		}
		
		if( Input.GetKeyDown( KeyCode.Escape ) )
		{
			Stop();
		}
		
		if( Input.GetKeyDown( KeyCode.P ) )
		{
			Play();
		}
		
		if( Input.GetKeyDown( KeyCode.L ) )
		{
			Pause();
		}
		
		if( Input.GetKey( KeyCode.LeftShift) && Input.GetKeyDown( KeyCode.S ) )
		{
			Save();
		}
		*/
	}
		
	public void FixedUpdate()
	{
	    // Recording	
	    if( m_State == State.Recording )
		{			
	        if( Input.GetKeyDown( KeyCode.Escape ) )
			{ 	
	            Stop();
	        }
			 
			if( m_RecordedFloat != debugInitFloat )
				m_Recorded = true;

			m_RecordedFloats.Add( 		m_RecordedFloat );
			m_RecordedFloatTimes.Add( 	Time.time - m_StartRecordTime );
			m_RecordingDuration = Time.time - m_StartRecordTime;
	        m_Framecounter++;
		
		}				
		else if ( m_State == State.Playing )
		{
	        if( Input.GetKeyDown( KeyCode.Escape ) )
			{ 	
	            Stop();
	        }
			
	        if ( m_RecordedFloats == null )
			{
	            Debug.Log("Eventlist is null!");	
	             Stop();	
	        } 
			else if( m_RecordedFloats.Count < m_Framecounter )
			{
				Debug.Log( "Not enough recorded floats" );	
	            Stop();	
			}
			else
			{
				/*
				//m_RecordedFloat = m_RecordedFloats[ m_Framecounter ];
				// GET NORMALIZED TIME 
				//float time = m_RecordedFloatTimes[ m_Framecounter ];
	           // m_Framecounter++;
	
	            if (m_Framecounter == m_RecordedFloats.Count - 1 )
				{
					m_Framecounter = 0; // only if loop
	                //Stop();
	            }
	            */
	        }
		}
	}

	public float GetRecordedValueAtRhythm()
	{
		return GetRecordedValueAtNormalizedTime( MasterSpeedController.Instance.GetPositionInCycle( m_Rhythm ) );
	}

	public float GetRecordedValueAtNormalizedTime( float normalizedTime )
	{
		if( m_RecordedFloats.Count < 2 ) return 0;
		
		normalizedTime = normalizedTime.WrapFloatToRange( 0, 1 );
		float realTimeValue = normalizedTime * m_RecordingDuration; // Gets the real duration
		int frameIndex = (int)(realTimeValue / Time.fixedDeltaTime); 		// Get the frame index by dividing by the fixed delta time at which it was recorded

		float lerpValue = ( realTimeValue % Time.fixedDeltaTime );
		int nextFrameIndex = frameIndex + 1;
		nextFrameIndex = nextFrameIndex.WrapIntToRange( 0, m_RecordedFloats.Count );

		return Mathf.Lerp( m_RecordedFloats[ frameIndex ], m_RecordedFloats[ nextFrameIndex ], lerpValue );
	}
	
	void Stop()
	{
		Debug.Log ( name + " Stop " );
		m_Framecounter = 0;
		m_State = State.Paused;
	}
	
	void Pause()
	{
		Debug.Log ( name + " Pause " );
		m_State = State.Paused;
	}	
	
	void Play()
	{
		m_Framecounter = 0;

		if( m_Recorded )
		{
			m_State = State.Playing;
			Debug.Log ( name + " Playing" );
		}
		else
		{
			m_State = State.Paused;
			Debug.Log ( name + " Paused. Init value: " + debugInitFloat + " final val: " + m_RecordedFloat );
		}
	}
	
	void Recording()
	{
		debugInitFloat = m_RecordedFloat;
		Debug.Log ( name + " recording " );
		m_RecordedFloats.Clear();
		m_RecordedFloatTimes.Clear();
		m_Recorded = false;
		m_Framecounter = 0;
		m_StartRecordTime = Time.time;
		m_State = State.Recording;
	}
	bool m_Recorded = false;
	void Load()
	{
		m_RecordedFloats = new List<float>();
		m_Framecounter = PlayerPrefs.GetInt( name + "m_Framecounter", 0 );
		
		if( m_Framecounter == 0 ) return;
		
		for( int i = 0; i < m_Framecounter; i++ )
		{
			m_RecordedFloats.Add( PlayerPrefs.GetFloat( name + "frame" + i ) );
		}
		
		m_Framecounter = 0;
		//Debug.Log( name + " recording loaded" );	
	}
	
	void Save()
	{
		
		PlayerPrefs.SetInt( name + "m_Framecounter", m_Framecounter );
		
		for( int i = 0; i < m_RecordedFloats.Count; i++ )
		{
			PlayerPrefs.SetFloat( name + "frame" + i, m_RecordedFloats[ i ] );
		}
		Debug.Log( name + " recording saved" );	
	}

	
}
