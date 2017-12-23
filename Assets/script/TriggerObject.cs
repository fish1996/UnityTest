using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour {
    public bool isEnter = false;
	Collider2D player;
	void Start(){
		player = GameObject.Find ("Sprite").GetComponent<Collider2D> ();
	}
    private void OnTriggerEnter2D(Collider2D collision) {
		if (collision == player) {
			isEnter = true;
		}
    }

    private void OnTriggerExit2D(Collider2D collision) {
		if (collision == player) {
			isEnter = false;
		}
    }
}