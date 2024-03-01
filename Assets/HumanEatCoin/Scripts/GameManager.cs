using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform GameElementsParent;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private GameObject coinPrefab;
    private Transform playerTrans;
    public Transform PlayerTrans => playerTrans;

    [SerializeField]
    private int coinPoint;
    public int CoinPoint { get => coinPoint; internal set => coinPoint = value; }

    [SerializeField]
    private int CoinCount = 5;
    [SerializeField]
    private int randomRange = 6;
    public List<Transform> Coins = new();

    private bool win;
    public static Action WinEvent;

    public Text FPSTxt;

    private void Start()
    {
        Application.targetFrameRate = 240;

        Instance = this;
        CreatePlayer();
        CreateCoins();
    }

    private void Update()
    {
        if (!win && Coins.Count <= 0)
        {
            win = true;
            WinEvent?.Invoke();
        }

        FPSTxt.text = "FPS: "+(int)(1 / Time.deltaTime) + "";
    }

    private void CreateCoins()
    {
        for (int i = 0; i < CoinCount; i++)
        {
            var obj = Instantiate(coinPrefab, GameElementsParent);

            var trans = obj.transform;
            trans.position = SpawnRandomPos(trans, randomRange);
            Coins.Add(trans);
        }
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
}
