using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarScanner : MonoBehaviour {
    public Action<AStarNode[]> executeResultEvent;
    public Action<AStarScanner, AStarNode> checkScanEvent;
    [SerializeField] private bool useUpdate;
    private Dictionary<string, AStarNode> dictScannedSteps;
    private List<AStarNode> gainedSteps;
    private List<AStarNode> reTracedSteps;
    private List<AStarNode> fastestPathSteps;
    private Vector3 startingPosition;
    private Vector3 currentNodePosition;
    private Vector3 targetPosition;
    private Vector3 usedVector;
    private bool reScan;
    private bool scanStart;
    private bool reTraceStart;
    private bool reTrace;
    public void Init() {
        dictScannedSteps = new Dictionary<string, AStarNode>();
        gainedSteps = new List<AStarNode>();
        reTracedSteps = new List<AStarNode>();
        fastestPathSteps = new List<AStarNode>();
        currentNodePosition = new Vector3();
        usedVector = new Vector3(1, 0, 1);
        targetPosition = default;
    }

    public void StartScan(Vector3 targetPos) {
        startingPosition = RoundVector(new Vector3(currentNodePosition.x, currentNodePosition.y, currentNodePosition.z));
        targetPosition = RoundVector(new Vector3(targetPos.x, targetPos.y, targetPos.z));
        PrepareScan();
    }

    public void StartScan(Vector3 targetPos, Vector3 startingPos) {
        startingPosition = RoundVector(new Vector3(startingPos.x, startingPos.y, startingPos.z));
        targetPosition = RoundVector(new Vector3(targetPos.x, targetPos.y, targetPos.z));
        PrepareScan();
    }

    private void Update() {
        if (reScan) {
            reScan = false;
            PerformScanXZ();
        }
        if (reTrace) {
            reTrace = false;
            PerformReTrace();
        }
    }

    private void PrepareScan() {
        dictScannedSteps.Clear();
        gainedSteps.Clear();
        reTracedSteps.Clear();
        fastestPathSteps.Clear();
        currentNodePosition = startingPosition;
        gainedSteps.Add(new AStarNode(startingPosition, startingPosition, targetPosition));
        dictScannedSteps.Add(GetNodeID(startingPosition), gainedSteps[0]);
        PerformScanXZ();
    }

    public void PerformScanXZ() {
        scanStart = false;
        CheckNode(Vector3.forward + Vector3.left);
        CheckNode(Vector3.forward);
        CheckNode(Vector3.forward + Vector3.right);
        CheckNode(Vector3.right);
        CheckNode(Vector3.back + Vector3.right);
        CheckNode(Vector3.back);
        CheckNode(Vector3.back + Vector3.left);
        CheckNode(Vector3.left);
        if (dictScannedSteps.Count > 0) {
            AStarNode node = default;
            foreach (var keyPair in dictScannedSteps) {
                if (keyPair.Value.isACollider || gainedSteps.Contains(keyPair.Value)) {
                    continue;
                }
                if (!scanStart) {
                    node = keyPair.Value;
                    scanStart = true;
                } else {
                    if (keyPair.Value.hCost < node.hCost) {
                        if (keyPair.Value.GetFCost() <= node.GetFCost()) {
                            node = keyPair.Value;
                        }
                    }
                }
            }
            gainedSteps.Add(node);
            currentNodePosition = node.position;
            if (node.position == targetPosition) {
                fastestPathSteps.Add(new AStarNode(startingPosition, targetPosition, targetPosition));
                PerformReTrace();
            } else {
                if (useUpdate) {
                    reScan = true;
                } else {
                    PerformScanXZ();
                }
            }
        }
    }

    public void ConfirmNodeCollider(AStarNode node) {
        dictScannedSteps.Add(node.GetNodeID(), node);
    }

    private void PerformReTrace() {
        reTraceStart = false;
        reTracedSteps.Clear();
        ReTraceNode(Vector3.forward + Vector3.left);
        ReTraceNode(Vector3.forward);
        ReTraceNode(Vector3.forward + Vector3.right);
        ReTraceNode(Vector3.right);
        ReTraceNode(Vector3.back + Vector3.right);
        ReTraceNode(Vector3.back);
        ReTraceNode(Vector3.back + Vector3.left);
        ReTraceNode(Vector3.left);

        AStarNode node = default;
        for (int i = 0; i < reTracedSteps.Count; i++) {
            if (!reTraceStart) {
                node = reTracedSteps[i];
                reTraceStart = true;
            } else {
                if (reTracedSteps[i].gCost < node.gCost) {
                    node = reTracedSteps[i];
                }
            }
        }
        fastestPathSteps.Add(node);
        currentNodePosition = node.position;
        if (node.position == startingPosition) {
            fastestPathSteps.Reverse();
            executeResultEvent?.Invoke(fastestPathSteps.ToArray());
        } else {
            if (useUpdate) {
                reTrace = true;
            } else {
                PerformReTrace();
            }
        }
    }

    private void CheckNode(Vector3 nodePosition) {
        if (startingPosition == currentNodePosition + nodePosition) return;

        AStarNode node;
        if (!dictScannedSteps.ContainsKey(GetNodeID(currentNodePosition + nodePosition))) {
            node = new AStarNode(currentNodePosition, currentNodePosition + nodePosition, targetPosition, GetPreviousGCost());
            if (!dictScannedSteps.ContainsKey(node.GetNodeID())) {
                checkScanEvent.Invoke(this, node);
            }
        } else {
            node = dictScannedSteps[GetNodeID(currentNodePosition + nodePosition)];
            if (!gainedSteps.Contains(node)) {
                node.UpdateNode(currentNodePosition, currentNodePosition + nodePosition, targetPosition, GetPreviousGCost());
            }
        }
    }

    private void ReTraceNode(Vector3 nodePosition) {
        if (!dictScannedSteps.ContainsKey(GetNodeID(currentNodePosition + nodePosition))) return;
        if (!gainedSteps.Contains(dictScannedSteps[GetNodeID(currentNodePosition + nodePosition)])) return;
        if (fastestPathSteps.Contains(dictScannedSteps[GetNodeID(currentNodePosition + nodePosition)])) return;

        reTracedSteps.Add(dictScannedSteps[GetNodeID(currentNodePosition + nodePosition)]);
    }

    private Vector3 RoundVector(Vector3 vector3) {
        vector3.x = Mathf.Round(vector3.x) * usedVector.x;
        vector3.y = Mathf.Round(vector3.y) * usedVector.y;
        vector3.z = Mathf.Round(vector3.z) * usedVector.z;
        return vector3;
    }

    private string GetNodeID(Vector3 position) {
        return $"{position.x},{position.y},{position.z}";
    }

    private float GetPreviousGCost() {
        if (dictScannedSteps.ContainsKey(GetNodeID(currentNodePosition))) {
            return dictScannedSteps[GetNodeID(currentNodePosition)].gCost;
        } else {
            return 0;
        }
    }
}
