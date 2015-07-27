using UnityEngine;
using System.Collections;


public class turnOn : MonoBehaviour {
public GameObject lightParent;
	private AudioSource audio;
	private bool isLight;
	// Use this for initialization
	void Start () {
	audio = GameObject.FindWithTag("lever").GetComponent<AudioSource>();
	isLight = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Test")){
			if (!isLight) {
				isLight = true;
				var parentLight = GameObject.FindWithTag("allLights");
				parentLight.GetComponent<RotateLight>().enabled = true;

				var lights = lightParent.GetComponentsInChildren<Light>(true);
				
				foreach (Light light in lights)
				{
					light.enabled = true;
				}
				audio.Play();

			} else {

				isLight = false;
				var parentLight = GameObject.FindWithTag("allLights");
				parentLight.GetComponent<RotateLight>().enabled = false;
				
				var lights = lightParent.GetComponentsInChildren<Light>(true);
				
				foreach (Light light in lights)
				{
					light.enabled = false;
				}
				audio.Play();
			}
		}
	}
}

