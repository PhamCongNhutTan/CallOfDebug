using Unity.Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleMovement : MonoBehaviour
{
    CharacterController controller;
    float inputX;
    float inputY;
    public PhotonView photon;
    public AxisState xAxist;
    public AxisState yAxist;
    public Camera mainCamera;
    public GameObject lookAt;
    // Start is called before the first frame update
    void Start()
    {
        if (!photon.IsMine)
        {
            mainCamera.gameObject.SetActive(false);
        }
        controller = GetComponent<CharacterController>();
        photon = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photon.IsMine) return;
        if (this.photon.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        controller.Move((new Vector3(inputX,0,inputY))*0.1f);  
        this.xAxist.Update(Time.fixedDeltaTime);
        this.yAxist.Update(Time.fixedDeltaTime);
        float y = this.mainCamera.transform.rotation.eulerAngles.y;
        lookAt.transform.eulerAngles = new Vector3(yAxist.Value,xAxist.Value,0);
        base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.Euler(0f, y, 0f), 15 * Time.fixedDeltaTime);
    }
}
