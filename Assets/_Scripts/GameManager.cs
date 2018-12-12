using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public Text resultText;

    // Use this for initialization
    void Start () {
        resultText.enabled = false;
    }


    private void OnEnable()
    {
        PlayerActions.OnPlayerDeath += OnPlayerDeathEventHandler;
    }

    private void OnDisable()
    {
        PlayerActions.OnPlayerDeath -= OnPlayerDeathEventHandler;
    }

    void OnPlayerDeathEventHandler(bool didLose) {
        string result = string.Empty;

        if (didLose)
        {
            result = "PERDEU";
        }
		else
        {
            result = "GANHOU";
        }

        resultText.text = result;
        resultText.enabled = true;
    }


}
