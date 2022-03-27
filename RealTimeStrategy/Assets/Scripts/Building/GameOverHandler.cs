using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action<string> ClientOnGameOver; 

    private List<UnitBase> bases = new List<UnitBase>();
    #region server
    public override void OnStartServer()
    {
        UnitBase.ServerOnBaseSpawnned += ServerHandlerBaseSpawned;
        UnitBase.ServerOnBaseDespawnned += ServerHandlerBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnBaseSpawnned -= ServerHandlerBaseSpawned;
        UnitBase.ServerOnBaseDespawnned -= ServerHandlerBaseDespawned;
    }

    [Server]
    private void ServerHandlerBaseSpawned(UnitBase unitbase)
    {
        bases.Add(unitbase);
    }

    [Server]
    private void ServerHandlerBaseDespawned(UnitBase unitbase)
    {
        bases.Remove(unitbase);

        if(bases.Count != 1) { return; }

        int playerId = bases[0].connectionToClient.connectionId;

        RpcGameOver($"Player {playerId}");
    }
    #endregion

    #region client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion
}
