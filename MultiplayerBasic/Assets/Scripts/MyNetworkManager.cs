using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        MyNetworkPlayer player = conn.identity.GetComponent<MyNetworkPlayer>();
        player.SetDisplayName($"Player {numPlayers}");

        float R = Random.Range(0, 226/255f);
        float G = Random.Range(0, 226/255f);
        float B = Random.Range(0, 226/255f);
        player.SetDisplayColor(new Color(R,G,B));
    }
}
