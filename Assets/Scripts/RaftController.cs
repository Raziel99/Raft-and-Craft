using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class RaftController : MonoBehaviour
{
    public event Action OnGameFail;

    public event Action<Raft[,]> OnRaftChange;
    public event Action OnRaftHit;
    public static RaftController Instance { get; private set; }
    public Transform Bag { get; private set; }
    public int BagPlanksCount { get; private set; }
    public float RaftSize => raftSize;
    public bool Built
    {
        get
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (rafts[i, j] == null || !rafts[i, j].IsBuilt)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
    public bool HasAnyPlank
    {
        get
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (rafts[i, j] != null && rafts[i, j].HasAnyPlank)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    [SerializeField]
    private Transform p_Rope;
    [SerializeField]
    private Transform[] particlesParents;

    [SerializeField]
    private Raft startRaft;
    [SerializeField]
    private RaftSurvivor startSurvivor;
    [SerializeField]
    private Collider c_StartSurvivor;
    [SerializeField]
    private Raft p_Raft;
    [SerializeField]
    private float raftSize;
    [SerializeField]
    private float drowningTime;

    private Raft[,] rafts;
    private Transform raftsParent;
    private Transform pickupablesParent;
    private Coroutine drownCoroutine;
    private Rigidbody rb;
    private ParticleSystem[][] particles;
    private GameObject bagGO;
    private bool[] bagPlanksBuilt;


    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody>();

        rafts = new Raft[3, 3];

        startRaft.Init(this, 1, 1, 270);

        particles = new ParticleSystem[3][];

        for (int i = 0; i < 3; i++)
        {
            particles[i] = new ParticleSystem[3];

            for (int j = 0; j < 3; j++)
            {
                particles[i][j] = particlesParents[i].GetChild(0).GetChild(j).GetComponent<ParticleSystem>();
            }
        }

        bagGO = startSurvivor.transform.GetChild(0).GetChild(0).GetChild(0).gameObject;
        Bag = bagGO.transform.GetChild(0);
        bagPlanksBuilt = new bool[6];
    }
    private void Start()
    {
        Island.Instance.OnGameComplete += OnGameComplete;
        OnRaftChange += RaftChange;

        for (int i = 0; i < 8; i++)
        {
            startRaft.GetFreePlank().Build(0);
        }

        rafts[1, 1] = startRaft;
        raftsParent = transform.GetChild(0);
        pickupablesParent = transform.GetChild(1);
        AddPickupable(startRaft, startSurvivor, 0.01f);

        OnRaftChange?.Invoke(rafts);
    }
    private void Update()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
    private void GetNextRaftIndices(out int x, out int y)
    {
        if (rafts[1, 1] == null && ((rafts[2, 1] != null || rafts[1, 2] != null || rafts[1, 0] != null || rafts[0, 1] != null) ||
                                    (rafts[2, 1] == null && rafts[1, 2] == null && rafts[1, 0] == null && rafts[0, 1] == null &&
                                     rafts[0, 0] == null && rafts[2, 2] == null && rafts[2, 0] == null && rafts[0, 2] == null)))
        {
            x = 1;
            y = 1;
            return;
        }

        var builtRaftsIndices = new List<(int i, int j)>();
        var availableRaftsIndices = new List<(int i, int j)>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (rafts[i, j] != null && rafts[i, j].IsBuilt)
                {
                    builtRaftsIndices.Add((i, j));
                }
            }
        }

        if (builtRaftsIndices.Count == 9)
        {
            x = -1;
            y = -1;
            return;
        }

        foreach ((int i, int j) in builtRaftsIndices)
        {
            if (i + 1 < 3)
            {
                if (rafts[i + 1, j] == null)
                {
                    availableRaftsIndices.Add((i + 1, j));
                }
            }
            if (i - 1 >= 0)
            {
                if (rafts[i - 1, j] == null)
                {
                    availableRaftsIndices.Add((i - 1, j));
                }
            }
            if (j + 1 < 3)
            {
                if (rafts[i, j + 1] == null)
                {
                    availableRaftsIndices.Add((i, j + 1));
                }
            }
            if (j - 1 >= 0)
            {
                if (rafts[i, j - 1] == null)
                {
                    availableRaftsIndices.Add((i, j - 1));
                }
            }
        }

        (x, y) = availableRaftsIndices[UnityEngine.Random.Range(0, availableRaftsIndices.Count)];
    }
    private int GetRaftAngle(int x, int y)
    {
        if (y == 1)
        {
            if (rafts[x, 2] != null && rafts[x, 0] == null)
            {
                return 90;
            }
        }

        if (y == 0)
        {
            return 90;
        }

        return 270;
    }
    public void BreakPlank()
    {
        OnRaftChange?.Invoke(rafts);
        OnRaftHit?.Invoke();
    }
    public void RemovePickupable(int x, int y)
    {
        Raft raft = rafts[x, y];

        if (raft != null)
        {
            Raft freeRaft = GetFreeRaft(out bool isMain, raft);

            if (raft.IsBusy)
            {
                if (freeRaft != null)
                {
                    if (freeRaft.RaftPickupable != null)
                    {
                        freeRaft.RaftPickupable.transform.DOKill();
                        Destroy(freeRaft.RaftPickupable.gameObject);
                    }

                    freeRaft.RaftPickupable = raft.RaftPickupable;
                    raft.RaftPickupable = null;
                    freeRaft.RaftPickupable.PickupableRaft = freeRaft;
                    freeRaft.RaftPickupable.transform.SetParent(pickupablesParent);
                    Vector3 pickupablePosition = freeRaft.transform.localPosition;
                    if (freeRaft.RaftPickupable is Item item)
                    {
                        pickupablePosition += Vector3.up * item.HeightOnRaft;
                    }
                    else if(freeRaft.RaftPickupable is RaftSurvivor survivor)
                    {
                        survivor.SetOnRaft(true, 0.5f);
                    }
                    freeRaft.RaftPickupable.transform.DOKill();
                    freeRaft.RaftPickupable.transform.DOLocalJump(pickupablePosition, 2f, 1, 0.5f);
                }
            }


            if (isMain && freeRaft == null)
            {
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        Raft currentRaft = rafts[i, j];

                        if (currentRaft != null)
                        {
                            if (currentRaft.IsBusy && !ReferenceEquals(startSurvivor, raft.RaftPickupable))
                            {
                                currentRaft.RaftPickupable.DOKill();
                                Destroy(currentRaft.RaftPickupable.gameObject);
                            }

                            if (i == 1 && j == 0)
                            {
                                Destroy(currentRaft.gameObject);
                                rafts[i, j] = null;
                                OnRaftChange?.Invoke(rafts);
                            }
                            //if (!ReferenceEquals(currentRaft, startSurvivor.PickupableRaft))
                            //{
                            //    Destroy(currentRaft.gameObject);
                            //    rafts[i, j] = null;
                            //}
                        }
                    }
                }

                c_StartSurvivor.isTrigger = false;
                startSurvivor.PickupableRaft.RaftPickupable = null;
                startSurvivor.PickupableRaft = null;
                MoveController.Instance.ChangeMaxSpeed(false);
                startSurvivor.transform.DOKill();
                startSurvivor.transform.DOLocalJump(Vector3.up * -1.9f + Vector3.forward * -1.673f, 2f, 1, 0.5f);
                startSurvivor.SetOnRaft(false, 0.5f);
            }
            else
            {
                if (raft.IsBusy)
                {
                    raft.RaftPickupable.DOKill();
                    Destroy(raft.RaftPickupable.gameObject);
                }
                //Destroy(raft.gameObject);
            }
        }
    }
    public void BreakRaft(int x, int y)
    {
        if (rafts[x, y] != null)
        {
            if (rafts[x, y].IsBusy)
            {
                RemovePickupable(x, y);
            }

            Destroy(rafts[x, y].gameObject);
            rafts[x, y] = null;

            OnRaftChange?.Invoke(rafts);

            if (!HasAnyPlank)
            {
                drownCoroutine = StartCoroutine(IE_Drown());
            }
        }
    }
    public void AddPickupable(Raft raft, Pickupable pickupable, float duration)
    {
        Raft selectedRaft = !raft.IsBusy && raft.IsBuilt ? raft : GetFreeRaft(out _);

        if (selectedRaft != null)
        {
            if (selectedRaft.RaftPickupable != null)
            {
                Destroy(selectedRaft.RaftPickupable.gameObject);
            }

            selectedRaft.RaftPickupable = pickupable;
            pickupable.transform.SetParent(pickupablesParent);
            Vector3 pickupablePosition = selectedRaft.transform.localPosition;
            if (pickupable is Item item)
            {
                pickupablePosition.y = item.HeightOnRaft;
            }
            else if (pickupable is RaftSurvivor survivor)
            {
                survivor.SetOnRaft(true, duration);
                pickupablePosition.y = 0.2f;
            }
            pickupable.PickupableRaft = selectedRaft;
            Vector3 angle = Vector3.zero;
            pickupable.DOKill();
            pickupable.transform.DOLocalJump(pickupablePosition, 2f, 1, duration);
            pickupable.transform.DOLocalRotate(angle, duration);
        }
        else
        {
            pickupable.transform.DOKill();
            Destroy(pickupable.gameObject);
        }
    }
    private Raft GetFreeRaft(out bool isMain, Raft currentRaft = null)
    {
        var freeRafts = new List<Raft>();
        var barrelRafts = new List<Raft>();
        var survivorRafts = new List<Raft>();

        isMain = currentRaft != null && ReferenceEquals(startSurvivor, currentRaft.RaftPickupable);

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Raft raft = rafts[i, j];

                if (raft != null && raft.IsBuilt)
                {
                    if (!raft.IsBusy)
                    {
                        freeRafts.Add(raft);
                    }
                    else
                    {
                        if (raft.RaftPickupable is RaftSurvivor survivor && !ReferenceEquals(survivor, startSurvivor))
                        {
                            survivorRafts.Add(raft);
                        }
                        else if (raft.RaftPickupable is Item)
                        {
                            barrelRafts.Add(raft);
                        }
                    }
                }
            }
        }

        if (currentRaft != null)
        {
            if (freeRafts.Contains(currentRaft))
            {
                freeRafts.Remove(currentRaft);
            }
            else if (barrelRafts.Contains(currentRaft))
            {
                barrelRafts.Remove(currentRaft);
            }
            else if (survivorRafts.Contains(currentRaft))
            {
                survivorRafts.Remove(currentRaft);
            }
        }

        if (freeRafts.Any())
        {
            return freeRafts[UnityEngine.Random.Range(0, freeRafts.Count)];
        }

        if (barrelRafts.Any() && ((currentRaft != null && currentRaft.RaftPickupable is RaftSurvivor) || currentRaft == null))
        {
            return barrelRafts[UnityEngine.Random.Range(0, barrelRafts.Count)];
        }

        if (currentRaft != null && survivorRafts.Any() && isMain)
        {
            return survivorRafts[UnityEngine.Random.Range(0, survivorRafts.Count)];
        }

        return null;
    }
    public Vector3? GetPlankPosition(out Raft raft, out Action<float> endAction)
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                raft = rafts[i, j];

                if (raft != null && !raft.IsBuilt)
                {
                    RaftFragment plank = raft.GetFreePlank();
                    endAction = (float x) =>
                    {
                        plank.Build(x);

                        if (drownCoroutine != null)
                        {
                            StopCoroutine(drownCoroutine);
                        }

                        if (startSurvivor.PickupableRaft == null)
                        {
                            Raft raft = GetFreeRaft(out _);

                            if (raft != null)
                            {
                                c_StartSurvivor.isTrigger = true;
                                AddPickupable(raft, startSurvivor, 0.5f);
                                MoveController.Instance.ChangeMaxSpeed(true);
                            }
                        }
                        OnRaftChange?.Invoke(rafts);
                    };
                    return plank.transform.localPosition;
                }
            }
        }

        GetNextRaftIndices(out int x, out int y);

        if (x >= 0 && y >= 0)
        {
            raft = Instantiate(p_Raft, raftsParent);
            raft.transform.localPosition = new Vector3(x * raftSize - raftSize, 0, y * raftSize - raftSize);
            int rotation = GetRaftAngle(x, y);
            raft.transform.localEulerAngles = Vector3.up * rotation;
            raft.Init(this, x, y, rotation);
            rafts[x, y] = raft;


            RaftFragment plank = raft.GetFreePlank();
            endAction = (float x) =>
            {
                plank.Build(x);

                if (drownCoroutine != null)
                {
                    StopCoroutine(drownCoroutine);
                }

                if (startSurvivor.PickupableRaft == null)
                {
                    Raft raft = GetFreeRaft(out _);

                    if (raft != null)
                    {
                        c_StartSurvivor.isTrigger = true;
                        AddPickupable(raft, startSurvivor, 0.5f);
                        MoveController.Instance.ChangeMaxSpeed(true);
                    }
                }
                OnRaftChange?.Invoke(rafts);
            };
            return plank.transform.localPosition;
        }

        raft = null;
        endAction = null;
        return null;
    }
    private IEnumerator IE_Drown()
    {
        float drownTimer = 0;
        float vibrationTimer = 0;

        while (true)
        {
            drownTimer += Time.deltaTime;
            vibrationTimer += Time.deltaTime;

            if(vibrationTimer >= 0.5f)
            {
                VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Heavy);
                vibrationTimer = 0;
            }
            if (drownTimer >= drowningTime)
            {
                OnGameFail?.Invoke();

                transform.DOMoveY(-3f, 0.5f);
                break;
            }

            yield return null;
        }
    }
    public List<RaftFragment> GetAllPlanks()
    {
        var allPlanks = new List<RaftFragment>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Raft raft = rafts[i, j];

                if (raft != null && !ReferenceEquals(raft, startSurvivor.PickupableRaft))
                {
                    var raftPlanks = raft.GetAllPlanks();

                    if (raftPlanks.Count > 0)
                    {
                        allPlanks.AddRange(raftPlanks);
                    }
                }
            }
        }

        return allPlanks;
    }
    public (List<RaftSurvivor> survivors, List<Item> items) GetAllPickupables()
    {
        var survivors = new List<RaftSurvivor>();
        var items = new List<Item>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Raft raft = rafts[i, j];

                if (raft != null && raft.IsBusy)
                {
                    if (raft.RaftPickupable is RaftSurvivor && !ReferenceEquals(raft.RaftPickupable, startSurvivor))
                    {
                        survivors.Add((raft.RaftPickupable as RaftSurvivor));
                    }
                    else if (raft.RaftPickupable is Item)
                    {
                        items.Add((raft.RaftPickupable as Item));
                    }
                }
            }
        }

        return (survivors, items);
    }
    private void OnGameComplete()
    {
        if (drownCoroutine != null)
        {
            StopCoroutine(drownCoroutine);
        }

        transform.GetChild(2).gameObject.SetActive(false);
    }
    private void RaftChange(Raft[,] rafts)
    {
        Raft leftRaft = null;
        Raft middleRaft = null;
        Raft rightRaft = null;

        for (int i = 2; i >= 0; i--)
        {
            if (rafts[0, i] != null && rafts[0, i].HasAnyPlank)
            {
                leftRaft = rafts[0, i];
            }
            if (rafts[1, i] != null && rafts[1, i].HasAnyPlank)
            {
                middleRaft = rafts[1, i];
            }
            if (rafts[2, i] != null && rafts[2, i].HasAnyPlank)
            {
                rightRaft = rafts[2, i];
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                particles[i][j].Stop();
            }
        }

        if (leftRaft != null)
        {
            particlesParents[0].position = leftRaft.transform.position;

            Vector3 plankLocalPosition = leftRaft.GetBackPlankLocalPosition();

            particles[0][2].transform.localPosition = plankLocalPosition;

            for (int i = 0; i < 3; i++)
            {
                particles[0][i].Play();
            }
        }
        if (middleRaft != null)
        {
            particlesParents[1].position = middleRaft.transform.position;

            Vector3 plankLocalPosition = middleRaft.GetBackPlankLocalPosition();

            particles[1][2].transform.localPosition = plankLocalPosition;

            for (int i = 0; i < 3; i++)
            {
                particles[1][i].Play();
            }
        }
        if (rightRaft != null)
        {
            particlesParents[2].position = rightRaft.transform.position;

            Vector3 plankLocalPosition = rightRaft.GetBackPlankLocalPosition();

            particles[2][2].transform.localPosition = plankLocalPosition;

            for (int i = 0; i < 3; i++)
            {
                particles[2][i].Play();
            }
        }
    }
    public Transform GetBagPlank(out Transform bag)
    {
        bag = this.Bag;

        if(BagPlanksCount == 0)
        {
            bagGO.SetActive(true);
        }

        BagPlanksCount++;
        OnRaftChange?.Invoke(rafts);

        for (int i = 0; i < 6; i++)
        {
            if(!bagPlanksBuilt[i])
            {
                bagPlanksBuilt[i] = true;
                return bag.GetChild(i).transform;
            }
        }

        return bag.GetChild(6).transform;
    }
    public RaftSurvivor GetStartSurvivor()
    {
        return startSurvivor;
    }
}
