
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Photon;
using Photon.Pun;

public class ObjectPool : BaseManager<ObjectPool>
{
    
    private void Start()
    {
        if (objectToPool == null) return;
        if (BaseManager<DataManager>.HasInstance())
        {
            this.totalEnemy = BaseManager<DataManager>.Instance.GlobalConfig.totalEnemy;
        }
        this.spawnPos = GameObject.FindGameObjectsWithTag("Respawn").ToList<GameObject>();
        this.poolObjects = new List<Bullet>();
        this.amountToPool = 50;
        for (int i = 0; i < this.amountToPool; i++)
        {
            Bullet bullet = UnityEngine.Object.Instantiate<Bullet>(this.objectToPool, base.transform, true);
            bullet.Deactive();
            this.poolObjects.Add(bullet);
        }
        for (int j = 0; j < this.amountToPool; j++)
        {
            Bullet bullet = UnityEngine.Object.Instantiate<Bullet>(this.objectToPool, base.transform, true);
            bullet.Deactive();
            this.poolAiObjects.Add(bullet);
        }


        if(PhotonNetwork.CurrentRoom == null)
        {
            for (int k = 0; k < this.totalEnemy; k++)
            {
                GameObject item = UnityEngine.Object.Instantiate<GameObject>(this.aiToPool, this.spawnPos[k].transform.position, Quaternion.identity);
                this.aiPool.Add(item);
            }
        }
        



    }


    
    public Bullet GetPooledObject()
    {
        for (int i = 0; i < this.amountToPool; i++)
        {
            if (!this.poolObjects[i].IsActive)
            {
                return this.poolObjects[i];
            }
        }
        return null;
    }

    
    public Bullet GetPooledAiObject()
    {
        for (int i = 0; i < this.amountToPool; i++)
        {
            if (!this.poolAiObjects[i].IsActive)
            {
                return this.poolAiObjects[i];
            }
        }
        return null;
    }

    
    [HideInInspector]
    public List<Bullet> poolObjects;

    
    public List<Bullet> poolAiObjects;

    
    public Bullet objectToPool;

    
    public List<GameObject> spawnPos;

    
    public GameObject aiToPool;

    
    public List<GameObject> aiPool;

    
    private int totalEnemy;

    
    private List<GameObject> aiPoolSPawn = new List<GameObject>();

    
    public GameObject aiTest;

    
    private int amountToPool = 100;
}
