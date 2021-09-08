using UnityEngine;

public class FloatingRaft : MonoBehaviour
{
    [SerializeField]
    private int planksCount;
    [SerializeField]
    private FloatingRaftFragment p_Fragment;
    [SerializeField]
    private float fragmentsDistance;

    private Transform fragmentsParent;

    private void Awake()
    {
        fragmentsParent = transform.GetChild(0);
        
        for (int i = 0; i < planksCount; i++)
        {
            FloatingRaftFragment fragment = Instantiate(p_Fragment, fragmentsParent);
            fragment.transform.localPosition = Vector3.zero;
            fragment.transform.localRotation = Quaternion.identity;

            fragment.transform.localPosition += Vector3.forward * i * fragmentsDistance;
        }
    }
}
