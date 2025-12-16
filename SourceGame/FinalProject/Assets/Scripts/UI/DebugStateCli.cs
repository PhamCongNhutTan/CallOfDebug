using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class DebugStateCli : MonoBehaviourPun
{

    public TextMeshProUGUI text;
    string change = "";
    // Update is called once per frame
    void Update()
    {

        text.SetText(PhotonNetwork.NetworkClientState.ToString());
        if (change.Equals(text.text)) return;
        Debug.Log(PhotonNetwork.NetworkClientState.ToString());
        change = text.text;
    }
}
