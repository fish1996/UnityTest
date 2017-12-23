using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationObject : MonoBehaviour {

	int count = 0;
	private Sprite[] waterfall;
	private SpriteRenderer waterfallRO;

	void Start () {
		waterfall = Resources.LoadAll<Sprite>("image/Waterfall");
		waterfallRO = GameObject.Find("Waterfall").GetComponent<SpriteRenderer>();
	}
	
	public void UpdateShape() {
		count = (count + 1) % 3;
		waterfallRO.sprite = waterfall[count];
	}
}
