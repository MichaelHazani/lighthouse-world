using UnityEngine;
using System.Collections;

public class playNocturneAtNight : MonoBehaviour {
	private AudioSource audio;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
		int time = System.Int32.Parse(System.DateTime.Now.ToString("%H%mm"));
		Debug.Log (time);
		if (time > 2000) {
			audio.Play();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
