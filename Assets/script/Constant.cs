using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Dir : int { LEFT, RIGHT, FRONT, BACK };
public enum Region : int { WATER, LADDER, NORMAL };
public class Value {
    static public Vector2 v0_right = new Vector2(0.3f, 0.0f);
    static public Vector2 v0_left = new Vector2(-0.3f, 0.0f);
    static public Vector2 v0_back = new Vector2(0.0f, 0.3f);
    static public Vector2 v0_front = new Vector2(0.0f, 5.0f);
    static public Vector2[] revivePosTbl = {
        new Vector2(-6.24492f,1.215286f),
    };
    static public Vector3 cornerPos_left = Camera.main.ViewportToWorldPoint(new Vector3(0.3f, 0, 0));
    static public Vector3 cornerPos_right = Camera.main.ViewportToWorldPoint(new Vector3(0.7f, 0, 0));
    static public Vector3 cornerPos_up = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 0));
    static public Vector3 cornerPos_down = Camera.main.ViewportToWorldPoint(new Vector3(0,0.3f, 0));
};
