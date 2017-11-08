using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant
{
	public static class TAGS {
		public readonly static List<string> Attacker = new List<string>() { "Player", "Enemy" };  // 可以攻击的物体
		public readonly static List<string> Attackable = new List<string>() { "Player", "Enemy" }; // 可被攻击的物体
		public readonly static List<string> AttackableCollider = new List<string>() { "PlayerCollider", "EnemyCollider" }; // 可被攻击的物体碰撞体
	};
}
