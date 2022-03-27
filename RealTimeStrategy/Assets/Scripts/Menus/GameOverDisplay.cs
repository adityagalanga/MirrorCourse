using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverDisplay : NetworkBehaviour
{
    [SerializeField] private TMP_Text winnerNameText = null;
    [SerializeField] private GameObject gameOverDisplayParent = null;

    public void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} has Won !";
        gameOverDisplayParent.SetActive(true);
    }
}
