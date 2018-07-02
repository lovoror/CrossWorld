using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
	unknow, melee, singleLoader, autoDistant
}

public enum WeaponNameType
{
	unknown, M16, Knife, Machinegun, Sniper, Shotgun
}

public enum DirectionType8
{
	none, Up, UpRight, Right, DownRight, Down, DownLeft, Left, UpLeft
}

public enum DirectionType4
{
	none, Up, Down, Left, Right
}

public class Constant
{
	public static class TAGS {
		public readonly static List<string> Attacker = new List<string>() { "Player", "Enemy" };  // 可以攻击的物体
		public readonly static List<string> Attackable = new List<string>() { "Player", "Enemy" }; // 可被攻击的物体
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

	public readonly static Dictionary<WeaponNameType, List<float>> MAX_WEAPON_ENERGY = new Dictionary<WeaponNameType, List<float>>() {
		{WeaponNameType.M16, new List<float>(){0, 100, 200, 300, 350}},
		{WeaponNameType.Machinegun, new List<float>(){0, 150, 300, 450, 600}},
		{WeaponNameType.Shotgun, new List<float>(){0, 150, 300, 450, 600}},
		{WeaponNameType.Knife, new List<float>(){0, 150, 300, 450, 600}},
		{WeaponNameType.Sniper, new List<float>(){0, 150, 300, 400, 450}},
	};

	public readonly static Dictionary<WeaponNameType, List<float>> WEAPON_SPEED_RATE = new Dictionary<WeaponNameType, List<float>>() {
		{WeaponNameType.M16, new List<float>(){1.0f, 1.5f, 2.0f, 2.0f}},
		{WeaponNameType.Machinegun, new List<float>(){1.0f, 1.5f, 2.0f, 2.0f}},
		{WeaponNameType.Shotgun, new List<float>(){1.2f, 1.5f, 1.8f, 1.8f}},
		{WeaponNameType.Knife, new List<float>(){1.0f, 1.2f, 1.5f, 1.5f}},
		{WeaponNameType.Sniper, new List<float>(){1.0f, 1.0f, 1.0f, 1.0f}},
	};

	public readonly static Dictionary<WeaponNameType, List<float>> WEAPON_DAMAGE_RATE = new Dictionary<WeaponNameType, List<float>>() {
		{WeaponNameType.M16, new List<float>(){1.0f, 1.2f, 1.5f, 1.5f}},
		{WeaponNameType.Machinegun, new List<float>(){1.0f, 1.0f, 1.0f, 1.0f}},
		{WeaponNameType.Shotgun, new List<float>(){1.0f, 1.2f, 1.5f, 1.5f}},
		{WeaponNameType.Knife, new List<float>(){1.0f, 1.2f, 1.5f, 1.5f}},
		{WeaponNameType.Sniper, new List<float>(){1.0f, 1.5f, 2.0f, 2.0f}},
	};

	// 造成伤害后加血
	public readonly static Dictionary<WeaponNameType, List<float>> WEAPON_STRENGTH_RESTORE_RATE = new Dictionary<WeaponNameType, List<float>>() {
		{WeaponNameType.M16, new List<float>(){0f, 0f, 0.05f, 0.05f}},
		{WeaponNameType.Machinegun, new List<float>(){0f, 0f, 0.1f, 0.1f}},
		{WeaponNameType.Shotgun, new List<float>(){0f, 0f, 0.1f, 0.1f}},
		{WeaponNameType.Knife, new List<float>(){0f, 0f, 0.1f, 0.1f}},
		{WeaponNameType.Sniper, new List<float>(){0f, 0f, 0.15f, 0.15f}},
	};

	public readonly static Dictionary<WeaponNameType, int> BULLET_PENETRATE_LEVEL = new Dictionary<WeaponNameType, int>() {
		{ WeaponNameType.M16, 999 },
		{ WeaponNameType.Machinegun, 999 },
		{ WeaponNameType.Shotgun, 999 },
		{ WeaponNameType.Sniper, 2 },
	};

	// 散弹枪单次射击的子弹数
	public readonly static Dictionary<WeaponNameType, List<int>> BULLET_NUMBER = new Dictionary<WeaponNameType, List<int>>() {
		{WeaponNameType.Shotgun, new List<int>(){9, 14, 18, 18}},
	};

	public static Dictionary<WeaponNameType, float> BaseDamage = new Dictionary<WeaponNameType, float>() {
		{ WeaponNameType.Knife, 150 },
		{ WeaponNameType.M16, 50 },
		{ WeaponNameType.Machinegun, 50 },
		{ WeaponNameType.Shotgun, 25 },
		{ WeaponNameType.Sniper, 150 },
	};

	// 远程武器瞄准的辅助距离
	public static Dictionary<WeaponNameType, float> AimAssistDist = new Dictionary<WeaponNameType, float>() {
		{ WeaponNameType.M16, 5 },
		{ WeaponNameType.Machinegun, 5 },
		{ WeaponNameType.Sniper, 3 },
		{ WeaponNameType.Shotgun, 5 },
	};

	// 远程武器瞄准的最远距离
	public static Dictionary<WeaponNameType, float> AimMaxDist = new Dictionary<WeaponNameType, float>() {
		{ WeaponNameType.M16, 50 },
		{ WeaponNameType.Machinegun, 50 },
		{ WeaponNameType.Sniper, 90 },
		{ WeaponNameType.Shotgun, 40 },
	};

	// 远程武器弹夹容量
	public static Dictionary<WeaponNameType, int> MagazineSize = new Dictionary<WeaponNameType, int>{
		{ WeaponNameType.M16, 30 },
		{ WeaponNameType.Machinegun, 30 },
		{ WeaponNameType.Sniper, 6 },
		{ WeaponNameType.Shotgun, 6 },
	};

	// 武器能量增减与伤害的比值
	public static float increaseRate = 0.2f;
	public static float decreaseRate = -1f;

	public static float GetBaseDamage(WeaponNameType weaponName)
	{
		if (BaseDamage.ContainsKey(weaponName)) {
			return BaseDamage[weaponName];
		}
		return -1;
	}
}
