using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectCoinManager : MonoBehaviour
{
    public Transform CoinBasketCenterTrans;
    private List<Transform> coins => GameManager.Instance.Coins;
    public float GetPointRange = 1;
    public int CoinPoint { get => GameManager.Instance.CoinPoint; set => GameManager.Instance.CoinPoint=value; }

    private void Update()
    {
        var coinsToRemove = coins.Where(coin => Vector3.Distance(CoinBasketCenterTrans.position, coin.position) < GetPointRange).ToList();

        foreach (var coin in coinsToRemove)
        {
            this.coins.Remove(coin);
            Destroy(coin.gameObject);
            CoinPoint++;
            Debug.Log("»ýÒ»·Ö");
        }



        //var coins = new List<Transform>(this.coins);
        //foreach (var coin in coins)
        //{
        //    var distance = Vector3.Distance(CoinBasketCenterTrans.position, coin.position);
        //    if ( distance < GetPointRange)
        //    {
        //        this.coins.Remove(coin);
        //        Destroy(coin);
        //        CoinPoint += 1;
        //    }
        //}
    }
}
