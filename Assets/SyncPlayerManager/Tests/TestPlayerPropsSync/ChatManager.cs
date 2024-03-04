using GoldSprite.BasicUIs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public static ChatManager Instance { get; private set; }
    public SimpleToggleClick Toggle;
    public Transform ChatPanel;

    public InputField ChatInput;
    public Text ChatMessages;
    public string ChatMes="";


    private void Start()
    {
        Instance = this;
    }

    public void Network_SendChat()
    {
        ChatMes = ChatInput.text;
        Debug.Log("·¢ËÍChat-" + ChatMes);
    }

    public void AddChatMes(string playerName, string message)
    {
        DateTime currentTime = DateTime.Now;
        string formattedTime = currentTime.ToString("HH:mm:ss");
        ChatMessages.text += "\n" +
            $"[{playerName}]: {message}" +"\n"
            ;

        var scroll = ChatPanel.GetComponentInChildren<ScrollRect>();

        LayoutRebuilder.ForceRebuildLayoutImmediate(scroll.GetComponent<RectTransform>());
        scroll.normalizedPosition = Vector2.zero;
    }

    public void ClearChatMessage()
    {
        ChatMessages.text = "";
    }
}
