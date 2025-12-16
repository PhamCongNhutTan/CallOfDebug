using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreRaycastCam : MonoBehaviour
{
    public int defLayer;
    private void OnTriggerEnter(Collider other)
    {
        defLayer = other.gameObject.layer;
        other.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }
    private void OnTriggerExit(Collider other)
    {
        other.gameObject.layer = defLayer;
    }
}
