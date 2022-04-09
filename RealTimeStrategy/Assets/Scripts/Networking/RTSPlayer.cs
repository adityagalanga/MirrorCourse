using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private Building[] buildings = new Building[0];
    [SerializeField] private float buildingRangeLimit = 5;

    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    public event Action<int> ClientOnResourcesUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    private Color teamColor = new Color();
    private List<Unit> MyUnit = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();
    public bool GetIsPartyOwner() => isPartyOwner;
    public Transform GetCameraTransform() => cameraTransform;
    public Color GetTeamColor() => teamColor;
    public List<Unit> GetMyUnits()
    {
        return MyUnit;
    }
    public List<Building> GetMyBuildings() => myBuildings;
    public int GetResources() => resources;

    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 point)
    {
        if (Physics.CheckBox(point + buildingCollider.center, buildingCollider.size / 2, Quaternion.identity, buildingBlockLayer))
        {
            return false;
        }
        foreach (Building building in myBuildings)
        {
            if ((point - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }

        return false;

    }

    #region Server

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandlerUnitDespawned;
        Building.ServerOnBuildingSpawned += ServerHandlerBuildingSpawned;
        Building.ServerOnBuildingDespawned += ServerHandlerBuildingDespawned;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDespawned;
        Building.ServerOnBuildingSpawned -= ServerHandlerBuildingSpawned;
        Building.ServerOnBuildingDespawned -= ServerHandlerBuildingDespawned;
    }
    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }
    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) { return; }

        ((RTSNetworkManager)NetworkManager.singleton).StartGame();
    }
    [Command]
    public void CmdTryPlaceBuilding(int buildingId,Vector3 point)
    {
        Building buildingToPlace = null;
        foreach(Building building in buildings)
        {
            if(building.GetID() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }

        if(buildingToPlace == null) { return; }

        if(resources < buildingToPlace.GetPrice()) { return; }

        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();

        if (!CanPlaceBuilding(buildingCollider,point)) { return; }
       
        GameObject buildingInstance =  Instantiate(buildingToPlace.gameObject, point, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);

        SetResources(resources - buildingToPlace.GetPrice());
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

    private void ServerHandlerBuildingSpawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myBuildings.Add(building);
    }

    private void ServerHandlerBuildingDespawned(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
        myBuildings.Remove(building);
    }


    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
    }

    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    #endregion


    #region Client

    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }
        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDespawned;
        Building.AuthorityOnBuildingSpawned += AuthorityHandlerBuildingSpawned;
        Building.AuthorityOnBuildingDespawned += AuthorityHandlerBuildingDespawned;
    }

    public override void OnStartClient()
    {
        Debug.Log("masuk");
        if (NetworkServer.active) { return; }
        ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        Debug.Log("stop");
        if (!isClientOnly) { return; }
        ((RTSNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) { return; }
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDespawned;
        Building.AuthorityOnBuildingSpawned -= AuthorityHandlerBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandlerBuildingDespawned;
    }

    private void ClientHandleResourcesUpdated(int oldResources,int newResources)
    {
        ClientOnResourcesUpdated?.Invoke(newResources);
    }
    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void AuthorityHandlerUnitSpawned(Unit obj)
    {
        MyUnit.Add(obj);
    }

    private void AuthorityHandlerUnitDespawned(Unit obj)
    {
        MyUnit.Remove(obj);
    }

    private void AuthorityHandlerBuildingSpawned(Building obj)
    {
        myBuildings.Add(obj);
    }

    private void AuthorityHandlerBuildingDespawned(Building obj)
    {
        myBuildings.Remove(obj);
    }


    #endregion

}
