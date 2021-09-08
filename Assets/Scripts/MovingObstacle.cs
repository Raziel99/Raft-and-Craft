using System.Collections;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveBounds;
    [SerializeField]
    private float moveSpeed;

    private Transform pivot;

    private void Awake()
    {
        pivot = transform.GetChild(0);

        Vector3 randomAngle = new Vector3(UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f));
        pivot.localEulerAngles = randomAngle;

        StartCoroutine(IE_Move());
    }
    private IEnumerator IE_Move()
    {
        bool movingRight = UnityEngine.Random.Range(0, 100) > 50;

        while (true)
        {
            if (movingRight)
            {
                pivot.localPosition += Vector3.right * moveSpeed * Time.deltaTime;
            }
            else
            {
                pivot.localPosition += Vector3.left * moveSpeed * Time.deltaTime;
            }

            if(pivot.position.x > moveBounds.y)
            {
                movingRight = false;
            }
            else if (pivot.position.x < moveBounds.x)
            {
                movingRight = true;
            }

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
