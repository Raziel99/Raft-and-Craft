using System.Collections;
using UnityEngine;

public class Sharks : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed;

    private Transform pivot;

    private void Awake()
    {
        pivot = transform.GetChild(0);

        StartCoroutine(IE_Rotate());
    }
    private IEnumerator IE_Rotate()
    {
        bool clockwise = UnityEngine.Random.Range(0, 100) > 50;

        if(clockwise)
        {
            Vector3 scale = Vector3.one;
            scale.x = -1f;
            pivot.localScale = scale;
        }

        float direction = clockwise ? 1f : -1f;

        while (true)
        {
            pivot.Rotate(Vector3.up * direction * rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DestroyZone"))
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }
}