using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour {
	public enum TriggerType {
		WaterFall,
		Ladder,
	};
	public TriggerType type;
    public bool isEnter = false;
	public float moveLength;
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