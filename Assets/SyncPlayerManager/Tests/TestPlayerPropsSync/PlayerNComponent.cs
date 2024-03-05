using GoldSprite.MySyncPlayerManager;
using GoldSprite.TestSyncTemp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNComponent : NetworkBehaviour
{
    public static uint LocalPlayerTransportId = 0;

    public NetworkVariable<FixedString512Bytes> syncName = new();
    public Text Name_Tag;
    public Rigidbody rb;
    public NetworkVariable<Vector3> syncPos = new();
    public NetworkVariable<Vector3> syncVelo = new();
    public NetworkVariable<Quaternion> syncRot = new();
    public NetworkVariable<bool> syncReady = new();
    public Dictionary<string, NetworkVariable<Vector3>> syncCoinsPos = new();
    public static PlayerNComponent LocalPlayer;

    public Image emojiPanel;

    public float MoveVel = 6f;
    public string PlayerName => NetworkGameManager.Instance.PlayerInitName;

    public float rotForce = 1.8f;

    public bool GravityLock = true;
    public bool VelLock = true;
    public bool AVelLock = true;
    public bool RotLock = true;
    public bool RotHalfLock = true;
    public float HalfLockAngle = 45;
    public float AngleBackVelRate = 20;

    //public RelayPlayer player;
    //public RelayPlayer Player
    //{
    //    get
    //    {
    //        if (player == null || player.Name == "")
    //        {
    //            if (GameManager.Instance.RoomInfo.Players != null)
    //                player = GameManager.Instance.RoomInfo.Players.Values.First((p) => syncName.Value.ToString() == p.Name);
    //            return null;
    //        }
    //        return player;
    //    }
    //}

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsLocalPlayer)
        {
            GameManager.Instance.PlayerTrans = transform;
            //LocalPlayerTransportId = 
            Debug.Log("本机玩家生成, 已连接连接远程房间.");
            ChatManager.Instance.ClearChatMessage();
            if (IsServer)
            {
                //GameManager.Instance.CreateCoins(syncCoinsPos);
                //foreach (var (k, v) in syncCoinsPos)
                //{
                //    sendCoinsPos_ClientRpc(k, v.Value);
                //}
            }
            else
            {
                //GameManager.Instance.CreateCoinsClient(syncCoinsPos.Values.ToList());
            }
        }
    }

    [ClientRpc]
    private void sendCoinsPos_ClientRpc(string guid, Vector3 v)
    {
        if (IsLocalPlayer)
        {
            syncCoinsPos[guid].Value = v;
        }
    }


    void Start()
    {
        if (IsLocalPlayer)
            LocalPlayer = this;

        rb = GetComponent<Rigidbody>();
        Joy = FindObjectOfType<Joystick>();
    }


    Joystick Joy;
    float v, h;
    bool isInit;
    void FixedUpdate()
    {
        v = Input.GetAxis("Vertical");
        h = Input.GetAxis("Horizontal");
        if (Joy != null)
        {
            if (v == 0) v = Joy.Vertical;
            if (h == 0) h = Joy.Horizontal;
        }


        try
        {
            var localPlayer = TestManager.Instance.netTrans.GetCurrentPlayer();
            if (!isInit)
            {
                isInit = true;
            }
        }
        catch (Exception) { }


        //操作与上载
        if (IsServer && IsLocalPlayer)
        {
            HandleData();  //服务端本地玩家直接操作
            UpdateData();  //服务端本地玩家上载数据
            UpdateHandleData();
        }
        if (!IsServer && IsLocalPlayer)
        {
            HandleData();  //客户端本地玩家直接操作
            UpdateData_ServerRPC(transform.position, transform.rotation);  //客户端本地玩家上载数据
            UpdateHandleData_ServerRPC(rb.velocity, PlayerName);
        }

        //收发RPC事件
        if (IsLocalPlayer)
        {
            var emojiId = EmojiManager.Instance.emojiStatu;
            if (emojiId != -1)
            {
                if (IsHost) SendEmojiEvent_ClientRpc(emojiId);
                else
                {
                    ExecuteShowEmojiEvent(emojiId);
                    SendEmojiEvent_ServerRpc(emojiId);
                }
            }

            var chatMes = ChatManager.Instance.ChatMes;
            string sendPlayerName = syncName.Value.ToString();
            if (chatMes != "")
            {
                if (IsHost) SendChatEvent_ClientRpc(sendPlayerName, chatMes);
                else
                {
                    ExecuteShowChatEvent(sendPlayerName, chatMes);
                    SendChatEvent_ServerRpc(sendPlayerName, chatMes);
                }
            }

        }

        //下载
        if (!IsLocalPlayer)
        {
            DownloadData();  //非本地玩家下载数据
            DownloadHandleData();
        }


        //Debug.Log($"来自{(IsServer ? "服务端" : "客户端")}-{(IsLocalPlayer ? "本地" : "非本地")}数据: 位置:{syncPos.Value}, 移动:, {syncName.Value}, Properties-Ready:{syncReady.Value}");

    }

    [ClientRpc]
    private void SendEmojiEvent_ClientRpc(int emojiId)
    {
        ExecuteShowEmojiEvent(emojiId);
    }
    [ServerRpc]
    private void SendEmojiEvent_ServerRpc(int emojiId)
    {
        ExecuteShowEmojiEvent(emojiId);
    }
    private void ExecuteShowEmojiEvent(int emojiId)
    {
        if (ShowEmojiTaskInstance != null) StopCoroutine(ShowEmojiTaskInstance);
        ShowEmojiTaskInstance = StartCoroutine(ShowEmojiTask(emojiId));
        EmojiManager.Instance.emojiStatu = -1;
    }
    Coroutine ShowEmojiTaskInstance;
    private IEnumerator ShowEmojiTask(int emojiId)
    {
        ShowEmoji(emojiId);
        yield return new WaitForSeconds(2);
        ShowEmoji(-1);
    }
    internal void ShowEmoji(int emojiId = -1)
    {
        emojiPanel.gameObject.SetActive(emojiId != -1);
        if (emojiId != -1)
            emojiPanel.sprite = EmojiManager.Instance.GetEmojiSprite(emojiId);
    }


    [ClientRpc]
    private void SendChatEvent_ClientRpc(string sendPlayerName, string chatMes)
    {
        ExecuteShowChatEvent(sendPlayerName, chatMes);
    }
    [ServerRpc]
    private void SendChatEvent_ServerRpc(string sendPlayerName, string chatMes)
    {
        ExecuteShowChatEvent(sendPlayerName, chatMes);
    }
    private void ExecuteShowChatEvent(string sendPlayerName, string chatMes)
    {
        ChatManager.Instance.AddChatMes(sendPlayerName, chatMes);
        ChatManager.Instance.ChatMes = "";
    }


    private void DownloadData()
    {
        transform.position = Vector3.Lerp(transform.position, syncPos.Value, Time.deltaTime * 20);
        transform.rotation = Quaternion.Lerp(transform.rotation, syncRot.Value, Time.deltaTime * 20);
        name = Name_Tag.text = syncName.Value.ToString();
    }


    private void DownloadHandleData()
    {
        //rb.velocity = Vector2.Lerp(rb.velocity, syncVelo.Value, Time.deltaTime * 10);
        //rb.velocity = syncVelo.Value;
    }

    private void HandleData()
    {
        //防撞修正
        var lVel = rb.velocity;
        if (VelLock) { lVel.x = lVel.z = 0; }
        if (GravityLock) lVel.y = 0;
        rb.velocity = lVel;

        var lAnguVel = rb.angularVelocity;
        if (AVelLock) { lAnguVel.x = lAnguVel.y = lAnguVel.z = 0; }
        rb.angularVelocity = lAnguVel;

        rb.velocity = GetNextVelo();
        transform.rotation = GetNextRotation();
        name = Name_Tag.text = PlayerName;
    }


    private void UpdateHandleData()
    {
        //var vel = GetMove();
        var name = PlayerName;

        //syncVelo.Value = vel;
        syncName.Value = name;
    }

    private void UpdateHandleData(Vector3 vel, string name)
    {
        syncVelo.Value = vel;
        syncName.Value = name;
    }


    private void UpdateData()
    {
        UpdateData(transform.position, transform.rotation);
    }

    private void UpdateData(Vector3 pos, Quaternion rotation)
    {
        syncPos.Value = pos;
        syncRot.Value = rotation;
    }


    [ServerRpc]
    private void UpdateHandleData_ServerRPC(Vector3 vel, string name)
    {
        UpdateHandleData(vel, name);
    }

    [ServerRpc]
    private void UpdateData_ServerRPC(Vector3 pos, Quaternion rotation)
    {
        UpdateData(pos, rotation);
    }



    private Vector3 GetNextVelo()
    {
        var distance = GetNextVec(v);
        return distance;
    }

    private Vector3 GetNextVec(float v)
    {
        var lVelY = rb.velocity.y;
        var distance = v * transform.forward * MoveVel * 1 / 60f;
        if (!GravityLock) distance.y = lVelY;
        return distance;
    }


    private Quaternion GetNextRotation()
    {
        var rotate = Quaternion.Euler(0, h * rotForce, 0);
        var rotation = transform.rotation * rotate;
        var rotEuler = rotation.eulerAngles;
        if (RotLock) rotEuler.x = rotEuler.z = 0;
        if (RotHalfLock)
        {
            var backAngle = HalfLockAngle;
            //var backAngle = 10;
            //if (Math.Abs(rotEuler.x) > HalfLockAngle) rotEuler.x = Math.Clamp(rotEuler.x, -backAngle, backAngle);
            //if (Math.Abs(rotEuler.z) > HalfLockAngle) rotEuler.z = Math.Clamp(rotEuler.x, -backAngle, backAngle);

            rotEuler.x = Mathf.Lerp(rotEuler.x, 0, rotEuler.x / backAngle * Time.fixedDeltaTime * AngleBackVelRate);
            rotEuler.z = Mathf.Lerp(rotEuler.z, 0, rotEuler.z / backAngle * Time.fixedDeltaTime * AngleBackVelRate);
        }
        //var halfLockAngleEuler = HalfLockAngle / 360f;
        //if(RotHalfLock && (Math.Abs(rotation.x) > halfLockAngleEuler || Math.Abs(rotation.z) > halfLockAngleEuler)){
        //    var backAngle = halfLockAngleEuler - 10/360f;
        //    rotation.x = Math.Clamp(rotation.x, -backAngle, backAngle);
        //    rotation.z = Math.Clamp(rotation.z, -backAngle, backAngle);
        //    rotation.x = rotation.z = 0;
        //}
        rotation.eulerAngles = rotEuler;
        return rotation;
    }


    internal void CGReady()
    {
        if (IsServer)
        {
            syncReady.Value = !syncReady.Value;
        }
        else
        {
            CGReady_ServerRPC(!syncReady.Value);
        }
    }

    [ServerRpc]
    public void CGReady_ServerRPC(bool val)
    {
        syncReady.Value = val;
    }

}
