using UnityEngine;
using System.Collections;

public class setTime : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var script = GameObject.Find("InstantGoodDay").GetComponent<InstantGoodDay>();
		script.SetMilitaryHour(System.DateTime.Now.ToString ("HH:mm"));

	}
}