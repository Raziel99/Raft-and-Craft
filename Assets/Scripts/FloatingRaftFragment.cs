using System;
using DG.Tweening;
using UnityEngine;

public class FloatingRaftFragment : MonoBehaviour
{
    private bool added;

    private void Awake()
    {
        int gfxIndex = UnityEngine.Random.Range(0, 100) < 50 ? 0 : 1;

        transform.GetChild(0).GetChild(gfxIndex).gameObject.SetActive(true);

        float sinkAmount = UnityEngine.Random.Range(0.2f, 0.27f);
        Vector3 position = transform.position;

        DOVirtual.DelayedCall(UnityEngine.Random.Range(0, 2f), () => 
        transform.DOMoveY(position.y - sinkAmount, 0.5f).SetLoops(-1, LoopType.Yoyo));
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Plank") || other.CompareTag("Character"))
        {
            if (!added)
            {
                VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Light);

                added = true;
                transform.DOKill();

                Vector3? plankPosition = RaftController.Instance.GetPlankPosition(out Raft raft, out Action<float> endAction);

                if(plankPosition != null)
                {
                    Transform parent = transform.parent;
                    transform.SetParent(raft.transform);

                    if (parent.childCount == 0)
                    {
                        Destroy(parent.parent.gameObject);
                    }

                    float duration = 0.3f;
                    endAction?.Invoke(duration);

                    transform.DOLocalJump(plankPosition.Value, 2f, 1, duration).OnComplete(() =>
                    {
                        Destroy(gameObject);
                    });
                }
                else
                {
                    float duration = 0.3f;
                    Transform bagPlank = RaftController.Instance.GetBagPlank(out Transform bag);

                    transform.SetParent(bag);
                    Sequence seq = DOTween.Sequence();
                    seq.Append(transform.DOLocalJump(bagPlank.transform.localPosition, 3f, 1, duration));
                    seq.Join(transform.DOLocalRotateQuaternion(bagPlank.transform.localRotation, duration));
                    seq.Join(transform.DOScale(bagPlank.transform.localScale, duration));
                    seq.OnComplete(() =>
                    {
                        bagPlank.gameObject.SetActive(true);
                        Destroy(gameObject);
                    });

                }
            }
        }
        else if (other.CompareTag("DestroyZone"))
        {
            DestroyRaftFragment();
        }
    }
    private void DestroyRaftFragment()
    {
        if (transform.parent.childCount == 1)
        {
            Destroy(transform.parent.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
