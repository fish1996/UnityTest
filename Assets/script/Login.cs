using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Login : MonoBehaviour {
	private Data data = Data.getInstance();
	private bool flag = false;
	void Start () {
		GameObject btnObj = GameObject.Find ("Canvas/Button");
		GameObject inputObj = GameObject.Find ("Canvas/InputField");
		Button btn = btnObj.GetComponent<Button> ();
		InputField input = inputObj.GetComponent<InputField> ();
		btn.onClick.AddListener (delegate() {
			if(flag) return;
			if(input.text == ""){
			//	UnityEditor.EditorUtility.DisplayDialog("Error","请输入id","确定");
				return;
			}
			int id = int.Parse(input.text);
			if(id != 1 && id!= 2){
			//	UnityEditor.EditorUtility.DisplayDialog("Error","输入id不存在","确定");
				return;
			}
			flag = true;
			data.playerId = id;
			Application.LoadLevel("scene");
		});
	}
}
