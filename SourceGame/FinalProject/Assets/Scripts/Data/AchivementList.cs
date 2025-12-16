
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AchivementData", menuName = "Data/Achivement Data", order = 2)]
public class AchivementList : ScriptableObject
{
	
	public List<AchivementTime> TimeList;

	
	public List<AchivementKill> KillList;
}
