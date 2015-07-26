#pragma strict

function Start () {
     var myListeners : AudioListener[] = FindObjectsOfType(AudioListener) as AudioListener[];
     for (var hidden : AudioListener in myListeners) {
        Debug.Log("Found:  " + hidden.gameObject);
     }
 }


function Update () {

}