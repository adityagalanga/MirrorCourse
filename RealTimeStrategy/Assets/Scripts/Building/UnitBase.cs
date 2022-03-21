using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitBase : NetworkBehaviour
{
    [SerializeField]private Health health;

    public static event Action<UnitBase> ServerOnBaseSpawnned;
    public static event Action<UnitBase> ServerOnBaseDespawnned;

    #region server
    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawnned?.Invoke(this);
    }
    public override void OnStopServer()
    {

        health.ServerOnDie -= ServerHandleDie;
        ServerOnBaseDespawnned?.Invoke(this);
    }


    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion


    #region client

    #endregion
}
