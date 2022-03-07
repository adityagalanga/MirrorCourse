using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> MyUnit = new List<Unit>();

    public List<Unit> GetMyUnits()
    {
        return MyUnit;
    }

    #region Server
    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandlerUnitDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDespawned;
    }

    private void ServerHandlerUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        MyUnit.Add(unit);
    }

    private void ServerHandlerUnitDespawned(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        MyUnit.Remove(unit);
    }
    #endregion


    #region Client

    public override void OnStartClient()
    {
        if(!isClientOnly) { return; }
        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDespawned;
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDespawned;
    }


    private void AuthorityHandlerUnitSpawned(Unit obj)
    {
        if (!hasAuthority) { return; }
        MyUnit.Add(obj);
    }

    private void AuthorityHandlerUnitDespawned(Unit obj)
    {
        if (!hasAuthority) { return; }
        MyUnit.Remove(obj);
    }


    #endregion

}
