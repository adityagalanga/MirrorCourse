using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7;
    [SerializeField] private float unitSpawnDuration = 5f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queueUnits;
    [SyncVar]
    private float unitTimer;

    private float progressImageVelocity;

    private void Update()
    {
        if (isServer)
        {
            ProduceUnits();
        }

        if (isClient)
        {
            UpdateTimerDisplay();
        }
    }

    [Server]
    private void ProduceUnits()
    {
        if (queueUnits == 0) return;

        unitTimer += Time.deltaTime;

        if(unitTimer< unitSpawnDuration) { return; }

        GameObject unitInstance = Instantiate(unitPrefab.gameObject, unitSpawnPoint.position, unitSpawnPoint.rotation);
        NetworkServer.Spawn(unitInstance, connectionToClient);

        Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = unitSpawnPoint.position.y;

        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);

        queueUnits--;
        unitTimer = 0f;
    }

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
    }

    [Command]
    private void CmdSpawnUnit()
    {
        if(queueUnits == maxUnitQueue) { return; }
        RTSPlayer RTS = connectionToClient.identity.GetComponent<RTSPlayer>();

        if(RTS.GetResources() < unitPrefab.GetResourceCost()) { return; }

        queueUnits++;

        RTS.SetResources(RTS.GetResources() - unitPrefab.GetResourceCost());
    }

    [Server]
    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        float newProgress = unitTimer / unitSpawnDuration;

        if(newProgress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = newProgress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(unitProgressImage.fillAmount, newProgress, ref progressImageVelocity, 0.1f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Masuk");
        if(eventData.button != PointerEventData.InputButton.Left) {  return;}

        if (!hasAuthority) { return; }

        CmdSpawnUnit();
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits,int newUnits)
    {
        remainingUnitsText.text = newUnits.ToString();
    }


    #endregion
}
