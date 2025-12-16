using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


public class MissionPanel : BasePopup
{
	
	public override void Hide()
	{
		base.Hide();
	}

	
	public override void Init()
	{
		base.Init();
		this.rows = new List<MissionRow>();
	}

	
	public override void Show(object data)
	{
		base.Show(data);
		foreach (MissionRow missionRow in this.rows)
		{
			Object.Destroy(missionRow.gameObject);
		}
		this.rows.Clear();
		if (BaseManager<MissionManager>.HasInstance())
		{
			foreach (DailyMissionData dailyMissionData in BaseManager<MissionManager>.Instance.missionList)
			{
				MissionRow missionRow2 = Object.Instantiate<MissionRow>(this.prf, this.content[0], false);
				if (!this.rows.Contains(missionRow2))
				{
					missionRow2.Init(dailyMissionData);
					if (dailyMissionData.isFinished)
					{
						missionRow2.gameObject.transform.SetAsFirstSibling();
					}
					this.rows.Add(missionRow2);
				}
			}
			foreach (AchivementKill achivementKill in BaseManager<MissionManager>.Instance.achivementKillsList)
			{
				MissionRow missionRow3 = Object.Instantiate<MissionRow>(this.prf, this.content[1], false);
				if (!this.rows.Contains(missionRow3))
				{
					missionRow3.Init(achivementKill);
					if (achivementKill.killTarget <= BaseManager<MissionManager>.Instance.totalKill && !achivementKill.isClaimed)
					{
						missionRow3.gameObject.transform.SetAsFirstSibling();
					}
					this.rows.Add(missionRow3);
				}
			}
			foreach (AchivementTime achivementTime in BaseManager<MissionManager>.Instance.achivementTimeList)
			{
				MissionRow missionRow4 = Object.Instantiate<MissionRow>(this.prf, this.content[1], false);
				if (!this.rows.Contains(missionRow4))
				{
					missionRow4.Init(achivementTime);
					if ((double)achivementTime.timeTarget <= Math.Floor((double)(BaseManager<MissionManager>.Instance.timeOnline / 60f)) && !achivementTime.isClaimed)
					{
						missionRow4.gameObject.transform.SetAsFirstSibling();
					}
					this.rows.Add(missionRow4);
				}
			}
		}
		this.OpenTab(0);
	}

	
	public void OnCloseButton()
	{
		this.Hide();
		Debug.Log("hide panel");
	}

	
	public void OpenTab(int n)
	{
		for (int i = 0; i < this.tabs.Count; i++)
		{
			this.tabs[i].TurnOff();
		}
		this.tabs[n].TurnOn();
	}

	
	public MissionRow prf;

	
	public Transform[] content;

	
	private List<MissionRow> rows;

	
	public List<TabMission> tabs;
}
