using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private List<Collider> brokenPlanks;

    private void Awake()
    {
        brokenPlanks = new List<Collider>();

        if (transform.parent.name.ToUpper().Contains("LEVEL"))
        {
            transform.localEulerAngles = Vector3.up * UnityEngine.Random.Range(0, 360f);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Plank"))
        {
            Raft raft = other.GetComponentInParent<Raft>();

            if (raft != null && !brokenPlanks.Contains(other))
            {
                brokenPlanks.Add(other);
                raft.RemovePlank(other.GetComponent<RaftFragment>());
                MoveController.Instance.SlowDown();
                VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Heavy);
            }
        }
        else if (other.CompareTag("DestroyZone"))
        {
            Destroy(gameObject);
        }
    }
}
