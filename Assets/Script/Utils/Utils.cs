using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils {
	// 获得son的所有者，tag == "Player" || "Enemy"
	public static Transform GetOwner(Transform son, List<string> tags)
	{
		if (!son) return null;
		if (tags.IndexOf(son.tag) >= 0) {
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
}
