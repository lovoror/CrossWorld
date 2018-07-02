using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirthPointManager : MonoBehaviour
{
	static Transform[] alertPoints;

	void Start ()
	{
		alertPoints = new Transform[transform.childCount];
		int index = 0;
		foreach(Transform point in transform){
			//int index = int.Parse(point.name.Substring(5)) - 1;
			//Transform alertPoint = point.Find("Alert");
			//alertPoints[index] = alertPoint;
			alertPoints[index++] = point;
		}
	}

	void Update ()
	{
		
	}

	public static void ShowBirthPoint(int index, bool show)
	{
		if (index >= alertPoints.Length) {
			Debug.LogError("index is out of range.");
			return;
		}
		alertPoints[index].gameObject.SetActive(show);
	}
}
