using System.Collections.Generic;
using UnityEngine;

public class AStarManager : MonoBehaviour {
    [SerializeField] private List<AStarScanner> aStarScanners;
    [SerializeField] private List<Transform> colliderTransform;
    [SerializeField] private List<Vector3> colliderPositions;
    private List<string> colliderIDs;
    public void Init() {
        colliderIDs = new List<string>();
        for (int i = 0; i < aStarScanners.Count; i++) {
            aStarScanners[i].Init();
            aStarScanners[i].checkScanEvent += OnCheckScan;
        }
        for (int i = 0; i < colliderTransform.Count; i++) {
            colliderIDs.Add(GetNodeID(colliderTransform[i].position));
        }
        for (int i = 0; i < colliderPositions.Count; i++) {
            colliderIDs.Add(GetNodeID(colliderPositions[i]));
        }
    }

    public void AddAStarScanner(AStarScanner value) {
        if (aStarScanners.Contains(value)) return;

        value.Init();
        value.checkScanEvent += OnCheckScan;
        aStarScanners.Add(value);
    }

    public void AddColliderTransform(Transform value) {
        colliderIDs.Add(GetNodeID(value.position));
    }

    public void AddColliderVector3(Vector3 value) {
        colliderIDs.Add(GetNodeID(value));
    }

    public void SetColliderIDs(List<string> value) {
        colliderIDs = value;
    }

    public List<string> CreateColliderIDs(List<Vector3> colliderPositions) {
        List<string> newColliderIDs = new List<string>();
        for (int i = 0; i < colliderPositions.Count; i++) {
            newColliderIDs.Add(GetNodeID(colliderPositions[i]));
        }
        return newColliderIDs;
    }

    private void OnCheckScan(AStarScanner aStarScanner, AStarNode aStarNode) {
        for (int i = 0; i < colliderIDs.Count; i++) {
            if (CheckNodeCollider(ref aStarNode, i)) {
                break;
            }
        }
        aStarScanner.ConfirmNodeCollider(aStarNode);
    }

    private string GetNodeID(Vector3 position) {
        return $"{position.x},{position.y},{position.z}";
    }

    private bool CheckNodeCollider(ref AStarNode aStarNode, int index) {
        aStarNode.isACollider = aStarNode.GetNodeID() == colliderIDs[index];
        return aStarNode.isACollider;
    }
}
