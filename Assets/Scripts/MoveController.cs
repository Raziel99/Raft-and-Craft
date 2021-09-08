using System;
using UnityEngine;
using UnityEngine.UI;

public class MoveController : MonoBehaviour
{
    public event Action OnGameStart;

    public static MoveController Instance { get; private set; }
    public float MaxSpeed => moveSpeed;
    public float CurrentSpeed => currentSpeed;

    [SerializeField]
    private Transform[] particles;

    [SerializeField]
    private Material m_Water;
    [SerializeField]
    private Transform water;
    [SerializeField]
    private Transform character;
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float slowMoveSpeed;
    [SerializeField]
    private float accelerationSpeed;
    [SerializeField]
    private float accelerationLerpSpeed;
    [SerializeField]
    private float slowDownModifier;

    [SerializeField]
    private InputField in_speed;


    private Transform[] particlesParents;
    private float currentMaxSpeed;
    private float currentSpeed;
    private bool gameStart;
    private bool gameEnd;
    private bool isHolding;

    private void Awake()
    {
        Instance = this;
        currentMaxSpeed = moveSpeed;

        moveSpeed = PlayerPrefs.GetFloat("Speed", moveSpeed);

        in_speed.onEndEdit.AddListener(ChangeSpeed);

        particlesParents = new Transform[3];

        for (int i = 0; i < 3; i++)
        {
            particlesParents[i] = particles[i].GetChild(0);
        }
    }
    private void Start()
    {
        Island.Instance.OnGameComplete += () => gameEnd = true;
        RaftController.Instance.OnGameFail += () => gameEnd = true;
    }
    private void Update()
    {
        if (gameEnd)
        {
            currentSpeed = 0;

            for (int i = 0; i < 3; i++)
            {
                particlesParents[i].localPosition = Vector3.up * -2f;
            }

            return;
        }

        float acceleration = -accelerationSpeed * Time.deltaTime;

        if (isHolding)
        {
            acceleration = Mathf.Abs(acceleration);
        }

        float targetSpeed = currentSpeed + acceleration;

        targetSpeed = Mathf.Clamp(targetSpeed, 0, currentMaxSpeed);

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * accelerationLerpSpeed);

        Vector3 targetPosition = character.position + character.transform.forward * currentSpeed * Time.deltaTime;

        character.transform.position = targetPosition;

        Vector3 waterPosition = Vector3.forward * (targetPosition.z + 50f);
        m_Water.SetVector("_PlaneWorldPos", waterPosition);
        water.position = waterPosition;

        Vector3 p = Vector3.zero;
        p.y = Mathf.Lerp(-2f, 0, currentSpeed / MaxSpeed);

        for (int i = 0; i < 3; i++)
        {
            particlesParents[i].localPosition = p;
        }
    }
    public void SlowDown()
    {
        if(currentSpeed > moveSpeed * slowDownModifier)
        {
            currentSpeed = moveSpeed * slowDownModifier;
        }
    }
    public void ChangeMaxSpeed(bool hasRaft)
    {
        currentMaxSpeed = hasRaft ? moveSpeed : slowMoveSpeed;
    }
    public void OnHold()
    {
        if (!gameStart)
        {
            gameStart = true;
            OnGameStart?.Invoke();
        }

        isHolding = true;
    }
    public void OnRelease()
    {
        isHolding = false;
    }
    private void ChangeSpeed(string s)
    {
        if (float.TryParse(s, out float number))
        {
            PlayerPrefs.SetFloat("Speed", number);

            moveSpeed = number;
        }
    }
}
