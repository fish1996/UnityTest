using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteObject : MonoBehaviour {

    private MySprite mainSprite = new MySprite();
    private List<MySprite> spriteList = new List<MySprite>();

    private List<AnimationObject> animationList = new List<AnimationObject>();
    private Prefab prefab = Prefab.getInstance();
    private List<TriggerObject> triggerList = new List<TriggerObject>();
    

    void Awake() {

        // load animation
        foreach (string path in prefab.animationPath) {
            AnimationObject obj = GameObject.Find(path).GetComponent<AnimationObject>();
            animationList.Add(obj);
        }

        //load trigger
        foreach (string path in prefab.triggerPath) {
            TriggerObject obj = GameObject.Find(path).GetComponent<TriggerObject>();
            triggerList.Add(obj);
        }
        mainSprite.initialize("Sprite");
        spriteList.Add(mainSprite);
    }
    private void KeyControl() {
        // 鼠标按下
        if (Input.GetKeyDown(KeyCode.W)) {
            mainSprite.OnKeyDown_W();
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            mainSprite.OnKeyDown_S();
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            mainSprite.OnKeyDown_A();
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            mainSprite.OnKeyDown_D();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            mainSprite.OnKeyDown_Space();
        }


        // 鼠标抬起
        if (Input.GetKeyUp(KeyCode.W)) {
            mainSprite.OnKeyUp_W();
        }
        if (Input.GetKeyUp(KeyCode.S)) {
            mainSprite.OnKeyUp_S();
        }

        if (Input.GetKeyUp(KeyCode.A)) {
            mainSprite.OnKeyUp_A();
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            mainSprite.OnKeyUp_D();
        }
    }





    bool Equal(float v1, float v2) {
        float eps = 0.5f;
        Debug.Log(v1 - v2);
        return v1 - v2 < eps && v1 - v2 > -eps;
    }

    void CheckLeaveJump() {
        foreach(MySprite sprite in spriteList) {
            sprite.CheckLeaveJump();
        }
    }

    void CheckInRevivePos() {
        foreach (MySprite sprite in spriteList) {
            sprite.CheckInRevivePos();
        }
    }

    void CheckStopMoving() {
        foreach (MySprite sprite in spriteList) {
            sprite.CheckStopMoving();
        }
    }
    void Update() {
        CheckWithTrigger();
        CheckLeaveJump();
        CheckInRevivePos();
        CheckStopMoving();
        KeyControl();
    }


    void CheckWithTrigger() {
        foreach (TriggerObject triggerObj in triggerList) {
            if (triggerObj.isEnter) {
                foreach (MySprite player in triggerObj.playerList) {
                    player.CheckWithEnterTrigger(triggerObj);
                }
            }
        }

        foreach (MySprite player in spriteList) {
            player.CheckWithLeaveTrigger();
        }

    }

    void FixedUpdate() {
        mainSprite.UpdateCamera();
        UpdateTrigger();
        UpdateSprite();
        UpdateFrameAnimation();
    }


    private void UpdateSprite() {
        foreach (MySprite sprite in spriteList) {
            sprite.UpdateSprite();
        }
    }

    private void UpdateFrameAnimation() {
        foreach (AnimationObject obj in animationList) {
            obj.UpdateShape();
        }
    }

    private void UpdateTrigger() {
        foreach (TriggerObject triggerObj in triggerList) {
            if (triggerObj.isStart) {
                GameObject m_water = triggerObj.gameObject;
                Vector3 scale = m_water.transform.localScale;
                m_water.transform.localScale = new Vector3(scale.x, scale.y += 0.15f, scale.z);
                foreach(MySprite player in triggerObj.playerList) {
                    if (player.specialregion == Region.WATER) {
                        player.MoveBack(new Vector2(0, 0.1f));
                    }
                }
                if (m_water.transform.localScale.y >= triggerObj.moveLength + triggerObj.beginy) {
                    triggerObj.isStart = false;
                    m_water.transform.localScale = new Vector3(m_water.transform.localScale.x, triggerObj.beginy, m_water.transform.localScale.z);
                }
            }
        }
    }
}