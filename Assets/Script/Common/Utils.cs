using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
	// 返回tag为tags之一的son或son的父物体
	public static Transform GetOwner(Transform son, List<string> tags)
	{
		if (!son) return null;
		if (tags.Contains(son.tag)) {
			return son;
		}
		else {
			return GetOwner(son.parent, tags);
		}
	}
	public static Transform GetOwner(Transform son, string tag)
	{
		if (!son) return null;
		if (tag == son.tag) {
			return son;
		}
		else {
			return GetOwner(son.parent, tag);
		}
	}

	public static Transform FindChildRecursively(Transform top, string name)
	{
		Transform tar;
		tar = top.Find(name);
		if (!tar) {
			foreach (Transform child in top) {
				tar = FindChildRecursively(child, name);
				if (tar) break;
			}
		}
		return tar;
	}

	// 以Y轴正方向为参考，获得一个0-360度的夹角
	public static float GetAnglePY(Vector3 from, Vector3 to)
	{
		float angle = Vector3.Angle(from, to);
		Vector3 normal = Vector3.Cross(from, to);
		if (normal.y < 0) {
			angle = 360 - angle;
		}
		return angle;
	}
	public static float GetAnglePY2D(Vector2 from, Vector2 to)
	{
		float angle = Vector2.Angle(from, to);
		Vector2 normal = Vector3.Cross(from, to);
		if (normal.y < 0) {
			angle = 360 - angle;
		}
		return angle;
	}

	// 以Z轴正方向为参考，获得一个0-360度的夹角
	public static float GetAnglePZ(Vector3 from, Vector3 to)
	{
		float angle = Vector3.Angle(from, to);
		Vector3 normal = Vector3.Cross(from, to);
		if (normal.z < 0) {
			angle = 360 - angle;
		}
		return angle;
	}
	public static float GetAnglePZ2D(Vector2 from, Vector2 to)
	{
		float angle = Vector2.Angle(from, to);
		Vector3 normal = Vector3.Cross(from, to);
		if (normal.z < 0) {
			angle = 360 - angle;
		}
		return angle;
	}

	public static WeaponType GetWeaponTypeByName(WeaponNameType weaponName)
	{
		if (weaponName == WeaponNameType.M16 || weaponName == WeaponNameType.Machinegun) {
			return WeaponType.autoDistant;
		}
		else if (weaponName == WeaponNameType.Knife) {
			return WeaponType.melee;
		}
		else if (weaponName == WeaponNameType.Sniper) {
			return WeaponType.singleLoader;
		}
		else {
			return WeaponType.unknow;
		}
	}

	public static BaseData GetBaseData(Transform gamer)
	{
		BaseData baseData = null;
		if (gamer == PlayerData.Instance.target) {
			baseData = PlayerData.Instance;
		}
		else {
			baseData = EnemysData.Instance.GetEnemyDataByTransform(gamer);
		}
		return baseData;
	}
}
