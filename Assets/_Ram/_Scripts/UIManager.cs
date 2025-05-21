using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net.Mail;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region VARIABLES
    public static UIManager Instance;

    [Header("Panels")]
    public GameObject OptionPanel;

    [Header("UI Reference")]
    public Button Options;
    [SerializeField] private Image fadeImage;
    [SerializeField] private Toggle Sound;
    [SerializeField] private Toggle Music;
    [SerializeField] private Toggle themeChange;
    [SerializeField] private TMP_Text themeMode;

    [Header("DontDestroy")]
    public List<GameObject> dontDestroy;

    [Space(20)]
    [Header("Theme")]
    public ThemeAssets MyThemeAssets;
    public ThemeData MyThemeData;

    [Header("AlertPanel")]
    public GameObject AlertPanel;
    public Button positive;
    public Button negative;
    #endregion


    #region UNITY FUNCTION
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        foreach (var item in dontDestroy)
        {
            DontDestroyOnLoad(item);
        }
    }

    private void Start()
    {
        Options.onClick.AddListener(() =>
        {
            AudioManager.instance.PlaySound("UI");
            OptionPanel.gameObject.SetActive(!OptionPanel.activeSelf);
        });
        themeChange.onValueChanged.AddListener((value) =>
        {
            themeMode.text = $"{(value ? "Normal Mode" : "Theme Mode")}";
            AudioManager.instance.PlaySound("UI");
            OnChangeTheme(value? 0 : 1);
        });
        Sound.onValueChanged.AddListener((value) =>
        {
            AudioManager.instance.audioSource.volume = value ? 1 : 0;
            AudioManager.instance.PlaySound("UI");
        });
        Music.onValueChanged.AddListener((value) =>
        {
            AudioManager.instance.BgmSource.volume = value ? 1 : 0;
            AudioManager.instance.PlaySound("UI");
        });

        OnChangeTheme(0);

        positive.onClick.AddListener(() => { ExitGame(); });
        negative.onClick.AddListener(() => { AlertPanel.gameObject.SetActive(false); });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlertPanel.gameObject.SetActive(true); 
        }
    }

    #endregion


    #region THEME FUNCTION
    public void OnChangeTheme(int value)
    {
        if (SceneManager.GetActiveScene().name == "Login")
        {
            LobbyHandler.Instance.ApplyTheme(value);
        }
        else
        {
            GameManager.instance.ApplyTheme(value);
        }
        ApplyTheme(value);
    }

    public void ApplyTheme(int _index)
    {
        ThemeData.Theme _data = new();
        _data = MyThemeData.GetTheme(_index);
        if (MyThemeAssets.CurrentThemeIndex != _index || true)
        {
            MyThemeAssets.CurrentThemeIndex = _index;
            foreach (var item in MyThemeAssets.AllPanelBG)
            {
                item.color = _data.PanelBGColor;
            }
            foreach (var item in MyThemeAssets.AllBG)
            {
                item.color = _data.backgroundColor;
            }

            foreach (var item in MyThemeAssets.AllTexts)
            {
                item.color = _data.textColor;
            }
            foreach (var item in MyThemeAssets.AllButtons)
            {
                item.color = _data.buttonColor;
            }
            foreach (var item in MyThemeAssets.AllButtonsText)
            {
                item.color = _data.buttonTextColor;
            }
            foreach (var item in MyThemeAssets.AllIcons)
            {
                item.color = _data.IconColor;
            }
        }
    }

    public void ForceUpdateTheme()
    {
        OnChangeTheme(themeChange.isOn ? 0 : 1);
    }
    #endregion


    #region CHANGE_PANEL
    public void ChangePanel(Action _onComplete)
    {
        Debug.Log($"Fade Calling");
        fadeImage.DOKill();
        fadeImage.DOFade(1, 0.5f).OnComplete(() =>
        {
            Debug.Log($"Fade Calling 2222");
            fadeImage.DOFade(0, 0);
            _onComplete?.Invoke();
        });
    }
    #endregion


    #region EXIT
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}
