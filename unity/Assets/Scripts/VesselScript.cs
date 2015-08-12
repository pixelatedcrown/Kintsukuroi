// The following script was written by Chelsea Saunders/pixelatedcrown - provided for non-commercial use only
	
// VesselScript deals with the vessels, namely during gameplay as well as gameplay itself
// Pretty much anything to do with the vessels is handled here

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class VesselScript : MonoBehaviour {

	// public
	public static bool newStartingVessel, newVessel, clearVessel, dragging = false, vesselIsMoving;
	public static int currentSlotCount;
	public List<int> tetrisRot; // 1 -> 0 / 2 - > 90 / 3 -> 180 / 4 -> 270
	public List<Vector3> explosionDirections;
	public List<Transform> pieceMenuSlots;
	public GameObject currentVessel, currentShadow, touchPlane, bobPoint, depthSliderMenu, emptyPrefab, 
	titleEffects, titleVessels, cameraPoint, levelBG;
	public List<GameObject> allVessels, allShadows, allTitleVessels, allVesselsNS, allShadowsNS, currentPieces, 
	currentGoldPieces, empties, titleVesselPieces, shelvedPieces, placedPieces;
	public AudioSource gameAudiosource;
	public AudioClip ding, sfxDrag, sfxRotate, sfxPutBack, sfxSlide;
	public Animator bobPointAnimator;
	public Text levelText, pieceText;
	public Light directionalLight;
	public Camera mainCam;
	public Material mainMat, rimMat, shadowMat, titleShadowMat;
	public Plane plane;

	// private
	private bool wasRotating, hasDung, canPlace, bobCheck, onShadow, startExplodeTimer, explodePieces, 
	startPickTitleScreenVessels = true, startCycle, isCycling, endCycle, startTouchTimer, startCanPlaceCountdown,
	freezeTouch, tapped, piecesPicked = false;
	private int totalPieces, freePieces, fingerCount = 0, currentPiecesLeft, touchesNeeded, tetrisAdd, random,
	cycleIndex, currentSlotNumber;
	private float rotationRate = 0.5f, dragPosZ, shadowAlpha, dist, explodeTimer, touchTime, tapLength = 0.8f, 
	offset = 1f, canPlaceTimer, distanceToPlace;
	private Vector3 dragPos, placeablePiecePos, placeablePieceRot;
	private GameObject dragged, draggedSlot, placeablePiece, touched, currentTitleVessel;
	private AudioSource audioSource;
	
	void Start ()
	{
		// prepares vessel and shadow models
		// basicaly does a bunch of stuff I'd otherwise have to do by hand in the inspector at game start instead
		for (int i = 0; i < allVesselsNS.Count; i++)
		{
			// for each vessel and shadow, instantiate one off screen
			// add these instantiated versions to the allVessels and allShadows lists

			// it might seem a bit odd to be creating instances like this but there are things later on that can't
			// be edited unless they're copies

			GameObject item = (GameObject)UnityEngine.Object.Instantiate
				(allVesselsNS[i].gameObject, new Vector3(100f, 100f, 100f), allVesselsNS[i].transform.rotation);
			allVessels.Add(item);

			GameObject item2 = (GameObject)UnityEngine.Object.Instantiate
				(allShadowsNS[i].gameObject, new Vector3(100f, 100f, 100f), allShadowsNS[i].transform.rotation);
			allShadows.Add(item2);
		}
		for (int j = 0; j < allVessels.Count; j++)
		{
			// tag each vessel as "vessel", make it inactive
			allVessels[j].tag = "vessel";
			allVessels[j].SetActive(false);
	
			foreach (Transform transform in allVessels[j].transform)
			{
				// for the vessels...
				// add a rigidbody without gravity and freeze all constraints, add to the Ignore Raycast layer
				Rigidbody rigidbody = transform.gameObject.AddComponent<Rigidbody>();
				rigidbody.useGravity = false;
				rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				transform.gameObject.layer = 2;
			}
		}
		for (int k = 0; k < allShadows.Count; k++)
		{
			// for the shadows...
			// add a mesh collider, tag as "shadow" and give them the shadowMat material
			allShadows[k].AddComponent<MeshCollider>();
			allShadows[k].tag = "shadow";
			allShadows[k].GetComponent<Renderer>().material = shadowMat;
		}
	}

	void Update ()
	{
		// timer for duration of outwards explosion of title pieces
		if (startExplodeTimer == true){explodeTimer += Time.deltaTime;}
		else{explodeTimer = 0f;}

		// if is during the title screen...
		if (GlobalValues.currentScreen == 1)
		{
			// enable title effects, rotate titleVessels and effects
			titleEffects.SetActive(true);
			titleVessels.transform.Rotate(0f, 5f * Time.deltaTime, 0f);

			// explodes title pieces outwards for 40 seconds
			// limits it to avoid them just floating away off screen forever
			if (explodePieces == true)
			{
				for (int i = 0; i < titleVesselPieces.Count; i++)
				{
					if (explodeTimer <= 40f)
					{
						startExplodeTimer = true;
						titleVesselPieces[i].transform.Translate(-explosionDirections[i] * (Time.deltaTime * 0.1f));
					}
				}
			}
			// start coroutine to load in title vessels
			if (startPickTitleScreenVessels == true)
			{
				StartCoroutine(PickTitleScreenVessels());
				startPickTitleScreenVessels = false;
			}
		}

		// else if is any screen but the title screen...
		else
		{
			// end the title vessel cycle and pick new title screen vessels for the next time
			endCycle = true;
			startPickTitleScreenVessels = true;
		}

		// start/stop coroutines instantly
		// doing it like this saved a LOT of hassle with coroutines carrying on while in the middle of yield wait pauses
		if (startCycle == true)
		{
			if (isCycling == false)
			{
				StartCoroutine("CycleTitleScreenVessels");
			}
			startCycle = false;
		}
		if (endCycle == true)
		{
			StopCoroutine("CycleTitleScreenVessels");
			currentTitleVessel = null;

			// reset some variables
			cycleIndex = 0;
			startExplodeTimer = false;
			explodeTimer = 0f;

			// destroy title vessels
			foreach (Transform transform in titleVessels.transform)
			{
				Destroy(transform.gameObject);
			}

			// clear lists
			allTitleVessels.Clear();
			titleVesselPieces.Clear();
			explosionDirections.Clear();

			// reset some more variables
			explodePieces = false;
			isCycling = false;
			endCycle = false;
		}
		if(clearVessel == true)
		{
			ClearVessel();
			clearVessel = false;
		}

		// if the current screen is the gameplay screen...
		if (GlobalValues.currentScreen == 4)
		{
			// disable the title effects
			titleEffects.SetActive(false);
			// set the currentSlotCount to be equal to the number of shelvedPieces
			currentSlotCount = shelvedPieces.Count;

			if(newStartingVessel == true)
			{
				StartCoroutine(StartingVessel());
				newStartingVessel = false;
			}

			// timer for counting if touch is a tap or a touch
			if (startTouchTimer == true){touchTime += Time.deltaTime;}
			else{touchTime = 0f;}

			// timer for delay on piece being placeable
			// this is so if a piece is in place but the finger moves slightly when letting go, a piece still places
			if (startCanPlaceCountdown == true){canPlaceTimer += Time.deltaTime;}
			if (canPlaceTimer >= 0.5f)
			{
				canPlace = false;
				startCanPlaceCountdown = false;
				canPlaceTimer = 0f;
			}
	
			///////////// NEW VESSEL
			// all the stuff for loading in new vessels

			// prepares the scene for a new vessel
			// clears lists, removes previous pieces, repopulates lists, ends with calling PickRandomPieces coroutine
			if(newVessel == true)
			{
				UIScript.resetSlider = true;
				currentPiecesLeft = 0;
				Destroy(currentVessel); Destroy(currentShadow);

				currentVessel = (GameObject)UnityEngine.Object.Instantiate(allVessels[GlobalValues.currentLevel], transform.position, bobPoint.transform.rotation);
				currentVessel.SetActive(false);
				currentShadow = (GameObject)UnityEngine.Object.Instantiate(allShadows[GlobalValues.currentLevel], bobPoint.transform.position, bobPoint.transform.rotation);
				foreach (Transform transform2 in currentVessel.transform)
				{
					transform2.GetComponent<Renderer>().material = mainMat;
				}

				totalPieces = 0;
				currentSlotNumber = 0;
				currentPieces.Clear();
				empties.Clear();
				currentGoldPieces.Clear();
				shelvedPieces.Clear();
				placedPieces.Clear();
				tetrisRot.Clear();
				touched = null;
				tapped = false;

				foreach (Transform current in pieceMenuSlots)
				{
					foreach (Transform transform3 in current)
					{
						Destroy(transform3.gameObject);
					}
				}

				piecesPicked = false;
				totalPieces = currentVessel.transform.childCount;
				freePieces = totalPieces - 1;

				int num = 0;
				foreach (Transform transform4 in currentVessel.transform)
				{
					currentPieces.Add(transform4.gameObject);
					transform4.gameObject.tag = "placed";
					foreach (Transform transform5 in currentPieces[num].transform)
					{
						currentGoldPieces.Add(transform5.gameObject);
						num++;
					}
				}
				StartCoroutine(PickRandomPieces());
				VesselScript.newVessel = false;
			}
			///////////// NEW VESSEL ^

			///////////// DURING MAIN GAMEPLAY
			// when a vessel isn't entering/leaving and pieces have been picked

			if (piecesPicked == true)
			{
				///////////// TOUCH STUFF
				// deals with... touch stuff. when there's a touch, what happens?

				if (freezeTouch == false)
				{
					fingerCount = Input.touchCount;
					if (fingerCount > 0)
					{
						Touch touch = Input.touches[0];

						// if is during a touch's first frame...
						if (touch.phase == TouchPhase.Began && !VesselScript.vesselIsMoving && fingerCount == 1)
						{
							// record the position
							Vector3 position = touch.position;
							// send a ray out to the position
							Ray ray = Camera.main.ScreenPointToRay(position);
							// if the ray hits something tagged as either "slot" or "shelved"...
							RaycastHit raycastHit;
							if (Physics.Raycast(ray, out raycastHit) && (raycastHit.transform.gameObject.tag == "slot" || raycastHit.transform.gameObject.tag == "shelved"))
							{
								// set the hit object as touched, start the touchTimer
								touched = raycastHit.transform.gameObject;
								startTouchTimer = true;
							}
						}

						// if is during the touch when it isn't moving and within the time to be considered a tap
						// and a piece isn't currently already being dragged around
						if (touch.phase == TouchPhase.Stationary && touched != null && touchTime < tapLength && !VesselScript.dragging)
						{
							// record that a tap has taken place
							tapped = true;
						}

						// if is during the touch when moving and it's been confirmed something touchable has been touched
						if (touch.phase == TouchPhase.Moved)
						{
							if (touched != null)
							{
								// touched objects can either be the slot the piece is in (slot)
								// or the piece itself (shelved)

								// if the touch is a slot...
								if (touched.tag == "slot")
								{
									// gets the piece parented to the slot, sets it to dragged
									foreach (Transform transform6 in touched.transform)
									{
										if (transform6 != null)
										{
											// records the number of the slot that the piece is taken from
											for (int j = 0; j < pieceMenuSlots.Count; j++)
											{
												if (touched.gameObject == pieceMenuSlots[j].gameObject)
												{
													currentSlotNumber = j;
												}
											}
											draggedSlot = transform6.parent.gameObject;
											transform6.parent = currentVessel.transform;
											transform6.gameObject.layer = 0;
											transform6.gameObject.tag = "dragged";
											dragged = transform6.gameObject;
											// changes the layer to one that renders over everything else
											dragged.layer = 9;
											dragged.transform.localScale = new Vector3(1f, 1f, 1f);

											// play a sound, set as dragging and set tapped to false
											gameAudiosource.PlayOneShot(sfxDrag, 1f);
											VesselScript.dragging = true;
											tapped = false;
										}
									}
								}

								// if the touch is a shelved piece...
								if (touched.tag == "shelved")
								{
									// basically the same as if it were a slot but without having to get the child piece
									draggedSlot = touched.transform.parent.gameObject;
									for (int k = 0; k < pieceMenuSlots.Count; k++)
									{
										if (draggedSlot == pieceMenuSlots[k].gameObject)
										{
											currentSlotNumber = k;
										}
									}
									touched.transform.parent = currentVessel.transform;
									touched.transform.gameObject.layer = 0;
									touched.transform.gameObject.tag = "dragged";
									dragged = touched.transform.gameObject;
									dragged.layer = 2;
									dragged.transform.localScale = new Vector3(1f, 1f, 1f);
									gameAudiosource.PlayOneShot(sfxDrag, 1f);
									VesselScript.dragging = true;
									tapped = false;
								}
							}

							// if a piece is being dragged...
							if (dragged != null)
							{
								///////////// DRAGGED PIECE POSITION
	
								// ensures the dragged piece updates its position under the moving finger touch
								Vector2 position2 = Input.GetTouch(0).position;
								plane.SetNormalAndPosition(Camera.main.transform.forward, Camera.main.transform.forward);
								Ray ray2 = mainCam.ScreenPointToRay(position2);
								float distance;
								plane.Raycast(ray2, out distance);
								Vector3 point = ray2.GetPoint(distance);
								point = new Vector3(point.x, point.y + offset, point.z);

								RaycastHit raycastHit2;
								if (Physics.Raycast(ray2, out raycastHit2))
								{
									// if the touch is over the solid shadow of the whole object...
									if (raycastHit2.transform.tag == "shadow")
									{
										// set as being over the shadown, add an offset to the y
										onShadow = true;
										point = new Vector3(raycastHit2.point.x, raycastHit2.point.y + offset, raycastHit2.point.z);
									}
									if (raycastHit2.transform.tag == "placed")
									{
										// set as not being over the shadown, add an offset to the y
										onShadow = false;
										point = new Vector3(raycastHit2.point.x, raycastHit2.point.y + offset, raycastHit2.point.z);
									}
								}
								// updated the dragged piece's position to the previously set position "point"
								dragged.transform.position = point;

								///////////// DRAGGED PIECE POSITION ^

								///////////// HOLDING PIECES OVER CORRECT POSITION
								// if the dragged piece is within range of its correct position and held, play a sound and change the material
								// if the piece is moved from within range of its correct position, change back its material
								for (int l = 0; l < shelvedPieces.Count; l++)
								{
									if (onShadow && shelvedPieces[l] == dragged)
									{
										Vector3 position3 = dragged.transform.position;
										Vector3 position4 = empties[l].transform.position;
										position3.z = 0f;
										position4.z = 0f;
										distanceToPlace = Vector3.Distance(position3, position4);
										if (distanceToPlace < 0.3f && tetrisRot[l] == 1)
										{
											if (shelvedPieces[l] == dragged)
											{
												dragged.GetComponent<Renderer>().material = rimMat;
												if (!hasDung)
												{
													gameAudiosource.PlayOneShot(ding, 1f);
													hasDung = true;
												}
												canPlace = true;
												startCanPlaceCountdown = false;
												canPlaceTimer = 0f;
											}
										}
										else if (shelvedPieces[l] == dragged)
										{
											dragged.GetComponent<Renderer>().material = mainMat;
											placeablePiece = null;
											hasDung = false;
											startCanPlaceCountdown = true;
										}
									}
								}
								///////////// HOLDING PIECES OVER CORRECT POSITION ^
							}
						}

						// if the touch is ended or canceled...
						if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
						{
							if (tapped && touched != null)
							{
								for (int m = 0; m < shelvedPieces.Count; m++)
								{
									if (touched.tag == "slot")
									{
										foreach (Transform transform7 in touched.transform)
										{
											touched = transform7.gameObject;
										}
									}
									if (touched == shelvedPieces[m])
									{
										gameAudiosource.PlayOneShot(sfxRotate, 1f);
										List<int> list;
										List<int> expr_E15 = list = tetrisRot;
										int num2;
										int expr_E1A = num2 = m;
										num2 = list[num2];
										expr_E15[expr_E1A] = num2 - 1;
										if (tetrisRot[m] < 1)
										{
											tetrisRot[m] = 4;
										}
										tapped = false;
									}
								}
							}

							///////////// LETTING PIECES GO
							if (fingerCount == 1 && dragged != null)
							{
								// if the piece is let go and cannot be placed, return it to the shelf and restore
								if (!canPlace)
								{
									gameAudiosource.PlayOneShot(sfxPutBack, 1f);
									dragged.GetComponent<Renderer>().material = mainMat;
									dragged.transform.position = draggedSlot.transform.position;
									dragged.tag = "shelved";
									dragged.transform.parent = draggedSlot.transform;
									dragged.layer = 5;
									foreach (Transform transform8 in dragged.transform)
									{
										transform8.gameObject.SetActive(false);
									}
									VesselScript.dragging = false;
									dragged = null;
									placeablePiece = null;
									placeablePiecePos = Vector2.zero;
									placeablePieceRot = Vector3.zero;
									hasDung = false;
									canPlace = false;
								}
								// if the piece is let go and CAN be placed, start the EndDraggedTouch coroutine
								if (canPlace)
								{
									StartCoroutine(EndDraggedTouch());

									//
								}
							}
							///////////// LETTING PIECES GO ^

							touched = null;
							startTouchTimer = false;
							hasDung = false;
							canPlace = false;
						}
					}
				}
				///////////// TOUCH STUFF ^

				// if a piece is being dragged, set the placablePiece, the placeablePiecePos and the placeablePieceRot
				if (dragged)
				{
					for (int n = 0; n < shelvedPieces.Count; n++)
					{
						if (dragged == shelvedPieces[n].gameObject)
						{
							placeablePiece = dragged;
							placeablePiecePos = empties[n].transform.position;
							placeablePieceRot = empties[n].transform.eulerAngles;
						}
					}
				}

				// piece rotations (called tetris rotations since that's where the inspiration came from)
				for (int num3 = 0; num3 < shelvedPieces.Count; num3++)
				{
					if (tetrisRot[num3] == 1)
					{
						tetrisAdd = 0;
					}
					if (tetrisRot[num3] == 2)
					{
						tetrisAdd = 90;
					}
					if (tetrisRot[num3] == 3)
					{
						tetrisAdd = 180;
					}
					if (tetrisRot[num3] == 4)
					{
						tetrisAdd = 270;
					}
					shelvedPieces[num3].transform.eulerAngles = new Vector3(empties[num3].transform.eulerAngles.x, empties[num3].transform.eulerAngles.y, (float)tetrisAdd);
				}
			}
			///////////// DURING MAIN GAMEPLAY ^

			///////////// ENDING A LEVEL/THE GAME
			if (!bobCheck && placedPieces.Count == totalPieces - 1)
			{
				// if not the final level, progress a level, record time and load in a new vessel
				if (GlobalValues.currentLevel <= GlobalValues.totalLevelCount - 1)
				{
					GlobalValues.currentLevel++;
					if (GlobalValues.currentLevel == GlobalValues.currentMaxLevel + 1)
					{
						GlobalValues.currentMaxLevel++;
					}
					GameModes.recordTime = true;
					base.StartCoroutine(NextVessel());
					bobCheck = true;
				}
				// if final level, set it as so and force close to level screen
				else
				{
					ClearVessel();
					GlobalValues.mainLevelsCompleted = 1;
					UIScript.forceClose = true;
					touched = null;
					tapped = false;
				}
			}
			///////////// ENDING A LEVEL/THE GAME ^
		}
	}
	public void FixedUpdate()
	{
		// rotate the vessel in the gameplay view with left and right touch motions
		if(GlobalValues.currentScreen == 4)
		{
			if ((freezeTouch == false) && (vesselIsMoving == false) && (Input.touchCount > 0))
			{
				Touch touch = Input.GetTouch(0);
				Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
				RaycastHit raycastHit;
				if (dragged == null && Input.touchCount == 1 && Physics.Raycast(ray, out raycastHit) && raycastHit.transform.gameObject.layer != 5)
				{
					if (touch.phase == TouchPhase.Began)
					{
						wasRotating = false;
					}
					if (touch.phase == TouchPhase.Moved)
					{
						currentVessel.transform.Rotate(0f, touch.deltaPosition.x * -rotationRate, 0f, Space.World);
						currentShadow.transform.rotation = currentVessel.transform.rotation;
						wasRotating = true;
					}
				}
			}
		}
	}
	
	public void ClearVessel()
	{
		// when called, remove a vessel from the scene and reset variables
		if (currentVessel != null)
		{
			currentVessel.SetActive(false);
			UnityEngine.Object.Destroy(currentVessel);
			UnityEngine.Object.Destroy(currentShadow);
			currentPiecesLeft = 0;

			currentSlotNumber = 0;

			totalPieces = 0;
			currentPieces.Clear();
			empties.Clear();
			currentGoldPieces.Clear();
			shelvedPieces.Clear();
			placedPieces.Clear();
			tetrisRot.Clear();

			touched = null;
			tapped = false;

			foreach (Transform current in pieceMenuSlots)
			{
				foreach (Transform transform in current)
				{
					Destroy(transform.gameObject);
				}
			}
			piecesPicked = false;

			// exit animation
			bobPointAnimator.SetBool("none", false);
			bobPointAnimator.SetBool("snapOut", true);
			bobPointAnimator.SetBool("in", false);
			bobPointAnimator.SetBool("out", false);
		}
		
	}
	
	IEnumerator StartingVessel()
	{
		// when called, load in the FIRST vessel, deals with animating it too

		freezeTouch = true;
		vesselIsMoving = true;
		piecesPicked = false;
		
		bobPointAnimator.SetBool("snapOut", true);
		newVessel = true;

		StartCoroutine(FadeNewLevelBG());
		yield return new WaitForSeconds(1f);
		
		shadowAlpha = currentShadow.transform.GetComponent<Renderer>().material.color.a;
		shadowAlpha = 0;
		
		currentVessel.transform.position = bobPoint.transform.position;
		currentVessel.transform.rotation = bobPoint.transform.rotation;
		
		currentShadow.transform.position = bobPoint.transform.position;
		currentShadow.transform.rotation = bobPoint.transform.rotation;
		
		currentVessel.SetActive(true);
		
		currentVessel.transform.parent = bobPoint.transform;
		currentShadow.transform.parent = bobPoint.transform;
		
		currentVessel.SetActive(true); currentShadow.SetActive(true);
		
		bobPointAnimator.SetBool("snapOut", false);
		bobPointAnimator.SetBool("out", false);
		bobPointAnimator.SetBool("in", true);
		
		yield return new WaitForSeconds(2f);
		
		shadowAlpha = currentShadow.transform.GetComponent<Renderer>().material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(shadowAlpha, 0.1f, t));
			currentShadow.transform.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}
		
		yield return new WaitForSeconds(1f);
		
		currentVessel.transform.parent = null;
		currentShadow.transform.parent = null;
		
		bobPointAnimator.SetBool("in", false);
		
		bobPointAnimator.SetBool("none", true);
		
		currentVessel.transform.eulerAngles = Vector2.zero;
		currentShadow.transform.eulerAngles = Vector2.zero;
		bobCheck = false;
		
		vesselIsMoving = false;
		freezeTouch = false;

		yield return null;
	}
	
	IEnumerator NextVessel()
	{
		// when called, load in the NEXT vessel, deals with animating it too

		freezeTouch = true;
		MusicScript.changeToSolution = true;
		vesselIsMoving = true;
		
		shadowAlpha = currentShadow.transform.GetComponent<Renderer>().material.color.a;
		shadowAlpha = 0;
		
		piecesPicked = false;
		
		Destroy(currentShadow.gameObject);
		
		currentVessel.transform.parent = bobPoint.transform;
		
		bobPointAnimator.SetBool("none", false); bobPointAnimator.SetBool("in", false);
		bobPointAnimator.SetBool("out", true);
		
		yield return new WaitForSeconds(5f);

		StartCoroutine(FadeNewLevelBG());
		currentVessel.transform.parent = null;

		newVessel = true;
		MusicScript.changeFromSolution = true;
		GameModes.clearTime = true;
		yield return new WaitForSeconds(1f);

		currentVessel.transform.position = bobPoint.transform.position;
		currentVessel.transform.rotation = bobPoint.transform.rotation;
		
		currentVessel.SetActive(true);
		
		currentVessel.transform.parent = bobPoint.transform;
		currentShadow.transform.parent = bobPoint.transform;
		
		currentVessel.SetActive(true); currentShadow.SetActive(true);
		
		bobPointAnimator.SetBool("out", false);
		bobPointAnimator.SetBool("in", true);
		
		yield return new WaitForSeconds(2f);
		
		shadowAlpha = currentShadow.transform.GetComponent<Renderer>().material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(shadowAlpha, 0.1f, t));
			currentShadow.transform.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}

		yield return new WaitForSeconds(1f);
		
		currentVessel.transform.parent = null;
		currentShadow.transform.parent = null;
		
		bobPointAnimator.SetBool("in", false);
		bobPointAnimator.SetBool("none", true);
		
		currentVessel.transform.eulerAngles = Vector2.zero;
		currentShadow.transform.eulerAngles = Vector2.zero;
		bobCheck = false;
		
		vesselIsMoving = false;
		freezeTouch = false;

		yield return null;
	}
	
	
	///////////// PICK RANDOM VESSEL PIECES
	/// picks random pieces and adds them to the shelved pieces menu, hides gold
	IEnumerator PickRandomPieces()
	{
		// repeat until the count of shelvedPieces is equal to freePieces...
		while(shelvedPieces.Count != freePieces)
		{
			// pick a random number between 0 and ...
			random = Random.Range(0, currentPieces.Count);

			GameObject emptyClone = Instantiate(emptyPrefab, 
			                                    currentPieces[random].transform.position, 
			                                    currentPieces[random].transform.rotation) as GameObject;
			
			emptyClone.transform.parent = currentPieces[random].transform.parent;
			empties.Add(emptyClone);
			
			
			// add this piece to shelvedPieces (if already picked, will repeat due to while loop)
			shelvedPieces.Add (currentPieces[random]);
			// tag the piece as "shelved"
			currentPieces[random].tag = "shelved";

			// remove this shelved piece from currentPieces
			currentPieces.Remove(currentPieces[random]);
		}
		
		piecesPicked = true;
		
		// for the amount of pieces in shelvedPieces...
		for(int b = 0; b < shelvedPieces.Count; b++)
		{
			currentPiecesLeft++;
			pieceText.text = "Pieces: " + currentPiecesLeft;
			// move the piece to the position of a slot
			shelvedPieces[b].transform.position =
				new Vector3(pieceMenuSlots[b].position.x, pieceMenuSlots[b].position.y, pieceMenuSlots[b].position.z);
			shelvedPieces[b].transform.localScale = new Vector3(1f, 1f, 1f);
			// parent the piece to the slot
			shelvedPieces[b].transform.parent = pieceMenuSlots[b];
			// change the piece's layer to UI (5)
			shelvedPieces[b].layer = 5;


			tetrisAdd = Random.Range(1, 4);
			tetrisRot.Add(tetrisAdd);
			
			// hide the gold of the shelved pieces
			foreach (Transform goldChild in shelvedPieces[b].transform)
			{
				goldChild.gameObject.SetActive(false);
			}
		}
		piecesPicked = true;

		yield return null;
	}
	///////////// PICK RANDOM VESSEL PIECES ^

	private IEnumerator EndDraggedTouch()
	{
		freezeTouch = true;
		// move the dragged piece to its correct position, rotation and fix scale
		dragged.transform.position = placeablePiecePos;
		dragged.transform.eulerAngles = placeablePieceRot;
		dragged.transform.localScale = new Vector3(1, 1, 1);

		int slide = 0;
		for (int a = 0; a < shelvedPieces.Count; a++ )
		{
			if(a > currentSlotNumber)
			{
				shelvedPieces[a].transform.parent = null;
				slide++;

				if(slide <= 3)
				{
					gameAudiosource.PlayOneShot(sfxSlide, 1f);
					Vector3 piecePos = shelvedPieces[a].transform.position;
					for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.1f)
					{
						Vector3 newPos = pieceMenuSlots[a-1].transform.position;
						shelvedPieces[a].transform.position = newPos;
						yield return null;
					}
				}
				shelvedPieces[a].transform.position = pieceMenuSlots[a-1].transform.position;
				shelvedPieces[a].transform.parent = pieceMenuSlots[a-1].transform;
			}
		}

		// remove things from lists: dragged piece from shelvedPieces, dragged piece's tetris rot, empities pos
		for(int b = 0; b < shelvedPieces.Count; b++)
		{
			if(shelvedPieces[b] == dragged)
			{
				tetrisRot.Remove(tetrisRot[b]);
				shelvedPieces.Remove(shelvedPieces[b]);
				empties.Remove(empties[b]);
			}
		}

		// restore the dragged piece's material
		dragged.GetComponent<Renderer>().material = mainMat;
		
		// tag dragged piece as "placed"
		dragged.tag = "placed";
		dragged.layer = 0;

		// show the hidden gold (woah)
		foreach (Transform goldChild in dragged.transform)
		{
			goldChild.gameObject.SetActive(true);
		}

		// add to placedPieces list
		placedPieces.Add(dragged);

		VesselScript.dragging = false;
		dragged = null;
		placeablePiece = null;
		placeablePiecePos = Vector2.zero;
		placeablePieceRot = Vector3.zero;
		hasDung = false;
		canPlace = false;
		freezeTouch = false;

		yield return null;
	}

	private IEnumerator PickTitleScreenVessels()
	{
		// when called, picks vessels to use on the title screen

		titleVessels.transform.eulerAngles = Vector3.zero;
		explosionDirections.Clear();

		for (int b = 0; b < allVessels.Count; b++ )
		{
			// load in title vessels (instantiate, group, add)
			GameObject titleVessel = (GameObject)Instantiate(allVessels[b].gameObject, 
			                                                 titleVessels.transform.position, 
			                                                 titleVessels.transform.rotation);
			titleVessel.transform.parent = titleVessels.transform;
			allTitleVessels.Add(titleVessel);

			foreach(Transform vesselPiece in allTitleVessels[b].transform)
			{
				vesselPiece.GetComponent<Renderer>().material = mainMat;
			}

			// group vesselPieces, hide gold
			foreach(Transform vesselPiece in titleVessel.transform)
			{
				titleVesselPieces.Add(vesselPiece.gameObject);

				foreach(Transform gold in vesselPiece.transform)
				{
					gold.gameObject.SetActive(false);
				}
			}
		}

		for(int a = 0; a < titleVesselPieces.Count; a++)
		{
			Vector3 fromPosition = titleVesselPieces[a].transform.position;
			Vector3 toPosition = titleVessels.transform.position;
			Vector3 direction = toPosition - fromPosition;
			direction.Normalize();
			explosionDirections.Add(direction);
		}

		// shuffle title vessel
		for (int t = 0; t < allVessels.Count; t++ )
		{
			GameObject tmp = allTitleVessels[t];
			int r = Random.Range(t, allTitleVessels.Count);
			allTitleVessels[t] = allTitleVessels[r];
			allTitleVessels[r] = tmp;
		}
		explodePieces = true;
		startCycle = true;

		yield return null;
	}

	private IEnumerator CycleTitleScreenVessels()
	{
		// deals with cycling through the chosen title screen vessels
		currentTitleVessel = null;
		cycleIndex++;

		if(cycleIndex > 20){cycleIndex = 0;}

		currentTitleVessel = allTitleVessels[cycleIndex];
		titleEffects.transform.rotation = Random.rotation;
		currentTitleVessel.transform.rotation = Random.rotation;
		currentTitleVessel.SetActive(true);

		yield return new WaitForSeconds(7.5f);

		if(currentTitleVessel != null)
		{
			currentTitleVessel.SetActive(false);
			StartCoroutine(CycleTitleScreenVessels());
		}
		yield return null;
	}

	private IEnumerator FadeNewLevelBG()
	{
		// called to fade in the screen that appears before levels
		levelText.text = "Level: " + GlobalValues.currentLevel;

		levelBG.SetActive(true);
		float fadeAlpha = levelBG.GetComponent<Renderer>().material.color.a;

		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(fadeAlpha, 0.5f, t));
			levelBG.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}
		levelText.enabled = true;
		yield return new WaitForSeconds(3f);

		StartCoroutine(FadeOutNewLevelBG());
		yield return null;
	}

	private IEnumerator FadeOutNewLevelBG()
	{
		// called to fade out the screen that appears before levels
		levelText.enabled = false;
		float fadeAlpha = levelBG.GetComponent<Renderer>().material.color.a;
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
		{
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(fadeAlpha, 0f, t));
			levelBG.GetComponent<Renderer>().material.color = newColor;
			yield return null;
		}
		levelBG.SetActive(false);
		yield return null;
	}
}