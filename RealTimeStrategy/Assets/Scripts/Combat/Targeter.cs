using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    public Targetable GetTarget => target;
    #region Server
    
    [Command]
    public void CmdSetTarget(Targetable targetGameObject)
    {
        this.target = targetGameObject;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    #endregion

}
