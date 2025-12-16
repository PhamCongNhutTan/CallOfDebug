using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Chat.Demo;
using UnityEngine.UI;

public static class ColorTextChat
{
    public const string all = "f0dc8cff";
    public const string ally = "279effff";
    public const string enemy = "ff0000ff";
}
public class ChatMulti : MonoBehaviour, IChatClientListener
{
    public ChatClient chatClient;
    public ChatAppSettings chatAppSettings;
    public Text chatText;
    public InputField chatField;
    public string currentChannel = "Global";
    public float time;
    public GameObject chatBox;
    public string[] defaultChannels;
    private void Start()
    {
        chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
        this.chatClient = new ChatClient(this);
        this.chatClient.AuthValues = new AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        this.chatClient.ConnectUsingSettings(this.chatAppSettings);
        
    }
    private void Update()
    {
        if (this.chatClient != null)
        {
            this.chatClient.Service(); // make sure to call this regularly! it limits effort internally, so calling often is ok!
        }
        if (chatField.isFocused)
        {
            if (MultiplayerManager.HasInstance())
            {
                if (MultiplayerManager.Instance.curCharacterLocomotion!= null)
                MultiplayerManager.Instance.curCharacterLocomotion.isChatting = true;
            }
        }
        else
        {
            if (MultiplayerManager.HasInstance())
            {
                if (MultiplayerManager.Instance.curCharacterLocomotion != null)
                    MultiplayerManager.Instance.curCharacterLocomotion.isChatting = false;
            }
        }
        if (!string.IsNullOrEmpty(chatField.text))
        {
            time = 10;
        }
        else
        {
            time -= Time.deltaTime;
        }
        if (time <= 0)
        {
            chatBox.SetActive(false);
        }
        OnEnterSend();
    }
    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            if (chatBox.active)
            {
                SendMsg(chatField.text);
                chatField.text = "";
            }
            else
            {
                chatBox.SetActive(true);
                chatField.Select();
                time = 10;
            }
        }
        
    }
    private void SendMsg(string msg)
    {
        if (string.IsNullOrEmpty(msg)) { return; }
        string[] tokens = msg.Split(" ");
        Debug.Log(tokens[0]);
        bool changeChannel = false;
        if (tokens[0].StartsWith('/'))
        {
            Debug.Log("token: "+tokens[0]);
            if (tokens[0].Equals("/team")&&PhotonNetwork.CurrentRoom!= null)
            {
                if (!string.IsNullOrEmpty(defaultChannels[1]))
                {
                    currentChannel = defaultChannels[1];
                    changeChannel = true;

                }
            }
            if (tokens[0].Equals("/all") && PhotonNetwork.CurrentRoom != null)
            {
                if (!string.IsNullOrEmpty(defaultChannels[0]))
                {
                    currentChannel = defaultChannels[0];
                    changeChannel= true;
                }
            }
        }
        string posfix = "";
        if (PhotonNetwork.CurrentRoom != null)
        {
            posfix = PhotonNetwork.CurrentRoom.Name;
        }
        chatClient.PublishMessage(currentChannel + posfix, changeChannel ? msg.Substring(tokens[0].Length+1): msg);
    }
    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnConnected()
    {
        chatClient.SetOnlineStatus(ChatUserStatus.Online);
        
        foreach(string c in defaultChannels)
        {
            if (string.IsNullOrEmpty (c)) { continue; }
            string posfix = "";
            if(PhotonNetwork.CurrentRoom != null)
            {
                posfix = PhotonNetwork.CurrentRoom.Name;
            }
            chatClient.Subscribe(c+posfix,0,0);
        }
        currentChannel = defaultChannels[0];
        Debug.Log("Onconnected Chat");
    }

    public void OnDisconnected()
    {
        chatClient.SetOnlineStatus(ChatUserStatus.Offline);
        chatClient.Unsubscribe(defaultChannels);
        Debug.Log("OnDisconect Chat");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        Debug.Log("OnGetMessages Channel: " + channelName);
        Debug.Log(messages.Length + " messages received.");
        string posfix = "";
        if (PhotonNetwork.CurrentRoom != null)
        {
            posfix = PhotonNetwork.CurrentRoom.Name;
        }
        if (channelName == currentChannel+posfix)
        {
            ShowChannel(channelName);
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        ShowChannel(channels[0]);
    }

    public void OnUnsubscribed(string[] channels)
    {
        
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
        {
            return;
        }

        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("ShowChannel failed to find channel: " + channelName);
            return;
        }

        //this.selectedChannelName = channelName;
        string[] txt = channel.ToStringMessages().Split('\n');
        string chat = "";
        foreach (string s in txt)
        {
            if (string.IsNullOrEmpty(s)) continue;
            string[] strings = s.Split(':');
            chat += $"<color=#{ColorTextChat.all}>[{currentChannel}] {strings[0]}:</color>{strings[1]}\n";
        }
        chatText.text = chat;
        
    }
}
