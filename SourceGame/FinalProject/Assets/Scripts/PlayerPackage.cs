
using UnityEngine;
using Photon.Pun;
public class PlayerPackage : MonoBehaviourPun
{
    public Transform[] pack;
    private void Awake()
    {
        foreach (var p in pack)
        {
            p.SetParent(null);
        }
        Destroy(this.gameObject);
    }
}
