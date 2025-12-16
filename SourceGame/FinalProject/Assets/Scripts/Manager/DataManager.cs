
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class DataManager : BaseManager<DataManager>
{
	public ColorUI ColorUI;
    protected override void Awake()
    {
        base.Awake();
    }
    
    private void Start()
	{
		// Data will be loaded from GameManager after login
	}

	public List<DailyMissionData> GetMissions()
	{
		return Resources.LoadAll<MissionList>("Mission")[0].missionsData;
	}

	public AchivementList GetAchivementList()
	{
		return Resources.LoadAll<AchivementList>("Mission")[0];
	}

	public Dictionary<KeyInfo, int> GetInfo(string nameWeapon)
	{
		Dictionary<KeyInfo, int> dictionary = new Dictionary<KeyInfo, int>();
		if (!(nameWeapon == "scar"))
		{
			if (!(nameWeapon == "pistol"))
			{
				if (nameWeapon == "tommy")
				{
					dictionary.Add(KeyInfo.maxAmmo, this.TommyInfo.totalAmmo);
					dictionary.Add(KeyInfo.fireRate, this.TommyInfo.fireRate);
					dictionary.Add(KeyInfo.bulletSpeed, this.TommyInfo.bulletSpeed);
					dictionary.Add(KeyInfo.damage, this.TommyInfo.damage);
				}
			}
			else
			{
				dictionary.Add(KeyInfo.maxAmmo, this.PistolInfo.totalAmmo);
				dictionary.Add(KeyInfo.fireRate, this.PistolInfo.fireRate);
				dictionary.Add(KeyInfo.bulletSpeed, this.PistolInfo.bulletSpeed);
				dictionary.Add(KeyInfo.damage, this.PistolInfo.damage);
			}
		}
		else
		{
			dictionary.Add(KeyInfo.maxAmmo, this.ScarInfo.totalAmmo);
			dictionary.Add(KeyInfo.fireRate, this.ScarInfo.fireRate);
			dictionary.Add(KeyInfo.bulletSpeed, this.ScarInfo.bulletSpeed);
			dictionary.Add(KeyInfo.damage, this.ScarInfo.damage);
		}
		return dictionary;
	}

	/// <summary>
	/// Load player data from PlayFab after login
	/// </summary>
	public void LoadPlayerDataFromPlayFab(Dictionary<string, UserDataRecord> data, bool isNewAccount)
	{
		if (isNewAccount || data == null || data.Count == 0)
		{
			// New account or no data - use default values from ScriptableObject
			Debug.Log("New account or no data found. Using default values.");
			SavePlayerData(); // Save default values to PlayFab
			return;
		}

		// Load PlayerData
		if (data.ContainsKey("PlayerData") && !string.IsNullOrEmpty(data["PlayerData"].Value))
		{
			JsonUtility.FromJsonOverwrite(data["PlayerData"].Value, this.PlayerData);
			Debug.Log("PlayerData loaded from PlayFab");
		}

		// Load MissionData
		if (data.ContainsKey("MissionData") && !string.IsNullOrEmpty(data["MissionData"].Value))
		{
			JsonUtility.FromJsonOverwrite(data["MissionData"].Value, this.MissionList);
			Debug.Log("MissionData loaded from PlayFab");
		}

		// Load AchivementData
		if (data.ContainsKey("AchivementData") && !string.IsNullOrEmpty(data["AchivementData"].Value))
		{
			JsonUtility.FromJsonOverwrite(data["AchivementData"].Value, this.AchivementList);
			Debug.Log("AchivementData loaded from PlayFab");
		}

		Debug.Log("All player data loaded successfully from PlayFab");
	}

	/// <summary>
	/// Save player data to PlayFab
	/// </summary>
	public void SavePlayerData()
	{
		PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
		{
			Data = new Dictionary<string, string>
			{
				{ "PlayerData", JsonUtility.ToJson(this.PlayerData) },
				{ "MissionData", JsonUtility.ToJson(this.MissionList) },
				{ "AchivementData", JsonUtility.ToJson(this.AchivementList) }
			}
		}, result =>
		{
			Debug.Log("Player data saved to PlayFab successfully.");
		}, error =>
		{
			Debug.LogError("Failed to save player data to PlayFab: " + error.GenerateErrorReport());
		});
	}

	public GunInfo ScarInfo;


	public GunInfo PistolInfo;


	public GunInfo TommyInfo;


	public GlobalConfig GlobalConfig;


	public PlayerDataSO PlayerData;


	public MissionList MissionList;


	public AchivementList AchivementList;
}
