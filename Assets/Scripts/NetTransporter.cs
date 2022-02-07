using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

[System.Serializable]
public enum ActionEnum
{
	Nothing,
	Up,
	Down,
	Left,
	Right,
	Bomb
};


[System.Serializable]
public class ActionMessage
{
	// String action;
	public ActionEnum	action;
}

public class NetTransporter : MonoBehaviour
{
	private TcpListener 	server 	= null;
	private TcpClient 		client 	= null;
	private NetworkStream 	stream 	= null;

	public	Player			player;

	private Byte[] 			bytes 	= new Byte[256];
	private String 			data 	= null;

	private void StartListenerServer()
	{
		if (server == null)
		{
			Int32 port = 13000;
			IPAddress localAddr = IPAddress.Parse("127.0.0.1");
			server = new TcpListener(localAddr, port);
			server.Start();
		}
		else
		{
			Debug.Log("TRYING TO START ALREADY STARTED SERVER");
		}
	}


	public void AcceptConnection()
	{
		if (server.Pending() == false)
		{
			Debug.Log("There are no pending connecttions");
			return;
		}
		if (client == null)
		{
			Debug.Log("Waiting for a connection... ");
			// JsonUtility.FromJson
			// Perform a blocking call to accept requests.
			// You could also use server.AcceptSocket() here.
			client = server.AcceptTcpClient();
			Debug.Log("Connected!");
			stream = client.GetStream();
			server.Stop();
		}
	}


	public void RecieveMessage()
	{
		Debug.Log("RecieveMessage");
		try
		{
			int i = 0;
			// Byte[] bytes 	= new Byte[256];
			Array.Clear(bytes, 0, 256);
			// Loop to receive all the data sent by the client.
			if((stream.DataAvailable) && (i = stream.Read(bytes, 0, bytes.Length))!=0)
			{
				// Translate data bytes to a ASCII string.
				data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
				Debug.Log("Received: {0}" + data);

				ActionMessage a = JsonUtility.FromJson<ActionMessage>(data);
				Debug.Log("Parsed: " + a);
				player.DoAction(a.action);
				Debug.Log("did a");
				// Process the data sent by the client.
				// data = data.ToUpper();

				// byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

				// // Send back a response.
				// stream.Write(msg, 0, msg.Length);
				// Debug.Log("Sent: {0}" + data);
			}

			// Shutdown and end connection
			// client.Close(); 
			// !
		}
		catch(SocketException e)
		{
			Debug.Log("SocketException: {0}" + e);
		}
		finally
		{
			// Stop listening for new clients.
			// server.Stop();
		}

		// Debug.Log("\nHit enter to continue...");
		// Console.Read();
	}

	public void Start()
	{
		StartListenerServer();
		// ActionMessage a;
		// a.action = ActionEnum.Up;
		// Debug.Log("JSON " + JsonUtility.ToJson(a));
	}

	public void Update()
	{
		if (client != null)
		{
			RecieveMessage();
		}
	}
}
