using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public static UI_Manager Instance { get; private set; }

    [SerializeField]
    private CanvasGroup cg_Sky;
    [SerializeField]
    private SpriteRenderer cloud1;
    [SerializeField]
    private SpriteRenderer cloud2;
    [SerializeField]
    private CanvasGroup cg_Planks;
    [SerializeField]
    private TextMeshProUGUI t_Planks;
    [SerializeField]
    private CanvasGroup cg_LevelComplete;
    [SerializeField]
    private CanvasGroup cg_Title;
    [SerializeField]
    private CanvasGroup cg_Next;
    [SerializeField]
    private Button b_Next;
    [SerializeField]
    private CanvasGroup cg_Restart;
    [SerializeField]
    private CanvasGroup cg_Fade;
    [SerializeField]
    private CanvasGroup cg_LevelFade;
    [SerializeField]
    private Button b_Restart;
    [SerializeField]
    private CanvasGroup cg_Fail;
    [SerializeField]
    private CanvasGroup cg_Tutorial;
    [SerializeField]
    private float fingerSpeed;
    [SerializeField]
    private float shiningSpeed;

    private Coroutine c_Tutorial;

    private void Awake()
    {
        Instance = this;
        b_Restart.onClick.AddListener(Restart);
        b_Next.onClick.AddListener(Next);

        cg_LevelFade.alpha = 1f;
        cg_LevelFade.DOFade(0, 0.2f);

        c_Tutorial = StartCoroutine(IE_Tutorial());
    }
    private void Start()
    {
        RaftController.Instance.OnGameFail += EnableRestart;
        MoveController.Instance.OnGameStart += OnGameStart;
        Island.Instance.OnGameComplete += () =>
        {
            cg_Planks.DOFade(1f, 0.5f);
        };
        RaftController.Instance.OnRaftChange += (Raft[,] rafts) =>
        {
            int count = 0;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Raft raft = rafts[i, j];

                    if (raft != null)
                    {
                        count += raft.GetAllPlanks().Count;
                    }
                }
            }

            count += RaftController.Instance.BagPlanksCount;

            t_Planks.text = count.ToString();
        };
    }
    private void Restart()
    {
        VibController.Vibrate(ArmNomads.Haptic.ImpactFeedback.Medium);

        cg_LevelFade.DOFade(1, 0.2f).OnComplete(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public void HideSky()
    {
        cg_Sky.DOFade(0, 0.5f);
        cloud1.DOFade(0, 0.5f);
        cloud2.DOFade(0, 0.5f);
        cg_Planks.DOFade(0, 0.5f);
    }

    public void EnableNext()
    {
        cg_Next.blocksRaycasts = true;

        float duration = 0.5f;

        StartCoroutine(IE_RotateShining());
        cg_LevelComplete.transform.localScale = Vector3.zero;
        cg_LevelComplete.alpha = 1f;
        cg_LevelComplete.transform.DOScale(1f, duration);
        cg_Next.DOFade(1f, duration);
    }
    private void EnableRestart()
    {
        cg_Restart.blocksRaycasts = true;

        cg_Fail.transform.localScale = Vector3.one * 5f;

        float duration = 0.4f;

        Sequence seq = DOTween.Sequence();

        seq.Append(cg_Fade.DOFade(1f, duration / 2f));
        seq.Append(cg_Fail.DOFade(1f, duration).SetEase(Ease.InCubic));
        seq.Join(cg_Fail.transform.DOScale(Vector3.one, duration).SetEase(Ease.InCubic));
        seq.Append(cg_Restart.DOFade(1f, duration));
    }
    private void Next()
    {
        PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
        Restart();
    }
    private void OnGameStart()
    {
        StopCoroutine(c_Tutorial);
        float duration = 0.5f;

        cg_Tutorial.DOFade(0, duration);
        cg_Title.DOFade(0, duration);
        cg_Planks.DOFade(1f, duration);
    }
    private IEnumerator IE_Tutorial()
    {
        bool right = true;
        Transform finger = cg_Tutorial.transform.Find("Finger");
        float bound = 85f;

        while (true)
        {
            if (right)
            {
                finger.localPosition += Vector3.right * fingerSpeed * Time.deltaTime;
            }
            else
            {
                finger.localPosition -= Vector3.right * fingerSpeed * Time.deltaTime;
            }

            if(finger.localPosition.x > bound)
            {
                right = false;
            }
            else if (finger.localPosition.x < -bound)
            {
                right = true;
            }

            yield return null;
        }
    }
    private IEnumerator IE_RotateShining()
    {
        Transform shining = cg_LevelComplete.transform.GetChild(0);

        while (true)
        {
            shining.Rotate(Vector3.forward * Time.deltaTime * shiningSpeed);

            yield return null;
        }
    }
}
