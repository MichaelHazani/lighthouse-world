/**
 * This file and its contents are confidential and intended solely for the 
 * use of Amable Rodríguez or outside parties permitted to view this file and its
 * contents per agreement between Amable Rodríguez and said parties.  
 * Unauthorized publication, use, dissemination, forwarding, printing or 
 * copying of this file and its contents is strictly prohibited.
 *
 * Copyright © 2015 Amable Rodríguez | blexbox Interactive. 
 * All Rights Reserved 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ui : MonoBehaviour
{
	// script code examples here.
	//
	// this script is responsable for displaying the ui, but in particular we will
	// manipulate the InstantGoodDay component in order to get the current time of the day
	// to show it on the bottom-left corner of the screen.

	void Awake()
	{
		// first, get a reference to the InstantGoodDay prefab in the scene, like this:
		_instantGoodDay = GameObject.FindObjectOfType<InstantGoodDay>();
	}

	void Update ()
	{
		// then we fill the _timeOfDay class member variable with the string representation
		// of the current time since later on its content is what is shown on the screen,
		// so this line gives us the current time:
		_timeOfDay = _instantGoodDay.GetMilitaryHour();
	}

	/**
	 * (this method is never called since it is only used to hold script code examples)
	 */
	void Example()
	{
		// Here you will see all the examples about how to manipulate all the 5
		// editor properties:

		// 1. we can set and get the RenderCamera property:
		Camera camera = gameObject.AddComponent<Camera>();
		_instantGoodDay.SetRenderCamera(camera);
		// and
		camera = _instantGoodDay.GetRenderCamera();
		// note: setting a new Camera will automatically adjust the prefab position and scale
		// according to the Camera location and ClippingPlanesFar properties


		// 2. we can set and get the hour of the day in one of either this two ways:

		// a) using the numeric format with a float value between 0 to 23.99f
		float numericTimeOfDay = 15.3f;
		_instantGoodDay.SetNumericHour(numericTimeOfDay);
		// and
		numericTimeOfDay = _instantGoodDay.GetNumericHour();

		// b) using the text time with HH:MM format
		string stringTimeOfDay = "08:50";
		_instantGoodDay.SetMilitaryHour(stringTimeOfDay);
		// and
		stringTimeOfDay = _instantGoodDay.GetMilitaryHour();


		// 3. we can let the time pass or just stop it by using this methods:
		_instantGoodDay.PassTime();
		_instantGoodDay.StopTime();
		// note: even when the time is stopped, the clouds will keep moving on a natural way


		// 4. we can set and get the total duration of the day cycle in real time seconds:
		int dayDurationInSeconds = 300; // 5min
		_instantGoodDay.SetDayDurationInSeconds(dayDurationInSeconds);
		// and
		dayDurationInSeconds = _instantGoodDay.GetDayDurationInSeconds();


		// 5. we can get the custom animation list by using this method:
		List<GameObject> animList = _instantGoodDay.GetAdditionalDailyAnimationsList();
		// you can manipulate the list, for example you can add another animated object:
		GameObject myAnimatedObject = new GameObject();
		animList.Add(myAnimatedObject);
		// after adding a new item to the list, you have to call this method:
		_instantGoodDay.SyncDailyAnimations();
		// this way you will assure that all the daily animations will be in sync with the
		// main daily animation
	}

	// --------------------------------------------------------------------------------------- //

	void Start ()
	{
		InitUI();
	}

	void OnGUI ()
	{
		if (ShowDemoUI)
			DrawDemoUI();

		if (ShowDemoHelp)
			DrawDemoHelp();

		if (ShowScreenShotLogo)
			DrawScreenShotLogo();
	}

	private void InitUI()
	{
		_timeOfDay = string.Empty;

		_emptySpace = 10;
		_smallLogoSize = 30;
		_bigLogoSize = 100;

		_frameWidth = 160;
		_frameHeight = 40;

		_framePositionAndSize = new Rect(_emptySpace, Screen.height - (_emptySpace + _frameHeight), _frameWidth, _frameHeight);
		_title = ".     " + PRODUCT_NAME;

		_subtitlePositionAndSize = new Rect (_emptySpace * 2 + _smallLogoSize, Screen.height + _emptySpace - _frameHeight - 3,100,20);
		_subtitle = "v"+VERSION+" | ";

		_smallLogoPositionAndSize = new Rect (_emptySpace * 1.5f, Screen.height - (_emptySpace + _frameHeight) + (_frameHeight - _smallLogoSize) * 0.5f, _smallLogoSize, _smallLogoSize);
		_bigLogoPositionAndSize = new Rect (_emptySpace,Screen.height - (_emptySpace + _bigLogoSize * 1.1f),_bigLogoSize,_bigLogoSize);

		var helpSize = 430;
		_helpPositionAndSize = new Rect((Screen.width - helpSize)*0.5f,Screen.height - (_emptySpace + _frameHeight * 0.6f),helpSize,_frameHeight * 0.6f);
	}

	private void DrawDemoUI()
	{
		GUI.Box(_framePositionAndSize, _title);
		GUI.Label(_subtitlePositionAndSize, _subtitle + _timeOfDay);
		GUI.DrawTexture(_smallLogoPositionAndSize, Logo, ScaleMode.ScaleToFit);
	}

	private void DrawDemoHelp()
	{
		GUI.Box(_helpPositionAndSize, HELP);
	}

	private void DrawScreenShotLogo()
	{
		GUI.DrawTexture(_bigLogoPositionAndSize, Logo, ScaleMode.ScaleToFit);
	}

	public Texture Logo;
	public bool ShowDemoUI = true;
	public bool ShowDemoHelp = true;
	public bool ShowScreenShotLogo = false;
	
	private const string PRODUCT_NAME = "Instant Good Day";
	private const string VERSION = "1.1";
	private const string HELP = "(use A,S,D,W and Space keys to move, Mouse to control the camera)";
	
	private float _frameWidth;
	private float _frameHeight;
	private Rect _framePositionAndSize;
	
	private float _smallLogoSize;
	private Rect _smallLogoPositionAndSize;
	private float _bigLogoSize;
	private Rect _bigLogoPositionAndSize;
	private float _emptySpace;
	
	private string _title;
	private string _subtitle;
	private Rect _subtitlePositionAndSize;
	
	private Rect _helpPositionAndSize;
	
	private string _timeOfDay;
	private InstantGoodDay _instantGoodDay;
}
