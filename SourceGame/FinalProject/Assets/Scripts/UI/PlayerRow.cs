using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRow : MonoBehaviour
{
    public Text name;
    public GameObject isReady;
    public void Init(string playerName , bool ready)
    {
        name.text = playerName;
        isReady.SetActive(ready);
    }
    public void SetPlayerReady(bool ready)
    {
        isReady.SetActive(ready);
    }
}
