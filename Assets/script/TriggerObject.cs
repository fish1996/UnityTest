using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour {
	public enum TriggerType {
		WaterFall,
		Ladder,
        DieRegion,
        Ground,
	};
	public TriggerType type;
    public bool isEnter = false;
	public float moveLength;
    public bool isStart = false;
    public float beginy;
    public List<MySprite> playerList = new List<MySprite>();

    public float getBottom() {
        return transform.position.y - transform.localScale.y/2;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
		isEnter = true;
		if (collision.gameObject.CompareTag ("Player")) {
			playerList.Add (collision.gameObject.GetComponent<MySprite> ());
			Debug.Log ("Enter " + playerList.Count);
		}
    }

    private void OnTriggerExit2D(Collider2D collision) {
        isEnter = false;
		if (collision.gameObject.CompareTag ("Player")) {
			playerList.Remove (collision.gameObject.GetComponent<MySprite> ());
			Debug.Log ("Leave " + playerList.Count);
		}
    }
}