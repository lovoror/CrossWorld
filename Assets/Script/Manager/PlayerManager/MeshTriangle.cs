using UnityEngine;
using System.Collections;

public class MeshTriangle: MonoBehaviour
{
	//public float length = 10; // 三角形的长
	//public float angle = 5;  // 与长说成的度数

	private MeshFilter filter;
	private MeshRenderer meshRender;
	private Mesh mesh;

	void Awake()
	{
		// 获取GameObject的Filter组件
		filter = GetComponent<MeshFilter>();
		// 获取GameObject的MeshRenderer组件
		meshRender = GetComponent<MeshRenderer>();
		meshRender.sortingLayerName = "Default";
		meshRender.sortingOrder = 1;
		// 并新建一个mesh给它
		mesh = new Mesh();
		filter.mesh = mesh;
		mesh.name = "MyMesh";
	}

	public void UpdateMesh(float length, float degree)
	{
		if (length == 0) {
			meshRender.enabled = false;
			return;
		}
		else {
			meshRender.enabled = true;
		}

		float width = length * (Mathf.Deg2Rad * degree);
		// 为网格创建顶点数组
		Vector3[] vertices = new Vector3[3] {
            new Vector3(0, 0, 0),
            new Vector3(width, 0, length),
            new Vector3(0, 0, length),
        };

		mesh.vertices = vertices;

		// 通过顶点为网格创建三角形
		int[] triangles = new int[1 * 3]{
            0, 1, 2
        };

		mesh.triangles = triangles;
	}
}
