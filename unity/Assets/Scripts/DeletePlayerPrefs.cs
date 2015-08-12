// The following script was written by Chelsea Saunders/pixelatedcrown - provided for non-commercial use only

// DeletePlayerPrefs is strictly for deleting player preferences when it comes to deleting save data
// Nothing else!

using System;
using UnityEngine;

public class DeletePlayerPrefs : MonoBehaviour
{
	public bool deleteCompletedLevels, deleteCompletedTimedLevels, deleteLevelTimes, deleteWinMessage;
	public static bool clearTimes, clearData;
	
	private void Start()
	{
		if (deleteCompletedLevels)
		{
			PlayerPrefs.DeleteKey("Levels Completed");
			PlayerPrefs.DeleteKey("Main Levels Completed");
		}
		if (deleteCompletedTimedLevels)
		{
			PlayerPrefs.DeleteKey("Completed Timed Levels");
		}
		if (deleteLevelTimes)
		{
			PlayerPrefs.DeleteKey("Top Recorded Times");
		}
		if (deleteWinMessage)
		{
			PlayerPrefs.DeleteKey("Has Displayed Win Message");
		}
	}
	
	private void Update()
	{
		if (DeletePlayerPrefs.clearTimes)
		{
			PlayerPrefs.DeleteKey("Completed Timed Levels");
			PlayerPrefs.DeleteKey("Top Recorded Times");
			GlobalValues.gameMode = 1;
			GlobalValues.clearTimes = true;
			UIScript.forceClose = true;
			DeletePlayerPrefs.clearTimes = false;
		}
		if (DeletePlayerPrefs.clearData)
		{
			PlayerPrefs.DeleteKey("Has Displayed Win Message");
			PlayerPrefs.DeleteKey("Levels Completed");
			PlayerPrefs.DeleteKey("Main Levels Completed");
			PlayerPrefs.DeleteKey("Completed Timed Levels");
			PlayerPrefs.DeleteKey("Top Recorded Times");
			GlobalValues.currentLevel = 1;
			GlobalValues.currentMaxLevel = 1;
			GlobalValues.mainLevelsCompleted = 0;
			GlobalValues.gameMode = 1;
			GlobalValues.clearTimes = true;
			UIScript.forceClose = true;
			DeletePlayerPrefs.clearData = false;
		}
	}
}