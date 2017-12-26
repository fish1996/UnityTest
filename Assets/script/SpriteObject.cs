using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteObject : MonoBehaviour {

    private Sprite[] sprites;
	private List<AnimationObject> animationList = new List<AnimationObject>();
    private Prefab prefab = Prefab.getInstance();

    private int index = 1; // 当前指向的精灵
    private const int reviveNum = 1;
    private Dir dir = Dir.FRONT;
    private int bMoving = 0; // 0代表静止,1代表运动，2代表运动->静止

    private bool bInRevivePos = false;
    private bool bJumping = false;
    
    private bool bRemoveGravity = false;
	private bool bKeyDown = false;
	private bool bDelayRemoveGravity = false;

    private Region specialregion = Region.NORMAL;

    private Rigidbody2D rigidBody;

    private Vector2 v0_right = new Vector2(0.3f, 0.0f);
    private Vector2 v0_left = new Vector2(-0.3f, 0.0f);
    private Vector2 v0_back = new Vector2(0.0f, 0.3f);
	public Vector2 v0_front;
    
    //private ColliderObject building;

    private List<TriggerObject> triggerList = new List<TriggerObject>();
	private TriggerObject currentTriggerObj;
    private Vector2 revivePos = new Vector2(-6.24492f, 3.215286f);
    private Vector2[] revivePosTbl = {
        new Vector2(-6.24492f,1.215286f),
    };
    private enum Constant:int {
        LEFT_INDEX = 12,
        RIGHT_INDEX = 24,
        FRONT_INDEX = 0,
        BACK_INDEX = 36,
    };

    private enum Dir : int { LEFT,RIGHT,FRONT,BACK};
    private enum Region : int { WATER,LADDER,NORMAL};
    private SpriteRenderer spriteRO;
    private float beginy;

    void Awake() {

        // load animation
        foreach(string path in prefab.animationPath) {
            AnimationObject obj = GameObject.Find(path).GetComponent<AnimationObject>();
            animationList.Add(obj);
        }

        //load trigger
        foreach (string path in prefab.triggerPath) {
            TriggerObject obj = GameObject.Find(path).GetComponent<TriggerObject>();
            triggerList.Add(obj);
        }
    }
    void Start () {
        sprites = Resources.LoadAll<Sprite>("image/people");
        foreach(Object obj in sprites) {
        }
        spriteRO = GameObject.Find("Sprite").GetComponent<SpriteRenderer>();
        rigidBody = GameObject.Find("Sprite").GetComponent<Rigidbody2D>();

		currentTriggerObj = null;

        rigidBody.drag = 0;
        rigidBody.angularDrag = 0;
       
    }

    private void OnEnterJump() {
        Debug.Log("OnEnterJump");
        //rigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        bJumping = true;
        SetJumpInitState();
    }

    private void OnLeaveJump() {
        Debug.Log("OnLeaveJump");

        if (bDelayRemoveGravity) {
            rigidBody.gravityScale = 0;
            bDelayRemoveGravity = false;
        }
        if (bMoving == 1) {
            ResetJumpInitState();
        }
        bJumping = false;
    }
    private void OnEnterJump(Vector2 speed) {
        Debug.Log("OnEnterJump");
        bJumping = true;
        
        SetJumpInitState();
        rigidBody.velocity = speed;
    }

    private bool CanJump() {
        if (specialregion == Region.LADDER || specialregion == Region.WATER) {
            return false;
        }
        return true;
    }

    private void updateIndex(int i,int move) {
        if (move == 1) {
            index = i;
        }
        else {
            index = i + 1;
        }
        bMoving = move;
    }

    private void SetJumpInitState() {
        if(dir == Dir.FRONT) {
            index = (int)Constant.FRONT_INDEX + 1;
        }
        else if(dir == Dir.BACK) {
            index = (int)Constant.BACK_INDEX + 1;
        }
        else if(dir == Dir.LEFT) {
            index = (int)Constant.LEFT_INDEX + 1;
        }
        else if(dir == Dir.RIGHT) {
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

    private void KeyControl() {
        // 鼠标按下

        if (Input.GetKeyDown(KeyCode.W)) {
			bKeyDown = true;
            dir = Dir.BACK;
            updateIndex((int)Constant.BACK_INDEX, 1);
            Vector2 vel = rigidBody.velocity;
            rigidBody.velocity = new Vector2(0, vel.y);
        }
        if (Input.GetKeyDown(KeyCode.S)) {
			bKeyDown = true;
            dir = Dir.FRONT;
            updateIndex((int)Constant.FRONT_INDEX, 1);
            Vector2 vel = rigidBody.velocity;
            rigidBody.velocity = new Vector2(0, vel.y);
        }
        if (Input.GetKeyDown(KeyCode.A)) {
	 		bKeyDown = true;
            dir = Dir.LEFT;
            updateIndex((int)Constant.LEFT_INDEX, 1);
        }
        if (Input.GetKeyDown(KeyCode.D)) {
			bKeyDown = true;
            dir = Dir.RIGHT;
            updateIndex((int)Constant.RIGHT_INDEX, 1);
        }
		if(!bJumping){
            if (Input.GetKeyDown(KeyCode.Space)) {
                if (CanJump()) {
                    OnEnterJump(v0_front);
                }
            }
            if (Input.GetKeyDown(KeyCode.Q)) {
                Die();
            }
        }
        // 鼠标抬起
        if (Input.GetKeyUp(KeyCode.W)) {
			bKeyDown = false;
            if (!bJumping) {
                updateIndex((int)Constant.BACK_INDEX, 2);
            }
            else {
                bMoving = 2;
            }
        }
        if (Input.GetKeyUp(KeyCode.S)) {
			bKeyDown = false;
            if (!bJumping) {
                updateIndex((int)Constant.FRONT_INDEX, 2);
            }
            else {
                bMoving = 2;
            }
        }

        if (Input.GetKeyUp(KeyCode.A)) {
			bKeyDown = false;
            if (!bJumping) {
                updateIndex((int)Constant.LEFT_INDEX, 2);
            }
            else {
                bMoving = 2;
            }
        }
        if (Input.GetKeyUp(KeyCode.D)) {
			bKeyDown = false;
            if (!bJumping) {
                updateIndex((int)Constant.RIGHT_INDEX, 2);
            }
            else {
                bMoving = 2;
            }
        }
    }

    private void Die() {

        bJumping = true;
        bMoving = 0;
        dir = Dir.FRONT;
        
        index = (int)Constant.FRONT_INDEX + 1;
        spriteRO.transform.localPosition =  revivePos;
        transform.localPosition = new Vector3(revivePos.x,0, transform.localPosition.z);
    }

    bool Equal(Vector2 v1,Vector2 v2) {
        float eps = 0.5f;
        return (v1.x - v2.x < eps && v1.x - v2.x > -eps &&
            v1.y - v2.y < eps && v1.y - v2.y > -eps);
    }

    bool Equal(float v1, float v2) {
        float eps = 0.5f;
        Debug.Log(v1 - v2);
        return v1 - v2 < eps && v1 - v2 > -eps;
    }

    void Update () {
        CheckWithTrigger();
        if (bJumping && rigidBody.velocity.y == 0) {
            
            OnLeaveJump();

        }
        Vector2 pos = spriteRO.transform.localPosition;
        foreach(Vector2 rpos in revivePosTbl) {
            if (Equal(rpos,pos)) {
                if(!bInRevivePos) {

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

        if (bMoving == 2) {
            bMoving = 0;
            spriteRO.sprite = sprites[index];
        }
        KeyControl();

    }

    void MoveLeft() {
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.3f, 0, 0));
        if(spriteRO.transform.position.x <= cornerPos.x) {
            transform.Translate(v0_left);
        }
        spriteRO.transform.Translate(v0_left);
    }

    void MoveRight() {
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0.7f, 0, 0));
        if (spriteRO.transform.position.x >= cornerPos.x) {
            transform.Translate(v0_right);
        }
        spriteRO.transform.Translate(v0_right);
    }

	void MoveBack(Vector2 speed) {
		Vector2 m_speed = speed;
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 0));
        if (spriteRO.transform.position.y>= cornerPos.y) {
			transform.Translate(m_speed);
        }
		spriteRO.transform.Translate(m_speed);
    }

	void MoveBack() {
		Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.7f, 0));
		if (spriteRO.transform.position.y>= cornerPos.y) {
			transform.Translate(v0_back);
		}
		spriteRO.transform.Translate(v0_back);
	}

	void MoveFront() {

        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.4f, 0));
        if (spriteRO.transform.position.y <= cornerPos.y) {
			transform.Translate(-v0_back);
        }
        spriteRO.transform.Translate(-v0_back);
    }

    void UpdateView() {
        Vector3 cornerPos = Camera.main.ViewportToWorldPoint(new Vector3(0, 0.4f, 0));
        if (spriteRO.transform.position.y <= cornerPos.y) {
            transform.Translate(-v0_back);
        }
    }

	void CheckWithTrigger(){
		foreach (TriggerObject triggerObj in triggerList) {
			if (triggerObj.isEnter && !bRemoveGravity) {
                Debug.Log("collision");
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
                    Debug.Log("Die");
                    bRemoveGravity = false;
                    Die();
                }
                break;
			}
		}
		if (currentTriggerObj && !currentTriggerObj.isEnter) {
            if(currentTriggerObj.type == TriggerObject.TriggerType.Ladder) {
                OnEnterJump(new Vector2(0,0.1f));
            }
            specialregion = Region.NORMAL;
            bRemoveGravity = false;
            currentTriggerObj = null;
            rigidBody.gravityScale = 1;
		}
	}

    void FixedUpdate() {
        UpdateView();
        UpdateTrigger();
        UpdateSprite();
        UpdateFrameAnimation();
    }

    private void UpdateSprite() {
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
                if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder
                    && currentTriggerObj.isEnter) {
                    MoveFront();
                }
            }
            else if (dir == Dir.BACK) {
                if (index == (int)Constant.BACK_INDEX) {
                    index = (int)Constant.BACK_INDEX + 2;
                }
                else if (index == (int)Constant.BACK_INDEX + 2) {
                    index = (int)Constant.BACK_INDEX;
                }
                if (currentTriggerObj && currentTriggerObj.type == TriggerObject.TriggerType.Ladder
                    && currentTriggerObj.isEnter) {
                    MoveBack();
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
                if (specialregion == Region.WATER) {
                    MoveBack(new Vector2(0, 0.1f));
                }
                if (m_water.transform.localScale.y >= triggerObj.moveLength + triggerObj.beginy) {
                    triggerObj.isStart = false;
                    m_water.transform.localScale = new Vector3(m_water.transform.localScale.x, triggerObj.beginy, m_water.transform.localScale.z);
                }
            }
        }
    }
}
