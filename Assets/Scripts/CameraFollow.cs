using DG.Tweening;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance { get; private set; }
    public Transform[] CameraPositions => cameraPositions;

    [SerializeField]
    private Transform[] cameraPositions;
    [SerializeField]
    private Vector3[] zoomPositions;
    [SerializeField]
    private Transform followObject;
    [SerializeField]
    private float followLerpSpeed;

    private MoveController moveController;
    private Transform cameraTransform;
    private Vector3 cameraClampPosition1;
    private Vector3 cameraClampPosition2;
    private bool gameEnd;

    private void Awake()
    {
        Instance = this;
        cameraTransform = Camera.main.transform;
        moveController = MoveController.Instance;
        gameEnd = false;

        cameraClampPosition1 = zoomPositions[1];
        cameraClampPosition2 = zoomPositions[0];
    }
    private void Start()
    {
        RaftController.Instance.OnRaftHit += ShakeCamera;
        RaftController.Instance.OnRaftChange += OnRaftChange;
        Island.Instance.OnGameComplete += () =>
        {
            transform.GetChild(1).gameObject.SetActive(false);
            gameEnd = true;
        };
    }
    private void LateUpdate()
    {
        if (gameEnd) return;

        float lerpValue = moveController.CurrentSpeed / moveController.MaxSpeed;
        Vector3 targetLocalPosition = Vector3.Lerp(cameraClampPosition1, cameraClampPosition2, lerpValue);

        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, targetLocalPosition, followLerpSpeed * Time.deltaTime / 2f);

        Vector3 targerPosition = followObject.position;

        targerPosition.x = 0;

        transform.position = Vector3.Lerp(transform.position, targerPosition, followLerpSpeed * Time.deltaTime);
    }
    private void ShakeCamera()
    {
        Camera.main.DOShakePosition(0.1f, (Vector3.up + Vector3.right) * 0.3f);
    }
    public void SetCameraOnBuilding(out Vector3 cameraStartLocalPosition, out Vector3 cameraStartLocalRotation)
    {
        float duration = 1.5f;

        cameraStartLocalPosition = new Vector3(0, 94f, -126f);
        cameraStartLocalRotation = Vector3.right * 35f;

        Sequence seq = DOTween.Sequence();

        seq.Join(cameraTransform.DOLocalMove(cameraStartLocalPosition, duration));
        seq.Join(cameraTransform.DOLocalRotate(cameraStartLocalRotation, duration));
    }
    private void OnRaftChange(Raft[,] raft)
    {
        bool isBig = raft[0, 0] != null || raft[0, 1] != null || raft[0, 2] != null || raft[2, 0] != null || raft[2, 1] != null || raft[2, 2] != null;

        cameraClampPosition1 = isBig ? zoomPositions[3] : zoomPositions[1];
        cameraClampPosition2 = isBig ? zoomPositions[2] : zoomPositions[0];
    }
}
