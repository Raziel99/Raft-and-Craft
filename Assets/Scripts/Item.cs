using DG.Tweening;
using UnityEngine;

public class Item : Pickupable
{
    public float HeightOnRaft => heightOnRaft;

    [SerializeField]
    private Vector3 minAngles;
    [SerializeField]
    private Vector3 maxAngles;
    [SerializeField]
    private float heightOnRaft;

    private void Awake()
    {
        transform.localEulerAngles = new Vector3(UnityEngine.Random.Range(minAngles.x, maxAngles.x), UnityEngine.Random.Range(minAngles.y, maxAngles.y), UnityEngine.Random.Range(minAngles.z, maxAngles.z));

        float sinkAmount = UnityEngine.Random.Range(0.2f, 0.27f);
        Vector3 position = transform.position;
        transform.DOMoveY(position.y - sinkAmount, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }
    protected override void AddPickupable(Raft raft)
    {
        base.AddPickupable(raft);

        raft.AddPickupable(this, 0.5f);
    }
}
