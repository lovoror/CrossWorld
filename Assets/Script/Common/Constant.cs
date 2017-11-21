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

	public static class COLOR
	{
		public readonly static Color WHITE  = new Color(1, 1, 1);
		public readonly static Color YELLOW = new Color(1, 1, 0);
		public readonly static Color RED    = new Color(1, 0, 0);
		public readonly static Color PURPLE = new Color(0.5f, 0, 0.5f);
	};

	public static Dictionary<int, Color> WEAPON_COLORS = new Dictionary<int, Color>() {
		{1, COLOR.WHITE},
		{2, COLOR.YELLOW},
		{3, COLOR.RED},
		{4, COLOR.PURPLE},

	};

	public readonly static Dictionary<int, List<float>> MAX_WEAPON_ENERGY = new Dictionary<int, List<float>>() {
		{(int)WEAPON_NAME.M16, new List<float>(){0, 100, 200, 300, 350}},
		{(int)WEAPON_NAME.Knife, new List<float>(){0, 100, 200, 300, 350}},
	};

	public readonly static Dictionary<int, List<float>> WEAPON_SPEED_RATE = new Dictionary<int, List<float>>() {
		{(int)WEAPON_NAME.M16, new List<float>(){1.0f, 1.5f, 2.0f, 2.0f}},
		{(int)WEAPON_NAME.Knife, new List<float>(){1.0f, 1.5f, 2.0f, 2.0f}},
	};

	public enum WEAPON_NAME {
		M16, Knife,
	};
}
