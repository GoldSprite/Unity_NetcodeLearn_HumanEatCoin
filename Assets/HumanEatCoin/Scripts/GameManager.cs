using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Sync.Relay.Transport.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public NetworkManager networkManager;
    public RelayTransportNetcode netTrans;

    public Transform GameElementsParent;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject coinPrefab;
    private Transform playerTrans;
    public Transform PlayerTrans { get; set; }

    [SerializeField]
    private int coinPoint;
    public int CoinPoint { get => coinPoint; internal set => coinPoint = value; }

    [SerializeField]
    private int CoinCount = 5;
    [SerializeField]
    private int randomRange = 6;
    public List<Transform> Coins = new();

    public Text PointTxt;

    public bool Win { get; set; }
    public static Action WinEvent;

    public Text FPSTxt;

    public bool IsConnected { get; set; }
    public bool IsCoinSpawned { get; set; }

    public float resetHigh = -10;
    public Vector3 ResetPos = new Vector3(0, 10, 0);

    public float randomPlayerRange = 7;


    private void Start()
    {
        Application.targetFrameRate = 240;

        Instance = this;
        //CreatePlayer();
        networkManager = NetworkManager.Singleton;
        netTrans = networkManager.GetComponent<RelayTransportNetcode>();

        networkManager.OnClientConnectedCallback += (id) =>
        {
            Debug.Log("新客户端连接: " + id);
        };
        networkManager.OnClientDisconnectCallback += (id) =>
        {
            Debug.Log("客户端断开连接: " + id);
        };
        networkManager.OnServerStarted += () =>
        {
            Debug.Log("服务端已开启.");
            CreateCoins();
        };
    }

    private void Update()
    {
        if (!Win && IsCoinSpawned && Coins.Count <= 0)
        {
            Win = true;
            WinEvent?.Invoke();
        }

        //人类一败涂地
        foreach (var coin in Coins)
        {
            ResetLowHighPlayer(coin);
        }

        IsConnected = false;
        try { IsConnected = netTrans.GetCurrentPlayer() != null; } catch (Exception) { }
        Debug.Log($"IsConnected: {IsConnected}");
        FPSTxt.text = 
            "FPS: " + (int)(1 / Time.deltaTime) + "\n"+
            "CloudMs: " + (IsConnected?netTrans.GetRelayServerRtt():-1)+ "\n"+
            "HostMs: " + (IsConnected?netTrans.GetCurrentRtt(netTrans.ServerClientId):-1)
            ;
        PointTxt.text = "分数: " + coinPoint;
    }

    public void CreateCoins()
    {
        for (int i = 0; i < CoinCount; i++)
        {
            var obj = Instantiate(coinPrefab, GameElementsParent);
            var nwObj = obj.GetComponent<NetworkObject>();

            var trans = obj.transform;
            trans.position = SpawnRandomPos(trans, randomRange);
            Coins.Add(trans);

            nwObj.Spawn();
        }
        IsCoinSpawned = true;
    }

    private Vector3 SpawnRandomPos(Transform trans, float range)
    {
        var randomPos = trans.position;
        randomPos.x = Random.Range(-range, range);
        randomPos.z = Random.Range(-range, range);
        return randomPos;
    }

    private void CreatePlayer()
    {
        var obj = Instantiate(playerPrefab, GameElementsParent);
        playerTrans = obj.transform;
    }

    internal void ResetLowHighPlayer(Transform transform, bool force=false)
    {
        if (force || transform.position.y < resetHigh)
        {
            transform.position = ResetPos + PlayerRandomPos();
        }
    }


    public float PlayerRandomRange()
    {
        return Random.Range(-randomPlayerRange, randomPlayerRange);
    }

    Vector3 randomPos = Vector3.zero;
    public Vector3 PlayerRandomPos()
    {
        randomPos.x = PlayerRandomRange();
        randomPos.z = PlayerRandomRange();
        randomPos.y = 0;
        return randomPos;
    }
}
