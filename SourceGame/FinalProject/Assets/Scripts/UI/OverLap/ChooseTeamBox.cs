using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseTeamBox : BaseOverlap
{
    public override void Hide()
    {
        base.Hide();
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Show(object data)
    {
        base.Show(data);
    }
    public void OnButtonChoose(bool team)
    {
        if (BaseManager<AudioManager>.HasInstance())
        {
            BaseManager<AudioManager>.Instance.AttachBGMSource.Stop();
            BaseManager<AudioManager>.Instance.PlayGT();
        }
        if (MultiplayerManager.HasInstance())
        {
            MultiplayerManager.Instance.ChooseTeam(team);
        }
        //if (UIManager.HasInstance())
        //{
        //    UIManager.Instance.ShowScreen<GameUIMulti>(null,true);
        //}
        Hide();
    }
}
