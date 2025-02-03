using UnityEngine;

public struct AStarNode {
    public Vector3 position;
    public float gCost;
    public float hCost;
    public bool isACollider;

    public AStarNode(Vector3 rootPosition, Vector3 nodePosition, Vector3 targetPosition, float previousGCost = 0) {
        this.gCost = Mathf.Round(Vector3.Distance(rootPosition, nodePosition) * 10) + previousGCost;
        this.hCost = Mathf.Round(Vector3.Distance(nodePosition, targetPosition) * 10);
        this.position = nodePosition;
        this.isACollider = false;
    }

    public void UpdateNode(Vector3 rootPosition, Vector3 nodePosition, Vector3 targetPosition, float previousGCost = 0) {
        this.gCost = Mathf.Round(Vector3.Distance(rootPosition, nodePosition) * 10) + previousGCost;
        this.hCost = Mathf.Round(Vector3.Distance(nodePosition, targetPosition) * 10);
        this.position = nodePosition;
    }

    public string GetNodeID() {
        return $"{position.x},{position.y},{position.z}";
    }

    public float GetFCost() {
        return gCost + hCost;
    }
}
