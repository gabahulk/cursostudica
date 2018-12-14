using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

	public delegate void GameRestart();
	public static event GameRestart OnRestartGame;

	public TMP_Text resultText;
	public TMP_Text restartText;
	public TMP_Text whiteScoreText;
	public TMP_Text blackScoreText;

	public int whiteScore = 0;
	public int blackScore = 0;

    void Start () {
		resultText.gameObject.SetActive (false);
		restartText.gameObject.SetActive (false);
 		whiteScoreText.SetText("White Player Score: " + whiteScore);
		blackScoreText.SetText("Black Player Score: " + blackScore);
    }


    private void OnEnable()
    {
        PlayerActions.OnPlayerDeath += OnPlayerDeathEventHandler;
    }

    private void OnDisable()
    {
        PlayerActions.OnPlayerDeath -= OnPlayerDeathEventHandler;
    }

	void OnPlayerDeathEventHandler(bool didLose, bool isWhite) {
        string result = string.Empty;

        if (didLose)
        {
            result = "Wasted";
			resultText.color = Color.red;
        }
		else
        {
            result = "The winner is you";
			resultText.color = Color.green;
        }
		handleScore (didLose, isWhite);
		resultText.SetText (result);
		resultText.gameObject.SetActive (true);
		restartText.gameObject.SetActive (true);

		StartCoroutine (RestartGame ());
    }

	void handleScore(bool didLose, bool isWhite){
		if (didLose) {
			if (isWhite) {
				blackScore++;
			} else {
				whiteScore++;
			}
		} else {
			if (isWhite) {
				whiteScore++;
			} else {
				blackScore++;
			}
		}

		whiteScoreText.SetText("White Player Score: " + whiteScore);
		blackScoreText.SetText("Black Player Score: " + blackScore);
	}

	IEnumerator RestartGame(){
		restartText.SetText("Restarts in: " + 5);
		yield return new WaitForSeconds (1);
		restartText.SetText("Restarts in: " + 4);
		yield return new WaitForSeconds (1);
		restartText.SetText("Restarts in: " + 3);
		yield return new WaitForSeconds (1);
		restartText.SetText("Restarts in: " + 2);
		yield return new WaitForSeconds (1);
		restartText.SetText("Restarts in: " + 1);
		yield return new WaitForSeconds (1);
		resultText.gameObject.SetActive (false);
		restartText.gameObject.SetActive (false);
		var shurikens = GameObject.FindGameObjectsWithTag ("Projectile");
		foreach (var shuriken in shurikens) {
			Destroy (shuriken);
		}

		if (OnRestartGame != null) {
			OnRestartGame ();
		}
	}


}
