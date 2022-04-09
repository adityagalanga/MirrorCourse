using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;

    private RTSPlayer player;
    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        ClientHandlerResourceUpdate(player.GetResources());
        player.ClientOnResourcesUpdated += ClientHandlerResourceUpdate;
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandlerResourceUpdate;
    }

    private void ClientHandlerResourceUpdate(int resources)
    {
        resourcesText.text = $"Resources {resources}";
    }
}
