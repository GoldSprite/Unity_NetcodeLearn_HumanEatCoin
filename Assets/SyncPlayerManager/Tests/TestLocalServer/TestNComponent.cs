using GoldSprite.TestSyncTemp;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TestNComponent : NetworkBehaviour
{
    private void Start()
    {
        if (IsLocalPlayer)
        {
            Debug.Log("LocalPlayer-Start.");
            TestManager2.Instance.IsConnected = true;
        }
    }
    public override void OnNetworkSpawn()
    {
        if (IsLocalPlayer)
        {
            Debug.Log("LocalPlayer-Spawn.");
            TestManager2.Instance.IsConnected = true;
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsLocalPlayer)
        {
            Debug.Log("LocalPlayer-DeSpawn.");
            TestManager2.Instance.IsConnected = false;
        }
    }

    bool updated = false;
    private void Update()
    {
        if (!updated)
        {
            updated = true;
            Debug.Log("¿ªÊ¼UpdateÑ­»·.");
        }
    }
}
