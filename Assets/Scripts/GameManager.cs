using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; internal set; }

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        Input.multiTouchEnabled = false;
    }
}