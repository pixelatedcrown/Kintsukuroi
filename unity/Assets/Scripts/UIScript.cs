using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIScript : MonoBehaviour {

	public GameObject titleScreen, playScreen, galleryScreen, gameScreen, 
	pauseScreen, piecesMenu, floor, cameraPoint, gameModeButton, settingsScreen, aboutScreen,
	popupTut1, popupTut2, popupConfirm, popupWin1, aboutSprites;

	public Color unlocked, locked;

	public Material timedComplete, timedIncomplete, boxInactive, boxActive, black;

	public Text gameModeText; //levelNumberText, topTimeText;

	public GameObject pieceSlots, levelButtons, fadeScreen, menuScroll; 
	//levelNumberBox, topTimeBox;

	public Slider pieceMenuSlider;
	public static bool resetSlider;
	public Scrollbar levelSelectScrollbar;
	private float slotsY;

	public GameObject[] levelSelectButtons, pauseSpritesIncomplete, pauseSpritesComplete;

	public static bool forceClose, hidePiecesMenu, levelsCompleted;
	private bool hasFadedOut, hasFadedBlack, hasIntroFaded, freezeTouch, galleryTrigger, initialRepo;
	private float fadeAlpha, fadeVolume;
	
	private int currentSelectedLevel;

	public AudioSource gameScreenAudioSource, cameraPointAudioSource;
	public AudioClip sfxPause, sfxUnpause;
	
	public Camera mainCam;

	public Skybox titleSkybox;
	
	public Transform[] playCombo;

	public bool playMenuOpen, settingsOpen, aboutOpen;
	
	public bool volumeIsOn;
	
	public GameObject buttonModeUp, buttonModeDown, bMusicUp, bMusicDown, bMusicUp2, bMusicDown2, bTimesUp, bTimesDown, 
	bDataUp, bDataDown, bYesUp, bYesDown, bNoUp, bNoDown, yesSprite, noSprite, bMainUp, bMainDown, bResumeUp, bResumeDown;

	public Text[] menuTimes;
	public Text settingsMusic, settingsTimes, settingsData, pausedMusic, pausedMain, pausedResume;

	private int settingsChoice;

	public bool tapped, startTouchTimer;
	public float touchTime, tapLength = 0.4f;

	public GameObject normalDotYes, timedDotYes;

	private GameObject currentButton;
	public Color clicked;
	private Color tempColor;

	private bool tutStarted;

	public float[] slotPositions;

	void Update () 
	{
		slotsY = pieceSlots.transform.localPosition.y;

		//5 = -8.34
		//6 = -13.37
		//7 = -18.4
		//8 = -23.34
		//9 = -28.48
		//10 = -33.49
		//11 = -38.39
		//12 = -43.49
		//13 = -48.5
		//14 = -53.49
		//15 = -58.44

		pieceSlots.transform.localPosition = new Vector3(Mathf.Lerp(0f, slotPositions[VesselScript.currentSlotCount], 
		                                                            pieceMenuSlider.value), slotsY, 0f);

		// timer for counting if touch is a tap or a touch
		if (startTouchTimer == true){touchTime += Time.deltaTime;}
		else{touchTime = 0f;}

		if(freezeTouch == false)
		{
			for (var i = 0; i < Input.touchCount; ++i) 
			{
				if (Input.GetTouch(i).phase == TouchPhase.Began)
				{
					startTouchTimer = true;

				}

				if ((Input.GetTouch(i).phase == TouchPhase.Moved) || (Input.GetTouch(i).phase == TouchPhase.Stationary))
				{
					if(touchTime < tapLength)
					{
						tapped = true;
					}
					else
					{
						tapped = false;
					}
				}

				if(Input.GetTouch(i).phase == TouchPhase.Ended) 
				{
					CheckTouch();
				}

			}
		}

		if(hidePiecesMenu == true){piecesMenu.SetActive(false);}
		if(hidePiecesMenu == false){piecesMenu.SetActive(true);}

		if (forceClose == true)
		{
			if (GlobalValues.gameMode == 2)
			{
				GlobalValues.recordedTimes[GlobalValues.currentLevel - 1] = GameModes.playTime;
				if (GlobalValues.recordedTimes[GlobalValues.currentLevel - 1] < GlobalValues.topRecordedTimes[GlobalValues.currentLevel - 1])
				{
					GlobalValues.forceRecord = true;
				}
			}
			GlobalValues.currentScreen = 5;
			StartCoroutine(FadeBlack());
			forceClose = false;
		}

		if(GlobalValues.changeScreen == true)
		{
			// if the current screen is set to be the title screen (1), show that screen, hide others
			if(GlobalValues.currentScreen == 1)
			{
				// reposition the camera
				cameraPoint.transform.position = new Vector3(0, 0f, 0);
				cameraPoint.transform.eulerAngles = new Vector3(0, 0, 0);
				mainCam.transform.position = new Vector3(0, -1.2f, -15);
				mainCam.clearFlags = CameraClearFlags.Skybox;

				menuScroll.transform.localPosition = Vector3.zero;

				// change music
				MusicScript.changeMusic = true;

				// enable the floor
				floor.SetActive(false);

				titleScreen.SetActive(true);
				playScreen.SetActive(false);
				gameScreen.SetActive(false);
				pauseScreen.SetActive(false);
				galleryScreen.SetActive(false);
			}

			// if the current screen is set to be the level select screen (2)
			if(GlobalValues.currentScreen == 2)
			{
				UpdateLevelButtons();

				titleScreen.SetActive(true);
				playScreen.SetActive(true);
				gameScreen.SetActive(false);
				pauseScreen.SetActive(false);
			}

			if (GlobalValues.currentScreen == 6)
			{
				titleScreen.SetActive(false);
				playScreen.SetActive(false);
				gameScreen.SetActive(false);
				pauseScreen.SetActive(false);
			}

			// if the current screen is set to be main gameplay (4)
			if(GlobalValues.currentScreen == 4)
			{
				// change music
				MusicScript.changeMusic = true;

				mainCam.clearFlags = CameraClearFlags.Color;
				mainCam.backgroundColor = Color.white;
				floor.SetActive(true);
				titleScreen.SetActive(false);
				playScreen.SetActive(false);
				gameScreen.SetActive(true);
				pauseScreen.SetActive(false);
			}

			// if the current screen is set to be paused gameplay (5)
			if(GlobalValues.currentScreen == 5)
			{
				// change music
				MusicScript.changeMusic = true;

				titleScreen.SetActive(false);
				playScreen.SetActive(false);
				gameScreen.SetActive(true);
				pauseScreen.SetActive(true);
			}
			
			GlobalValues.changeScreen = false;
		}

		// if is during the title screen...
		if(GlobalValues.currentScreen == 1)
		{

			if(hasIntroFaded == false)
			{
				StartCoroutine(FadeOutMenu());
				hasIntroFaded = true;
			}

			if(hasFadedBlack == true)
			{
				GlobalValues.currentScreen = 3;
				GlobalValues.changeScreen = true;
				
				StartCoroutine(FadeOut());
				hasFadedBlack = false;
			}
		}

		// if is during the pause screen...
		if(GlobalValues.currentScreen == 5)
		{
			if(hasFadedBlack == true)
			{
				GlobalValues.currentLevel = 1;
				VesselScript.clearVessel = true;
				hidePiecesMenu = false;

				StartCoroutine(FadeOutMenu());
				hasFadedBlack = false;
			}
		}

		// if is during the level select screen...
		if(GlobalValues.currentScreen == 2)
		{

			if(Input.touchCount > 0)
			{
				Touch touch2 = Input.touches[0];
				if (touch2.phase == TouchPhase.Moved)
				{
					Vector3 temp = menuScroll.transform.position;
					temp.y += touch2.deltaPosition.y/22;
					menuScroll.transform.position = temp;

				}
			}

			if(menuScroll.transform.localPosition.y > 495)
			{
				Vector3 temp = menuScroll.transform.localPosition;
				temp.y = 512;
				menuScroll.transform.localPosition = temp;
			}

			if(menuScroll.transform.localPosition.y < 0)
			{
				Vector3 temp = menuScroll.transform.localPosition;
				temp.y = 0;
				menuScroll.transform.localPosition = temp;
			}

			if(GlobalValues.gameMode == 1)
			{
				for(int a = 0; a < menuTimes.Length; a++)
				{
					menuTimes[a].enabled = false;
				}
			}

			if(GlobalValues.gameMode == 2)
			{
				for(int a = 0; a < menuTimes.Length; a++)
				{
					menuTimes[a].enabled = true;
				}
			}

			if(hasFadedBlack == true)
			{

				GlobalValues.currentScreen = 4;
				VesselScript.newStartingVessel = true;

				GlobalValues.changeScreen = true;
				StartCoroutine(FadeOut());
				hasFadedBlack = false;
			}
		}


		if (GlobalValues.currentScreen == 4)
		{
			if (GlobalValues.gameMode == 1)
			{
				if (GlobalValues.currentLevel == 1)
				{
					popupTut1.SetActive(true);
					popupTut2.SetActive(false);
				}

				if (GlobalValues.currentLevel == 2)
				{
					popupTut1.SetActive(false);
					popupTut2.SetActive(true);
				}

				if ((GlobalValues.currentLevel != 1) && (GlobalValues.currentLevel != 2))
				{
					popupTut1.SetActive(false);
					popupTut2.SetActive(false);
				}
			}
		}
		else
		{
			popupTut1.SetActive(false);
			popupTut2.SetActive(false);
			tutStarted = false;
		}

		if (resetSlider)
		{
			pieceMenuSlider.value = 0f;
			resetSlider = false;
		}

	}

	private void CheckTouch()
	{
		if (freezeTouch == false)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
			if (GlobalValues.currentScreen == 6)
			{
				GlobalValues.currentScreen = 4;
				VesselScript.newStartingVessel = true;
				
				GlobalValues.changeScreen = true;
				StartCoroutine(FadeOut());
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_win1")
			{
				Debug.Log("aa");
				popupWin1.SetActive(false);
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_gameMode_normal")
			{
				if(GlobalValues.gameMode != 1)
				{
					GlobalValues.gameMode = 1;

					normalDotYes.SetActive(true);
					timedDotYes.SetActive(false);
		
					cameraPointAudioSource.PlayOneShot(sfxUnpause, 1f);
					UpdateLevelButtons();
				}
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_gameMode_timed")
			{
				if(GlobalValues.gameMode != 2)
				{
					GlobalValues.gameMode = 2;

					normalDotYes.SetActive(false);
					timedDotYes.SetActive(true);
									
					cameraPointAudioSource.PlayOneShot(sfxUnpause, 1f);
					UpdateLevelButtons();
				}
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_play")
			{
				UpdateLevelButtons();
				GlobalValues.currentScreen = 2;
				GlobalValues.changeScreen = true;
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_back")
			{
				GlobalValues.currentScreen = 1;
				GlobalValues.changeScreen = true;
			}
			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.tag == "unlocked")
			{
				for (int i = 0; i < levelSelectButtons.Length; i++)
				{
					if (raycastHit.transform == levelSelectButtons[i].transform)
					{
						if(tapped == true)
						{
							GlobalValues.currentLevel = i+1;
							StartCoroutine(FadeBlack());
						}
					}
				}
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_settings")
			{
				if (!settingsOpen)
				{
					settingsScreen.SetActive(true);
					settingsOpen = true;
				}
				else if (settingsOpen)
				{
					settingsScreen.SetActive(false);
					settingsOpen = false;
				}
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_about")
			{
				if (!aboutOpen)
				{
					aboutScreen.SetActive(true);
					aboutOpen = true;

				}
				else if (aboutOpen)
				{
					aboutScreen.SetActive(false);
					aboutOpen = false;
				}
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_music")
			{
				currentButton = raycastHit.transform.gameObject;
				StartCoroutine(AnimateButton());

				if (volumeIsOn == false)
				{
					settingsMusic.text = "music: off";
					pausedMusic.text = "music: off";
					MusicScript.muteAll = true;
					volumeIsOn = true;
				}
				else if (volumeIsOn == true)
				{
					settingsMusic.text = "music: on";
					pausedMusic.text = "music: on";
					MusicScript.muteAll = false;
					volumeIsOn = false;
				}
				MusicScript.volumePressed = true;
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_clearTimes")
			{
				currentButton = raycastHit.transform.gameObject;
				settingsChoice = 1;
				StartCoroutine(AnimateButton());

				popupConfirm.SetActive(true);
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_clearData")
			{
				currentButton = raycastHit.transform.gameObject;
				settingsChoice = 2;
				StartCoroutine(AnimateButton());

				popupConfirm.SetActive(true);
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_yes")
			{
				StartCoroutine(YesButton());
			}
			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_no")
			{
				StartCoroutine(NoButton());
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "pauseButton" && !VesselScript.vesselIsMoving)
			{
				if (GlobalValues.currentScreen == 4)
				{
					if (!VesselScript.vesselIsMoving)
					{
						gameScreenAudioSource.PlayOneShot(sfxPause, 1f);

						if(GlobalValues.currentLevel == GlobalValues.currentMaxLevel)
						{
							for(int a = 0; a < pauseSpritesIncomplete.Length; a++)
							{
								pauseSpritesComplete[a].SetActive(false);
								if(a != GlobalValues.currentLevel-1)
								{
									pauseSpritesIncomplete[a].SetActive(false);
								}
								else
								{
									pauseSpritesIncomplete[a].SetActive(true);
								}
							}
						}

						if(GlobalValues.currentLevel < GlobalValues.currentMaxLevel)
						{
							for(int a = 0; a < pauseSpritesIncomplete.Length; a++)
							{
								pauseSpritesIncomplete[a].SetActive(false);
								if(a != GlobalValues.currentLevel-1)
								{
									pauseSpritesComplete[a].SetActive(false);
								}
								else
								{
									pauseSpritesComplete[a].SetActive(true);
								}
							}
						}

						UIScript.hidePiecesMenu = true;
						GlobalValues.currentScreen = 5;
						GlobalValues.changeScreen = true;
					}
				}
				else if (GlobalValues.currentScreen == 5)
				{
					currentButton = raycastHit.transform.gameObject;
					StartCoroutine(AnimatePauseButton());
				}
			}
			if (!Physics.Raycast(ray, out raycastHit) || !(raycastHit.transform.gameObject.name == "piecesMenuButton") || !VesselScript.vesselIsMoving)
			{
			}
			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "mainMenuButton")
			{
				currentButton = raycastHit.transform.gameObject;
				StartCoroutine(AnimateMenuButton());
			}
			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "button_quit")
			{
				base.StartCoroutine(QuitFade());
			}
			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "debugButton_newVessel")
			{
				VesselScript.newVessel = true;
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "link_chelsea")
			{
				Application.OpenURL ("http://www.chelseasaunders.com/");
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "link_clark")
			{
				Application.OpenURL ("http://clarkpowell.bandcamp.com/");
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "link_amatic")
			{
				Application.OpenURL ("http://scripts.sil.org/cms/scripts/page.php?item_id=OFL_web");
			}

			if (Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.name == "link_worksans")
			{
				Application.OpenURL ("https://github.com/weiweihuanghuang/Work-Sans/blob/master/LICENSE.txt");
			}

		}
		startTouchTimer = false;
		tapped = false;
	}

	private void UpdateLevelButtons()
	{
		for (int i = 0; i < levelSelectButtons.Length; i++)
		{
			if(GlobalValues.mainLevelsCompleted == 0)
			{
				if (i < GlobalValues.currentMaxLevel - 1)
				{
					levelSelectButtons[i].tag = "unlocked";
					
					foreach(Transform child in levelSelectButtons[i].transform)
					{
						if(child.gameObject.name == "Text 1")
						{
							child.gameObject.GetComponent<Text>().color = unlocked;
						}
						if(child.gameObject.name.StartsWith("s"))
						{
							child.gameObject.SetActive(false);
						}
						if(child.gameObject.name.StartsWith("c"))
						{
							child.gameObject.SetActive(true);
						}
					}
				}

				if (i == GlobalValues.currentMaxLevel - 1)
				{
					levelSelectButtons[i].tag = "unlocked";
					
					foreach(Transform child in levelSelectButtons[i].transform)
					{
						if(child.gameObject.name == "Text 1")
						{
							child.gameObject.GetComponent<Text>().color = unlocked;
						}
						if(child.gameObject.name.StartsWith("s"))
						{
							child.gameObject.SetActive(true);
							child.gameObject.GetComponent<SpriteRenderer>().color = unlocked;
						}
						if(child.gameObject.name.StartsWith("c"))
						{
							child.gameObject.SetActive(false);
						}
					}
				}

				if (i > GlobalValues.currentMaxLevel - 1)
				{
					levelSelectButtons[i].tag = "locked";

					foreach(Transform child in levelSelectButtons[i].transform)
					{
						if(child.gameObject.name == "Text 1")
						{
							child.gameObject.GetComponent<Text>().color = locked;
						}
						if(child.gameObject.name.StartsWith("s"))
						{
							child.gameObject.SetActive(true);
							child.gameObject.GetComponent<SpriteRenderer>().color = locked;
						}
							if(child.gameObject.name.StartsWith("c"))
							{
								child.gameObject.SetActive(false);
							}
						}
				}
			}
			else
			{
				foreach(Transform child in levelSelectButtons[i].transform)
				{
					levelSelectButtons[i].tag = "unlocked";
					if(child.gameObject.name == "Text 1")
					{
						child.gameObject.GetComponent<Text>().color = unlocked;
					}
					if(child.gameObject.name.StartsWith("s"))
					{
						child.gameObject.SetActive(false);
					}
					if(child.gameObject.name.StartsWith("c"))
					{
						child.gameObject.SetActive(true);
					}
				}
			}
		}
		
		if (GlobalValues.timedLevelsBeat.Length != levelSelectButtons.Length)
		{
			GlobalValues.timedLevelsBeat = new bool[GlobalValues.totalLevelCount];
		}

		for(int a = 0; a < menuTimes.Length; a++)
		{
			menuTimes[a].text = GlobalValues.topRecordedTimes[a].ToString("F2");
		}
	}

	IEnumerator FadeBlack()
	{
		freezeTouch = true;
		fadeAlpha = fadeScreen.transform.GetComponent<Renderer>().material.color.a;
		fadeVolume = gameObject.GetComponent<AudioSource>().volume;

		StartCoroutine(FadeOutMusic());

		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.54f)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp(fadeAlpha, 1f, t));
			fadeScreen.transform.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}
		fadeScreen.transform.GetComponent<Renderer>().material.color = Color.black;

		hasFadedBlack = true;
		freezeTouch = false;

		if (GlobalValues.currentScreen == 6)
		{
			GlobalValues.currentScreen = 4;
			GlobalValues.changeScreen = true;
			StartCoroutine(FadeOut());
		}

		settingsOpen = false;
		popupConfirm.SetActive(false);
		settingsScreen.SetActive(false);


		yield return null;
	}

	IEnumerator FadeOutMusic()
	{
		float startTime = Time.time;
		float elapsedTime = 0;
		do
		{
			elapsedTime = Time.time - startTime;
			gameScreenAudioSource.volume = Mathf.Lerp(1, 0, elapsedTime/0.5f);
			yield return null;
		} while(elapsedTime < 0.5f);    
	}

	IEnumerator FadeOutMenu()
	{
		freezeTouch = true;

		if(GlobalValues.currentScreen == 5)
		{
			GlobalValues.currentScreen = 2;
			GlobalValues.changeScreen = true;
			MusicScript.changeToLevels = true;
		}
		
		gameScreenAudioSource.volume = 1;
		yield return new WaitForSeconds(2f);
		fadeAlpha = fadeScreen.transform.GetComponent<Renderer>().material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.54f)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp(fadeAlpha, 0f, t));
			fadeScreen.transform.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		fadeScreen.transform.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
		freezeTouch = false;
		yield return null;
	}

	IEnumerator FadeOut()
	{
		freezeTouch = true;

		yield return new WaitForSeconds(1f);
		fadeAlpha = fadeScreen.transform.GetComponent<Renderer>().material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.54f)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp(fadeAlpha, 0f, t));
			fadeScreen.transform.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}
		yield return new WaitForSeconds(0.5f);
		fadeScreen.transform.GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
		freezeTouch = false;
		yield return null;
	}

	IEnumerator QuitFade()
	{
		StartCoroutine(FadeOutMusic());
		fadeAlpha = fadeScreen.transform.GetComponent<Renderer>().material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.54f)
		{
			Color newColor = new Color(0, 0, 0, Mathf.Lerp(fadeAlpha, 1f, t));
			fadeScreen.transform.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}
		fadeScreen.transform.GetComponent<Renderer>().material.color = Color.black;
		yield return new WaitForSeconds(1f);
		
		Application.Quit(); 
		yield return null;
	}

	IEnumerator AnimateButton()
	{
		cameraPointAudioSource.PlayOneShot(sfxUnpause, 1f);

		tempColor = currentButton.GetComponent<Renderer>().material.color;
		currentButton.GetComponent<Renderer>().material.color = clicked;
		yield return new WaitForSeconds(0.1f);
		currentButton.GetComponent<Renderer>().material.color = tempColor;
		currentButton = null;

		yield return null;
	}

	IEnumerator AnimateMenuButton()
	{
		cameraPointAudioSource.PlayOneShot(sfxUnpause, 1f);
		
		tempColor = currentButton.GetComponent<Renderer>().material.color;
		currentButton.GetComponent<Renderer>().material.color = clicked;
		yield return new WaitForSeconds(0.1f);
		currentButton.GetComponent<Renderer>().material.color = tempColor;
		currentButton = null;
		
		StartCoroutine(FadeBlack());
	}

	IEnumerator AnimatePauseButton()
	{
		cameraPointAudioSource.PlayOneShot(sfxUnpause, 1f);
		
		tempColor = currentButton.GetComponent<Renderer>().material.color;
		currentButton.GetComponent<Renderer>().material.color = clicked;
		yield return new WaitForSeconds(0.1f);
		currentButton.GetComponent<Renderer>().material.color = tempColor;
		currentButton = null;
		
		GlobalValues.currentScreen = 4;
		GlobalValues.changeScreen = true;
		UIScript.hidePiecesMenu = false;
	}
	
	IEnumerator YesButton()
	{
		cameraPointAudioSource.PlayOneShot(sfxUnpause, 1f);
		yesSprite.SetActive(false);
		bYesUp.SetActive(false);
		bYesDown.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		bYesUp.SetActive(true);
		bYesDown.SetActive(false);
		yesSprite.SetActive(true);

		if(settingsChoice == 1)
		{
			//CLEAR TIMES
			GlobalValues.gameMode = 1;
			normalDotYes.SetActive(true);
			timedDotYes.SetActive(false);
			DeletePlayerPrefs.clearTimes = true;
		}

		if(settingsChoice == 2)
		{
			//CLEAR DATA
			GlobalValues.gameMode = 1;
			normalDotYes.SetActive(true);
			timedDotYes.SetActive(false);
			DeletePlayerPrefs.clearData = true;
		}

		yield return new WaitForSeconds(0.01f);
		UpdateLevelButtons();

		yield return null;
	}

	IEnumerator NoButton()
	{
		cameraPointAudioSource.PlayOneShot(sfxUnpause, 1f);
		noSprite.SetActive(false);
		bNoUp.SetActive(false);
		bNoDown.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		bNoUp.SetActive(true);
		bNoDown.SetActive(false);
		noSprite.SetActive(true);

		popupConfirm.SetActive(false);

		yield return null;
	}

	IEnumerator TutMessages()
	{
		tutStarted = true;

		popupTut1.SetActive(true);
		popupTut2.SetActive(false);

		yield return new WaitForSeconds(8f);

		popupTut1.SetActive(false);
		popupTut2.SetActive(true);

		yield return new WaitForSeconds(5f);

		popupTut1.SetActive(false);
		popupTut2.SetActive(false);

		yield return null;
	}
}

