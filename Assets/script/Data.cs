using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data {
	private static Data instance = new Data();
	private Data() { }
	public static Data getInstance() {
		return instance;
	}
	public int playerId;
}
