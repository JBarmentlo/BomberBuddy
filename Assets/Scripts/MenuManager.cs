using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MenuManager : MonoBehaviour
{


    public  List<TMPro.TextMeshProUGUI> winners;
    public  Button  restartButton;

    // Start is called before the first frame update
    public void DisplayWinner(int winner)
    {
        winners[winner].gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
    }

	public void StopDisplayWinner(int winner)
    {
		foreach (TMPro.TextMeshProUGUI w in winners)
		{
			w.gameObject.SetActive(false);
		}
        restartButton.gameObject.SetActive(false);
    }

    // public void RestartGame()
    // {
    //     SceneManager.LoadScene(1);
    // }


    // public void GoMenu()
    // {
    //     SceneManager.LoadScene(0);
    // }

    public void ExitGame()
    {
        Application.Quit();
    }
}
