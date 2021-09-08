using DG.Tweening;
using TMPro;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool IsBuilt => currentPlanksCount >= maxPlanksCount;

    [SerializeField]
    private int maxPlanksCount;

    private TextMeshPro t_Progress;
    private ParticleSystem ps_Build;
    private Transform boxesParent;
    private Transform building;
    private Camera mainCamera;
    private int currentPlanksCount;
    private int builtPlanksCount;
    private int currentBox;
    private bool progressEnabled;

    private void Awake()
    {
        t_Progress = GetComponentInChildren<TextMeshPro>();
        currentPlanksCount = PlayerPrefs.GetInt($"PlanksCount{transform.GetSiblingIndex()}", 0);
        builtPlanksCount = currentPlanksCount;
        currentBox = currentPlanksCount / 36;
        building = transform.GetChild(1);
        ps_Build = GetComponentInChildren<ParticleSystem>();

        mainCamera = Camera.main;

        if (IsBuilt)
        {
            Build();
        }
        else
        {
            boxesParent = transform.GetChild(0).GetChild(1);

            for (int i = 0; i < currentBox; i++)
            {
                boxesParent.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }

            Transform planksParent = boxesParent.GetChild(currentBox).GetChild(1);

            for (int i = 0; i < currentPlanksCount % 36; i++)
            {
                planksParent.GetChild(i).gameObject.SetActive(true);
            }
        }
    }
    public void AddPlank(float delay)
    {
        int plankIndex = currentPlanksCount % 36;
        int boxIndex = currentBox;

        DOVirtual.DelayedCall(delay, () => boxesParent.GetChild(boxIndex).GetChild(1).GetChild(plankIndex).gameObject.SetActive(true)).OnComplete(() =>
        {
            if(!progressEnabled)
            {
                progressEnabled = true;
                t_Progress.text = $"{(int)(builtPlanksCount / (float)maxPlanksCount)}%";
                t_Progress.transform.LookAt(mainCamera.transform.position, mainCamera.transform.rotation * Vector3.up);
                Vector3 angle = t_Progress.transform.localEulerAngles;
                angle.x = -angle.x;
                angle.y += 180f;
                angle.z = -angle.z;
                t_Progress.transform.localEulerAngles = angle;
                t_Progress.color = Color.white;
            }


            builtPlanksCount++;
            VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Light);
            t_Progress.text = $"{(int)((float)builtPlanksCount / maxPlanksCount * 100f)}%";

            if(builtPlanksCount >= maxPlanksCount)
            {
                Build();
            }
        });

        currentPlanksCount += 1;
        PlayerPrefs.SetInt($"PlanksCount{transform.GetSiblingIndex()}", currentPlanksCount);

        if (currentPlanksCount % 36 == 0 && currentPlanksCount < maxPlanksCount)
        {
            currentBox++;
        }
    }
    private void Build()
    {
        ps_Build.Play();
        transform.GetChild(0).gameObject.SetActive(false);
        building.gameObject.SetActive(true);
        VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Heavy);
    }
    public Transform GetPlankTransform()
    {
        return boxesParent.GetChild(currentBox).GetChild(1).GetChild(currentPlanksCount % 36);
    }
    public void HideProgress()
    {
        t_Progress.DOFade(0, 0.5f);
    }
}
