
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MissionData", menuName = "Data/Mission Data", order = 3)]
public class MissionList : ScriptableObject
{
	
	public List<DailyMissionData> missionsData;
}
