//
//	  UnityOSC - Open Sound Control interface for the Unity3d game engine	  
//
//	  Copyright (c) 2012 Jorge Garcia Martin
//
// 	  Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// 	  documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// 	  the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
// 	  and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// 	  The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// 	  of the Software.
//
// 	  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// 	  TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// 	  THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// 	  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// 	  IN THE SOFTWARE.
//
//	  Inspired by http://www.unifycommunity.com/wiki/index.php?title=AManagerClass

using System;
using System.Net;
using System.Collections.Generic;

using UnityEngine;
using UnityOSC;


[System.Serializable]
public class OSCListener
{
	//OSCHandler 		m_OSCHandler;
	public string 	m_Address = "/test";
	public int 		m_DefualtValueIndex = 0;
	public List<object> 	m_Data;
	public bool 	m_IsInitialized { get{ if( m_Data == null ) return false; else return true; } }
	
	long 			m_LastCheckTimestamp;
	
	public bool 	m_ScaleIncoming = false;
	public Vector2 	m_ScaleFromRange  = new Vector2( 0, 1 );
	public Vector2 	m_ScaleToRange  = new Vector2( 0, 1 );

	
	public OSCListener( string address )
	{
		m_Address = address;
		OSCHandler.Instance.AddListener( this, false );


	}
	
	public void Init()
	{
		OSCHandler.Instance.AddListener( this, false );
	}	
	
	public OSCListener( string address, bool doNotDestroyOnLoad )
	{
		m_Address = address;
		OSCHandler.Instance.AddListener( this, doNotDestroyOnLoad );

	}
		
	public bool Updated		
	{
		get
		{			
			foreach( OSCPacket oscPack in OSCHandler.Instance.m_OSCAll )
			{	
				if( oscPack == null )
				{
					Debug.Log( "Null osc pack " );
					break;
				}
				if( oscPack.Address == m_Address )
				{
					if( oscPack.TimeStamp != m_LastCheckTimestamp )
					{
						m_LastCheckTimestamp = oscPack.TimeStamp;
						m_Data = oscPack.Data;
						return true;
					}
				}
			}
			return false;
		}
	}
	
	public object GetData( int index )
	{	
		return m_Data[ index ];
	}
	
	public float GetDataAsFloat( int index )
	{
		if( m_ScaleIncoming )
		{
			float val = (float)m_Data[ index ];
			val = val.Scale( m_ScaleFromRange.x, m_ScaleFromRange.y, m_ScaleToRange.x,  m_ScaleToRange.y );
			return val;
		}
		else
		{
			return (float)m_Data[ index ];	
		}	
	}
	
	public float GetDataAsFloat( )
	{
		if( m_ScaleIncoming )
		{
			float val = (float)m_Data[ m_DefualtValueIndex ];
			val = val.Scale( m_ScaleFromRange.x, m_ScaleFromRange.y, m_ScaleToRange.x,  m_ScaleToRange.y );
			return val;
		}
		else
		{
			return (float)m_Data[ m_DefualtValueIndex ];	
		}	
	}
}

[RequireComponent(typeof(ET_GUIWindow))]
/// <summary>
/// Handles all the OSC servers and clients of the current Unity game/application.
/// Tracks incoming and outgoing messages.
/// 
/// 
/// TODO - make listener optional
/// </summary>
public class OSCHandler : MonoBehaviour
{
	 // Static singleton property
    static OSCHandler m_Instance { get; set; }
 
    // Static singleton property
    public static OSCHandler Instance
    {
        // Here we use the ?? operator, to return 'instance' if 'instance' does not equal null
        // otherwise we assign instance to a new component and return that
        get 
		{
			if( m_Instance == null )
			{
				GameObject managerParent = GameObject.Find( "*Managers" );
				if( managerParent == null )
					managerParent = new GameObject( "*Managers" );
				
				m_Instance = new GameObject("OSCHandler").AddComponent< OSCHandler >();
				m_Instance.ManualInstantiation();
					m_Instance.transform.parent = managerParent.transform;
			}
			
			return m_Instance; 
		}
    }
	
	public OSCServer 	m_Server;
	public int 			m_ServerPort = 9000;	
	List< OSCClient > 	m_Clients = new List<OSCClient>();
	
	//public Server m_Server;
	public List< OSCListener > 	m_OSCListeners = 	new List<OSCListener>();
	public List< OSCListener > 	m_DoNoDestroyOSCListeners = new List<OSCListener>();
	List< string > 				m_OSCListenerAddresses = new List<string>();
	public List< OSCPacket > 	m_OSCAll = new List<OSCPacket>();
	private Dictionary< string, OSCPacket > m_SentMessageLog = new Dictionary< string, OSCPacket >();
	
	private const int _loglength = 50;
	
	static bool m_MuteOSCOut = false;
	public bool MuteOSCOut
	{
		get{ return m_MuteOSCOut; }
		
		set
		{
			if( m_MuteOSCOut != value )
			{
				m_MuteOSCOut = value;
				if( m_MuteOSCOut )
					SendOSCMessage( "/osc/mute", 1 );
				else
					SendOSCMessage( "/osc/mute", 0 );
			}
		}
	}
	
	public bool m_Debug = false;
	

	
	public void AddListener( OSCListener listener, bool doNotDestroyOnLoad )
	{
		if( !m_OSCListeners.Contains( listener ) )
		{
			if( listener.m_Address == null )
			{
				print ( "-- OSC: address is null" );
			}
			else if( listener.m_Address == "" )
			{
				print ( "-- OSC: Address is empty. Cannot register listener" );
			}
			else if( listener.m_Address[0] != '/' )
			{
				print ( "-- OSC: address does not contain leading slash" );
			}
			else
				m_OSCListeners.Add( listener );
		}
		
		if( doNotDestroyOnLoad )
		{
			if( !m_DoNoDestroyOSCListeners.Contains( listener ) )
			{
				m_DoNoDestroyOSCListeners.Add( listener );
			}
		}
	}
	
	public void RemoveListener( OSCListener listener )
	{
		/*
		if( !m_OSCListenerAddresses.Contains( listener.m_Address ) )
		{
			m_OSCListenerAddresses.Add( listener.m_Address );
		}
		*/
	}
	
	// OSC control listeners
	OSCListener m_OSCMute;
	public ET_GUIWindow m_GUIWindow;
	
	void Awake()
	{
		if( m_Instance == null )
			m_Instance = this;
	}
	
	
	void Start()
	{		
		m_GUIWindow = gameObject.GetComponent< ET_GUIWindow >() as ET_GUIWindow;	// Add GUI window componant
		m_GUIWindow.Init( "OSC Settings", gameObject, KeyCode.O );			// Initalize GUI Window

		m_OSCMute = new OSCListener( "/osc/mute" );		// Setup the osc muste listener
		Debug.Log( "My IP: " + Network.player.ipAddress );

		if( m_AutoSaveLoad )
			Load("AutoSave");
		else
			StartServer( m_ServerPort );	// Start server

		m_ServerPort = m_Server.LocalPort;
		tempPortIn = m_Server.LocalPort.ToString();	// Set the temp port in to display in GUI

	}
	
	void ManualInstantiation()
	{
		StartServer( m_ServerPort );
		m_Clients = new List<OSCClient>();
		if( m_OSCListeners == null )
			m_OSCListeners = new List<OSCListener>();
		if( m_DoNoDestroyOSCListeners == null )
			m_DoNoDestroyOSCListeners = new List<OSCListener>();
		if( m_OSCListenerAddresses == null )
			m_OSCListenerAddresses = new List<string>();
		if( m_OSCAll == null )
			m_OSCAll = new List<OSCPacket>();
		if( m_SentMessageLog == null )
			m_SentMessageLog = new Dictionary< string, OSCPacket >();
	}
	
	
	// Clear all listeners when new level is loaded. Listeners that are in the do no destroy heirarchy will need to be added again
	void OnLevelWasLoaded(int level) 
	{
		// Clear listener lists
		m_OSCListeners.Clear();
		m_OSCListenerAddresses.Clear();
		m_OSCAll.Clear();		
		
		foreach( OSCListener oscListener in m_DoNoDestroyOSCListeners )
		{	
			AddListener( oscListener, false );
		}
	}
	
	List< OSCPacket > newPacketsCopy = new List<OSCPacket>();
	//void FixedUpdate()
	void Update()
	{
		// ----------------------------------------KEY BOARD INPUTS

		if( Input.GetKeyDown( KeyCode.T ) )
		{
			SendOSCMessage( "/test", UnityEngine.Random.Range( 0f, 1f ) );
		}	
		
		if( m_OSCMute.Updated )		// Update osc muting from external OSC msg
		{
			if( (int)m_OSCMute.GetData( 0 ) == 0 )
			{
				print("Unmuted");
				MuteOSCOut = false;
			}
			else
			{
				print("muted");
				MuteOSCOut = true;
			}
		}
		// --------------------------------------------------------------
		
		// ------------------------ UPDATE RECIEVED PACKETS FROM SERVER
		if( m_Server != null )
		{
			newPackets.Clear();
			m_LockNewPackets = true;		
			newPackets = new List<OSCPacket>( m_Server.LastReceivedPackets );
			m_Server.LastReceivedPackets.Clear();
			m_LockNewPackets = false;		
		
			m_LockNewPackets2 = true;				
			if( m_Server.LastRecievedPacketsSecondary.Count > 0 )
			{
				for( int i = 0; i < m_Server.LastRecievedPacketsSecondary.Count; i++ )
					newPackets.Add( m_Server.LastRecievedPacketsSecondary[i] );
			}
			m_Server.LastRecievedPacketsSecondary.Clear();
			m_LockNewPackets2 = false;
		}
		
		newPackets.RemoveAll( item => item == null );
		// --------------------------------------------------------------
		
		// ------------------------ UPDATE RECIEVED PACKETS FROM SERVER		
		foreach( OSCPacket serverOSCPack in newPackets )	// Get the packets from the servers
		{				
			bool foundInAllOSC = false;	// Is there a packet with the same address in the list of osc packets?
			
			foreach( OSCPacket allOSCPack in m_OSCAll )	// Consider turning the OSC_All into a dictionary so compares are quicker
			{		
				/*
				if( serverOSCPack == null )		// Not sure if these checks really need to be here
				{
					print ("serverOSCPack == null " + Time.time );
					break;
				}
				if( allOSCPack == null )
				{
					print ("allOSCPack == null " + Time.time);
					break;
				}
				*/
				
				if( serverOSCPack.Address == allOSCPack.Address )	// Check to see if the OSCPacket has the same address as one stored in the AllOSC list. Consider turning the OSC_All into a dictionary so compares are quicker
				{
					// Flag as found so that it is not added to the list
					foundInAllOSC = true;
					
					if( serverOSCPack.TimeStamp > allOSCPack.TimeStamp )	// Check the timestamps to update to the latest stamp
					{
						allOSCPack.TimeStamp = 	serverOSCPack.TimeStamp;
						allOSCPack.Data = 		serverOSCPack.Data;
					}
					break;
				}
			}
			
			// If the oscmessage that was recieved isn't found in the AllOSC list then add it
			if( !foundInAllOSC )
			{
				m_OSCAll.Add( serverOSCPack );
			}
		}
		
		
		foreach( OSCClient client in m_ClientsToRemove )	// Remove any clients that need to be removed
		{
			Debug.Log( "OSC: Client removed " + client.ClientIPAddress.ToString() );
			m_Clients.Remove( client );
		}
		m_ClientsToRemove.Clear();
		
	}
	
	public void Save( string prefix )
	{
		PlayerPrefs.SetInt( prefix + "_clients.Count", m_Clients.Count );
		//print ( "OSC - Saving " + m_Clients.Count + " clients." );
		PlayerPrefs.SetInt( prefix + "serverPort", m_ServerPort );

		int clientCount = 0;
		foreach( OSCClient client in m_Clients )
		{
			string ip = client.ClientIPAddress.ToString();
			print ( "OSC: " + ip + " saved" );
			PlayerPrefs.SetString( prefix + "clientIP" + clientCount		, ip );
			PlayerPrefs.SetInt( prefix + "clientPort" + clientCount	, client.Port );
			
			clientCount++;
		}
	}
	
	public void Load( string prefix )
	{
		m_Clients.Clear();
		StopAllServers();

		m_ServerPort = PlayerPrefs.GetInt( prefix + "serverPort" );
		
		int clientCount = PlayerPrefs.GetInt( prefix + "_clients.Count" );	
		print ( "OSC - Loading " + clientCount + " clients." );
		
		for( int i = 0; i < clientCount; i++ )
		{
			string ip = PlayerPrefs.GetString( prefix + "clientIP" + i );
			int port = PlayerPrefs.GetInt( prefix + "clientPort" + i );
			AddNewClient( ip, port.ToString() );
		}	

		StartServer( m_ServerPort );
	}
	
	/*
	#region Properties
	public Dictionary<string, ClientLog> Clients
	{
		get
		{
			return _clients;
		}
	}
	
	public Dictionary<string, ServerLog> Servers
	{
		get
		{
			return _servers;
		}
	}
	#endregion
	*/
	#region Methods
	public bool m_AutoSaveLoad = true;
	/// <summary>
	/// Ensure that the instance is destroyed when the game is stopped in the Unity editor
	/// Close all the OSC clients and servers
	/// </summary>
	public void OnApplicationQuit() 
	{		
		if( m_AutoSaveLoad )
			Save("AutoSave");
		
		StopOSCHandelr();
		
	}
	
	public void StopOSCHandelr()
	{
		StopAllClients();
		StopAllServers();
			
		m_Instance = null;
	}
	
	void StopAllClients()
	{
		foreach( OSCClient client in m_Clients )
		{
			client.Close();
		}
	}
	
	bool StopAllServers()
	{
		if( m_Server != null )
		{
			if( m_Server.UDPClient != null )
				m_Server.Close();
		}
		else
		{
			print ( "No servers found" );
			return false;	
		}
		
		print ( "OSC:  Server closed" );
		
		return true;
	}
	
	/// <summary>
	/// Creates an OSC Client (sends OSC messages) given an outgoing port and address.
	/// </summary>
	/// <param name="clientId">
	/// A <see cref="System.String"/>
	/// </param>
	/// <param name="destination">
	/// A <see cref="IPAddress"/>
	/// </param>
	/// <param name="port">
	/// A <see cref="System.Int32"/>
	/// </param>
	public void CreateClient( string clientId, IPAddress destination, int port )
	{		
		OSCClient newOSCClient = new OSCClient( destination, port );
		m_Clients.Add( newOSCClient );
		
		// Send test message
		string testaddress = "/test/alive/";
		OSCMessage message = new OSCMessage(testaddress, destination.ToString());
		message.Append(port); message.Append("OK");
		
		//_clients[clientId].log.Add(String.Concat(DateTime.UtcNow.ToString(),".",
		//                                         FormatMilliseconds(DateTime.Now.Millisecond), " : ",
		//                                         testaddress," ", DataToString(message.Data)));
		//_clients[clientId].messages.Add(message);
		newOSCClient.Send( message );
		
		Debug.Log( "Client created. " + clientId + ", " + destination.ToString() + ", " + port.ToString() );
	}

	
	/// <summary>
	/// Creates an OSC Server (listens to upcoming OSC messages) given an incoming port.
	/// </summary>	
	bool m_ServerInit = false;
	public bool StartServer( int port )
	{
		if( m_Server != null )
			StopAllServers();

		m_Server = new OSCServer( port );		
		Debug.Log( "OSC:  Server created.");
		m_ServerInit = !m_Server.PortUnavailable;

		return !m_Server.PortUnavailable;
	}

	void SendMessageToAllClients( OSCMessage oscMessage )
	{
		if( m_MuteOSCOut ) return;
		
		for( int i = 0; i < m_Clients.Count; i++ )
		{		
			m_Clients[ i ].Send( oscMessage );
		}
		
		if( m_LogOutput )
		{				
			if( !m_SentMessageLog.ContainsKey( oscMessage.Address ) )
			{			
				m_SentMessageLog.Add( oscMessage.Address, oscMessage );				
			}
			else
			{
				m_SentMessageLog[ oscMessage.Address ] = oscMessage;
			}
		}
	}
	
	public void SendOSCMessage( OSCMessage oscMessage )
	{
		SendMessageToAllClients( oscMessage );
	}
	
	public void SendOSCMessage(string address, float val )
	{
		UnityOSC.OSCMessage newMsg = new UnityOSC.OSCMessage( address, val );
		SendMessageToAllClients( newMsg );
	}

	public void SendBlankOSCMessage(string address )
	{
		UnityOSC.OSCMessage newMsg = new UnityOSC.OSCMessage( address );
		SendMessageToAllClients( newMsg );
	}

	public void SendOSCMessage(string address, string val )
	{
		UnityOSC.OSCMessage newMsg = new UnityOSC.OSCMessage( address, val );
		SendMessageToAllClients( newMsg );
	}

	public void SendOSCMessage(string address, Vector2 vec2 )
	{
		UnityOSC.OSCMessage newMsg = new UnityOSC.OSCMessage( address );
		newMsg.AppendFloat( vec2.x );
		newMsg.AppendFloat( vec2.y );
		SendMessageToAllClients( newMsg );
	}
	
	public void SendOSCMessage(string address, Vector3 vec3 )
	{
		UnityOSC.OSCMessage newMsg = new UnityOSC.OSCMessage( address );
		newMsg.AppendFloat( vec3.x );
		newMsg.AppendFloat( vec3.y );
		newMsg.AppendFloat( vec3.z );
		SendMessageToAllClients( newMsg );
	}
	
	public bool m_LogOutput = false;	

	
	public static bool m_LockNewPackets = false;
	public static bool m_LockNewPackets2 = false;
	List< OSCPacket > newPackets = new List<OSCPacket>();
	
	
	/// <summary>
	/// Converts a collection of object values to a concatenated string.
	/// </summary>
	/// <param name="data">
	/// A <see cref="List<System.Object>"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	private string DataToString(List<object> data)
	{
		string buffer = "";
		
		for(int i = 0; i < data.Count; i++)
		{
			buffer += data[i].ToString() + " ";
		}
		
		buffer += "\n";
		
		return buffer;
	}
	
	/// <summary>
	/// Formats a milliseconds number to a 000 format. E.g. given 50, it outputs 050. Given 5, it outputs 005
	/// </summary>
	/// <param name="milliseconds">
	/// A <see cref="System.Int32"/>
	/// </param>
	/// <returns>
	/// A <see cref="System.String"/>
	/// </returns>
	private string FormatMilliseconds(int milliseconds)
	{	
		if(milliseconds < 100)
		{
			if(milliseconds < 10)
				return String.Concat("00",milliseconds.ToString());
			
			return String.Concat("0",milliseconds.ToString());
		}
		
		return milliseconds.ToString();
	}
			
	#endregion
	
	string GetTimeString( long timestamp )
	{
		string time = DateTime.FromFileTimeUtc( timestamp ).Minute.ToString() + "." 
							+ DateTime.FromFileTimeUtc( timestamp ).Second.ToString() + "." + DateTime.FromFileTimeUtc( timestamp ).Millisecond.ToString();
		
		return time;
	}
	
	void RestartServerWithNewPort( string port )
	{				
		m_ServerPort = 8000;
		Int32.TryParse( port, out m_ServerPort );
		
		Debug.Log( "Resetarting server with new port: " + m_ServerPort );
		
		StopAllServers();
		
		if( !StartServer( m_ServerPort ) )
		{
			Debug.Log( "Server NOT created with new port: " + m_ServerPort );
		}
		else
		{
			Debug.Log( "Server created with new port: " + m_ServerPort );
		}
	}
	
	void AddNewClient( string ip, string port )
	{
		string name = "Client" + m_Clients.Count;	
		int portInt = Int32.Parse( port );
		CreateClient( name, IPAddress.Parse(ip),  portInt );
		tempNewClientPort = port;
	}
	
	List< OSCClient > m_ClientsToRemove = new List< OSCClient >();	
	void RemoveClient( OSCClient client )
	{
		client.Close();
		m_ClientsToRemove.Add( client );
	}
	
	public bool m_LogListeners = false;
	public bool m_LogInput = true;
	
	Vector2 scrollPosition;
	
	string tempPortIn = "8001";
	string tempNewClientIP = "127.0.0.1";
	string tempNewClientPort = "9000";
	
	public Vector2 m_GUIOffset;



   	void OnGUI()
	{
		m_GUIWindow.BeginWindow();
    }

	void DrawGUIWindow()
	{
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(400), GUILayout.Height(Screen.height/2));
				
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label( "---OSC Settings---" );
			m_MuteOSCOut = GUILayout.Toggle( m_MuteOSCOut, "Mute output" );
		}
		GUILayout.EndHorizontal();


		/////////////////////////////////////  SERVER //////////////////////////////
		GUILayout.BeginHorizontal();
		{		
			GUILayout.Label( "Server -", GUILayout.Width(50) ); 
			if( !m_ServerInit )
				GUILayout.Label( "Port unavailable, try another port." ); 
			else
				GUILayout.Label( Network.player.ipAddress + ":" + m_Server.LocalPort.ToString(), GUILayout.Width(120) ); 

			tempPortIn = GUILayout.TextField( tempPortIn, 25, GUILayout.Width(50)  );
			if( GUILayout.Button( "Set", GUILayout.Width(50) ))
			{
				RestartServerWithNewPort( tempPortIn );
			}
		}		
		GUILayout.EndHorizontal();
		
		GUILayout.Space( 10 );
		
		/////////////////////////////////////  CLIENTS //////////////////////////////	
		/*
		foreach ( KeyValuePair<string, ClientLog> pair in _clients )
		{
			GUILayout.BeginHorizontal();
			
			GUILayout.Label( "Client -", GUILayout.Width(50) ); 
			GUILayout.Label( pair.Value.client.ClientIPAddress  + ":" + pair.Value.client.Port, GUILayout.Width(120) ); 
			if( GUILayout.Button( "-", GUILayout.Width(50) ) ) RemoveClient( pair.Key );
			
			GUILayout.EndHorizontal();
		}
		*/
		foreach ( OSCClient client in m_Clients )
		{
			GUILayout.BeginHorizontal();
			
			GUILayout.Label( "Client -", GUILayout.Width(50) ); 
			GUILayout.Label( client.ClientIPAddress  + ":" + client.Port, GUILayout.Width(120) ); 
			if( GUILayout.Button( "-", GUILayout.Width(50) ) ) RemoveClient( client );
			
			GUILayout.EndHorizontal();
		}
		
		
		GUILayout.Space( 10 );		
		GUILayout.BeginHorizontal();
		{
			GUILayout.Label( "IP:", GUILayout.Width(50)  ); 
			tempNewClientIP = GUILayout.TextField( tempNewClientIP, 25, GUILayout.Width(120)   );
			GUILayout.Label( "Port:", GUILayout.Width(50)  ); 
			tempNewClientPort = GUILayout.TextField( tempNewClientPort, 25, GUILayout.Width(50)   );	
			if( GUILayout.Button( "+" ) ) AddNewClient( tempNewClientIP,tempNewClientPort );
		}
		GUILayout.EndHorizontal();
		
		
		GUILayout.Space( 10 );	
		GUILayout.BeginHorizontal();
		{		
			//GUILayout.Label( "Listiners" ); 
			m_LogListeners = 	GUILayout.Toggle( m_LogListeners, "Listeners" );
			//GUILayout.Label( "Input" ); 
			m_LogInput = 		GUILayout.Toggle( m_LogInput, "Input" );
		//	GUILayout.Label( "O" ); 
			m_LogOutput = 		GUILayout.Toggle( m_LogOutput, "Output" );
		}
		GUILayout.EndHorizontal();
		
		
		if( m_LogListeners )
		{
			GUILayout.Label( "---OSC Listener Addresses---" );	
			
			bool uniqueString;
			for( int i = 0; i < m_OSCListeners.Count; i++ )
			{
				uniqueString = true;
				for( int j = i+1; j < m_OSCListeners.Count; j++ )
				{				
					if( m_OSCListeners[i].m_Address == m_OSCListeners[j].m_Address )
					{
						uniqueString = false;
						continue;
					}
				}
				
				if( uniqueString )
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label( m_OSCListeners[i].m_Address, GUILayout.Width(150) );
					if( !m_OSCListeners[i].m_IsInitialized ) 
						if( m_OSCListeners[i].Updated )
					{
					}
					GUILayout.Label( "In: " + m_OSCListeners[i].m_IsInitialized, GUILayout.Width(70) );
					if( m_OSCListeners[i].m_IsInitialized )
						GUILayout.Label( m_OSCListeners[i].GetData(0).ToString(), GUILayout.Width(100) );
					GUILayout.EndHorizontal();
				}
			}
		}
		
		
		if( m_LogInput )
		{
			GUILayout.Label( "---All OSC Packets In---" );
			
			foreach( OSCPacket oscPack in m_OSCAll )
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label( oscPack.Address, GUILayout.Width(150) );
				GUILayout.Label( GetTimeString( oscPack.TimeStamp ), GUILayout.Width(70) );
				if( oscPack.Data.Count > 0 )
					GUILayout.Label( oscPack.Data[0].ToString(), GUILayout.Width(100) );
				GUILayout.EndHorizontal();
			}
		}
		
		
		if( m_LogOutput )
		{
			GUILayout.Label( "---All OSC Packets Out--- " );
			
			foreach( KeyValuePair< string, OSCPacket > msgOut in m_SentMessageLog )
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label( msgOut.Key, GUILayout.Width(150)   );
				GUILayout.Label( GetTimeString( msgOut.Value.TimeStamp ), GUILayout.Width(70)  );
				GUILayout.Label( msgOut.Value.Data[0].ToString(), GUILayout.Width(100) );
				GUILayout.EndHorizontal();
				
			}
		}

		GUILayout.EndScrollView();
	}
}	

