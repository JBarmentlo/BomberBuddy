using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ActionEnum
{
	Nothing,
	Up,
	Down,
	Left,
	Right,
	Bomb,
	Reset,
	ReadyCheck
};

[System.Serializable]
public enum ClientTypeEnum
{
	Untyped,
	Player,
	Controller
};

// public class  Message
// {
// 	// public string msg;
// }

[System.Serializable]
public class ReadyMessage
{

	public ReadyMessage(bool redi)
	{
		ready 		= redi;
	}
	// String action;
	public bool 	ready;
}

[System.Serializable]
public class ErrorMessage
{

	public ErrorMessage(string mesag, int codo = 0)
	{
		msg 		= mesag;
		code		= codo;
	}
	// String action;
	public string 	msg;
	public int		code;
}


[System.Serializable]
public class PlayerMessage
{
	// String action;
	public ActionEnum	action;
	public int			playerNum;
	public string		pass;
}

[System.Serializable]
public class AcceptRequestMessage
{
	public AcceptRequestMessage(ClientTypeEnum type, int pnum)
	{
		requestedType 	= type;
		playerNum 		= pnum;
		pass			= "";
	}

	public ClientTypeEnum 	requestedType;
	public int			 	playerNum;
	public string			pass;
}

