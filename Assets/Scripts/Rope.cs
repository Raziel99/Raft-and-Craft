using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField]
    private RaftSide[] connectingRaftsSides;
    [SerializeField]
    private Vector2Int[] connectingRaftsIndices;
    [SerializeField]
    private Vector2Int[] blockingRaftsIndices;

    private GameObject gfx;

    private void Awake()
    {
        gfx = transform.GetChild(0).gameObject;
    }
    private void Start()
    {
        RaftController.Instance.OnRaftChange += OnRaftChange;
    }
    private void OnRaftChange(Raft[,] rafts)
    {
        foreach (Vector2Int indices in connectingRaftsIndices)
        {
            if (rafts[indices.x, indices.y] == null)
            {
                gfx.SetActive(false);
                return;
            }
        }
        foreach (Vector2Int indices in blockingRaftsIndices)
        {
            if (rafts[indices.x, indices.y] != null)
            {
                gfx.SetActive(false);
                return;
            }
        }

        Raft raft1 = rafts[connectingRaftsIndices[0].x, connectingRaftsIndices[0].y];
        Raft raft2 = rafts[connectingRaftsIndices[1].x, connectingRaftsIndices[1].y];

        if(raft1 == null || raft2 == null || !raft1.HasAnyPlank || !raft2.HasAnyPlank)
        {
            return;
        }

        Vector3 raft1SideCenter = raft1.GetMidPoint(connectingRaftsSides[0]);
        Vector3 raft2SideCenter = raft2.GetMidPoint(connectingRaftsSides[1]);

        raft1SideCenter.y = 0;
        raft2SideCenter.y = 0;

        Vector3 ropeVector = raft2SideCenter - raft1SideCenter;
        Vector3 ropePosition = (raft1SideCenter + raft2SideCenter) / 2f;
        
        transform.position = ropePosition;
        transform.LookAt(raft2SideCenter);
        Vector3 scale = Vector3.one;
        scale.z = ropeVector.magnitude;

        transform.localScale = scale;

        gfx.SetActive(true);
    }
}
