using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class MyNetworkPlayer : NetworkBehaviour
{
    [SerializeField] private TMP_Text displayNameText = null;
    [SerializeField] private Renderer displayColorRenderer = null;

    [SyncVar(hook=nameof(HandleDisplayNameUpdate))]
    [SerializeField] private string displayName = "Missing Name";

    [SyncVar(hook=nameof(HandleDisplayColourUpdate))]
    [SerializeField] private Color displayColor = Color.black;

#region SERVER
    [Server]
    public void SetDisplayName(string newDisplayName)
    {
        displayName = newDisplayName;
    }

    [Server]
    public void SetDisplayColor(Color newDisplayColor)
    {
        displayColor = newDisplayColor;
    }

    [Command]
    private void CmdSetDisplayName(string newDisplayName)
    {
        if (newDisplayName.Length < 2 || newDisplayName.Length > 20) { return; }

        RpcLogNewName(newDisplayName);
        SetDisplayName(newDisplayName);
    }
#endregion

#region CLIENT
    private void HandleDisplayNameUpdate(string oldName, string newName)
    {
        displayNameText.text = newName;
    }

    private void HandleDisplayColourUpdate(Color oldColour, Color newColour)
    {
        displayColorRenderer.material.SetColor("_BaseColor",newColour);
    }

    [ContextMenu("Set MyName")]
    public void SetMyName()
    {
        CmdSetDisplayName("My New Name");
    }

    [ClientRpc]
    private void RpcLogNewName(string newName)
    {
        Debug.Log(newName);
    }
#endregion
}
