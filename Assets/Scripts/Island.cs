using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SurvivorPosition
{
    public Vector3 Position => position;
    public bool IsBusy { get; set; }

    private Vector3 position;

    public SurvivorPosition(Vector3 position)
    {
        this.position = position;
        IsBusy = false;
    }
}

public class Island : MonoBehaviour
{
    public event Action OnGameComplete;

    public static Island Instance;

    [SerializeField]
    private Building[] buildings;
    [SerializeField]
    private Transform[] survivorsPositionsXZ;
    [SerializeField]
    private LayerMask layerMask;

    private SurvivorPosition[] survivorsPositions;
    private bool gameEnd;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DOVirtual.DelayedCall(1f, () =>
        {
            survivorsPositions = new SurvivorPosition[survivorsPositionsXZ.Length];

            for (int i = 0; i < survivorsPositionsXZ.Length; i++)
            {
                Vector3 position = survivorsPositionsXZ[i].position;

                position.y += 10f;

                if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 50f, layerMask))
                {
                    survivorsPositions[i] = new SurvivorPosition(hit.point);
                }
            }
        });
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Plank") || other.CompareTag("Character"))
        {
            if (!gameEnd)
            {
                gameEnd = true;
                DOVirtual.DelayedCall(0.5f, () => StartCoroutine(IE_Build()));
                OnGameComplete?.Invoke();
            }
        }
    }
    private IEnumerator IE_Build()
    {
        float fogStart = RenderSettings.fogStartDistance;
        float fogEnd = RenderSettings.fogEndDistance;

        DOTween.To(() => fogStart, x => fogStart = x, 150f, 1f).OnUpdate(() => RenderSettings.fogStartDistance = fogStart);
        DOTween.To(() => fogEnd, x => fogEnd = x, 256f, 1f).OnUpdate(() => RenderSettings.fogEndDistance = fogEnd);

        

        Transform cameraFollowTransform = CameraFollow.Instance.transform;
        Transform cameraTransform = Camera.main.transform;

        CameraFollow.Instance.SetCameraOnBuilding(out Vector3 cameraStartLocalPosition, out Vector3 cameraStartLocalRotation);
        UI_Manager.Instance.HideSky();

        var planks = RaftController.Instance.GetAllPlanks();
        (var survivors, var items) = RaftController.Instance.GetAllPickupables();

        float cameraMoveDuration = 1.5f;

        Sequence seq = DOTween.Sequence();
        Sequence cameraSeq = DOTween.Sequence();

        bool reached = false;

        if (survivors.Any() || items.Any())
        {
            cameraFollowTransform.DOMove(transform.GetChild(2).position, cameraMoveDuration).OnComplete(() =>
            {
                reached = true;
            });

            while (!reached) { yield return null; }

            if (survivors.Any())
            {
                for (int i = 0; i < survivors.Count; i++)
                {
                    SurvivorPosition position = GetSurvivorPosition();

                    position.IsBusy = true;

                    RaftSurvivor raftSurvivor = survivors[i];
                    raftSurvivor.transform.SetParent(null);

                    raftSurvivor.SetOnRaft(true, 2f);
                    raftSurvivor.SetVictory();
                    raftSurvivor.transform.DOJump(position.Position, 3f, 1, 2f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        raftSurvivor.transform.DORotate(Vector3.up * 180f, 0.5f);
                        VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Medium);
                     });
                    yield return new WaitForSeconds(0.2f);
                }
            }

            if (items.Any())
            {
                for (int i = 0; i < items.Count; i++)
                {
                    SurvivorPosition position = GetSurvivorPosition();

                    position.IsBusy = true;
                    Vector3 pos = position.Position;
                    pos.y += items[i].HeightOnRaft;

                    items[i].transform.SetParent(null);

                    items[i].transform.DOJump(pos, 3f, 1, 2f).SetEase(Ease.InOutSine).OnComplete(() => VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Medium));
                    yield return new WaitForSeconds(0.2f);
                }
            }

            yield return new WaitForSeconds(1.5f);
        }

        int buildingIndex = 0;

        while (buildingIndex < buildings.Length && buildings[buildingIndex].IsBuilt)
        {
            buildingIndex++;
        }

        if(buildingIndex < buildings.Length)
        {
            Transform[] cameraPositions = CameraFollow.Instance.CameraPositions;

            reached = false;

            cameraFollowTransform.DOMove(transform.position, cameraMoveDuration);
            cameraTransform.DOLocalMove(cameraPositions[buildingIndex].localPosition, cameraMoveDuration);
            cameraTransform.DOLocalRotate(cameraPositions[buildingIndex].localEulerAngles, cameraMoveDuration).OnComplete(() =>
            {
                reached = true;
            });

            while (!reached) { yield return null; }



            int bagPlanksCount = RaftController.Instance.BagPlanksCount;



            if (bagPlanksCount > 0)
            {
                Transform bag = RaftController.Instance.Bag;

                while (bagPlanksCount >= 0)
                {
                    Transform plankPrefab = bag.GetChild(6);

                    Transform plank = (bagPlanksCount > 7) ? Instantiate(plankPrefab, bag) : bag.GetChild(bagPlanksCount);
                    plank.localPosition = plankPrefab.localPosition;
                    plank.localRotation = plankPrefab.localRotation;
                    plank.localScale = plankPrefab.localScale;

                    if (buildingIndex >= buildings.Length)
                    {
                        plank.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (buildings[buildingIndex].IsBuilt)
                        {
                            buildingIndex++;

                            if (buildingIndex < buildings.Length)
                            {
                                yield return new WaitForSeconds(2.5f);

                                reached = false;

                                cameraFollowTransform.DOMove(transform.position, cameraMoveDuration);
                                cameraTransform.DOLocalMove(cameraPositions[buildingIndex].localPosition, cameraMoveDuration);
                                cameraTransform.DOLocalRotate(cameraPositions[buildingIndex].localEulerAngles, cameraMoveDuration).OnComplete(() =>
                                {
                                    reached = true;
                                });

                                while (!reached) { yield return null; }
                            }
                        }

                        if (buildingIndex < buildings.Length)
                        {
                            Transform buildingPlank = buildings[buildingIndex].GetPlankTransform();
                            float duration = 1.5f;
                            buildings[buildingIndex].AddPlank(duration);

                            Sequence s = DOTween.Sequence();
                            s.SetEase(Ease.InOutSine);
                            s.Append(plank.transform.DOJump(buildingPlank.position, 10f, 1, duration));
                            s.Join(plank.transform.DORotateQuaternion(buildingPlank.rotation, duration));
                            s.Join(plank.transform.DOScale(buildingPlank.localScale, duration));
                            s.OnComplete(() =>
                            {
                                plank.gameObject.SetActive(false);
                            });
                        }
                        else
                        {
                            plank.gameObject.SetActive(false);
                        }

                        bagPlanksCount--;
                    }

                    yield return null;
                }
            }

            if (planks.Any())
            {
                for (int i = 0; i < planks.Count; i++)
                {
                    RaftFragment plank = planks[i];

                    if (buildingIndex >= buildings.Length)
                    {
                        plank.gameObject.SetActive(false);
                    }
                    else
                    {
                        if (buildings[buildingIndex].IsBuilt)
                        {
                            buildingIndex++;

                            if (buildingIndex < buildings.Length)
                            {
                                yield return new WaitForSeconds(1.5f);

                                reached = false;

                                cameraFollowTransform.DOMove(transform.position, cameraMoveDuration);
                                cameraTransform.DOLocalMove(cameraPositions[buildingIndex].localPosition, cameraMoveDuration);
                                cameraTransform.DOLocalRotate(cameraPositions[buildingIndex].localEulerAngles, cameraMoveDuration).OnComplete(() =>
                                {
                                    reached = true;
                                });

                                while (!reached) { yield return null; }
                            }
                        }

                        if (buildingIndex < buildings.Length)
                        {
                            Transform buildingPlank = buildings[buildingIndex].GetPlankTransform();
                            float duration = 1.5f;
                            buildings[buildingIndex].AddPlank(duration);

                            Sequence s = DOTween.Sequence();
                            s.SetEase(Ease.InOutSine);
                            s.Append(plank.transform.DOJump(buildingPlank.position, 10f, 1, duration));
                            s.Join(plank.transform.DORotateQuaternion(buildingPlank.rotation, duration));
                            s.Join(plank.transform.DOScale(buildingPlank.localScale, duration));
                            s.OnComplete(() =>
                            {
                                plank.gameObject.SetActive(false);
                            });
                        }
                        else
                        {
                            plank.gameObject.SetActive(false);
                        }
                    }


                    yield return new WaitForSeconds(0.05f);
                }
            }
        }


        reached = false;

        cameraFollowTransform.DOMove(transform.position, cameraMoveDuration);
        cameraTransform.DOLocalMove(cameraStartLocalPosition, cameraMoveDuration);
        cameraTransform.DOLocalRotate(cameraStartLocalRotation, cameraMoveDuration).OnComplete(() =>
        {
            reached = true;
        });

        while (!reached) { yield return null; }

        yield return new WaitForSeconds(0.75f);

        Transform raftTransform = RaftController.Instance.transform;


        for (int i = 0; i < buildings.Length; i++)
        {
            buildings[i].HideProgress();
        }

        float startZ = raftTransform.position.z;
        raftTransform.DOMoveZ(startZ - 35f, 1f).SetEase(Ease.Linear).OnComplete(() => UI_Manager.Instance.EnableNext());
    }
    private SurvivorPosition GetSurvivorPosition()
    {
        for (int i = 0; i < survivorsPositions.Length; i++)
        {
            SurvivorPosition position = survivorsPositions[i];

            if(!position.IsBusy)
            {
                return position;
            }
        }

        return null;
    }
}
