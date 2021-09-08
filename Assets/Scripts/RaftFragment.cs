using DG.Tweening;
using UnityEngine;

public class RaftFragment : MonoBehaviour
{
    public bool IsBuilt => built;

    private Transform gfx;
    private BoxCollider boxCollider;
    private Tween buildTween;
    private ParticleSystem ps_Break;
    private bool built;

    private void Awake()
    {
        buildTween.Kill();
        gfx = transform.GetChild(0);
        boxCollider = GetComponent<BoxCollider>();
        ps_Break = GetComponentInChildren<ParticleSystem>();
        boxCollider.enabled = false;
        built = false;
    }
    public void Build(float buildDelay)
    {
        built = true;
        boxCollider.enabled = true;

        buildTween = DOVirtual.DelayedCall(buildDelay, () =>
        {
            gfx.gameObject.SetActive(true);
        });
    }
    public void Break()
    {
        buildTween.Kill();
        built = false;
        boxCollider.enabled = false;
        ParticleSystem ps = Instantiate(ps_Break);
        ps.transform.position = transform.position;
        ps.Play();
        gfx.gameObject.SetActive(false);
    }
}
