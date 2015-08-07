using System;
using UnityEngine;

public class ForceLevelsCompleted : MonoBehaviour
{
	public bool mainLevelsCompleted;
	
	private void Update()
	{
		if (this.mainLevelsCompleted)
		{
			GlobalValues.currentMaxLevel = GlobalValues.totalLevelCount;
			PlayerPrefs.SetInt("Levels Completed", GlobalValues.currentMaxLevel);
			GlobalValues.mainLevelsCompleted = 1;
			PlayerPrefs.SetInt("Main Levels Completed", GlobalValues.mainLevelsCompleted);
		}
	}
}