using GoldSprite.BasicUIs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmojiManager : MonoBehaviour
{
    public static EmojiManager Instance { get; private set; }
    public SimpleToggleClick Toggle;
    public Transform EmojiPanel;
    public GameObject EmojiItemPrefab;
    public Sprite[] emojis;

    public int emojiStatu = -1;


    private void Start()
    {
        Instance = this;
        var i = 0;
        foreach (var item in emojis)
        {
            var obj = Instantiate(EmojiItemPrefab, EmojiPanel);
            obj.SetActive(true);
            var trans = obj.transform;
            var btn = trans.GetComponent<Button>();
            var image = trans.GetComponent<Image>();

            //这里顺序很重要, 不然id对不上
            var index = i;
            btn.onClick.AddListener(() => { SendEmoji(index); });
            obj.name = "emoji-" + i++;
            image.sprite = item;
        }
    }

    public void SendEmoji(int id)
    {
        Toggle.CGDroping?.Invoke();

        emojiStatu = id;
        Debug.Log("发送Emoji-" + id);
    }

    internal Sprite GetEmojiSprite(int emojiId)
    {
        return emojiId < 0 || emojiId >= emojis.Length ? null : emojis[emojiId];
    }
}
