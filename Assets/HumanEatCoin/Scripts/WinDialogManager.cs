using System;
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
        TimeSpan time = TimeSpan.FromSeconds((int)Time.realtimeSinceStartup);

        string formattedTime = $"{(time.Days > 0 ? time.Days + "d " : "")}" +
                               $"{(time.Hours > 0 ? time.Hours + "h " : "")}" +
                               $"{(time.Minutes > 0 ? time.Minutes + "m " : "")}" +
                               $"{(time.Seconds > 0 ? time.Seconds + "s" : "")}";
        SpendTimes.text = "SpendTime: "+ formattedTime;
    }
}
