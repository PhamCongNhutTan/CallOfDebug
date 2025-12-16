
using UnityEngine;
using UnityEngine.UI;


public class TabMission : MonoBehaviour
{
    public Toggle toggle;
    public bool isTrueToggle = false;
    
    public void OnChangeToggleTab()
    {
        if (toggle.isOn)
        {
            TurnOn();
        }
        else
        {
            TurnOff();
        }

    }
    public void TurnOn()
    {
        //this.img.color = this.colors[3];
       
        this.img.color = DataManager.Instance.ColorUI.imgActiveColor;
        //this.text.color = this.colors[1];
        this.text.color = DataManager.Instance.ColorUI.textActiveColor;
        this.border.SetActive(true);
        this.board.SetActive(true);
    }

    
    public void TurnOff()
    {
        //this.img.color = this.colors[2];
        //this.text.color = this.colors[0];
       
        img.color = DataManager.Instance.ColorUI.imgNormalColor;
        text.color = DataManager.Instance.ColorUI.textNormalColor;
        this.border.SetActive(false);
        this.board.SetActive(false);
    }

    
    public Image img;

    
    public Text text;

    
    public GameObject border;

    
    public GameObject board;


}
