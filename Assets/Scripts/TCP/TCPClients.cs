using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Implements HandleMsg and returns the new client type.
/// </summary>
public abstract class Client
{
	protected	Byte[] 			bytes 		= new Byte[256];
	protected	String 			data 		= null;
	public		bool			removeMe	= false;
	public		TcpClient		tcpClient;


	public abstract Client HandleMsg();


	public bool RecievedMessage()
	{
		return (data != null);
	}


	/// <summary>
	/// Reads from TCPClient and Stores message in this.data.
	/// If error or no message data = null
	/// </summary>
	public void	RecieveMessage()
	{
		data = null;
		try
		{
			NetworkStream stream = tcpClient.GetStream();
			int i = 0;
			Array.Clear(bytes, 0, 256);
			if((stream.DataAvailable) && (i = stream.Read(bytes, 0, bytes.Length))!= 0)
			{
				data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
			}
		}
		catch(SocketException e)
		{
			Debug.LogError("SocketException: {0}" + e);
		}
	}


	public void	SendMessage(string message)
	{
		data = null;
		try
		{
			NetworkStream stream = tcpClient.GetStream();
			if (stream.CanWrite)
			{
				byte[] bytesMsg = System.Text.Encoding.ASCII.GetBytes(message);
				byte[] bytesMsgLen = System.Text.Encoding.ASCII.GetBytes(bytesMsg.Length.ToString("D8"));
				stream.Write(bytesMsgLen, 0, bytesMsgLen.Length);
				stream.Write(bytesMsg, 0, bytesMsg.Length);
			}
			else
			{
				Debug.LogError("cant Write stream");
			}
		}
		catch(SocketException e)
		{
			Debug.LogError("SocketException: {0}" + e);
		}
	}


	/// <summary>
	/// NOT IMPLEMENTED YET !    
	/// Checks if the TCP connection is still functional.
	/// </summary>
	/// <returns>true if TCP connected</returns>
	public bool IsAlive()
	{
		// NetworkStream stream = tcpClient.GetStream();
		// stream.Write(new byte[0], 0, 0);
		// tcpClient.Client.Receive(bytes, SocketFlags.Peek);
		// TClient.Client.Receive(buff, SocketFlags.Peek) == 0
		return (tcpClient.Connected);
	}
}

/// <summary>
/// A Class for a connection not yet assigned to a player or a Controller.
/// </summary>
public class UntypedClient : Client
{
	public 			UntypedClient(TcpClient tcpClient)
	{
		this.tcpClient = tcpClient;
	}

	public virtual 	AcceptRequestMessage ParseMsg(string data)
	{
		Debug.Log("Parse Basic: {0}" + data);
		return JsonUtility.FromJson<AcceptRequestMessage>(data);
	}

	/// <summary>
	/// Multiple connections for same player possible !!!!
	/// </summary>
	/// <returns></returns>
	public override	Client HandleMsg()
	{
		// if (!RecievedMessage())
		// 	return this;
		Debug.Log("Handle Basic");
		AcceptRequestMessage msg = ParseMsg(data);
		if (msg.requestedType == ClientTypeEnum.Player)
		{
			msg.playerNum = (msg.playerNum == -1) ? GlobalStateManager.Instance.GetAvailablePlayerNum() : msg.playerNum;
			if (msg.playerNum != -1 && NetTransporter.Instance.ValidatePlayerRequest(msg.playerNum, msg.pass))
			{
				if (GlobalStateManager.Instance.FindPlayer(msg.playerNum) == null)
					GlobalStateManager.Instance.InstantiatePlayer(msg.playerNum);
				NetTransporter.Instance.SetRemoveMePlayer(msg.playerNum);
				SendMessage(JsonUtility.ToJson(msg));
				return new PlayerClient(this.tcpClient, msg.playerNum);
			}
		}
		SendMessage(JsonUtility.ToJson(new AcceptRequestMessage(ClientTypeEnum.Untyped, -1)));
		return this;
	}
}


/// <summary>
/// A Class for a connection assigned to a player.
/// </summary>
public class PlayerClient : Client
{
	public int 	playerNum;

	public PlayerClient(TcpClient tcpClient, int playerNum) 
	{
		this.tcpClient = tcpClient;
		this.playerNum = playerNum;
		NetTransporter.Instance.SetReady(playerNum);
	}

	public PlayerMessage ParseMsg(string data)
	{
		// Debug.Log("Parse Player: {0}" + data);
		return JsonUtility.FromJson<PlayerMessage>(data);
	}

	public override Client HandleMsg()
	{
		try
		{
			if (!RecievedMessage())
				return this;
			// Debug.Log("Handle Player: {0}" + data);
			PlayerMessage msg = (PlayerMessage)ParseMsg(data);
			if (msg.action == ActionEnum.ReadyCheck) {
				SendMessage(JsonUtility.ToJson(new ReadyMessage(NetTransporter.Instance.IsEveryoneReady())));
			}
			else if (msg.action == ActionEnum.Nothing) {
				GlobalStateManager.Instance.DoAction(msg.action, msg.playerNum);
				SendMessage(GlobalStateManager.Instance.GetState());
			}
			else {
				if (NetTransporter.Instance.IsEveryoneReady())
				{
					GlobalStateManager.Instance.DoAction(msg.action, msg.playerNum);
					SendMessage(GlobalStateManager.Instance.GetState());
				}
				else
				{
					SendMessage(JsonUtility.ToJson(new ErrorMessage("Not everyone is ready")));
				}

			}

			return this;
		}
		catch
		{
			return this;
		}
	}
}


public class ControllerClient : Client
{
	public ControllerClient(TcpClient tcpClient)
	{
		this.tcpClient = tcpClient;
	}
	public override Client HandleMsg()
	{
		throw new NotImplementedException();
	}
}

