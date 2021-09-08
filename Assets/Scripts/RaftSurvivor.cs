using DG.Tweening;
using UnityEngine;

public class RaftSurvivor : Pickupable
{
    public float SwimHeight => swimHeight;

    [SerializeField]
    private bool isMain;
    [SerializeField]
    private float swimHeight;


    private Animator anim;
    private Tween delayTween;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        if (!isMain && PickupableRaft == null)
        {
            transform.localEulerAngles = Vector3.up * UnityEngine.Random.Range(0, 360f);
            Vector3 position = transform.position;
            position.y = swimHeight;
            transform.position = position;

            float sinkAmount = UnityEngine.Random.Range(0.2f, 0.27f);
            transform.GetChild(0).DOMoveY(position.y - sinkAmount, 0.5f).SetLoops(-1, LoopType.Yoyo);
            anim.SetTrigger("Swim");


            Transform help = transform.GetChild(1); 
            help.eulerAngles = Vector3.right * 16f;

            Vector3 angle = help.localEulerAngles;
            angle.z = 15f;
            Vector3 endAngle = angle;
            endAngle.z = -15f;
            help.localEulerAngles = angle;
            help.DOLocalRotate(endAngle, 2f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        } 
    }
    protected override void AddPickupable(Raft raft)
    {
        transform.GetChild(0).DOKill();
        transform.GetChild(0).localPosition = Vector3.zero;
        Transform help = transform.Find("Help");

        if(help != null)
        {
            help.gameObject.SetActive(false);
        }

        base.AddPickupable(raft);

        raft.AddPickupable(this, 0.5f);
    }
    public void SetOnRaft(bool onRaft, float delay)
    {
        delayTween.Kill();

        anim.SetBool("Jump", true);

        delay = delay > 0.2f ? delay - 0.2f : 0.01f;

        delayTween = DOVirtual.DelayedCall(delay, () =>
        {
            anim.SetBool("OnRaft", onRaft);
            anim.SetBool("Jump", false);
        });
    }
    public void SetVictory()
    {
        anim.SetInteger("Win", UnityEngine.Random.Range(1, 4));
    }
}