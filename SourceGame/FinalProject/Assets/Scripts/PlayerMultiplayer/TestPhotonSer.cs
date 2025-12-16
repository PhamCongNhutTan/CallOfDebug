using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class TestPhotonSer : MonoBehaviourPun, IPunObservable
{
    public int ind = 0;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting == true
            )
        {
            stream.SendNext(ind);
        }
        else
        {
            ind = (int)stream.ReceiveNext();
        }
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("ind: " + ind);
        if (photonView.IsMine && photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            if (Input.GetKeyUp(KeyCode.V))
            {
                ind++;
            }
        }
        
    }
}
