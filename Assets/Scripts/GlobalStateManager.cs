using UnityEngine;

using System.Collections;
using System.Collections.Generic;



public class GlobalStateManager : MonoBehaviour
{
    private static GlobalStateManager _instance;
    public  static GlobalStateManager Instance { get { return _instance; } }
    private int deadPlayers = 0;
    private int deadPlayerNumber = -1;  
    private int winnerNum;  

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

}
