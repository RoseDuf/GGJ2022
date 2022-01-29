using System;
using Game;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using StarterAssets;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private const int TIME_N = 2;
    private static readonly int Show = Animator.StringToHash("Show");
    private static readonly int Hide = Animator.StringToHash("Hide");

    [SerializeField] private Animator dayNightTransitionAnimator;
    [HideInInspector] public TextMeshProUGUI[] TimeUI = new TextMeshProUGUI[TIME_N];

    public Image DayNightCircleImage;
    [SerializeField] private Color DayColor;
    [SerializeField] private Color NightColor;
    public RectTransform IndicatorDayNightRectTransform;

    [SerializeField] private Slider _healthBar;
    [SerializeField] GameObject _uIinventoryObject;
    public UIInventory UIinventory { get; set; }

    [SerializeField] private GameObject _pauseMenu;
    private PauseAction _pauseAction;
    public bool GameIsPaused { get; set; }

    public bool DayNightTransitionIsFinished => !dayNightTransitionAnimator.IsInTransition(0) &&
                                                dayNightTransitionAnimator.GetCurrentAnimatorStateInfo(0)
                                                    .normalizedTime > 1;

    public static UIManager Instance { get; private set; }
    public static bool HasInstance { get { return Instance != null; } }

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _pauseAction = new PauseAction();
    }

    private void Start()
    {
        _pauseAction.Pause.Pause.performed += _ => HandleInput();
        Resume();

        GameManager.Instance.OnPlayerDied += OnPlayerDied;
        
        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
            DayNightCircleImage.color = DayColor;
        else
        {
            ShowHealthBar(true);
            DayNightCircleImage.color = NightColor;
        }

        UIinventory = _uIinventoryObject.GetComponent<UIInventory>();
    }

    private void OnEnable()
    {
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.Disable();
    }

    private void Update()
    {
        UpdateDayIndicator();
        UpdateInventory();
    }

    private void UpdateInventory()
    {
        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
        {
            if (!_uIinventoryObject.activeInHierarchy)
            {
                _uIinventoryObject.SetActive(true);
            }
        }

        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Night)
        {
            if (_uIinventoryObject.activeInHierarchy)
            {
                _uIinventoryObject.SetActive(false);
            }
        }
    }

    void OnValidate()
    {
        if (TimeUI.Length != TIME_N)
        {
            Array.Resize(ref TimeUI, TIME_N);
            Debug.LogWarning("Don't change the number of possible object this object");
        }
    }

    private void UpdateDayIndicator()
    {
        float timeAngle = (DaytimeManager.Instance.CurrentTime * -360);

        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
        {
            timeAngle -= 180;
        }

        IndicatorDayNightRectTransform.rotation = Quaternion.Euler(0, 0, timeAngle);
    }

    public void ShowDayNightTransition()
    {
        if (dayNightTransitionAnimator)
        {
            dayNightTransitionAnimator.SetTrigger(Show);
        }
    }

    public void HideDayNightTransition()
    {
        if (dayNightTransitionAnimator)
        {
            dayNightTransitionAnimator.SetTrigger(Hide);
        } 
        if (DaytimeManager.Instance.CurrentTimeOfDay == DaytimeManager.TimeOfDay.Day)
            DayNightCircleImage.color = DayColor;
        else
            DayNightCircleImage.color = NightColor;
        
    }

    public void UpdatePlayerHealth(float health)
    {
        _healthBar.value = health;
    }

    public void ShowHealthBar(bool state)
    {
        _healthBar.gameObject.SetActive(state);
    }

    private void OnPlayerDied()
    {
        Debug.Log("END_SCREEN");
    }

    private void HandleInput()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    void Resume()
    {
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        AkSoundEngine.WakeupFromSuspend();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Pause()
    {
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        AkSoundEngine.Suspend();
        Cursor.lockState = CursorLockMode.None;
    }
}