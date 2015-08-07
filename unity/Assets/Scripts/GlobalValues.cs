using System;
using UnityEngine;

public class GlobalValues : MonoBehaviour
{
	public static int currentLevel = 1, currentMaxLevel = 1, mainLevelsCompleted, hasDisplayedWin1, currentScreen = 1;
	
	public int currentLevelNS, currentMaxLevelNS;
	
	public static bool[] timedLevelsBeat;
	
	public bool[] timedLevelsBeatNS;
	
	public int currentScreenNS;
	
	public static int gameMode = 1;
	
	public int gameModeNS = 1;
	
	public Transform levelButtons;
	
	public static int totalLevelCount = 20;
	
	public int totalLevelCountNS;
	
	public static bool changeScreen;
	
	public static bool recordNewTopTime;
	
	public static bool recordTimedLevelBeat;
	
	public static bool forceRecord;
	
	public static bool clearTimes;
	
	public float[] timesToBeatP;
	
	public static float[] timesToBeatPS;
	
	public static float[] recordedTimes;
	
	public float[] recordedTimesNS;
	
	public static float[] topRecordedTimes;
	
	public float[] topRecordedTimesNS;
	
	public static float currentRecordedTime;
	
	private void Start()
	{
		GlobalValues.topRecordedTimes = new float[GlobalValues.totalLevelCount];
		topRecordedTimesNS = new float[GlobalValues.totalLevelCount];
		GlobalValues.recordedTimes = new float[GlobalValues.totalLevelCount];
		recordedTimesNS = new float[GlobalValues.totalLevelCount];
		GlobalValues.timedLevelsBeat = new bool[GlobalValues.totalLevelCount];
		timedLevelsBeatNS = new bool[GlobalValues.totalLevelCount];
		LoadData();
		GlobalValues.changeScreen = true;
		GlobalValues.gameMode = 1;
	}
	
	private void Update()
	{
		currentLevelNS = GlobalValues.currentLevel;
		currentMaxLevelNS = GlobalValues.currentMaxLevel;
		currentScreenNS = GlobalValues.currentScreen;
		totalLevelCountNS = GlobalValues.totalLevelCount;
		gameModeNS = GlobalValues.gameMode;
		recordedTimesNS = GlobalValues.recordedTimes;
		topRecordedTimesNS = GlobalValues.topRecordedTimes;
		timedLevelsBeatNS = GlobalValues.timedLevelsBeat;
		GlobalValues.timesToBeatPS = timesToBeatP;
		if (GlobalValues.currentMaxLevel > PlayerPrefs.GetInt("Levels Completed"))
		{
			PlayerPrefs.SetInt("Levels Completed", GlobalValues.currentMaxLevel);
		}
		if (GlobalValues.mainLevelsCompleted > PlayerPrefs.GetInt("Main Levels Completed"))
		{
			PlayerPrefs.SetInt("Main Levels Completed", GlobalValues.mainLevelsCompleted);
		}
		if (GlobalValues.mainLevelsCompleted == 1 && GlobalValues.hasDisplayedWin1 == 0)
		{
			PlayerPrefs.SetInt("Has Displayed Win Message", 1);
		}
		if (GlobalValues.currentScreen == 4)
		{
			if (GlobalValues.gameMode == 2 && GlobalValues.recordNewTopTime)
			{
				if (!GlobalValues.forceRecord)
				{
					if (GlobalValues.recordedTimes[GlobalValues.currentLevel - 1] < GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1])
					{
						GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 2] = GlobalValues.recordedTimes[GlobalValues.currentLevel - 2];
						topRecordedTimesNS[GlobalValues.currentLevel - 2] = GlobalValues.recordedTimes[GlobalValues.currentLevel - 2];
						PlayerPrefsX.SetFloatArray("Top Recorded Times", GlobalValues.topRecordedTimes);
					}
					if (GlobalValues.recordedTimes[GlobalValues.currentLevel - 1] < timesToBeatP[GlobalValues.currentLevel - 1])
					{
						GlobalValues.timedLevelsBeat[GlobalValues.currentLevel - 2] = true;
						PlayerPrefsX.SetBoolArray("Completed Timed Levels", GlobalValues.timedLevelsBeat);
					}
				}
				GlobalValues.recordNewTopTime = false;
			}
			if (GlobalValues.recordTimedLevelBeat)
			{
				GlobalValues.recordTimedLevelBeat = false;
			}
		}
		if (GlobalValues.forceRecord)
		{
			if (GlobalValues.recordedTimes[GlobalValues.currentLevel - 1] < GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1])
			{
				GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1] = GlobalValues.recordedTimes[GlobalValues.currentLevel - 1];
				topRecordedTimesNS[GlobalValues.currentLevel - 1] = GlobalValues.recordedTimes[GlobalValues.currentLevel - 1];
				PlayerPrefsX.SetFloatArray("Top Recorded Times", GlobalValues.topRecordedTimes);
			}
			if (GlobalValues.recordedTimes[GlobalValues.currentLevel - 1] < timesToBeatP[GlobalValues.currentLevel - 1])
			{
				GlobalValues.timedLevelsBeat[GlobalValues.currentLevel - 1] = true;
				PlayerPrefsX.SetBoolArray("Completed Timed Levels", GlobalValues.timedLevelsBeat);
			}
			GlobalValues.forceRecord = false;
		}
		if (GlobalValues.clearTimes)
		{
			ClearTimes();
			GlobalValues.clearTimes = false;
		}
	}
	
	private void LoadData()
	{
		GlobalValues.timedLevelsBeat = PlayerPrefsX.GetBoolArray("Completed Timed Levels");
		GlobalValues.hasDisplayedWin1 = PlayerPrefs.GetInt("Has Displayed Win Message");
		GlobalValues.currentMaxLevel = PlayerPrefs.GetInt("Levels Completed");
		GlobalValues.mainLevelsCompleted = PlayerPrefs.GetInt("Main Levels Completed");
		GlobalValues.topRecordedTimes = PlayerPrefsX.GetFloatArray("Top Recorded Times");
		if (GlobalValues.topRecordedTimes.Length == 0)
		{
			GlobalValues.topRecordedTimes = new float[GlobalValues.totalLevelCount];
			topRecordedTimesNS = new float[GlobalValues.totalLevelCount];
		}
		if (GlobalValues.topRecordedTimes.Length != 20)
		{
			GlobalValues.topRecordedTimes = new float[20];
		}
		if (GlobalValues.topRecordedTimes.Length == GlobalValues.totalLevelCount)
		{
			for (int i = 0; i < GlobalValues.totalLevelCount; i++)
			{
				if (GlobalValues.topRecordedTimes[i] == 0f)
				{
					GlobalValues.topRecordedTimes[i] = timesToBeatP[i];
					topRecordedTimesNS[i] = timesToBeatP[i];
				}
				if (GlobalValues.topRecordedTimes[i] > timesToBeatP[i])
				{
					GlobalValues.topRecordedTimes[i] = timesToBeatP[i];
					topRecordedTimesNS[i] = timesToBeatP[i];
				}
			}
		}
		if (GlobalValues.currentMaxLevel < 1)
		{
			GlobalValues.currentMaxLevel = 1;
		}
	}
	
	private void ClearTimes()
	{
		if (GlobalValues.topRecordedTimes.Length != 20)
		{
			GlobalValues.topRecordedTimes = new float[20];
		}
		for (int i = 0; i < GlobalValues.topRecordedTimes.Length; i++)
		{
			GlobalValues.topRecordedTimes[i] = timesToBeatP[i];
			topRecordedTimesNS[i] = timesToBeatP[i];
		}
	}
}