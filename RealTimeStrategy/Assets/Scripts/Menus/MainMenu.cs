using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject LandingPagePanel = null;
    
    public void HostLobby()
    {
        LandingPagePanel.SetActive(false);
        NetworkManager.singleton.StartHost();
    }
}
