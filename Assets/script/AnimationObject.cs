using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationObject : MonoBehaviour {

	int count = 0;
	private Sprite[] waterfall;
	private SpriteRenderer waterfallRO;
    public string imgPath;
    public string objPath;
    private int length;
	void Start () {
		waterfall = Resources.LoadAll<Sprite>(imgPath);
        length = waterfall.Length;
		waterfallRO = GameObject.Find(objPath).GetComponent<SpriteRenderer>();
	}
	
	public void UpdateShape() {
		count = (count + 1) % length;
		waterfallRO.sprite = waterfall[count];
	}
}
