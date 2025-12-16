using System;
using UnityEngine;
using UnityEngine.UI;


public class MissionRow : MonoBehaviour
{
	
	public void Init(object value)
	{
		if (value != null)
		{
			DailyMissionData dailyMissionData = value as DailyMissionData;
			if (dailyMissionData != null)
			{
				this.claimButton.gameObject.SetActive(dailyMissionData.isFinished && !dailyMissionData.isClaimed);
				this.tickImg.SetActive(dailyMissionData.isClaimed);
				this.idMission = dailyMissionData.id;
				this.missionDetail.text = string.Concat(new string[]
				{
					dailyMissionData.missionName,
					": Kill ",
					dailyMissionData.killTarget.ToString(),
					" enemies in ",
					dailyMissionData.timeTarget.ToString(),
					"m"
				});
			}
			AchivementKill achivementKill = value as AchivementKill;
			if (achivementKill != null)
			{
				this.claimButton.gameObject.SetActive(achivementKill.killTarget <= BaseManager<MissionManager>.Instance.totalKill && !achivementKill.isClaimed);
				this.tickImg.SetActive(achivementKill.isClaimed);
				this.idMission = achivementKill.id;
				this.missionDetail.text = string.Format("Reach {0} kills ({1}/{2})", achivementKill.killTarget, BaseManager<MissionManager>.Instance.totalKill, achivementKill.killTarget);
			}
			AchivementTime achivementTime = value as AchivementTime;
			if (achivementTime != null)
			{
				this.claimButton.gameObject.SetActive((double)achivementTime.timeTarget <= Math.Floor((double)(BaseManager<MissionManager>.Instance.timeOnline / 60f)) && !achivementTime.isClaimed);
				this.tickImg.SetActive(achivementTime.isClaimed);
				this.idMission = achivementTime.id;
				this.missionDetail.text = string.Format("Online {0} minutes ({1}/{2})", achivementTime.timeTarget, Math.Floor((double)(BaseManager<MissionManager>.Instance.timeOnline / 60f)), achivementTime.timeTarget);
			}
		}
	}

	
	public void OnClaimButton()
	{
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.BroadCast(ListenType.ON_CLAIM_BUTTON, this.idMission);
		}
		this.claimButton.gameObject.SetActive(false);
		this.tickImg.SetActive(true);
	}

	
	public int idMission;

	
	public Button claimButton;

	
	public GameObject tickImg;

	
	public Text missionDetail;
}
