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
