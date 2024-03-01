using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinDialogManager : MonoBehaviour
{
    [SerializeField]
    private GameObject winDialog;
    public Text ScoreTxt;
    public Text SpendTimes;

    private void Start()
    {
        GameManager.WinEvent += Win;
    }

    private void Win()
    {
        winDialog.SetActive(true);
        ScoreTxt.text = "Score: " + GameManager.Instance.CoinPoint;
        SpendTimes.text = "SpendTime: " + (int)Time.realtimeSinceStartup+"s";
    }
}
