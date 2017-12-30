using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Rpc{
	private Socket clientSocket;
	private byte[] result = new byte[1024];
	private static Rpc instance = new Rpc();
	private bool isConnect = false;
	private int mainSpriteId;
	private string[] statusList = {
		"Id",
		"isJumping",
		"isMoving",
		"dir",
	};
	private string[] typeList = {
		"Int",
		"Bool",
		"Int",
		"Dir",
	};
	private Dictionary<string,int> dic = new Dictionary<string,int> ();
	private Dictionary<int,MySprite> player = new Dictionary<int,MySprite> ();
	private Rpc() { 
		// init status table
		for(int i = 0;i < statusList.Length;i++) {
			string status = statusList [i];
			dic.Add (status, i);
		}

		//connect 2 server
		clientSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
		IPAddress mIp = IPAddress.Parse ("127.0.0.1");
		IPEndPoint ip_end_point = new IPEndPoint (mIp, 4547);
		try{
			clientSocket.Connect(ip_end_point);
			isConnect = true;
		}
		catch{
			isConnect = false;
			// ..
		}

		Thread th = new Thread (ReceiveMsg);
	}

	public void initPlayerList(){
		//init player table
		GameObject[] playerList = GameObject.FindGameObjectsWithTag ("Player");
		foreach(GameObject p in playerList) {
			MySprite sp = p.GetComponent<MySprite> ();
			player.Add (sp.Id, sp);
		}
	}
	public void SetMainSpriteId(int id){
		mainSpriteId = id;
	}
	// single instance
	public static Rpc getInstance() {
		return instance;
	}

	// 接收状态量
	public void ReceiveMsg(){
		int receiveLength = clientSocket.Receive (result);
		for (int i = 0; i < receiveLength; i+=3) {
			int id = result [i];
			int statusId = result [i + 1];
			int data = result [i + 2];
			string status = statusList [statusId];
			string type = typeList [statusId];
			switch (type) {
			case "Int":
				GetType ().GetField (status).SetValue (this, data);
				break;
			case "Bool":
				if (data == 1) {
					GetType ().GetField (status).SetValue (this, true);
				} 
				else if (data == 0) {
					GetType ().GetField (status).SetValue (this, false);
				}
				break;
			case "Dir":
				if (data == 0) {
					GetType ().GetField (status).SetValue (this, Dir.LEFT);
				} 
				else if (data == 1) {
					GetType ().GetField (status).SetValue (this, Dir.RIGHT);
				}
				else if (data == 2) {
					GetType ().GetField (status).SetValue (this, Dir.FRONT);
				}
				else if (data == 3) {
					GetType ().GetField (status).SetValue (this, Dir.BACK);
				}
				break;
			}
		}
	}

	// 同步状态量
	public void SetStatus(string status,int value){
		if (isConnect) {
			byte[] msg = new byte[2];
			msg [0] = (byte)(dic [status] & 0xff);
			msg [1] = (byte)value;
			clientSocket.Send (msg);
		}
	}

}
