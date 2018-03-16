using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	//坐标点数组
	Vector3[] points = new Vector3[5];
	public int pointIndex = 1;
	Vector3 nextPosition;

	void Start()
	{
		points[0] = new Vector3(5, 0.5f, 5);
		points[1] = new Vector3(5, 0.5f, -5);
		points[2] = new Vector3(-5, 0.5f, 5);
		points[3] = new Vector3(-5, 0.5f, 5);
	}

	void Update()
	{
		//new WaitForSeconds(3);
		RandMove();
	}

	void RandMove()
	{

		nextPosition = points[pointIndex];
		transform.position = Vector3.MoveTowards(transform.position, nextPosition, Time.deltaTime);  
		float dst = Vector3.Distance(this.transform.position, nextPosition);
		if ( dst < 0.1) { //判断距离 
			//更换下一个坐标点
			pointIndex = Random.Range(0, 4);//随机范围{0,1,2,3}
		}
	}
}
