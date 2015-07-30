using UnityEngine;
using System.Collections;

public class disableCursor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	Cursor.visible = false;

	}

	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown (KeyCode.F)) {
			Screen.fullScreen = !Screen.fullScreen;
		}
	}
}
