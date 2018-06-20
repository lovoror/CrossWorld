using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum AttackSdType
{
	none,						  // 没有按下AttackBtn
	A, AU, AD, AL, AR,			  // 短按，刚按下即赋值为短按
	HA, HAU, HAD, HAL, HAR,		  // 长按
	ARN, ARU, ARD, ARL, ARR,	  // 短按释放
	HARN, HARU, HARD, HARL, HARR // 长按释放
}

enum AttackFirstType
{
	none, A1, HA1, AR1, HAR1  // 未知、短按、长按、短按释放、长按释放
}

enum AttackSecondType
{
	none, N2, U2, D2, L2, R2,  // 未知、无偏移、上偏移、下偏移、左偏移、右偏移
	NR2, UR2, DR2, LR2, RR2    // 无(上、下、左、右)偏移释放
}

public class AttackButtonSd : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
	public delegate void UIAttackSdTypeEventHandler(AttackSdType attackSdType);
	public static event UIAttackSdTypeEventHandler UIAttackSdTypeEvent;

	class AttackFirstCheckModel  // 第一模块
	{
		public delegate void FirstInstructHandler(AttackFirstType firstType);
		public static event FirstInstructHandler FirstInstructEvent;

		public AttackFirstType firstType = AttackFirstType.none;
		AttackFirstType preFirstType = AttackFirstType.none;

		public float minHoldTime = 0;
		float touchStartTime = -1; // 本次触摸的开始时间
		bool isHold = false;
		bool inTouch = false;  // 是否在触摸事件中

		public void Reset()
		{
			firstType = AttackFirstType.none;
			preFirstType = AttackFirstType.none;
			touchStartTime = -1;
			isHold = false;
			inTouch = false;
		}

		public void OnTouchStart()
		{
			touchStartTime = Time.time;
			inTouch = true;
			SetFirstType(AttackFirstType.A1);
			Notify();
		}

		public void OnTouchEnd()
		{
			inTouch = false;
			AttackFirstType tmpType = AttackFirstType.none;
			switch (firstType) {
				case AttackFirstType.A1:
					tmpType = AttackFirstType.AR1;
					Notify();
					break;
				case AttackFirstType.HA1:
					tmpType = AttackFirstType.HAR1;
					break;
			}
			SetFirstType(tmpType);
			Notify();
			Reset();
		}

		public void Update()
		{
			if (!inTouch) return;
			// 检测是否是Hold
			if (isHold == false && Time.time - touchStartTime >= minHoldTime) {
				isHold = true;
				firstType = AttackFirstType.HA1;
				Notify();
			}
		}

		public void SetFirstType(AttackFirstType type)
		{
			preFirstType = firstType;
			firstType = type;
		}

		public void Notify()
		{
			if (FirstInstructEvent != null) {
				FirstInstructEvent(firstType);
			}
		}
	}

	class AttackSecondCheckModel // 第二模块
	{
		public delegate void SecondInstructHandler(AttackSecondType secondType);
		public static event SecondInstructHandler SecondInstructEvent;

		public AttackSecondType secondType = AttackSecondType.none;
		AttackSecondType preSecondType = AttackSecondType.none;

		public float dragThreshold = 0;
		Vector2 direction = Vector2.zero;

		public void Reset()
		{
			secondType = AttackSecondType.none;
			preSecondType = AttackSecondType.none;
			direction = Vector2.zero;
		}

		public void OnTouchStart()
		{
			SetSecondType(AttackSecondType.N2);
			Notify();
		}

		public void OnTouchEnd()
		{
			AttackSecondType tmpType = AttackSecondType.none;
			switch (secondType) {
				case AttackSecondType.N2:
					tmpType = AttackSecondType.NR2;
					break;
				case AttackSecondType.U2:
					tmpType = AttackSecondType.UR2;
					break;
				case AttackSecondType.D2:
					tmpType = AttackSecondType.DR2;
					break;
				case AttackSecondType.L2:
					tmpType = AttackSecondType.LR2;
					break;
				case AttackSecondType.R2:
					tmpType = AttackSecondType.RR2;
					break;
			}
			SetSecondType(tmpType);
			Notify();
			Reset();
		}

		public void OnDrag(Vector2 pointer)
		{
			if (pointer.sqrMagnitude > dragThreshold * dragThreshold) {
				direction = pointer;
			}
			else {
				// 取消滑动
				direction = Vector2.zero;
			}
			AttackSecondType tmpType = AttackSecondType.none;
			if (direction != Vector2.zero) {
				DirectionType4 type4 = Utils.GetDirection4(direction);
				switch (type4) {
					case DirectionType4.Up:
						tmpType = AttackSecondType.U2;
						break;
					case DirectionType4.Down:
						tmpType = AttackSecondType.D2;
						break;
					case DirectionType4.Left:
						tmpType = AttackSecondType.L2;
						break;
					case DirectionType4.Right:
						tmpType = AttackSecondType.R2;
						break;
				}
			}
			else {
				tmpType = AttackSecondType.N2;
			}

			if (tmpType != AttackSecondType.none) {
				if (tmpType != secondType) {
					preSecondType = secondType;
					secondType = tmpType;
					Notify();
				}
			}
		}

		public void SetSecondType(AttackSecondType type)
		{
			preSecondType = secondType;
			secondType = type;
		}

		public void Notify()
		{
			if (SecondInstructEvent != null) {
				SecondInstructEvent(secondType);
			}
		}
	}

	class AttackCheckModel  // 第三模块
	{
		public AttackSdType attackType = AttackSdType.none;
		public AttackFirstType firstType = AttackFirstType.none;
		public AttackSecondType secondType = AttackSecondType.none;
		AttackSdType preAttackType = AttackSdType.none;
		AttackFirstType preFirstType = AttackFirstType.none;
		AttackSecondType preSecondType = AttackSecondType.none;

		public void Reset()
		{
			attackType = AttackSdType.none;
			firstType = AttackFirstType.none;
			secondType = AttackSecondType.none;
			preAttackType = AttackSdType.none;
			preFirstType = AttackFirstType.none;
			preSecondType = AttackSecondType.none;
		}

		public void OnEnable()
		{
			//print("AttackCheckModule OnEnable");
			AttackFirstCheckModel.FirstInstructEvent += new AttackFirstCheckModel.FirstInstructHandler(FirstInstructFunc);
			AttackSecondCheckModel.SecondInstructEvent += new AttackSecondCheckModel.SecondInstructHandler(SecondInstructFunc);
		}

		public void OnDisable()
		{
			AttackFirstCheckModel.FirstInstructEvent -= FirstInstructFunc;
			AttackSecondCheckModel.SecondInstructEvent -= SecondInstructFunc;
		}

		public void OnTouchEnd()
		{
			Reset();
		}

		void FirstInstructFunc(AttackFirstType type)
		{
			if (type != AttackFirstType.none) {
				preFirstType = firstType;
				firstType = type;
				DealAttackType();
			}
		}

		void SecondInstructFunc(AttackSecondType type)
		{
			if (type != AttackSecondType.none) {
				preSecondType = secondType;
				secondType = type;
				DealAttackType();
			}
		}

		void DealAttackType()
		{
			AttackSdType tmpAttackType = GetAttackTypeByFirstAndSecond(firstType, secondType);
			if (tmpAttackType != AttackSdType.none && tmpAttackType != attackType) {
				preAttackType = attackType;
				attackType = tmpAttackType;
				Notify();
			}
		}

		void Notify()
		{
			//print("AttackType:" + attackType);
			if (UIAttackSdTypeEvent != null) {
				UIAttackSdTypeEvent(attackType);
			}
		}

		AttackSdType GetAttackTypeByFirstAndSecond(AttackFirstType firstType, AttackSecondType secondType)
		{
			AttackSdType attackType = AttackSdType.none;
			switch (firstType) {
				case AttackFirstType.A1:
					switch (secondType) {
						case AttackSecondType.N2:
							attackType = AttackSdType.A;
							break;
						case AttackSecondType.U2:
							attackType = AttackSdType.AU;
							break;
						case AttackSecondType.D2:
							attackType = AttackSdType.AD;
							break;
						case AttackSecondType.L2:
							attackType = AttackSdType.AL;
							break;
						case AttackSecondType.R2:
							attackType = AttackSdType.AR;
							break;
					}
					break;
				case AttackFirstType.HA1:
					switch (secondType) {
						case AttackSecondType.N2:
							attackType = AttackSdType.HA;
							break;
						case AttackSecondType.U2:
							attackType = AttackSdType.HAU;
							break;
						case AttackSecondType.D2:
							attackType = AttackSdType.HAD;
							break;
						case AttackSecondType.L2:
							attackType = AttackSdType.HAL;
							break;
						case AttackSecondType.R2:
							attackType = AttackSdType.HAR;
							break;
					}
					break;
				case AttackFirstType.AR1:
					switch (secondType) {
						case AttackSecondType.NR2:
							attackType = AttackSdType.ARN;
							break;
						case AttackSecondType.UR2:
							attackType = AttackSdType.ARU;
							break;
						case AttackSecondType.DR2:
							attackType = AttackSdType.ARD;
							break;
						case AttackSecondType.LR2:
							attackType = AttackSdType.ARL;
							break;
						case AttackSecondType.RR2:
							attackType = AttackSdType.ARR;
							break;
					}
					break;
				case AttackFirstType.HAR1:
					switch (secondType) {
						case AttackSecondType.NR2:
							attackType = AttackSdType.HARN;
							break;
						case AttackSecondType.UR2:
							attackType = AttackSdType.HARU;
							break;
						case AttackSecondType.DR2:
							attackType = AttackSdType.HARD;
							break;
						case AttackSecondType.LR2:
							attackType = AttackSdType.HARL;
							break;
						case AttackSecondType.RR2:
							attackType = AttackSdType.HARR;
							break;
					}
					break;
			}
			return attackType;
		}
	}

	[SerializeField]
	float dragThreshold = 0.2f;
	[SerializeField]
	float minHoldTime = 0.1f;

	float buttonRadious;

	AttackCheckModel CheckModel = new AttackCheckModel();
	AttackFirstCheckModel FirstModel = new AttackFirstCheckModel();
	AttackSecondCheckModel SecondModel = new AttackSecondCheckModel();

	[SerializeField]
	RectTransform AStickBg;
	[SerializeField]
	RectTransform AStick;
	Vector2 bgSize;
	Image bgImage;
	Image stickImage;
	Vector2 stickSize;
	Vector2 orgPosition;
	float radious;

	void Awake()
	{
		Image image = GetComponent<Image>();
		//image.alphaHitTestMinimumThreshold = 0.08f;
		buttonRadious = image.rectTransform.sizeDelta[0] / 2;
		dragThreshold *= buttonRadious;

		bgImage = AStickBg.GetComponent<Image>();
		stickImage = AStick.GetComponent<Image>();
		bgSize = AStickBg.sizeDelta;
		stickSize = AStick.sizeDelta;
		orgPosition = AStickBg.position;
		radious = (bgSize.x - stickSize.x) / 2;

		FirstModel.minHoldTime = minHoldTime;
		SecondModel.dragThreshold = dragThreshold;
	}

	void OnEnable()
	{
		CheckModel.OnEnable();
	}

	void OnDisable()
	{
		CheckModel.OnDisable();
	}

	public void Reset()
	{
		FirstModel.Reset();
		SecondModel.Reset();
		CheckModel.Reset();
	}

	void Update()
	{
		// 第一模块 检测是否是Hold
		FirstModel.Update();
	}

	public void OnPointerDown(PointerEventData data)
	{
		// 显示背景
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 1);
		AStickBg.position = data.pressPosition + new Vector2(bgSize.x / 2, 0) - new Vector2(0, bgSize.y / 2);
		stickImage.color = new Color(0.31f, 0.89f, 0.94f, 1);

		// 触摸开始
		// 第一模块
		FirstModel.OnTouchStart();
		// 第二模块
		SecondModel.OnTouchStart();
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		// 隐藏背景
		Color c = bgImage.color;
		bgImage.color = new Color(c.r, c.g, c.b, 0);
		AStickBg.position = orgPosition;
		AStick.position = orgPosition - new Vector2(AStickBg.sizeDelta.x / 2, 0) + new Vector2(0, AStickBg.sizeDelta.y / 2);
		stickImage.color = new Color(1, 1, 1, 0.8f);

		// 第一模块
		FirstModel.OnTouchEnd();
		// 第二模块
		SecondModel.OnTouchEnd();
		// 第三模块
		CheckModel.OnTouchEnd();

		Reset();
	}

	public void OnDrag(PointerEventData data)
	{
		// 准星滑动
		Vector2 localPointerPosition = data.position - data.pressPosition;
		if (localPointerPosition.magnitude <= radious) {
			AStick.position = data.position;
		}
		else {
			AStick.position = data.pressPosition + localPointerPosition.normalized * radious;
		}

		// 第二模块更新
		Vector2 pointer = data.position - data.pressPosition;
		SecondModel.OnDrag(pointer);
	}
}
