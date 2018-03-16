using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimController : MonoBehaviour {

	bool isVisible;

	MeshTriangle I_MeshT;
	LineRenderer I_Line;

	GameObject[] children;

	void Awake()
	{
		I_MeshT = GetComponentInChildren<MeshTriangle>();
		I_Line = GetComponentInChildren<LineRenderer>();
	}

	void Start ()
	{
		SetVisible(false);
		//SetVisible(true);
		//UpdateAim(4, 5);
	}

	public void SetVisible(bool visible)
	{
		this.isVisible = visible;
		foreach (Transform child in transform) {
			child.gameObject.SetActive(visible);
		}
	}

	public void UpdateAim(float length, float degree)
	{
		I_MeshT.UpdateMesh(length, degree);
		I_Line.SetPosition(1, new Vector3(0, 0, length));
	}
}
