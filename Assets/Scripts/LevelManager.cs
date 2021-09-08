using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] levels;
    [SerializeField]
    private float[] islandPositions;
    [SerializeField]
    private Island island;
    [SerializeField]
    private Image i_ProgressBar;
    [SerializeField]
    private CanvasGroup cg_LevelBar;
    [SerializeField]
    private TextMeshProUGUI t_Level;

    private RaftController raftController;

    private void Awake()
    {
        if(!PlayerPrefs.HasKey("Level"))
        {
            PlayerPrefs.SetInt("Level", 0);
        }

        int currentLevel = PlayerPrefs.GetInt("Level");
        int currentLevelIndex = currentLevel % levels.Length;

        t_Level.text = $"Level {currentLevel + 1}";

        island.transform.position = Vector3.forward * islandPositions[currentLevelIndex];
        Instantiate(levels[currentLevelIndex]);

        i_ProgressBar.rectTransform.sizeDelta = new Vector2(0, 27f);
        cg_LevelBar.alpha = 0;
        t_Level.DOFade(0, 0);
    }
    private void Start()
    {
        raftController = RaftController.Instance;
        MoveController.Instance.OnGameStart += OnGameStart;
        island.OnGameComplete += OnGameComplete;
        RaftController.Instance.OnGameFail += OnGameFail;

        StartCoroutine(IE_ProgressBar());
    }
    private void OnGameStart()
    {
        float duration = 0.5f;
        cg_LevelBar.DOFade(1f, duration);
        t_Level.DOFade(1f, duration);

    }
    private void OnGameComplete()
    {
        StopAllCoroutines();
        i_ProgressBar.rectTransform.sizeDelta = new Vector2(180f, 27f);
    }
    private void OnGameFail()
    {
        StopAllCoroutines();
        cg_LevelBar.DOFade(0, 0.5f);
        t_Level.DOFade(0, 0.5f);
    }
    private IEnumerator IE_ProgressBar()
    {
        while (true)
        {
            float distance = (raftController.transform.position.z / (island.transform.position.z - 18f)) * 180f;

            i_ProgressBar.rectTransform.sizeDelta = new Vector2(distance, 27f);

            yield return null;
        }
    }
}
