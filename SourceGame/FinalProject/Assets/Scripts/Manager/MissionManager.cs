using System;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;


public class MissionManager : BaseManager<MissionManager>
{
	
	private void Start()
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			//BaseManager<DataManager>.Instance.LoadPlayerData();
			this.missionList = BaseManager<DataManager>.Instance.GetMissions();
			AchivementList achivementList = BaseManager<DataManager>.Instance.GetAchivementList();
			this.achivementKillsList = achivementList.KillList;
			this.achivementTimeList = achivementList.TimeList;
			this.totalKill = BaseManager<DataManager>.Instance.PlayerData.totalKill;
			this.timeOnline = BaseManager<DataManager>.Instance.PlayerData.totalTime * 60f;
		}
		if (BaseManager<ListenerManager>.HasInstance())
		{
			BaseManager<ListenerManager>.Instance.Register(ListenType.ON_START_MISSION, new Action<object>(this.OnMissionStart));
			BaseManager<ListenerManager>.Instance.Register(ListenType.ON_END_MISSION, new Action<object>(this.OnMissionEnd));
			BaseManager<ListenerManager>.Instance.Register(ListenType.ON_END_MISSION, new Action<object>(this.updatePlayerData));
			BaseManager<ListenerManager>.Instance.Register(ListenType.ON_ALLY_KILL, new Action<object>(this.CheckMission));
			BaseManager<ListenerManager>.Instance.Register(ListenType.ON_CLAIM_BUTTON, new Action<object>(this.ClaimMission));
		}
	}

	
	private void Update()
	{
		this.timeOnline += Time.deltaTime;
		if (this.isPlaying)
		{
			this.time += Time.deltaTime;
		}
	}

	
	public void OnMissionStart(object data)
	{
		this.isPlaying = true;
	}

	
	public void OnMissionEnd(object data)
	{
		this.isPlaying = false;
		this.time = 0f;
		this.kill = 0;
	}

	
	public void CheckMission(object data)
	{
		if (this.isPlaying)
		{
			this.kill++;
			foreach (DailyMissionData dailyMissionData in this.missionList)
			{
				if (dailyMissionData.timeTarget * 60f >= this.time && dailyMissionData.killTarget <= (float)this.kill && !dailyMissionData.isFinished)
				{
					if (BaseManager<UIManager>.HasInstance())
					{
						BaseManager<UIManager>.Instance.ShowNotify<NotifyMission>(dailyMissionData.missionName, true);
					}
					Debug.Log("Done " + dailyMissionData.missionName);
					dailyMissionData.isFinished = true;
					if (BaseManager<DataManager>.HasInstance())
					{
						foreach (DailyMissionData dailyMissionData2 in BaseManager<DataManager>.Instance.MissionList.missionsData)
						{
							if (dailyMissionData2.id == dailyMissionData.id)
							{
								dailyMissionData2.isFinished = true;
							}
						}
					}
					return;
				}
			}
			this.totalKill++;
		}
	}

	
	public void updatePlayerData(object data)
	{
		if (BaseManager<DataManager>.HasInstance())
		{
			BaseManager<DataManager>.Instance.PlayerData.totalKill = this.totalKill;
			BaseManager<DataManager>.Instance.PlayerData.totalTime = math.floor(this.timeOnline / 60f);
			BaseManager<DataManager>.Instance.SavePlayerData();
		}
	}

	
	public void ClaimMission(object data)
	{
		if (data is int)
		{
			int num = (int)data;
			List<requiredData> list = new List<requiredData>();
			if (num >= 200)
			{
				list = new List<requiredData>(this.achivementKillsList);
			}
			else if (num >= 100)
			{
				list = new List<requiredData>(this.achivementTimeList);
			}
			else
			{
				list = new List<requiredData>(this.missionList);
			}
			foreach (requiredData requiredData in list)
			{
				if (requiredData.id == num)
				{
					requiredData.isClaimed = true;
				}
			}
			if (BaseManager<DataManager>.HasInstance())
			{
				foreach (DailyMissionData dailyMissionData in BaseManager<DataManager>.Instance.MissionList.missionsData)
				{
					if (dailyMissionData.id == num && dailyMissionData.isFinished)
					{
						dailyMissionData.isClaimed = true;
					}
				}
				foreach (requiredData requiredData2 in BaseManager<DataManager>.Instance.AchivementList.KillList)
				{
					if (requiredData2.id == num)
					{
						requiredData2.isClaimed = true;
					}
				}
				foreach (requiredData requiredData3 in BaseManager<DataManager>.Instance.AchivementList.TimeList)
				{
					if (requiredData3.id == num)
					{
						requiredData3.isClaimed = true;
					}
				}
				BaseManager<DataManager>.Instance.SavePlayerData();
			}
		}
	}

	
	private void OnApplicationQuit()
	{
		this.updatePlayerData(null);
	}

	
	public List<DailyMissionData> missionList;

	
	public List<AchivementTime> achivementTimeList;

	
	public List<AchivementKill> achivementKillsList;

	
	private float time;

	
	private int kill;

	
	public int totalKill;

	
	public float timeOnline;

	
	private bool isPlaying;
}
