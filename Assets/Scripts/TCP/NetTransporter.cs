using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;





public class NetTransporter : MonoBehaviour
{
	
    private static NetTransporter _instance;
    public  static NetTransporter Instance { get { return _instance; } }

    public string[] psw = {"default1", "default2"};


	private TcpListener 		server 			= null;
	private List<Client>		clients 		= new List<Client>();
	
	// private List<PlayerClient>	playerClients	= new List<PlayerClient>();

	// private NetworkStream 	stream 	= null;

	// public	List<Player>	players;

	private int				nPlayers { get { return GlobalStateManager.Instance.MaxPlayers ; } }



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
	public bool        ValidatePlayerRequest(int playerNum, string pass)
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
				clients.Add(new UntypedClient(client));
				Debug.Log("New client Connected!");
			}
			else
			{
				Debug.Log("New client Rejected!");
			}
		}
	}

	public void CleanDeadClients()
	{
		for (int i = 0; i < clients.Count ;i++)
		{
			clients[i].RecieveMessage();
			if (!clients[i].RecievedMessage())
				continue;
			clients[i] = clients[i].HandleMsg();
			clients[i].SendMessage(GlobalStateManager.Instance.GetState());
		}
	}


	public void RecieveAllMessages()
	{
		for (int i = 0; i < clients.Count ;i++)
		{
			if (!clients[i].IsAlive())
			{
				clients.RemoveAt(i);
				Debug.LogError("client connection lost");
				i--;
				continue;
			}
			clients[i].RecieveMessage();
			if (!clients[i].RecievedMessage())
				continue;
			clients[i] = clients[i].HandleMsg();
			clients[i].SendMessage(GlobalStateManager.Instance.GetState());
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
