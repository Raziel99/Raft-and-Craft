using System.Collections;
using DG.Tweening;
using UnityEngine;

public enum SpawnObjectType { Fragment, Pickupable, Obstalce, MovingObstacle, Sharks }

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private Island island;
    [SerializeField]
    private FloatingRaft p_FloatingRaft;
    [SerializeField]
    private Obstacle[] pl_StaticObstacles;
    [SerializeField]
    private MovingObstacle[] pl_MovingObstacles;
    [SerializeField]
    private Sharks p_Sharks;
    [SerializeField]
    private RaftSurvivor[] pl_Survivors;
    [SerializeField]
    private Item p_Barrel;
    [SerializeField]
    private Vector2 spawnBounds;
    [SerializeField]
    private float spawnRate;
    [SerializeField]
    private float spawnProbability;

    private float previousSpawnZ;
    private float islandLength;
    private bool gameEnd;

    private void Awake()
    {
        previousSpawnZ = transform.position.z;
    }
    private void Start()
    {
        StartCoroutine(IE_SpawnFragments());
        Island.Instance.OnGameComplete += () => gameEnd = true;
        RaftController.Instance.OnGameFail += () => gameEnd = true;
    }
    private IEnumerator IE_SpawnFragments()
    {
        float timer = 0;
        float currentRate = UnityEngine.Random.Range(spawnRate - 0.3f, spawnRate + 0.3f);

        while (true)
        {
            if (gameEnd) break;

            if (timer >= currentRate)
            {
                timer = 0;
                currentRate = UnityEngine.Random.Range(spawnRate - 0.3f, spawnRate + 0.3f);

                if (islandLength >= 1000f)
                {
                    island.transform.position = Vector3.forward * islandLength;
                    island.gameObject.SetActive(true);
                    break;
                }
                else if (UnityEngine.Random.Range(0, 100f) < spawnProbability)
                {
                    Spawn();
                }
            }
            else
            {
                timer += Time.deltaTime;
            }

            islandLength = transform.position.z;

            yield return null;
        }
    }
    private void Spawn()
    {
        float currentZ = transform.position.z;

        if (Mathf.Abs(currentZ - previousSpawnZ) >= 8f)
        {
            int value = UnityEngine.Random.Range(0, 100);


            SpawnObjectType spawnObjectType = value > 90 ? SpawnObjectType.Sharks : value > 80 ? SpawnObjectType.Obstalce : value > 70 ? SpawnObjectType.MovingObstacle : value > 40 ? SpawnObjectType.Fragment : SpawnObjectType.Pickupable;
            Vector3 randomAngle = Vector3.up * UnityEngine.Random.Range(0, 360f);
            float sinkAmountY = 0;


            Transform spawnPrefab;
            switch (spawnObjectType)
            {
                case SpawnObjectType.Fragment:
                    randomAngle = Vector3.zero;
                    spawnPrefab = p_FloatingRaft.transform;
                    break;
                case SpawnObjectType.Pickupable:
                    bool isBarrel = UnityEngine.Random.Range(0, 100) > 50;

                    if (isBarrel)
                    {
                        spawnPrefab = p_Barrel.transform;
                    }
                    else
                    {
                        spawnPrefab = pl_Survivors[UnityEngine.Random.Range(0, pl_Survivors.Length)].transform;
                    }

                    if (isBarrel)
                    {
                        randomAngle = new Vector3(UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f), UnityEngine.Random.Range(0, 360f));
                    }
                    else
                    {
                        sinkAmountY = 2f;
                    }
                    break;
                case SpawnObjectType.Obstalce:
                    spawnPrefab = pl_StaticObstacles[UnityEngine.Random.Range(0, pl_StaticObstacles.Length)].transform;
                    break;
                case SpawnObjectType.MovingObstacle:
                    spawnPrefab = pl_MovingObstacles[UnityEngine.Random.Range(0, pl_MovingObstacles.Length)].transform;
                    randomAngle = Vector3.zero;
                    break;
                case SpawnObjectType.Sharks:
                    spawnPrefab = p_Sharks.transform;
                    randomAngle = Vector3.zero;
                    break;
                default:
                    return;
            }

            Transform spawnObject = Instantiate(spawnPrefab);

            spawnObject.transform.localEulerAngles = randomAngle;
            Vector3 position = transform.position;
            position.x = UnityEngine.Random.Range(spawnBounds.x, spawnBounds.y);

            position.y -= sinkAmountY;

            if (spawnObjectType == SpawnObjectType.Obstalce)
            {
                float spawnX = 10f;
                float x = position.x > 0 ? spawnX : -spawnX;
                position.x = x;
            }
            else if (spawnObjectType == SpawnObjectType.Sharks)
            {
                position.x = 0;
            }

            spawnObject.position = position;
            if (spawnObjectType == SpawnObjectType.Pickupable || spawnObjectType == SpawnObjectType.Fragment)
            {
                float sinkAmount = UnityEngine.Random.Range(0.2f, 0.27f);
                spawnObject.DOMoveY(position.y - sinkAmount, 0.5f).SetLoops(-1, LoopType.Yoyo);
            }
            previousSpawnZ = currentZ;
        }
    }
}
