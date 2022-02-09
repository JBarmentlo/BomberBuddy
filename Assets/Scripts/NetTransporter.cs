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
	
    private static NetTransporter _instance;
    public  static NetTransporter Instance { get { return _instance; } }

	private TcpListener 	server 	= null;
	private List<TcpClient>	clients = new List<TcpClient>();
	// private NetworkStream 	stream 	= null;

	public	List<Player>	players;

	private int				nPlayers = 2;

	private Byte[] 			bytes 	= new Byte[256];
	private String 			data 	= null;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }


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


	bool        ValidateConnection(TcpClient newClient)
	{
		return true;
	}


	public void AcceptConnections()
	{
		if (server.Pending() == false)
		{
			Debug.Log("There are no pending connecttions");
			return;
		}
		while (server.Pending() && (clients.Count < nPlayers))
		{
			Debug.Log("Waiting for a connection... ");
			TcpClient client = server.AcceptTcpClient();
			if (ValidateConnection(client))
			{
				clients.Add(client);
				Debug.Log("Connected!");
			}
			else
			{
				Debug.Log("Rejected!");
			}
		}
	}


	public void RecieveMessage(NetworkStream stream, Player player)
	{
		// Debug.Log("RecieveMessage");
		try
		{
			int i = 0;
			Array.Clear(bytes, 0, 256);
			if((stream.DataAvailable) && (i = stream.Read(bytes, 0, bytes.Length))!=0)
			{
				data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
				Debug.Log("Received: {0}" + data);

				ActionMessage a = JsonUtility.FromJson<ActionMessage>(data);
				Debug.Log("Parsed: " + a);
				player.DoAction(a.action);
				Debug.Log("did a");

				// byte[] msg = System.Text.Encoding.ASCII.GetBytes("testzs");
				byte[] msg = System.Text.Encoding.ASCII.GetBytes(GlobalStateManager.Instance.GetState());

				stream.Write(msg, 0, msg.Length);
				Debug.Log("Sent: {0}" + data);
			}
		}
		catch(SocketException e)
		{
			Debug.LogError("SocketException: {0}" + e);
		}
	}


	public void RecieveAllMessages()
	{
		for (int i = 0; i < clients.Count ;i++)
		{
			RecieveMessage(clients[i].GetStream(), players[i]);
		}
	}


	public void Start()
	{
		StartListenerServer();
	}


	public void Update()
	{
		RecieveAllMessages();
	}


	void 		OnDestroy()
	{
		if (server != null)
			server.Stop();
	}
}
