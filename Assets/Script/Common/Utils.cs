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

	public static bool IsParallel(Vector3 a, Vector3 b)
	{
		float rx = a.x / b.x;
		float ry = a.y / b.y;
		float rz = a.z / b.z;
		if (Mathf.Abs(rx - ry) + Mathf.Abs(rz - ry) < 0.01) {
			return true;
		}
		else {
			return false;
		}
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
		else if (weaponName == WeaponNameType.Sniper || weaponName == WeaponNameType.Shotgun) {
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

	public static bool IsDead(Transform gamer)
	{
		BaseData baseData = GetBaseData(gamer);
		if (baseData != null) {
			return baseData.curHealth <= 0;
		}
		return false;
	}

	public static DirectionType4 GetDirection4(Vector2 direction)
	{
		float angle = Utils.GetAnglePZ2D(new Vector2(1, 0), direction);
		if (45 <= angle && angle < 135) {
			return DirectionType4.Up;
		}
		else if (135 <= angle && angle < 225) {
			return DirectionType4.Left;
		}
		else if (225 <= angle && angle < 315) {
			return DirectionType4.Down;
		}
		else {
			return DirectionType4.Right;
		}
	}

	public static DirectionType8 GetDirection8(Vector2 direction)
	{
		float angle = Utils.GetAnglePZ2D(new Vector2(1, 0), direction);
		if (angle >= 22.5 && angle < 67.5) {
			return DirectionType8.UpRight;
		}
		else if (angle >= 67.5 && angle < 112.5) {
			return DirectionType8.Right;
		}
		else if (angle >= 112.5 && angle < 157.5) {
			return DirectionType8.DownRight;
		}
		else if (angle >= 112.5 && angle < 157.5) {
			return DirectionType8.DownRight;
		}
		else if (angle >= 157.5 && angle < 202.5) {
			return DirectionType8.Down;
		}
		else if (angle >= 202.5 && angle < 247.5) {
			return DirectionType8.DownLeft;
		}
		else if (angle >= 247.5 && angle < 292.5) {
			return DirectionType8.Left;
		}
		else if (angle >= 292.5 && angle < 337.5) {
			return DirectionType8.UpLeft;
		}
		else {
			return DirectionType8.Up;
		}
	}

	public static float GetWeaponDamageRate(WeaponNameType weaponName)
	{
		if (Constant.WEAPON_DAMAGE_RATE.ContainsKey(weaponName)) {
			int level = PlayerData.Instance.curWeaponLevel;
			if (Constant.WEAPON_DAMAGE_RATE[weaponName].Count >= level) {
				return Constant.WEAPON_DAMAGE_RATE[weaponName][level - 1];
			}
		}
		return -1;
	}

	public static bool CanBulletPenetrate(WeaponNameType weaponName, int weaponLevel)
	{
		bool canPenetrate = false;
		if (Constant.BULLET_PENETRATE_LEVEL.ContainsKey(weaponName)) {
			int penetrateLevel = Constant.BULLET_PENETRATE_LEVEL[weaponName];
			canPenetrate = weaponLevel >= penetrateLevel;
		}
		return canPenetrate;
	}

	public static float GetWeaponStrengthRestoreRate(WeaponNameType weaponName, int weaponLevel)
	{
		float rate = 0;
		if (Constant.WEAPON_STRENGTH_RESTORE_RATE.ContainsKey(weaponName)) {
			if (Constant.WEAPON_STRENGTH_RESTORE_RATE[weaponName].Count >= weaponLevel) {
				rate = Constant.WEAPON_STRENGTH_RESTORE_RATE[weaponName][weaponLevel - 1];
			}
		}
		return rate;
	}
}
