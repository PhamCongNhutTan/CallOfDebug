using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.Linq;

public class ObjectPoolMulti : MonoBehaviourPun
{
    
    public List<BulletMulti> poolObjects;
    public BulletMulti objectToPool;
    private int amountToPool =50;
    // Start is called before the first frame update
    void Start()
    {
        poolObjects = GetComponentsInChildren<BulletMulti>().ToList<BulletMulti>();
        //this.transform.SetParent(null, false);
        for (int i = 0; i < this.amountToPool; i++)
        {
            BulletMulti bullet = UnityEngine.Object.Instantiate<BulletMulti>(this.objectToPool, base.transform, true);
            bullet.Deactive(Vector3.zero);
            this.poolObjects.Add(bullet);
        }
    }
    public BulletMulti GetPooledObject()
    {
        for (int i = 0; i < poolObjects.Count; i++)
        {
            if (!this.poolObjects[i].IsActive)
            {
                return this.poolObjects[i];
            }
        }
        return null;
    }
    public int GetIndexPooledObject()
    {
        for (int i = 0; i < poolObjects.Count; i++)
        {
            if (!this.poolObjects[i].IsActive)
            {
                return i;
            }
        }
        return -1;
    }
    public int GetIndexPooledObjectDeactive()
    {
        for (int i = 0; i < poolObjects.Count; i++)
        {

            if (this.poolObjects[i].IsActive&& poolObjects[i].time >3f)
            {
                
                return i;
            }
        }
        return -1;
    }
}
