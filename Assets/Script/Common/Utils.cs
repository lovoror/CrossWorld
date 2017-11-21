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

	// 以Y轴负方向为参考，获得一个0-360度的夹角
	public static float GetAngle(Vector3 from, Vector3 to)
	{
		float angle = Vector3.Angle(from, to);
		Vector3 normal = Vector3.Cross(from, to);
		if (normal.y > 0) {
			angle = 360 - angle;
		}
		return angle;
	}
}
