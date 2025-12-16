using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AddSourceObject : MonoBehaviour
{
    public MultiAimConstraint aim;
    private void Start()
    {
        aim = GetComponent<MultiAimConstraint>();
        Transform crossHairTarget = FindObjectOfType<CrossHairTarget>().transform;
        AddSource(crossHairTarget);
    }
    public void AddSource(Transform transform)
    {
        aim.data.sourceObjects.Add( new WeightedTransform(transform, 1f));
    }
}
