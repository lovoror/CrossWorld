using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : DistantWeaponManager
{
	public float scatteringAngle = 10;
	RangeAimController I_AimController;
	protected new void Awake()
	{
		base.Awake();
		weaponType = WeaponType.singleLoader;
		weaponName = WeaponNameType.Shotgun;
		I_AimController = transform.GetComponentInChildren<RangeAimController>();
	}

	protected new void Start()
	{
		base.Start();
		//UpdateRangeTriangle();
		I_AimController.SetVisible(false);
	}

	new void OnEnable()
	{
		base.OnEnable();
		I_Manager.I_Controller.ShowRangeTriangleEvent += new Controller.ShowRangeTriangleEventHandler(ShowRangeTriangleEventFunc);
	}

	new void OnDisable()
	{
		base.OnDisable();
		I_Manager.I_Controller.ShowRangeTriangleEvent -= ShowRangeTriangleEventFunc;
	}

	protected override void CreateBullet(Transform shooter, WeaponNameType weaponName)
	{
		int bulletNumber = GetOneShootBulletNumber();
		if (bulletNumber <= 0) return;
		Vector3 firePos = muzzle.position;
		Vector3 bodyAngle = body.rotation.eulerAngles;
		bool canPenetrate = Utils.CanBulletPenetrate(weaponName, GetWeaponLevel(weaponName));
		// 在中心扇形产生一定子弹数
		int centerBulletNum = Mathf.RoundToInt(bulletNumber / 4);
		for (int i = 0; i < centerBulletNum; ++i) {
			CreateOneBullet(shooter, bodyAngle, firePos, -scatteringAngle / 8, scatteringAngle / 4, canPenetrate);
		}
		// 剩余的子弹在全区域随机分布
		for (int i = 0; i < bulletNumber - centerBulletNum; ++i) {
			CreateOneBullet(shooter, bodyAngle, firePos, -scatteringAngle / 2, scatteringAngle, canPenetrate);
		}
	}

	void CreateOneBullet(Transform shooter, Vector3 bodyAngle, Vector3 firePos, float startAngle, float deltaAngle, bool canPenetrate)
	{
		float randomAngle = Random.Range(startAngle, startAngle + deltaAngle);
		Quaternion quaternion = Quaternion.Euler(new Vector3(bodyAngle.x, bodyAngle.y + randomAngle, bodyAngle.z));
		Transform I_Bullet = Instantiate(bullet, firePos, quaternion);
		BulletController I_BulletController = I_Bullet.GetComponent<BulletController>();
		I_BulletController.Init(shooter, 0, bulletSpeed, canPenetrate);
	}

	int GetOneShootBulletNumber()
	{
		int num = -1;
		if (Constant.BULLET_NUMBER.ContainsKey(weaponName)) {
			int level = GetWeaponLevel(weaponName);
			if (Constant.BULLET_NUMBER[weaponName].Count >= level) {
				num = Constant.BULLET_NUMBER[weaponName][level - 1];
			}

		}
		return num;
	}

	void UpdateRangeTriangle()
	{
		Vector3 v3_start = new Vector3(0, -scatteringAngle / 2, 0);
		float length = Constant.AimMaxDist[weaponName] - 4.5f;
		I_AimController.UpdateAim(v3_start, scatteringAngle, length / 10);
	}

	void ShowRangeTriangleEventFunc(WeaponNameType weaponName, bool show)
	{
		if (weaponName == this.weaponName) {
			if (show) {
				UpdateRangeTriangle();
			}
			I_AimController.SetVisible(show);
		}
	}
}
