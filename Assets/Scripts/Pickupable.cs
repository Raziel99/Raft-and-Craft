using DG.Tweening;
using UnityEngine;

public abstract class Pickupable : MonoBehaviour
{
    public Raft PickupableRaft { get; set; }
    private bool onRaft;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plank"))
        {
            if (!onRaft)
            {
                Raft raft = other.GetComponentInParent<Raft>();

                if (raft != null)
                {
                    AddPickupable(raft);
                }
            }
        }
        else if (other.CompareTag("DestroyZone"))
        {
            Destroy(gameObject);
        }
    }
    protected virtual void AddPickupable(Raft raft)
    {
        transform.DOKill();
        VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Heavy);
        onRaft = true;
    }
}
