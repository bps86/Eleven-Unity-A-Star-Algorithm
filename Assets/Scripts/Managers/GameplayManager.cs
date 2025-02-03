using UnityEngine;

public class GameplayManager : MonoBehaviour {
    [SerializeField] private AStarManager aStarManager;
    [SerializeField] private AStarScanner aStarScanner;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private bool startFromAwake;

    private void Awake() {
        aStarManager.Init();
        if (startFromAwake) {
            Test();
        }
    }

    public void Test() {
        aStarScanner.StartScan(targetTransform.position);
    }
}
