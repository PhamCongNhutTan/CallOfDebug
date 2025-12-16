using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardRow : MonoBehaviour
{
    public Text nameText;
    public Text killText;
    public Text deathText;
    public void InitRow(string name, int[] point)
    {
        nameText.text = name;
        killText.text = point[0].ToString();
        deathText.text = point[1].ToString();
    }
}
