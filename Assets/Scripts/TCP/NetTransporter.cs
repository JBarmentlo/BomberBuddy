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

    private string[] psw = {"default1", "default2"};

	// This checks if player 1 and 2 are ready, they will be considered ready until game restart
	// WILL NOT AUTO ADAPT TO NEW AMOUNT OF PLAYERS


    public bool[] readies = new bool[0];

	private TcpListener 		server 			= null;
	private List<Client>		clients 		= new List<Client>();


	private int				nPlayers { get { return GlobalStateManager.Instance.MaxPlayers ; } }



    public bool IsEveryoneReady()
	{
		if (readies.Length == 0)
		{
			Debug.Log("ready array built");
			readies = new bool[nPlayers];
			for (int i = 0; i < readies.Length;i++ ) {
				readies[i] = false;
			}
		}
		for (int i = 0; i < readies.Length;i++ ) {
			if (readies[i] == false) {
				return false;
			}
		}
		return true;
	}

	public void SetReady(int PlayerNum)
	{
		Debug.Log("Set Readi");
		if (readies.Length == 0)
		{
			Debug.Log("ready array built");
			readies = new bool[nPlayers];
			for (int i = 0; i < readies.Length;i++ ) {
				readies[i] = false;
			}
		}
		if (PlayerNum - 1 < nPlayers) {
			Debug.Log("Ready player: " + PlayerNum);
			readies[PlayerNum - 1] = true;
		}
	}

	public bool GetReady(int PlayerNum)
	{
		if (PlayerNum - 1 < nPlayers) {
			return readies[PlayerNum - 1];
		}
		return false;
	}


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

	/// <summary>
	/// Skelly returns TRUE ALWAYS
	/// </summary>
	/// <param name="playerNum"></param>
	/// <param name="pass"> Password </param>
	/// <returns></returns>
	public bool	ValidatePlayerRequest(int playerNum, string pass)
	{
		return true;
	}


	/// <summary>
	/// Always returns true for now
	/// </summary>
	/// <param name="pass"></param>
	public bool	ValidateControllerRequest(string pass)
	{
		return true;
	}

	public void AcceptConnections()
	{
		if (server.Pending() == false)
		{
			// Debug.Log("There are no pending connecttions");
			return;
		}
		while (server.Pending())
		{
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


	public void RemoveDeadClients()
	{
		clients.RemoveAll(x => x.removeMe);
	}

	/// <summary>
	/// If any playerClients for playerNum are present they are removed (removeMe is set to true and they are removed later)
	/// </summary>
	/// <param name="playerNum"> Number of the controlled player</param>
	public void SetRemoveMePlayer(int playerNum)
	{
		clients.FindAll(x => (x.GetType() == typeof(PlayerClient)) && (((PlayerClient)x).playerNum == playerNum)).ForEach(x => x.removeMe = true);
	}


	public void RecieveAllMessages()
	{
		// clients.ForEach(DoClientCycle);
		for (int i = 0; i < clients.Count ;i++)
		{
			clients[i].RecieveMessage();
			if (!clients[i].RecievedMessage())
				continue;
			clients[i] = clients[i].HandleMsg();
			// clients[i].SendMessage(GlobalStateManager.Instance.GetState());
		}
		RemoveDeadClients();
	}


	public void Start()
	{
		StartListenerServer();
	}


	public void Update()
	{
		AcceptConnections();
		RecieveAllMessages();
	}


	void 		OnDestroy()
	{
		if (server != null)
			server.Stop();
	}
}
