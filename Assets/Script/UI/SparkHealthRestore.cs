using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkHealthRestore : MonoBehaviour {
	public string context = "+0";

	TextMesh textMesh;
	MeshRenderer meshRender;
	Color c;
	float a;
	float fadeTime = 2;
	float starTime = 0;

	void Awake()
	{
		textMesh = GetComponent<TextMesh>();
		meshRender = GetComponent<MeshRenderer>();
	}

	void Start ()
	{
		c = textMesh.color;
		a = c.a;
		starTime = Time.time;
		textMesh.text = context;
		meshRender.sortingLayerName = "UI";
	}

	void Update()
	{
		if (a > 0) {
			textMesh.color = new Color(c.r, c.g, c.b, a);
			a = 1 - (Time.time - starTime) / fadeTime;
		}
		else {
			Destroy(transform.gameObject);
		}
	}
}
