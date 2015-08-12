// The following script was written by Chelsea Saunders/pixelatedcrown - provided for non-commercial use only

// MusicScript deals with all the music in-game
// Queues tracks up, plays them, stops them, makes sure the right thing is playing at the right time

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MusicScript : MonoBehaviour
{
	public AudioClip titleIntro;
	
	public AudioClip titleLoop;
	
	public AudioClip timedLoop, pauseLoop;
	
	public AudioClip[] gameplayTracks, gameplaySolutionTracks;
	
	public AudioSource gameAudiosource, solutionAuidosource;
	
	private AudioSource thisAudioSource;
	
	public float fadeVolume;
	
	public static bool changeMusic, changeToSolution, changeFromSolution;
	
	public static bool restartMusic;
	
	public static bool volumePressed;
	
	public static bool muteAll = false;
	
	public static bool startQueue;
	
	public static bool endQueue;
	
	public static bool startTitle;
	
	public static bool endTitle;
	
	private float waitLength;

	public static bool changeToLevels;

	private void Start()
	{
		thisAudioSource = base.GetComponent<AudioSource>();
	}
	
	private void Update()
	{
		if (startTitle == true)
		{
			StartCoroutine("TitleTrack");
			startTitle = false;
		}
		if (endTitle == true)
		{
			StopCoroutine("TitleTrack");
			endTitle = false;
		}

		if (changeMusic == true)
		{
			StartCoroutine(FadeOutMusic());
			changeMusic = false;
		}

		if(changeToSolution == true)
		{
			StartCoroutine(FadeInSolution());
			changeToSolution = false;
		}

		if(changeFromSolution == true)
		{
			StartCoroutine(FadeOutSolution());
			changeFromSolution = false;
		}

		if (startQueue == true)
		{
			StartCoroutine("QueueNormalGamplayTracks");
			startQueue = false;
		}

		if (endQueue == true)
		{
			StopCoroutine("QueueNormalGamplayTracks");
			endQueue = false;
		}

		if (MusicScript.volumePressed)
		{
			if (MusicScript.muteAll)
			{
				thisAudioSource.enabled = false;
				solutionAuidosource.enabled = false;
			}
			if (!MusicScript.muteAll)
			{
				thisAudioSource.enabled = true;
				solutionAuidosource.enabled = true;
			}
			MusicScript.volumePressed = false;
		}

		if(changeToLevels == true)
		{
			MusicScript.endQueue = true;
			if (!MusicScript.muteAll)
			{
				thisAudioSource.volume = 1f;
				solutionAuidosource.volume = 0f;
			}
			MusicScript.startTitle = true;
			changeToLevels =  false;
		}
	}

	private IEnumerator FadeOutMusic()
	{
		float startTime = Time.time;
		float elapsedTime = 0;
		do
		{
			elapsedTime = Time.time - startTime;
			thisAudioSource.volume = Mathf.Lerp(1, 0, elapsedTime/0.5f);
			solutionAuidosource.volume = 0f;
			yield return null;
		} while(elapsedTime < 0.5f);

		ChangeMusic();
		yield return null;
	}
	
	public void ChangeMusic()
	{
		gameAudiosource.volume = 1;

		if (GlobalValues.currentScreen == 1)
		{
			MusicScript.endQueue = true;
			if (!MusicScript.muteAll)
			{
				thisAudioSource.volume = 1f;
				solutionAuidosource.volume = 0f;
			}
			MusicScript.startTitle = true;
		}

		if (GlobalValues.currentScreen == 4)
		{
			MusicScript.endTitle = true;
			if (!MusicScript.muteAll)
			{
				thisAudioSource.volume = 1f;
				solutionAuidosource.volume = 0f;
			}
			if (GlobalValues.gameMode == 1)
			{
				for (int i = 0; i < gameplayTracks.Length; i++)
				{
					AudioClip audioClip = gameplayTracks[i];
					AudioClip audioClip2 = gameplaySolutionTracks[i];

					int num = UnityEngine.Random.Range(i, gameplayTracks.Length);
					gameplayTracks[i] = gameplayTracks[num];
					gameplayTracks[num] = audioClip;


					gameplaySolutionTracks[i] = gameplaySolutionTracks[num];
					gameplaySolutionTracks[num] = audioClip2;
				}
				MusicScript.startQueue = true;
			}
			if (GlobalValues.gameMode == 2)
			{
				thisAudioSource.clip = timedLoop;
				thisAudioSource.loop = true;
				thisAudioSource.Play();
			}
		}
		if (GlobalValues.currentScreen == 5)
		{
			MusicScript.endQueue = true;
			thisAudioSource.Stop();
			StartCoroutine(PauseTrack());
		}
	}

	private IEnumerator TitleTrack()
	{
		thisAudioSource.clip = titleIntro;
		thisAudioSource.loop = false;
		thisAudioSource.Play();
		yield return new WaitForSeconds(titleIntro.length);
		thisAudioSource.clip = titleLoop;
		thisAudioSource.loop = true;
		thisAudioSource.Play();
		yield return null;
	}

	private IEnumerator PauseTrack()
	{
		thisAudioSource.volume = 1;
		thisAudioSource.clip = pauseLoop;
		thisAudioSource.loop = true;
		thisAudioSource.Play();
		yield return null;
	}

	private IEnumerator QueueNormalGamplayTracks()
	{
		thisAudioSource.clip = gameplayTracks[0];
		solutionAuidosource.clip = gameplaySolutionTracks[0];
		thisAudioSource.loop = false;
		solutionAuidosource.loop = false;
		thisAudioSource.Play();
		solutionAuidosource.Play ();
		yield return new WaitForSeconds(gameplayTracks[0].length);
		thisAudioSource.clip = gameplayTracks[1];
		solutionAuidosource.clip = gameplaySolutionTracks[1];
		thisAudioSource.loop = false;
		solutionAuidosource.loop = false;
		thisAudioSource.Play();
		solutionAuidosource.Play();
		yield return new WaitForSeconds(gameplayTracks[1].length);
		startQueue = true;
	}

	private IEnumerator FadeInSolution()
	{
		if(GlobalValues.gameMode == 1)
		{
			float startTime = Time.time;
			float elapsedTime = 0;
			do
			{
				elapsedTime = Time.time - startTime;
				thisAudioSource.volume = Mathf.Lerp(1, 0, elapsedTime/0.5f);
				solutionAuidosource.volume = Mathf.Lerp(0, 1, elapsedTime/0.5f);

				yield return null;
			} while(elapsedTime < 0.5f);
		}
		yield return null;
	}

	private IEnumerator FadeOutSolution()
	{
		if(GlobalValues.gameMode == 1)
		{
			float startTime = Time.time;
			float elapsedTime = 0;
			do
			{
				elapsedTime = Time.time - startTime;
				thisAudioSource.volume = Mathf.Lerp(0, 1, elapsedTime/0.5f);
				solutionAuidosource.volume = Mathf.Lerp(1, 0, elapsedTime/0.5f);
				
				yield return null;
			} while(elapsedTime < 0.5f);	
		}
		yield return null;
	}
}