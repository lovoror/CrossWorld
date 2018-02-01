using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
	protected void OnEnable()
	{

	}

	protected void OnDisable()
	{

	}

	private static bool isInited = false; // 信息是否已经初始化
	protected void Start () {
		if (isInited) return;

		isInited = true;
	}

	// Gamer受伤血量同步，返回值:是否死亡
	protected static bool GamerHurt(Transform suffer, float damage)
	{
		BaseData sufferData = Utils.GetBaseData(suffer);
		if (sufferData != null) {
			return sufferData.ChangeCurHealth(-damage);
		}
		Debug.LogError("suffer data is not stored.");
		return false;
	}

	public static WeaponNameType GetPlayerCurWeaponName()
	{
		return PlayerData.Instance.curWeaponName;
	}

	public static Transform GetPlayer()
	{
		return PlayerData.Instance.target;
	}

	public static bool IsPlayerDead()
	{
		return PlayerData.Instance.isDead;
	}

	protected static void StageEnd()
	{
		isInited = false;
	}
}
