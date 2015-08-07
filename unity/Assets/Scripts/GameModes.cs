using System;
using UnityEngine;
using UnityEngine.UI;

public class GameModes : MonoBehaviour
{
	public static float playTime;
	public Text playTimeText, timeToBeatText;
	public static bool clearTime, recordTime;
	
	private float playTimeTimer;
	private bool startPlayTimeTimer;

	private void Update()
	{
		// if is during gameplay...
		if (GlobalValues.currentScreen == 4)
		{
			// if the game mode is normal...
			if (GlobalValues.gameMode == 1)
			{
				playTimeText.enabled = false;
				timeToBeatText.enabled = false;
			}
			// if the game mode is timed...
			if (GlobalValues.gameMode == 2)
			{
				startPlayTimeTimer = true;
				timeToBeatText.enabled = true;
				timeToBeatText.text = GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1].ToString("F2");

				playTimeTimer += Time.deltaTime;

				if (playTimeTimer >= 1f)
				{
					startPlayTimeTimer = false;
					if (VesselScript.vesselIsMoving == false)
					{
						playTimeText.enabled = true;
						playTime += Time.deltaTime;
						playTimeText.text = playTime.ToString("F2");
					}
				}

				if (VesselScript.vesselIsMoving == false)
				{
					if (playTime < GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1])
					{
						playTimeText.color = Color.black;
					}
					else if (playTime > GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1])
					{
						playTimeText.color = Color.red;
					}
				}

			}
		}

		// if is during the level select screen...
		if (GlobalValues.currentScreen == 2)
		{
			playTimeTimer = 0f;
			startPlayTimeTimer = false;
			clearTime = true;
		}

		if (recordTime == true)
		{
			if (GlobalValues.gameMode == 2)
			{
				GlobalValues.recordedTimes[GlobalValues.currentLevel - 2] = playTime;
				if (GlobalValues.recordedTimes[GlobalValues.currentLevel - 2] < GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 2])
				{
					GlobalValues.recordNewTopTime = true;
				}
			}
			recordTime = false;
		}

		if ((clearTime == true) && (playTime != 0f))
		{
			playTime = 0f;
			playTimeText.text = playTime.ToString("F2");
			timeToBeatText.text = GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1].ToString("F2");
			clearTime = false;
		}
	}
}