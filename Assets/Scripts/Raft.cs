using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public enum RaftSide { Right, Left, Up, Down }

public class Raft : MonoBehaviour
{
    public Pickupable RaftPickupable { get; set; }
    public bool IsBuilt => GetFreePlank() == null;
    public bool IsBusy => RaftPickupable != null;
    public int Rotation => rotation;
    public bool HasAnyPlank
    {
        get
        {
            for (int i = 0; i < planks.Length; i++)
            {
                RaftFragment plank = planks[i];

                if (plank.IsBuilt)
                {
                    return true;
                }
            }

            return false;
        }
    }


    [SerializeField]
    private RaftFragment[] planks;

    private RaftController controller;
    private int x;
    private int y;
    private int rotation;

    public void Init(RaftController controller, int x, int y, int rotation)
    {
        this.controller = controller;
        this.x = x;
        this.y = y;
        this.rotation = rotation;
    }
    //public bool AddPlank(float duration = 0.3f)
    //{
    //    RaftFragment plank = GetFreePlank();

    //    if (plank != null)
    //    {
    //        plank.Build(0);
    //        Place(plank, duration);
    //        return true;
    //    }

    //    return false;
    //}
    //private void Place(RaftFragment plank, float duration)
    //{
    //    float spawnHeight = 0.85f;
    //    Vector3 position = plank.transform.localPosition;
    //    position.y += spawnHeight;
    //    plank.transform.localPosition = position;
    //    plank.Build(0);
    //    position.y -= spawnHeight;
    //    plank.transform.DOLocalMove(position, duration);
    //}
    private void RemovePickupable()
    {
        controller.RemovePickupable(x, y);
    }
    //public void AddPlanks(int planksCount)
    //{
    //    controller.AddPlanks(planksCount);
    //}
    public void AddPickupable(Pickupable pickupable, float duration)
    {
        controller.AddPickupable(this, pickupable, duration);
    }
    public RaftFragment GetFreePlank()
    {
        bool firstToLast = planks[0].IsBuilt || (!planks[0].IsBuilt && !planks.Last().IsBuilt);

        if (firstToLast)
        {
            for (int i = 0; i < planks.Length; i++)
            {
                RaftFragment plank = planks[i];

                if (!plank.IsBuilt)
                {
                    return plank;
                }
            }
        }
        else
        {
            for (int i = planks.Length - 1; i >= 0; i--)
            {
                RaftFragment plank = planks[i];

                if (!plank.IsBuilt)
                {
                    return plank;
                }
            }
        }

        return null;
    }
    public void RemovePlank(RaftFragment plank)
    {
        if(IsBusy)
        {
            RemovePickupable();
        }

        plank.Break();

        foreach (RaftFragment p in planks)
        {
            if (p.IsBuilt)
            {
                controller.BreakPlank();
                return;
            }
        }

        controller.BreakRaft(x, y);
    }
    public Vector3 GetMidPoint(RaftSide raftSide)
    {
        Vector3 position = transform.position;

        switch (raftSide)
        {
            case RaftSide.Right:
                position = GetMidPlanksPoint();
                position += Vector3.right * (controller.RaftSize / 2f);
                break;
            case RaftSide.Left:
                position = GetMidPlanksPoint();
                position += Vector3.left * (controller.RaftSize / 2f);
                break;
            case RaftSide.Up:

                position = GetEdgePlankMidPoint(rotation == 90);
                break;
            case RaftSide.Down:
                position = GetEdgePlankMidPoint(rotation != 90);
                break;
        }

        return position;
    }
    private Vector3 GetMidPlanksPoint()
    {
        RaftFragment firstPlank = null;
        RaftFragment lastPlank = null;

        for (int i = 0; i < planks.Length; i++)
        {
            if (planks[i].IsBuilt)
            {
                firstPlank = planks[i];
                break;
            }
        }
        for (int i = planks.Length - 1; i >= 0; i--)
        {
            if (planks[i].IsBuilt)
            {
                lastPlank = planks[i];
                break;
            }
        }

        Vector3 middlePosition = firstPlank.transform.position + lastPlank.transform.position;
        middlePosition /= 2f;

        return middlePosition;
    }
    private Vector3 GetEdgePlankMidPoint(bool first)
    {
        if (first)
        {
            for (int i = 0; i < planks.Length; i++)
            {
                RaftFragment plank = planks[i];

                if (plank.IsBuilt)
                {
                    return plank.transform.position;
                }
            }
        }

        for (int i = planks.Length - 1; i >= 0; i--)
        {
            RaftFragment plank = planks[i];

            if (plank.IsBuilt)
            {
                return plank.transform.position;
            }
        }

        return transform.position;
    }
    public List<RaftFragment> GetAllPlanks()
    {
        var allPlanks = new List<RaftFragment>();

        for (int i = 0; i < planks.Length; i++)
        {
            RaftFragment plank = planks[i];

            if (plank.IsBuilt)
            {
                allPlanks.Add(plank);
            }
        }

        return allPlanks;
    }
    public Vector3 GetBackPlankLocalPosition()
    {
        float offset = 0.4f;

        if (rotation == 270)
        {
            for (int i = 0; i < planks.Length; i++)
            {
                if(planks[i].IsBuilt)
                {
                    Vector3 pos = planks[i].transform.localPosition;
                    pos.x -= offset;
                    return pos;
                }
            }
        }
        else
        {
            for (int i = planks.Length - 1; i >= 0; i--)
            {
                if (planks[i].IsBuilt)
                {
                    Vector3 pos = planks[i].transform.localPosition;
                    pos.x += offset;
                    return pos;
                }
            }
        }

        return Vector3.zero;
    }
}