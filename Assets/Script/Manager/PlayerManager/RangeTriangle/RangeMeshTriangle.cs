using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RangeMeshTriangle : MonoBehaviour
{
	//public float length = 10; // 三角形的长
	//public float angle = 5;  // 与长说成的度数

	private MeshFilter filter;
	private MeshRenderer meshRender;
	private Mesh mesh;

	LayerMask wallLayerMask;

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
		wallLayerMask = LayerMask.GetMask("Wall");
	}

	float delt = 1;
	public void UpdateMesh(Vector3 v3_start, float length, float degree)
	{
		if (length == 0) {
			meshRender.enabled = false;
			return;
		}
		else {
			meshRender.enabled = true;
		}

		List<Vector3> l_vertices = new List<Vector3>();
		l_vertices.Add(Vector3.zero);
		for (float r = v3_start.y; r < v3_start.y + degree + delt; r += delt) {
			float tr = r;
			if (r > v3_start.y + degree) {
				tr = v3_start.y + degree;
			}
			tr *= Mathf.Deg2Rad;
			// 得出最远可能到达的点
			Vector3 v3 = new Vector3(length * Mathf.Sin(tr), 0, length * Mathf.Cos(tr));
			// 如果撞到墙则需要调整长度
			Vector3 dir = transform.TransformPoint(v3) - transform.position;
			Vector3 hitPoint = GetHitWallPoint(transform.position, dir, length * 10);
			if (hitPoint.x != -1000) {
				float tl = (hitPoint - transform.position).magnitude / 10;
				v3 = v3.normalized * tl;
			}
			l_vertices.Add(v3);
		}
		Vector3[] vertices = new Vector3[l_vertices.Count];
		for (int i = 0; i < vertices.Length; ++i) {
			vertices[i] = l_vertices[i];
		}

		mesh.vertices = vertices;

		// 通过顶点为网格创建三角形
		int[] triangles = new int[vertices.Length * 3];
		for (int i = 2; i < vertices.Length; ++i) {
			triangles[3 * i - 6] = 0;
			triangles[3 * i - 5] = i - 1;
			triangles[3 * i - 4] = i;
		}
		mesh.triangles = triangles;
	}

	Vector3 GetHitWallPoint(Vector3 org, Vector3 dir, float maxDist)
	{
		Vector3 hitPoint = new Vector3(-1000, -1000, -1000);
		// 返回瞄准线与墙的交接点
		Ray ray = new Ray();
		ray.origin = org;
		ray.direction = dir;
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, maxDist, wallLayerMask)) {
			hitPoint = hitInfo.point;
		}
		return hitPoint;
	}
}
