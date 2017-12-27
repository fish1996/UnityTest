using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySprite : MonoBehaviour {
    public Sprite[] sprites;
    public int index = 1; // 当前指向的精灵
    public const int reviveNum = 1;
    public Dir dir = Dir.FRONT;
    public int bMoving = 0; // 0代表静止,1代表运动，2代表运动->静止
    public bool bInRevivePos = false;
    public bool bJumping = false;
    public bool bRemoveGravity = false;
    public bool bDelayRemoveGravity = false;
    public Region specialregion = Region.NORMAL;
    public Vector2 revivePos = new Vector2(-6.24492f, 3.215286f);
    private new GameObject camera;
    public bool bKeyDown = false;
    public TriggerObject currentTriggerObj;
    public enum Constant : int {
        LEFT_INDEX = 12,
        RIGHT_INDEX = 24,
        FRONT_INDEX = 0,
        BACK_INDEX = 36,
    };
    public SpriteRenderer spriteRO;
    public Rigidbody2D rigidBody;
    public float beginy;
    public void initialize(string path) {
        sprites = Resources.LoadAll<Sprite>("image/people");
        spriteRO = GameObject.Find(path).GetComponent<SpriteRenderer>();
        rigidBody = GameObject.Find(path).GetComponent<Rigidbody2D>();
        rigidBody.drag = 0;
        rigidBody.angularDrag = 0;
        camera = GameObject.Find("Main Camera");
        currentTriggerObj = null;
    }

    // 跳跃逻辑开始
    private void SetJumpInitState() {
        if (dir == Dir.FRONT) {
            index = (int)Constant.FRONT_INDEX + 1;
        }
        else if (dir == Dir.BACK) {
            index = (int)Constant.BACK_INDEX + 1;
        }
        else if (dir == Dir.LEFT) {
            index = (int)Constant.LEFT_INDEX + 1;
        }
        else if (dir == Dir.RIGHT) {
            index = (int)Constant.RIGHT_INDEX + 1;
        }
    }

    private void ResetJumpInitState() {
        if (dir == Dir.FRONT) {
            index = (int)Constant.FRONT_INDEX;
        }
        else if (dir == Dir.BACK) {
            index = (int)Constant.BACK_INDEX;
        }
        else if (dir == Dir.LEFT) {
            index = (int)Constant.LEFT_INDEX;
        }
        else if (dir == Dir.RIGHT) {
            index = (int)Constant.RIGHT_INDEX;
        }
    }

    public void OnEnterJump() {
        bJumping = true;
        SetJumpInitState();
    }

    public void OnLeaveJump() {
        if (bDelayRemoveGravity) {
            rigidBody.gravityScale = 0;
            bDelayRemoveGravity = false;
        }
        if (bMoving == 1) {
            ResetJumpInitState();
        }
        bJumping = false;
    }

    public void OnEnterJump(Vector2 speed) {
        bJumping = true;
        SetJumpInitState();
        rigidBody.velocity = speed;
    }

    public bool CanJump() { // 检查冲突状态
        return !bJumping
            && specialregion != Region.LADDER 
            && specialregion != Region.WATER;
    }

    // 跳跃逻辑结束

    // 帧动画逻辑开始
    private void updateIndex(int i, int move) {
        if (move == 1) {
            index = i;
        }
        else {
            index = i + 1;
        }
        bMoving = move;
    }

    //键盘回调
    public void OnKeyDown_W() {
        bKeyDown = true;
        dir = Dir.BACK;
        updateIndex((int)Constant.BACK_INDEX, 1);
        Vector2 vel = rigidBody.velocity;
        rigidBody.velocity = new Vector2(0, vel.y);
    }

    public void OnKeyDown_S() {
        bKeyDown = true;
        dir = Dir.FRONT;
        updateIndex((int)Constant.FRONT_INDEX, 1);
        Vector2 vel = rigidBody.velocity;
        rigidBody.velocity = new Vector2(0, vel.y);
    }

    public void OnKeyDown_A() {
        bKeyDown = true;
        dir = Dir.LEFT;
        updateIndex((int)Constant.LEFT_INDEX, 1);
    }

    public void OnKeyDown_D() {
        bKeyDown = true;
        dir = Dir.RIGHT;
        updateIndex((int)Constant.RIGHT_INDEX, 1);
    }

    public void OnKeyDown_Space() {
       // bKeyDown = true;
        if (CanJump()) {
            OnEnterJump(Value.v0_front);
        }
    }

    public void OnKeyUp_W() {
        bKeyDown = false;
        if (!bJumping) {
            updateIndex((int)Constant.BACK_INDEX, 2);
        }
        else {
            bMoving = 2;
        }
    }

    public void OnKeyUp_A() {
        bKeyDown = false;
        if (!bJumping) {
            updateIndex((int)Constant.LEFT_INDEX, 2);
        }
        else {
            bMoving = 2;
        }
    }

    public void OnKeyUp_S() {
        bKeyDown = false;
        if (!bJumping) {
            updateIndex((int)Constant.FRONT_INDEX, 2);
        }
        else {
            bMoving = 2;
        }
    }

    public void OnKeyUp_D() {
        bKeyDown = false;
        if (!bJumping) {
            updateIndex((int)Constant.RIGHT_INDEX, 2);
        }
        else {
            bMoving = 2;
        }
    }

    // 死亡
    public void Die() {
        bJumping = true;
        bMoving = 0;
        dir = Dir.FRONT;

        index = (int)Constant.FRONT_INDEX + 1;
        spriteRO.transform.localPosition = revivePos;
        camera.transform.localPosition = new Vector3(revivePos.x, 0, transform.localPosition.z);
    }

    public void CheckLeaveJump() {
        if (bJumping && rigidBody.velocity.y == 0) {
            OnLeaveJump();
        }
    }

    public void CheckInRevivePos() {
        Vector2 pos = spriteRO.transform.localPosition;
        foreach (Vector2 rpos in Value.revivePosTbl) {
            if (Equal(rpos, pos)) {
                if (!bInRevivePos) {
                    revivePos = new Vector2(rpos.x, rpos.y + 2);
                    bInRevivePos = true;
                }
            }
            else {
                if (bInRevivePos) {
                    bInRevivePos = false;
                }
            }
        }
    }

    public void CheckStopMoving() {
        if (bMoving == 2) {
            bMoving = 0;
            spriteRO.sprite = sprites[index];
        }
    }

    public void CheckWithLeaveTrigger() {
        if (currentTriggerObj && !currentTriggerObj.isEnter) {
            if (currentTriggerObj.type == TriggerObject.TriggerType.Ladder) {
                OnEnterJump(new Vector2(0, 0.1f));
            }
            specialregion = Region.NORMAL;
            bRemoveGravity = false;
            currentTriggerObj = null;
            rigidBody.gravityScale = 1;
        }
    }

    public void CheckWithEnterTrigger(TriggerObject triggerObj) {
        if (!bRemoveGravity) {
            bRemoveGravity = true;
            if (triggerObj.type == TriggerObject.TriggerType.WaterFall) {
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector2.zero;
                triggerObj.isStart = true;
                specialregion = Region.WATER;
                currentTriggerObj = triggerObj;

            }
            else if (triggerObj.type == TriggerObject.TriggerType.Ladder) {
                specialregion = Region.LADDER;
                if (bJumping) {
                    bDelayRemoveGravity = true;
                    currentTriggerObj = triggerObj;
                }
                else {
                    currentTriggerObj = triggerObj;
                    rigidBody.gravityScale = 0;
                }
            }
            else if (triggerObj.type == TriggerObject.TriggerType.DieRegion) {
                bRemoveGravity = false;
                Die();
            }
        }
    }
    public void UpdateCamera() {
        if (bMoving == 1 && !bJumping) {
            if (dir == Dir.FRONT) {
                if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder
                    && currentTriggerObj.isEnter) {
                    if (spriteRO.transform.position.x <= Value.cornerPos_down.y) {
                        camera.transform.Translate(-Value.v0_back);
                    }
                }
            }
            else if (dir == Dir.BACK) {
                if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder
                    && currentTriggerObj.isEnter) {
                    if (spriteRO.transform.position.x >= Value.cornerPos_up.y) {
                        camera.transform.Translate(Value.v0_back);
                    }
                }
            }
            else if (dir == Dir.LEFT) {
                if (spriteRO.transform.position.x <= Value.cornerPos_left.x) {
                    camera.transform.Translate(Value.v0_left);
                }
            }
            else if (dir == Dir.RIGHT) {
                if (spriteRO.transform.position.x >= Value.cornerPos_right.x) {
                    camera.transform.Translate(Value.v0_right);
                }
            }
        }
    }


    public void UpdateSprite() {
        if (bJumping && bKeyDown) {
            if (dir == Dir.LEFT) {
                MoveLeft();
            }
            else if (dir == Dir.RIGHT) {
                MoveRight();
            }
        }
        else if (bMoving == 1 && !bJumping) {
            if (dir == Dir.FRONT) {
                if (index == (int)Constant.FRONT_INDEX) {
                    index = (int)Constant.FRONT_INDEX + 2;
                }
                else if (index == (int)Constant.FRONT_INDEX + 2) {
                    index = (int)Constant.FRONT_INDEX;
                }
            }
            else if (dir == Dir.BACK) {
                if (index == (int)Constant.BACK_INDEX) {
                    index = (int)Constant.BACK_INDEX + 2;
                }
                else if (index == (int)Constant.BACK_INDEX + 2) {
                    index = (int)Constant.BACK_INDEX;
                }
            }
            else if (dir == Dir.LEFT) {
                if (index == (int)Constant.LEFT_INDEX) {
                    index = (int)Constant.LEFT_INDEX + 2;
                }
                else if (index == (int)Constant.LEFT_INDEX + 2) {
                    index = (int)Constant.LEFT_INDEX;
                }
                MoveLeft();
            }
            else if (dir == Dir.RIGHT) {
                if (index == (int)Constant.RIGHT_INDEX) {
                    index = (int)Constant.RIGHT_INDEX + 2;
                }
                else if (index == (int)Constant.RIGHT_INDEX + 2) {
                    index = (int)Constant.RIGHT_INDEX;
                }
                MoveRight();
            }
        }
        spriteRO.sprite = sprites[index];
    }

    // 移动
    public void MoveLeft() {
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.3f, 0, 0));
        spriteRO.transform.Translate(Value.v0_left);
    }

    public void MoveRight() {
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.7f, 0, 0));
        spriteRO.transform.Translate(Value.v0_right);
    }

    public void MoveBack(Vector2 speed) {
        Vector2 m_speed = speed;
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 0));
        spriteRO.transform.Translate(m_speed);
    }

    public void MoveBack() {
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 0));
        if (spriteRO.transform.position.y >= cornerPos.y) {
            camera.transform.Translate(Value.v0_back);
        }
        spriteRO.transform.Translate(Value.v0_back);
    }

    public void MoveFront() {
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.4f, 0));
        if (spriteRO.transform.position.y <= cornerPos.y) {
            camera.transform.Translate(-Value.v0_back);
        }
        spriteRO.transform.Translate(-Value.v0_back);
    }

    bool Equal(Vector2 v1, Vector2 v2) {
        float eps = 0.5f;
        return (v1.x - v2.x < eps && v1.x - v2.x > -eps &&
            v1.y - v2.y < eps && v1.y - v2.y > -eps);
    }
}
