using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;

    private Camera mainCamera;

    #region Server

    [Command]
    public void CmdMove(Vector3 position)
    {
        if (!NavMesh.SamplePosition(position,out NavMeshHit hit, 1f, NavMesh.AllAreas)) { return; }
        agent.SetDestination(hit.position);
    }

    #endregion
}
