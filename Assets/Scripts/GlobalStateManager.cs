using UnityEngine;

using System.Collections;
using System.Collections.Generic;

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
    private static GlobalStateManager _instance = null;
    public static GlobalStateManager Instance { get { return _instance; } }

    private List<GlobalStateLink> stateList = new List<GlobalStateLink>();

    private int deadPlayers = 0;
    private int deadPlayerNumber = -1;  
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

    void CheckPlayersDeath() 
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


    public void AddGameObject(GlobalStateLink obj)
    {
        // Debug.Log("GSL Added: " + obj);
        // Debug.Log(obj.JsonRep());
        stateList.Add(obj);
    }
    public void RemoveGameObject(GlobalStateLink obj)
    {
        // Debug.Log("GSL Removed: " + obj);
        stateList.Remove(obj);
    }

    // public void LateUpdate()
    // {
    //     stateList.RemoveAt(0);
    // }

    public string GetState()
    {
        string s = "[";
        for (int i = 0; i < stateList.Count; i++)
        {
            s += stateList[i].JsonRep();
            if (i != stateList.Count - 1)
                s += ",\n\n";
        }
        s += "]";
        return s;
    }
}
