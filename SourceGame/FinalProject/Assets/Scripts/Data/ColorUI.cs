using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="Color",menuName ="Data/Color")]
public class ColorUI : ScriptableObject
{
    [Header("ColorRoomRow")]
    public Color nameActiveText;
    public Color nameNormalText;
    public Color mapActiveText;
    public Color mapNormalText;
    public Color modeActiveText;
    public Color modeNormalText;
    public Color BGActive;
    public Color BGNormal;
    [Header("ColorTab")]
    public Color imgActiveColor;
    public Color imgNormalColor;
    public Color textActiveColor;
    public Color textNormalColor;
}
