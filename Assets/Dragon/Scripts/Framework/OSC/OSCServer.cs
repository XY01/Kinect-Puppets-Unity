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

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;

using UnityEngine;

namespace UnityOSC
{
	/// <summary>
	/// Receives incoming OSC messages
	/// </summary>
	public class OSCServer
	{
		#region Constructors
		public OSCServer (int localPort)
		{
			_localPort = localPort;
			Connect();
		}
		#endregion
		
		#region Member Variables
		private UdpClient 	_udpClient;
		private int 		_localPort;
		private Thread 		_receiverThread;
		
		private OSCPacket 	_lastReceivedPacket;
		
		List< OSCPacket > _lastRecievedPackets = new List<OSCPacket>();
		List< OSCPacket > _lastRecievedPacketsSecondary = new List<OSCPacket>();
		//ConcurrentQueue< OSCPacket > _lastRecievedPacketsQueue = new ConcurrentQueue< OSCPacket >();
		
		
		#endregion
		
		#region Properties
		public UdpClient UDPClient
		{
			get
			{
				return _udpClient;
			}
			set
			{
				_udpClient = value;
			}
		}
		
		public int LocalPort
		{
			get
			{
				return _localPort;
			}
			set
			{
				_localPort = value;
			}
		}
		
		public OSCPacket LastReceivedPacket
		{
			get
			{
				return _lastReceivedPacket;
			}
		}
		
		public List< OSCPacket > LastReceivedPackets
		{
			get
			{
				return _lastRecievedPackets;
			}
			
			set
			{
				_lastRecievedPackets = value;
			}
		}
		
		public List< OSCPacket > LastRecievedPacketsSecondary
		{
			get
			{
				return _lastRecievedPacketsSecondary;
			}
			
			set
			{
				_lastRecievedPacketsSecondary = value;
			}
		}
		
		/*
		public ConcurrentQueue< OSCPacket > LastReceivedPacketsQueue
		{
			get
			{
				return _lastRecievedPacketsQueue;
			}
			
			set
			{
				_lastRecievedPacketsQueue = value;
			}
		}
		*/
		#endregion
		
		//TESTING LOCKING
		//private Object thisLock = new Object();
	
		#region Methods
		public bool PortUnavailable = false;
		
		//HAX
		OSCHandler m_OSCHandler;
		
		/// <summary>
		/// Opens the server at the given port and starts the listener thread.
		/// </summary>
		public void Connect()
		{
			if(this._udpClient != null) Close();

			Debug.Log( "Trying to open an OSC server on port: " + _localPort );
			try
			{
				_udpClient = 		new UdpClient(_localPort);
				_receiverThread = 	new Thread(new ThreadStart(this.ReceivePool));
				_receiverThread.Start();
				PortUnavailable = false;
			}
			catch
			{
				PortUnavailable = true;
				throw new Exception(String.Format("Can't create server at port {0}", _localPort));
			}
		}
		
		/// <summary>
		/// Closes the server and terminates its listener thread.
		/// </summary>
		public void Close()
		{
			if(_receiverThread !=null) _receiverThread.Abort();
			_receiverThread = null;
			_udpClient.Close();
			_udpClient = null;
		}
		
		/// <summary>
		/// Receives and unpacks an OSC packet.
		/// </summary>
		/// <returns>
		/// A <see cref="OSCPacket"/>
		/// </returns>
		private OSCPacket Receive()
		{
			IPEndPoint ip = null;
			
			try
			{
				//if( OSCHandler.m_LockNewPackets ) 
				//{
				//	Debug.Log( "Packet dropped because of lock" );
					
					byte[] bytes = _udpClient.Receive(ref ip);

					if(bytes != null && bytes.Length > 0)
					{
						return OSCPacket.Unpack(bytes);
					}
				//}
				
				
			}
			catch
			{
				throw new Exception(String.Format("Can't unpack upcoming OSC data at port {0}", _localPort));
			}
			
			return null;
		}
		                                                                                                                                                                                                           
		                     
		/// <summary>
		/// Thread pool that receives upcoming messages.
		/// </summary>
		private void ReceivePool()
		{
			while(true)
			{
				_lastReceivedPacket = Receive();
				_lastReceivedPacket.TimeStamp = long.Parse(String.Concat(DateTime.Now.Ticks));	
				
				if( !OSCHandler.m_LockNewPackets ) 
				{
					//_lastReceivedPacket = Receive();
					//_lastReceivedPacket.TimeStamp = long.Parse(String.Concat(DateTime.Now.Ticks));	
					
					_lastRecievedPackets.Add( _lastReceivedPacket );
					//_lastRecievedPacketsQueue.Enqueue( _lastReceivedPacket );
				}
				else if( !OSCHandler.m_LockNewPackets2 ) 
				{
					_lastRecievedPacketsSecondary.Add( _lastReceivedPacket );
					//Debug.Log( "locked added to scondary" );
				}
				else
					Debug.Log( "locked" );
			}
		}
		#endregion
	}
}

