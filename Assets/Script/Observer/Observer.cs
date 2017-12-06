using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
	protected void Start () {
		InitGamerInfo();
	}
	
	private static bool isInited = false; // 信息是否已经初始化
	// 初始化Player和Enemys的信息
	static void InitGamerInfo () {
		if (isInited) return;
		// 初始化Gamers信息
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
		List<GameObject> gamers = new List<GameObject>(enemys);
		gamers.Add(player);
		foreach (GameObject gamer in gamers) {
			DataManager gamerData = gamer.transform.GetComponent<DataManager>();
			GameData.AddGamerInfo(gamer.transform, gamerData.curWeaponName, false, gamerData.health, gamerData.maxHealth);
		}
		HeadBarDisplay.instance.InitHeadBars();
		isInited = true;
	}

	// Gamer受伤血量同步，返回值:是否死亡
	protected static bool GamerHurt(string name, float damage)
	{
		return GameData.GamerHurt(name, damage);
	}
}
