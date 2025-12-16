using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomRow : MonoBehaviour
{
    [Header("Color")]
    public Color nameActiveText;
    public Color nameNormalText;
    public Color mapActiveText;
    public Color mapNormalText;
    public Color modeActiveText;
    public Color modeNormalText;
    public Color BGActive;
    public Color BGNormal;
    [Header("Text")]
    public Text ID;
    public Text nameText;
    public Text mapText;
    public Text modeText;
    public Text playerText;
    [Header("BG")]
    public Image BG;
    public GameObject border;
    public Toggle toggle;

    public void InitRow(byte id, string name, string map, byte player,byte maxPlayer)
    {
        ID.text = id.ToString();
        nameText.text = name;
        mapText.text = map;
        modeText.text = "Team Deathmatch";
        playerText.text = player + "/" + maxPlayer;
        toggle.isOn = false;
        InactiveRow();
    }
    public void OnToggleChange()
    {
        if (toggle.isOn)
        {
            ActiveRow();
        }
        else
        {
            InactiveRow();
        }
    }
    private void ActiveRow()
    {
        if (ListenerManager.HasInstance())
        {
            ListenerManager.Instance.BroadCast(ListenType.ON_SELECTED_ROOM, this);
        }
        ID.color = nameActiveText;
        nameText.color = nameActiveText;
        mapText.color = mapActiveText;
        playerText.color = mapActiveText;
        modeText.color = modeActiveText;
        BG.color = BGActive;
        border.SetActive(true);
    }
    private void InactiveRow()
    {
        if (ListenerManager.HasInstance())
        {
            ListenerManager.Instance.BroadCast(ListenType.ON_DESELECTED_ROOM, null);
        }
        ID.color = nameNormalText;
        nameText.color = nameNormalText;
        mapText.color = mapNormalText;
        playerText.color = mapNormalText;
        modeText.color = modeNormalText;
        BG.color = BGNormal;
        border.SetActive(false);
    }

}
