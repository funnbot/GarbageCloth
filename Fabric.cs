using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Fabric : MonoBehaviour {
	public int xSize;
	public int ySize;
	public float resolution = 1f;
	[SerializeField]
	public Transform[, ] childVertices;
	public Mesh mesh;
	public float VertexDistance;

	void Awake() {
		Generate();
	}

	[ContextMenu("Generate")]
	void Generate() {
		DestroyChildren();
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Mesh";
		CreateChildren();
		VertexDistance = Vector3.Distance(childVertices[0, 0].localPosition, childVertices[0, 1].localPosition);
		UpdateVertices();
		CreateUV();
		CreateTriangles();
	}

	void FixedUpdate() {
		// ClampVertexMovement();
		UpdateVertices();
	}

	Vector2Int[] offsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };

	void ClampVertexMovement() {
		for (int y = 0; y < ySize; y++) {
			for (int x = 0; x < xSize; x++) {
				ClampVertex(x, y);
			}
		}
	}

	void ClampVertex(int x, int y) {
		var c1 = childVertices[x, y];
		c1.localEulerAngles = Vector3.zero;
		for (int i = 0; i < 4; i++) {
			if (ValidVertex(x + offsets[i].x, y + offsets[i].y)) {
				var c2 = childVertices[x + offsets[i].x, y + offsets[i].y];
				Vector3 dir = (c2.localPosition - c1.localPosition).normalized;
				float dist = Mathf.Clamp(Vector3.Distance(c1.localPosition, c2.localPosition), 0, VertexDistance);
				c2.localPosition = c1.localPosition + dir * dist;
			}
		}
	}

	bool ValidVertex(int x, int y) {
		return x >= 0 && x <= xSize && y >= 0 && y <= ySize;
	}

	void UpdateVertices() {
		Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		for (int y = 0, i = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				vertices[i] = childVertices[x, y].localPosition;
			}
		}
		mesh.vertices = vertices;
		mesh.RecalculateNormals();
	}

	void CreateUV() {
		Vector2[] uv = new Vector2[(xSize + 1) * (ySize + 1)];
		for (int y = 0, i = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				uv[i] = new Vector2((float) x / xSize, (float) y / ySize) * resolution;
			}
		}
		mesh.uv = uv;
	}

	void CreateTriangles() {
		int[] triangles = new int[xSize * ySize * 6];
		for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) {
			for (int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
	}

	void CreateChildren() {
		childVertices = new Transform[xSize + 1, ySize + 1];
		for (int y = 0, i = 0; y <= ySize; y++) {
			for (int x = 0; x <= xSize; x++, i++) {
				var child = new GameObject("Vertex").transform;

				child.gameObject.AddComponent<SphereCollider>();
				child.gameObject.AddComponent<Rigidbody>();

				//if (xSize % 4 == 0 && ySize % 4 == 0) {
				FabricCollider col = child.gameObject.AddComponent<FabricCollider>();
				col.fabric = this;
				col.x = x;
				col.y = y;
				//}

				child.SetParent(transform);
				child.localPosition = new Vector3((x - xSize / 2) + 0.5f, 0, (y - ySize / 2) + 0.5f) * resolution;
				child.localScale *= 0.95f * resolution;

				childVertices[x, y] = child;
			}
		}
	}

	void DestroyChildren() {
		while (transform.childCount > 0) {
			DestroyImmediate(transform.GetChild(0).gameObject);
		}
	}
}
