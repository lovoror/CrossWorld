using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAimController : MonoBehaviour
{
	bool isVisible = true;

	RangeMeshTriangle I_MeshT;

	GameObject[] children;

	void Awake()
	{
		I_MeshT = GetComponentInChildren<RangeMeshTriangle>();
	}

	void Start ()
	{
		SetVisible(false);
	}

	public void SetVisible(bool visible)
	{
		if (this.isVisible == visible) return;
		this.isVisible = visible;
		foreach (Transform child in transform) {
			child.gameObject.SetActive(visible);
		}
	}

	public void UpdateAim(Vector3 v3_start, float degree, float length)
	{
		I_MeshT.UpdateMesh(v3_start, length, degree);
	}
}
