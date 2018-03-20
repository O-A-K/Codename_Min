using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[NetworkSettings(channel = 1, sendInterval = 0.0333f)]
public class JB_AmmoSync : NetworkBehaviour
{
    private Vector3 nextPos;
    private float netStep = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            netStep += Time.deltaTime;
            if (netStep >= GetNetworkSendInterval())
            {
                netStep = 0;
                CmdUpdateTransform(transform.position);
            }
        }
        else
        {
            LerpTransform();
        }
    }

    [Command]
    void CmdUpdateTransform(Vector3 _nextPos)
    {
        RpcUpdateClientTransform(_nextPos);
    }

    [ClientRpc]
    void RpcUpdateClientTransform(Vector3 _nextPos)
    {
        nextPos = _nextPos;
    }

    void LerpTransform()
    {
        transform.position = Vector3.Lerp(transform.position, nextPos, .5f);
    }
}
