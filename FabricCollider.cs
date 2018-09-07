using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabricCollider : MonoBehaviour {

	public Fabric fabric;
	public int x, y;

	public List<Transform> LineNeighbor;
	//public Transform[] DiagNeighbor;

	public float LineDist;
	//public float DiagDist;

	Vector2Int[] lineOffsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
	//Vector2Int[] diagOffsets = { new Vector2Int(1, 1), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(-1, -1) };

	void Start() {
		LineNeighbor = new List<Transform>(4);
		FindNeighbors();
		LineDist = Vector3.Distance(transform.position, LineNeighbor[0].position);
		//DiagDist = Vector3.Distance(transform.position, DiagNeighbor[0].position);
	}

	void FindNeighbors() {
		for (int i = 0; i < 4; i++) {
			if (ValidVertex(x + lineOffsets[i].x, y + lineOffsets[i].y)) {
				LineNeighbor.Add(fabric.childVertices[x + lineOffsets[i].x, y + lineOffsets[i].y]);
			}
		}
	}

	void FixedUpdate() {
		foreach (var n in LineNeighbor) {
			Vector3 dir = (n.localPosition - transform.localPosition).normalized;
			float dist = Mathf.Clamp(Vector3.Distance(transform.localPosition, n.localPosition), 0, LineDist);
			n.localPosition = transform.localPosition + dir * dist;
		}
	}

	bool ValidVertex(int x, int y) {
		return x >= 0 && x <= fabric.xSize && y >= 0 && y <= fabric.ySize;
	}
}
