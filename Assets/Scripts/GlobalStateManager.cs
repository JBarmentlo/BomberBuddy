using UnityEngine;

using System.Collections;
using System.Collections.Generic;

// int N_PLAYERS = 2;

[System.Serializable]
public class ResponseObject
{
    [SerializeField]
    public Vector3 pos;
    // float y;

    public string JsonRep;
}

// [System.Serializable]
// public struct State
// {
//     [SerializeField]
//     public	List<Player>			playerList;

//     [SerializeField]
//     private List<GlobalStateLink> 	stateList;
// }


public class GlobalStateManager : MonoBehaviour
{
    private static 	GlobalStateManager 		_instance 	= null;
    public static 	GlobalStateManager 		Instance 	{ get { return _instance; } }

    private 		List<GlobalStateLink> 	stateList;

	private			List<Player>			playerList;

	public			List<GameObject>		playerPrefabList;

    public 	int 	MaxPlayers 	{ get { return playerPrefabList.Count ; } }


    private int deadPlayers 		= 0;
    private int deadPlayerNumber 	= -1;  
    private int winnerNum			= -1;

    private bool resetting			= false;

    // private int w = 10;
    // private int h = 9;


    GameObject menu;

    private void		 Awake()
    {
        if (_instance != null && _instance != this)
        {
			Debug.LogError("destroyed GlobalStateManager");
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(gameObject);
			stateList 	= new List<GlobalStateLink>();
			playerList  = new List<Player>();
			// InstantiatePlayer(1);
			// InstantiatePlayer(2);
        }
    }

    public void 		PlayerDied(int playerNumber)
    {
        deadPlayers += 1;

        if (deadPlayers == 1) 
        {
            deadPlayerNumber = playerNumber;
			CheckPlayersDeath();
        }  
    }

    void 				CheckPlayersDeath() 
    {
        if (deadPlayers == 1) 
        { 
            if (deadPlayerNumber == 1) 
            { 
                Debug.Log("Player 2 is the winner!");
                winnerNum = 2;
            } 
            else 
            { 
                Debug.Log("Player 1 is the winner!");
                winnerNum = 1;
            }
        } 
        else 
        { 
            winnerNum = 0;
            Debug.Log("The game ended in a draw!");
        }
        menu = GameObject.Find("Menu");
        menu.GetComponent<MenuManager>().DisplayWinner(winnerNum);
    }  


	/// <summary>
	/// Returns the first available player number
	/// -1 if none
	/// </summary>
	/// <returns></returns>
	public int			GetAvailablePlayerNum()
	{
		for (int i = 1; i <= MaxPlayers; i++)
		{
			if (FindPlayer(i) == null)
			{
				return (i);
			}
		}
		return (-1);
	}


	public Player		InstantiatePlayer(int playerNum) // starts at 1
	{
		if (playerNum > this.MaxPlayers)
		{
			Debug.LogError("trying to instanciate a player with playernum > maxplayers");
			return null;
		}
		else
		{
			return Instantiate(this.playerPrefabList[playerNum - 1], MapCreatorScript.Instance.GetPlayerStartCoord(playerNum), this.playerPrefabList[playerNum - 1].transform.rotation).GetComponent<Player>();
		}
	}


	public Player		FindPlayer(int playerNum)
	{
		// Debug.Log(playerNum);
		// Debug.Log(playerList);
		// Debug.Log(	playerList.Find(x => x.playerNumber == playerNum));
		return playerList.Find(x => x.playerNumber == playerNum);

	}

	public void			DoAction(ActionEnum a, int playerNum)
	{
		if (a == ActionEnum.Reset)
			Reset();
		else
			FindPlayer(playerNum).DoAction(a);
	}

    public void 		AddGameObject(GlobalStateLink obj)
    {
        // Debug.Log("GSL Added: " + obj);
        // Debug.Log(obj.JsonRep());
		if (obj.is_player)
			playerList.Add((Player)obj);
		else
        	stateList.Add(obj);
    }

    public void 		RemoveGameObject(GlobalStateLink obj)
    {
        // Debug.Log("GSL Removed: " + obj);
		if (obj.is_player)
			playerList.Remove((Player)obj);
		else
        	stateList.Remove(obj);
    }

    public string 		GetState()
    {
        string s = "[";
		bool	comma = false;
		if (winnerNum != -1)
		{
			s += "{\"type\":-1, \"winner\":" + winnerNum + "}";
			comma = true;
		}
        for (int i = 0; i < stateList.Count; i++)
        {
            if (comma)
				s += ",\n\n";
            s += stateList[i].JsonRep();
			comma = true;
        }
		for (int i = 0; i < playerList.Count; i++)
        {
			if (!playerList[i].gameObject.activeSelf)
				continue;
			if (comma)
				s += ",\n\n";
			comma = true;
            s += playerList[i].JsonRep();
        }
        s += "]";
        return s;
    }


	public void			Reset()
	{
		resetting = true;
	}
	private void			InternalReset()
	{
		MapCreatorScript.Instance.ResetMap();
		foreach (Player p in playerList)
		{
			InstantiatePlayer(p.playerNumber);
			Destroy(p.gameObject);
		}
		foreach (GlobalStateLink p in stateList)
		{
			if (p.type == StateLinkType.ExtraBomb || p.type == StateLinkType.ExtraRange || p.type == StateLinkType.ExtraSpeed || p.type == StateLinkType.Bomb)
				Destroy(p.gameObject);
		}
		menu = GameObject.Find("Menu");
        menu.GetComponent<MenuManager>().StopDisplayWinner(winnerNum);
		deadPlayers = 0;
		winnerNum	= -1;
		deadPlayerNumber = -1;
		resetting = false; 
	}

	public bool			IsOccupiedByBomb(Vector3 pos)
	{
		foreach (GlobalStateLink obj in stateList)
		{
			if (obj.type == StateLinkType.Bomb && obj.transform.position == pos)
			{
				return (true);
			}
		}
		return (false);
	}
	public void			Update()
	{
		if (resetting)
		{
			InternalReset();
		}
	}

}

