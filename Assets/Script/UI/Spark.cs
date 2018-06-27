using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour {
	public float fadeTime = 0.5f;
	SpriteRenderer spriteRender;
	public Color c;
	float tarAlpha;
	float curFadeTime = 0;

	void Awake()
	{
		spriteRender = transform.GetComponent<SpriteRenderer>();
	}

	void OnEnable()
	{
		tarAlpha = 1;
		spriteRender.color = new Color(c.r, c.g, c.b, 0);
		curFadeTime = 0;
	}

	void Start ()
	{
		tarAlpha = 0;
		curFadeTime = 0;
	}
	
	void Update ()
	{
		curFadeTime += Time.deltaTime;
		if (curFadeTime <= fadeTime) {
			float ca = curFadeTime / fadeTime;
			if(tarAlpha == 0) {
				ca = 1 - ca;
			}
			spriteRender.color = new Color(c.r, c.g, c.b, ca);
		}
		else {
			tarAlpha = 1 - tarAlpha;
			curFadeTime = 0;
		}
	}
}
