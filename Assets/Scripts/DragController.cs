using System;
using ArmNomads;
using UnityEngine;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    [SerializeField]
    private Transform character;
    [SerializeField]
    private Vector2 moveBounds;
    [SerializeField]
    private float sensitivity;
    [SerializeField]
    private float lerpSpeed;
    [SerializeField]
    private float maxRotateAngle;

    [SerializeField]
    private InputField in_Sensitivity;
    [SerializeField]
    private InputField in_Angle;

    private Vector2 pointerStartPosition;
    private Quaternion targetQuaternion;
    private Vector3 pointerMoveVector;
    private bool gameEnd;

    private void Awake()
    {
        sensitivity = PlayerPrefs.GetFloat("Sensitivity", sensitivity);
        maxRotateAngle = PlayerPrefs.GetFloat("Angle", maxRotateAngle);


        in_Sensitivity.onEndEdit.AddListener(ChangeSensitivity);
        in_Angle.onEndEdit.AddListener(ChangeAngle);
        targetQuaternion = Quaternion.identity;
    }
    private void Start()
    {
        Island.Instance.OnGameComplete += () => gameEnd = true;
        RaftController.Instance.OnGameFail += () => gameEnd = true;
    }
    private void Update()
    {
        if (gameEnd) return;

        Vector3 startAngle = character.localEulerAngles;
        Quaternion startQuaternion = Quaternion.Euler(startAngle);

        if (Input.GetMouseButtonDown(0))
        {
            pointerStartPosition = ANInput.NormalizedPointerPos;
        }
        else if (Input.GetMouseButton(0))
        {
            pointerMoveVector = (ANInput.NormalizedPointerPos - pointerStartPosition) * sensitivity;

            if (startAngle.y > 180f)
            {
                startAngle.y -= 360f;
            }


            //targetPosition = character.transform.position + Vector3.right * pointerMoveVector.x * sensitivity * Time.deltaTime;



            Vector3 targetAngle = Vector3.up * pointerMoveVector.x * sensitivity * 3f;

            targetAngle.y = Mathf.Clamp(targetAngle.y, -maxRotateAngle, maxRotateAngle);

            targetQuaternion = Quaternion.Euler(targetAngle);

            //Vector3 targetPosition = Vector3.right * pointerMoveVector.x + character.transform.position;

            //targetPosition.x = Mathf.Clamp(targetPosition.x, moveBounds.x, moveBounds.y);

            //character.position = targetPosition;
            pointerStartPosition = ANInput.NormalizedPointerPos;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            targetQuaternion = Quaternion.identity;
        }

        Vector3 targetPosition = character.position;

        if (targetPosition.x < moveBounds.x && pointerMoveVector.x < 0)
        {
            targetPosition.x = moveBounds.x;
            targetQuaternion = Quaternion.identity;
        }
        else if (targetPosition.x > moveBounds.y && pointerMoveVector.x > 0)
        {
            targetPosition.x = moveBounds.y;
            targetQuaternion = Quaternion.identity;
        }

        character.position = targetPosition;
        character.rotation = Quaternion.Lerp(startQuaternion, targetQuaternion, Time.deltaTime * lerpSpeed);
    }
    private void ChangeSensitivity(string s)
    {
        if (float.TryParse(s, out float number))
        {
            PlayerPrefs.SetFloat("Sensitivity", number);

            sensitivity = number;
        }
    }
    private void ChangeAngle(string s)
    {
        if (float.TryParse(s, out float number))
        {
            PlayerPrefs.SetFloat("Angle", number);

            maxRotateAngle = number;
        }
    }
}
