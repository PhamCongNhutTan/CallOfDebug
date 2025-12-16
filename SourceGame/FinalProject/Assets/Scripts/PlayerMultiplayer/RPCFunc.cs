using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RPCFunc : MonoBehaviourPun
{
    public ActiveWeaponMultiplayer act;
    public ObjectPoolMulti pool;
    public PlayerHealthMultiplayer health;
    private void Awake()
    {
        if (photonView.IsMine && photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            photonView.RPC("ChangeNamePlayer", RpcTarget.AllBuffered, PhotonNetwork.NickName, photonView.ViewID);
        }
    }
    [PunRPC]
    public void RPCTakeDame(string shooter, float num, Vector3 direction, int idRb, int viewID)
    {
        if (viewID == photonView.ViewID)
        {
            this.health.shooter = shooter;
            this.health.TakeDamage(num, direction, idRb);
        }
    }
    [PunRPC]
    public void ChangeNamePlayer(string name, int id)
    {
        PhotonView view = GetComponent<PhotonView>();
        if (view.ViewID == id)
        {
            this.gameObject.name = name;
        }    
    }
    public bool CheckIsMine()
    {
        return photonView.IsMine && photonView.CreatorActorNr == PhotonNetwork.LocalPlayer.ActorNumber;
    }
    [PunRPC]
    public void Deactive(int ind, Vector3 pos)
    {
        //Debug.Log(ind + " " + pool.poolObjects.Count);
        //BulletMulti bullet = ;

        pool.poolObjects[ind].isActive = false;
        pool.poolObjects[ind].gameObject.SetActive(false);
        pool.poolObjects[ind].gameObject.transform.position = pos;
        pool.poolObjects[ind].initialPosition = pos;
        pool.poolObjects[ind].initialVelocity = Vector3.zero;
        pool.poolObjects[ind].tracer.emitting = false;
        pool.poolObjects[ind].tracer.Clear();
    }
    [PunRPC]
    public void Active(int ind, Vector3 pos, Vector3 velocity)
    {
        //BulletMulti bullet = pool.poolObjects[ind];
        pool.poolObjects[ind].isActive = true;
        pool.poolObjects[ind].gameObject.SetActive(true);
        pool.poolObjects[ind].time = 0;
        pool.poolObjects[ind].initialPosition = pos;
        pool.poolObjects[ind].initialVelocity = velocity;
        pool.poolObjects[ind].tracer.emitting = true;
        pool.poolObjects[ind].tracer.AddPosition(pos);
    }

}
