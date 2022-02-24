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
    private int winnerNum;
    // private int w = 10;
    // private int h = 9;


    GameObject menu;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
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

    public void PlayerDied(int playerNumber)
    {
        deadPlayers++; // 1

        if (deadPlayers == 1) 
        { // 2
            deadPlayerNumber = playerNumber; // 3
            Invoke("CheckPlayersDeath", .3f); // ! we check for a draw here, moght reduce delay ?
        }  
    }

    void 		CheckPlayersDeath() 
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
        deadPlayers = 0;
        deadPlayerNumber = -1;  
        menu = GameObject.Find("Menu");
        menu.GetComponent<MenuManager>().DisplayWinner(winnerNum);
    }  


	/// <summary>
	/// Returns the first available player number
	/// -1 if none
	/// </summary>
	/// <returns></returns>
	public int		GetAvailablePlayerNum()
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
			return Instantiate(this.playerPrefabList[playerNum - 1]).GetComponent<Player>();
		}
	}


	private Player		FindPlayer(int playerNum)
	{
		// Debug.Log(playerNum);
		// Debug.Log(playerList);
		// Debug.Log(	playerList.Find(x => x.playerNumber == playerNum));
		return playerList.Find(x => x.playerNumber == playerNum);

	}

	public void			DoAction(ActionEnum a, int playerNum)
	{
		FindPlayer(playerNum).DoAction(a);
	}

    public void AddGameObject(GlobalStateLink obj)
    {
        // Debug.Log("GSL Added: " + obj);
        // Debug.Log(obj.JsonRep());
		if (obj.is_player)
			playerList.Add((Player)obj);
		else
        	stateList.Add(obj);
    }

    public void RemoveGameObject(GlobalStateLink obj)
    {
        // Debug.Log("GSL Removed: " + obj);
		if (obj.is_player)
			playerList.Remove((Player)obj);
		else
        	stateList.Remove(obj);
    }

    public string GetState()
    {
        string s = "[";
        for (int i = 0; i < stateList.Count; i++)
        {
            s += stateList[i].JsonRep();
            if (i != stateList.Count - 1)
				s += ",\n\n";
        }
		for (int i = 0; i < playerList.Count; i++)
        {
			if ((i == 0) && (stateList.Count != 0))
				s += ",\n\n";
            s += stateList[i].JsonRep();
            if (i != playerList.Count - 1)
                s += ",\n\n";
        }
        s += "]";
        return s;
    }

}
