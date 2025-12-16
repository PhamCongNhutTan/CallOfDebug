using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chear : MonoBehaviour
{
    public PlayerHealth health;
    public WeaponRaycast weapon;
    string sq = "";

    public bool isCheat = false;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && sq.Equals(""))
        {
            sq += "O";
        }

        if (Input.GetKeyDown(KeyCode.I)&& sq.Equals("O"))
        {
            sq += "I";
        }

        if (Input.GetKeyDown(KeyCode.U) && sq.Equals("OI"))
        {
            sq += "U";
        }

        if (sq.Equals("OIU"))
        {
            isCheat = true;
        }
        if (isCheat)
        {
            health.currentHealth = 100;
            weapon.totalAmmo = 100;
        }
    }
}
