using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySprite : MonoBehaviour{
    public Sprite[] sprites;
	public int index;
    public const int reviveNum = 1;
    public Dir dir = Dir.FRONT;
    public int isMoving = 0; // 0代表静止,1代表运动，2代表运动->静止
    public bool isInRevivePos = false;
    public bool isJumping = false;
    public bool isRemoveGravity = false;
    public bool isDelayRemoveGravity = false;
	public int Id;
    public Region specialregion = Region.NORMAL;
    public Vector2 revivePos = new Vector2(-6.24492f, 3.215286f);

    public bool isKeyDown = false;
    public TriggerObject currentTriggerObj;

	public int LEFT_INDEX;
	public int RIGHT_INDEX;
	public int FRONT_INDEX;
	public int BACK_INDEX;

    public SpriteRenderer spriteRO;
    public Rigidbody2D rigidBody;
    public float beginy;

	private bool isMainPlayer = false;

	private void SetStatus(string status,int data){
		GetType ().GetField (status).SetValue (this, data);
		if (isMainPlayer) {
			rpc.SetStatus (status, data);
		}
	}

	private void SetStatus(string status,bool data){
		GetType ().GetField (status).SetValue (this, data);
		if (isMainPlayer) {
			if (data == true) {
				rpc.SetStatus (status, 1);
			} 
			else if (data == false) {
				rpc.SetStatus (status, 0);
			}
		}
	}

	private void SetStatus(string status,Dir data){
		GetType ().GetField (status).SetValue (this, data);
		if (isMainPlayer) {
			switch (data) {
			case Dir.LEFT:
				rpc.SetStatus (status, 0);
				break;
			case Dir.RIGHT:
				rpc.SetStatus (status, 1);
				break;
			case Dir.FRONT:
				rpc.SetStatus (status, 2);	
				break;
			case Dir.BACK:
				rpc.SetStatus (status, 3);	
				break;
			}
		}
	}

	public void SetMainPlayer(){
		isMainPlayer = true;
	}
	private Rpc rpc = Rpc.getInstance ();
    void Awake() {
        sprites = Resources.LoadAll<Sprite>("image/people");
        rigidBody.drag = 0;
        rigidBody.angularDrag = 0;
        currentTriggerObj = null;
		index = FRONT_INDEX + 1;
    }

    // 跳跃逻辑开始
    private void SetJumpInitState() {
        if (dir == Dir.FRONT) {
            index = FRONT_INDEX + 1;
        }
        else if (dir == Dir.BACK) {
            index = BACK_INDEX + 1;
        }
        else if (dir == Dir.LEFT) {
            index = LEFT_INDEX + 1;
        }
        else if (dir == Dir.RIGHT) {
            index = RIGHT_INDEX + 1;
        }
    }

    private void ResetJumpInitState() {
        if (dir == Dir.FRONT) {
            index = FRONT_INDEX;
        }
        else if (dir == Dir.BACK) {
            index = BACK_INDEX;
        }
        else if (dir == Dir.LEFT) {
            index = LEFT_INDEX;
        }
        else if (dir == Dir.RIGHT) {
            index = RIGHT_INDEX;
        }
    }

    public void OnEnterJump() {
		SetStatus ("isJumping", true);
        SetJumpInitState();
    }

    public void OnLeaveJump() {
        if (isDelayRemoveGravity) {
            rigidBody.gravityScale = 0;
            isDelayRemoveGravity = false;
        }
        if (isMoving == 1) {
            ResetJumpInitState();
        }
		SetStatus ("isJumping", false);
    }

    public void OnEnterJump(Vector2 speed) {
		SetStatus ("isJumping", true);
        SetJumpInitState();
        rigidBody.velocity = speed;
    }

    public bool CanJump() { // 检查冲突状态
        return !isJumping
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
		SetStatus ("isMoving", move);
    }

    //键盘回调
    public void OnKeyDown_W() {
        isKeyDown = true;
		SetStatus ("dir", Dir.BACK);
        updateIndex(BACK_INDEX, 1);
        Vector2 vel = rigidBody.velocity;
        rigidBody.velocity = new Vector2(0, vel.y);
    }

    public void OnKeyDown_S() {
        isKeyDown = true;
		SetStatus ("dir", Dir.FRONT);
        updateIndex(FRONT_INDEX, 1);
        Vector2 vel = rigidBody.velocity;
        rigidBody.velocity = new Vector2(0, vel.y);
    }

    public void OnKeyDown_A() {
        isKeyDown = true;
		SetStatus ("dir", Dir.LEFT);
        updateIndex(LEFT_INDEX, 1);
    }

    public void OnKeyDown_D() {
        isKeyDown = true;
		SetStatus ("dir", Dir.RIGHT);
        updateIndex(RIGHT_INDEX, 1);
    }

    public void OnKeyDown_Space() {
        if (CanJump()) {
            OnEnterJump(Value.v0_front);
        }
    }

    public void OnKeyUp_W() {
        isKeyDown = false;
        if (!isJumping) {
            updateIndex(BACK_INDEX, 2);
        }
        else {
			SetStatus ("isMoving", 2);
        }
    }

    public void OnKeyUp_A() {
        isKeyDown = false;
        if (!isJumping) {
            updateIndex(LEFT_INDEX, 2);
        }
        else {
			SetStatus ("isMoving", 2);
        }
    }

    public void OnKeyUp_S() {
        isKeyDown = false;
        if (!isJumping) {
            updateIndex(FRONT_INDEX, 2);
        }
        else {
			SetStatus ("isMoving", 2);
        }
    }

    public void OnKeyUp_D() {
        isKeyDown = false;
        if (!isJumping) {
            updateIndex(RIGHT_INDEX, 2);
        }
        else {
			SetStatus ("isMoving", 2);
        }
    }

    // 死亡
    public void OnEnterDie() {
		SetStatus ("isJumping", true);
		SetStatus ("isMoving", 0);
		SetStatus ("dir", Dir.FRONT);

        index = FRONT_INDEX + 1;
        spriteRO.transform.localPosition = revivePos;
        Camera.main.transform.localPosition = new Vector3(revivePos.x, 0, Camera.main.transform.localPosition.z);
    }

    public void CheckLeaveJump() {
        if (isJumping && rigidBody.velocity.y == 0) {
            OnLeaveJump();
        }
    }

    public void CheckInRevivePos() {
        Vector2 pos = spriteRO.transform.localPosition;
        foreach (Vector2 rpos in Value.revivePosTbl) {
            if (Equal(rpos, pos)) {
                if (!isInRevivePos) {
                    revivePos = new Vector2(rpos.x, rpos.y + 2);
                    isInRevivePos = true;
                }
            }
            else {
                if (isInRevivePos) {
                    isInRevivePos = false;
                }
            }
        }
    }

    public void CheckStopMoving() {
        if (isMoving == 2) {
			SetStatus ("isMoving", 0);
            spriteRO.sprite = sprites[index];
        }
    }

    public void CheckWithLeaveTrigger() {
        if (currentTriggerObj.type == TriggerObject.TriggerType.Ladder) {
            OnEnterJump(new Vector2(0, 0.1f));
        }
        specialregion = Region.NORMAL;
        isRemoveGravity = false;
        currentTriggerObj = null;
        rigidBody.gravityScale = 1;

    }
    
    public void CheckWithEnterTrigger(ref TriggerObject triggerObj) {
   
        if (!isRemoveGravity) {
            isRemoveGravity = true;
            if (triggerObj.type == TriggerObject.TriggerType.WaterFall) {
                Debug.Log("waterfall");
                rigidBody.gravityScale = 0;
                rigidBody.velocity = Vector2.zero;
                triggerObj.isStart = true;
                specialregion = Region.WATER;
                currentTriggerObj = triggerObj;
               
            }
            else if (triggerObj.type == TriggerObject.TriggerType.Ladder) {
                Debug.Log("ladder");
                specialregion = Region.LADDER;

                if (isJumping) {
                    isDelayRemoveGravity = true;
                    currentTriggerObj = triggerObj;
                }
                else {
                    currentTriggerObj = triggerObj;
                    rigidBody.gravityScale = 0;
                }

            }
            else if (triggerObj.type == TriggerObject.TriggerType.DieRegion) {
                Debug.Log("DieRegion");
                isRemoveGravity = false;
				OnEnterDie ();
            }
        }
    }
    public void UpdateCamera() {
		if (isMoving == 1 && !isJumping) {
			if (dir == Dir.FRONT) {
				if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder) {
					Vector3 cornerPos_down = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.3f, 0));
					if (spriteRO.transform.position.y <= cornerPos_down.y) {
						Camera.main.transform.Translate (-Value.v0_back);
					}
				}
			} else if (dir == Dir.BACK) {
				if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder) {
					Vector3 cornerPos_up = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.7f, 0));
					if (spriteRO.transform.position.y >= cornerPos_up.y) {
						Camera.main.transform.Translate (Value.v0_back);
					}
				}
			} else if (dir == Dir.LEFT) {
				Vector3 cornerPos_left = Camera.main.ViewportToWorldPoint (new Vector3 (0.3f, 0, 0));
				if (spriteRO.transform.position.x <= cornerPos_left.x) {
					Camera.main.transform.Translate (Value.v0_left);
				}
			} else if (dir == Dir.RIGHT) {
				Vector3 cornerPos_right = Camera.main.ViewportToWorldPoint (new Vector3 (0.7f, 0, 0));
				if (spriteRO.transform.position.x >= cornerPos_right.x) {
					Camera.main.transform.Translate (Value.v0_right);
				}
			}
		} else if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.WaterFall) {
			Vector3 cornerPos_down = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.3f, 0));
			Vector3 cornerPos_up = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.7f, 0));
			if (spriteRO.transform.position.y <= cornerPos_down.y) {
				Camera.main.transform.Translate (-Value.v0_back);
			} else if (spriteRO.transform.position.y >= cornerPos_up.y) {
				Camera.main.transform.Translate (Value.v0_back);
			}
		} 
		else if (isJumping && isKeyDown) {
			if (dir == Dir.LEFT) {
				Vector3 cornerPos_left = Camera.main.ViewportToWorldPoint (new Vector3 (0.3f, 0, 0));
				if (spriteRO.transform.position.x <= cornerPos_left.x) {
					Camera.main.transform.Translate (Value.v0_left);
				}
			} else if (dir == Dir.RIGHT) {
				Vector3 cornerPos_right = Camera.main.ViewportToWorldPoint (new Vector3 (0.7f, 0, 0));
				if (spriteRO.transform.position.x >= cornerPos_right.x) {
					Camera.main.transform.Translate (Value.v0_right);
				}
			}
		}
		else if (isMoving == 0) {
			Vector3 cornerPos_down = Camera.main.ViewportToWorldPoint (new Vector3 (0, 0.3f, 0));
			if (spriteRO.transform.position.y <= cornerPos_down.y) {
				Camera.main.transform.Translate (-Value.v0_back);
			}

		}

    }
		

    public void UpdateSprite() {
		if (isJumping && isKeyDown) {
			if (dir == Dir.LEFT) {
				MoveLeft ();
			} else if (dir == Dir.RIGHT) {
				MoveRight ();
			}
		} else if (isMoving == 1 && !isJumping) {
			if (dir == Dir.FRONT) {
				if (index == FRONT_INDEX) {
					index = FRONT_INDEX + 2;
				} else if (index == FRONT_INDEX + 2) {
					index = FRONT_INDEX;
				}
                
				if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder) {
					MoveFront ();
				}
			} else if (dir == Dir.BACK) {
				if (index == BACK_INDEX) {
					index = BACK_INDEX + 2;
				} else if (index == BACK_INDEX + 2) {
					index = BACK_INDEX;
				}

				if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder) {
					MoveBack ();
				}
			} else if (dir == Dir.LEFT) {
				if (index == LEFT_INDEX) {
					index = LEFT_INDEX + 2;
				} else if (index == LEFT_INDEX + 2) {
					index = LEFT_INDEX;
				}
				MoveLeft ();
			} else if (dir == Dir.RIGHT) {
				if (index == RIGHT_INDEX) {
					index = RIGHT_INDEX + 2;
				} else if (index == RIGHT_INDEX + 2) {
					index = RIGHT_INDEX;
				}
				MoveRight ();
			}
		}
        spriteRO.sprite = sprites[index];
    }

    // 移动
    public void MoveLeft() {
        spriteRO.transform.Translate(Value.v0_left);
    }

    public void MoveRight() {
        spriteRO.transform.Translate(Value.v0_right);
    }

    public void MoveBack(Vector2 speed) {
        Vector2 m_speed = speed;
        spriteRO.transform.Translate(m_speed);
    }

    public void MoveBack() {
        spriteRO.transform.Translate(Value.v0_back);
    }

    public void MoveFront() {
        spriteRO.transform.Translate(-Value.v0_back);
    }

    bool Equal(Vector2 v1, Vector2 v2) {
        float eps = 0.5f;
        return (v1.x - v2.x < eps && v1.x - v2.x > -eps &&
            v1.y - v2.y < eps && v1.y - v2.y > -eps);
    }
}
