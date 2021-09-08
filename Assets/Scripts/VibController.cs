using UnityEngine;
using UnityEngine.UI;
using ArmNomads.Haptic;
using DG.Tweening;

public class VibController : MonoBehaviour
{
    [SerializeField]
    private Button vibButton;
    [SerializeField]
    private CanvasGroup vibButtonGroup;
    [SerializeField]
    private Image vinIcon;
    [SerializeField]
    private Sprite vibOnIcon;
    [SerializeField]
    private Sprite vibOffIcon;

    private static bool vibOn;

    private void Awake()
    {
        if(!PlayerPrefs.HasKey("Vib"))
        {
            PlayerPrefs.SetInt("Vib", 1);
        }

        vibOn = PlayerPrefs.GetInt("Vib") == 1;

        vibButton.onClick.AddListener(OnPressButton);
    }
    private void Start()
    {
        MoveController.Instance.OnGameStart += HideButton;
    }
    private void HideButton()
    {
        float duration = 0.2f;

        vibButtonGroup.blocksRaycasts = false;
        vibButtonGroup.DOFade(0, duration);
    }
    public void OnPressButton()
    {
        vibOn = !vibOn;
        PlayerPrefs.SetInt("Vib", vibOn ? 1 : 0);

        float duration = 0.1f;
        vibButton.transform.DOKill();
        vibButton.transform.DOScale(Vector3.one * 1.3f, duration).OnComplete(() =>
        {
            vibButton.transform.DOScale(Vector3.one, duration);
        });
        
        SetVibButton();
        Vibrate(ImpactFeedback.Medium);
    }
    private void SetVibButton()
    {
        vinIcon.sprite = vibOn ? vibOnIcon : vibOffIcon;
    }
    public static void Vibrate(ImpactFeedback impact)
    {
        if(vibOn)
        {
            HapticManager.Impact(impact);
        }
    }
}
