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
public class  Message
{
	// public string msg;
}

[System.Serializable]
public class PlayerMessage : Message
{
	// String action;
	public ActionEnum	action;
	public int			playerNum;
	public string		pass;
}


/// <summary>
/// A Base Class for a connection not yet assigned to a player or a Controller.
/// </summary>
public class Client
{
	public TcpClient	tcpClient;

	public Client(TcpClient tcpClient)
	{
		this.tcpClient = tcpClient;
	}

	public virtual Message ParseMsg(string data)
	{
		Debug.Log("Parse Basic: {0}" + data);
		return JsonUtility.FromJson<Message>(data);
	}

	public virtual Client HandleMsg(string data)
	{
		Debug.Log("Handle Basic: {0}" + ParseMsg(data));
		return new PlayerClient(this, 1);
	}
}


/// <summary>
/// A Class for a connection assigned to a player.
/// </summary>
public class PlayerClient : Client
{
	int playerNum;

	public PlayerClient(TcpClient tcpClient, int playerNum) : base(tcpClient)
	{
		// this.tcpClient = tcpClient;
		this.playerNum = playerNum;
	}
	
	public PlayerClient(Client Client, int playerNum) : base(Client.tcpClient)
	{
		this.playerNum = playerNum;
	}

	// public override Message ParseMsg(string data)
	// {
	// 	Debug.Log("Parse Player: {0}" + data);
	// 	return JsonUtility.FromJson<PlayerMessage>(data);
	// }

	public override Client HandleMsg(string data)
	{
		PlayerMessage msg = JsonUtility.FromJson<PlayerMessage>(data);
		Debug.Log("Handle Player: {0}" + msg);
		GlobalStateManager.Instance.DoAction(msg.action, msg.playerNum);
		return this;
	}
}


public class ControllerClient : Client
{
	public ControllerClient(TcpClient tcpClient) : base(tcpClient)
	{

	}
}


public class NetTransporter : MonoBehaviour
{
	
    private static NetTransporter _instance;
    public  static NetTransporter Instance { get { return _instance; } }

	private TcpListener 		server 			= null;
	private List<Client>		clients 		= new List<Client>();
	private List<PlayerClient>	playerClients	= new List<PlayerClient>();

	// private NetworkStream 	stream 	= null;

	// public	List<Player>	players;

	private int				nPlayers { get { return GlobalStateManager.Instance.MaxPlayers ; } }
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
			// Debug.Log("Waiting for a connection... ");
			TcpClient client = server.AcceptTcpClient();
			if (ValidateConnection(client))
			{
				clients.Add(new Client(client));
				Debug.Log("New client Connected!");
			}
			else
			{
				Debug.Log("New client Rejected!");
			}
		}
	}


	public void RecieveMessage(Client client)
	{
		// Debug.Log("RecieveMessage");
		try
		{
			NetworkStream stream = client.tcpClient.GetStream();
			int i = 0;
			Array.Clear(bytes, 0, 256);
			if((stream.DataAvailable) && (i = stream.Read(bytes, 0, bytes.Length))!=0)
			{
				data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
				clients.Add(client.HandleMsg(data));
				clients.Remove(client);

				// HandleMsg(client, client.ParseMsg(data));
				// Debug.Log("Received: {0}" + data);

				// PlayerMessage a = JsonUtility.FromJson<PlayerMessage>(data);
				
				// // Debug.Log("Parsed: " + a.action + " player: " + a.playerNum);
				// GlobalStateManager.Instance.DoAction(a.action, a.playerNum);
				// Debug.Log("did a");

				// byte[] msg = System.Text.Encoding.ASCII.GetBytes("testzs");
				byte[] msg = System.Text.Encoding.ASCII.GetBytes(GlobalStateManager.Instance.GetState());

				stream.Write(msg, 0, msg.Length);
				// Debug.Log("Sent: {0}" + data);
			}
		}
		catch(SocketException e)
		{
			Debug.LogError("SocketException: {0}" + e);
		}
	}


	// public void HandleMsg(Client conn, Message msg)
	// {
	// 	Debug.Log("Handle Basic: {0}" + msg);
	// 	this.clients.Remove(conn);
	// 	this.clients.Add(new PlayerClient(conn, 1));

	// 	// this.playerClients.Add(new PlayerClient(conn, 1));
	// }

	// public void HandleMsg(PlayerClient conn, PlayerMessage msg)
	// {
	// 	Debug.Log("Handle Player: {0}" + msg);
	// 	GlobalStateManager.Instance.DoAction(msg.action, msg.playerNum);
	// }

	public void RecieveAllMessages()
	{
		for (int i = 0; i < clients.Count ;i++)
		{
			// clients.ForEach()
			RecieveMessage(clients[i]);
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
