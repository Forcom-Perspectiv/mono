// System.Net.Sockets.TcpListener.cs
//
// Author:
//    Phillip Pearson (pp@myelin.co.nz)
//
// Copyright (C) 2001, Phillip Pearson
//    http://www.myelin.co.nz
//

// NB: This is untested (probably buggy) code - take care using it

using System;
using System.Net;

namespace System.Net.Sockets
{
	/// <remarks>
	/// A slightly more abstracted way to listen for incoming
	/// network connections than a Socket.
	/// </remarks>
	public class TcpListener
	{
		// private data
		
		private bool active;
		private Socket server;
		
		// constructor

		/* TODO: Code common to all the constructors goes here.  I can't
		call a constructor from another constructor, for some
		reason.  Why? */
		
		/// <summary>
		/// Some code that is shared between the constructors.
		/// </summary>
		private void common_constructor ()
		{
			active = false;
			server = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);
		}
		
		/// <summary>
		/// Constructs a new TcpListener to listen on a specified port
		/// </summary>
		/// <param name="port">The port to listen on, e.g. 80 if you 
		/// are a web server</param>
		public TcpListener (int port)
		{
			common_constructor();
			server.Bind(new IPEndPoint(IPAddress.Any, port));
		}

		/// <summary>
		/// Constructs a new TcpListener with a specified local endpoint
		/// </summary>
		/// <param name="local_end_point">The endpoint</param>
		public TcpListener (IPEndPoint local_end_point)
		{
			common_constructor();
			server.Bind(local_end_point);
		}
		
		/// <summary>
		/// Constructs a new TcpListener, listening on a specified port
		/// and IP (for use on a multi-homed machine)
		/// </summary>
		/// <param name="listen_ip">The IP to listen on</param>
		/// <param name="port">The port to listen on</param>
		public TcpListener (IPAddress listen_ip, int port)
		{
			common_constructor();
			server.Bind(new IPEndPoint(listen_ip, port));
		}


		// properties

		/// <summary>
		/// A flag that is 'true' if the TcpListener is listening,
		/// or 'false' if it is not listening
		/// </summary>
		protected bool Active
		{
			get { return active; }
		}

		/// <summary>
		/// The local end point
		/// </summary>
		public EndPoint LocalEndPoint
		{
			get { return server.LocalEndPoint; }
		}
		
		/// <summary>
		/// The listening socket
		/// </summary>
		protected Socket Server
		{
			get { return server; }
		}
		
		
		// methods

		/// <summary>
		/// Accepts a pending connection
		/// <returns>A Socket object for the new connection</returns>
		public Socket AcceptSocket ()
		{
			return server.Accept();
		}
		
		/// <summary>
		/// Accepts a pending connection
		/// </summary>
		/// <returns>A TcpClient
		/// object made from the new socket.</returns>
		public TcpClient AcceptTcpClient ()
		{
			/*	TODO: How do we set the socket in the new client,
				without having tcpclient as our base class?
				Does C# have something like 'friend'?
				
				The commented code below doesn't work because
				TcpClient.Client is protected.  If we derive
				
			*/

			TcpClient client = new TcpClient();
			client.SetTcpClient(AcceptSocket());
			return client;
		}
		
		/// <summary>
		/// Destructor - stops the listener listening
		/// </summary>
		~TcpListener ()
		{
			if (active == true) {
				Stop();
			}
		}
	
		/// <returns>
		/// Returns 'true' if there is a connection waiting to be accepted
		/// with AcceptSocket() or AcceptTcpClient().
		/// </returns>
		public bool Pending ()
		{
			return server.Poll(1000, SelectMode.SelectRead);
		}
		
		/// <summary>
		/// Tells the TcpListener to start listening.
		/// </summary>
		public void Start ()
		{
			server.Listen(-1);	//TODO: How big a backlog should we specify?  -1 == MAX?
			active = true;
		}
		
		/// <summary>
		/// Tells the TcpListener to stop listening and dispose
		/// of all managed resources.
		/// </summary>
		public void Stop ()
		{
			server.Close();
		}

	}
}
