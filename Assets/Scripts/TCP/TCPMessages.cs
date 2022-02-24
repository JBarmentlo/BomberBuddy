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
	Bomb
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
	public ClientTypeEnum 	requestedType;
	public int			 	playerNum;
	public string			pass;
}

