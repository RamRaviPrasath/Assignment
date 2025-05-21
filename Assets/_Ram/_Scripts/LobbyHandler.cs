using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyHandler : MonoBehaviour
{
    #region VARIABLES

    public static LobbyHandler Instance;

    [Header("Panels")]
    [SerializeField] private GameObject welcomePanel;
    [SerializeField] private GameObject loginPanel;

    [Header("Buttons")]
    [SerializeField] private Button login;
    [SerializeField] private Button submit;

    [Header("InputFields")]
    [SerializeField] private TMP_InputField UserNameInput;
    [SerializeField] private TMP_InputField PasswordInput;

    [Header("OtherComponent")]
    [SerializeField] private Toggle showPassword;
    [SerializeField] private TMP_Text errorMessage;

    [Header("Loading Panel")]
    public GameObject LoadigPanel;
    public Slider loadingProcess;
    public TMP_Text LoadingText;

    private bool errorMessageShow;

    [Space(20)]
    [Header("Theme")]
    public ThemeAssets MyThemeAssets;
    public ThemeData MyThemeData;
    #endregion


    #region UNITY_FUNCTON
    private void Awake()
    {
        Instance = this;
        login.onClick.AddListener(() => { AudioManager.instance.PlaySound("UI"); OnPressedLogIn(); });
        submit.onClick.AddListener(() => { AudioManager.instance.PlaySound("UI"); ValidateCredentials(UserNameInput.text, PasswordInput.text); });
        UserNameInput.onSelect.AddListener((text) => { AudioManager.instance.PlaySound("UI"); OnInputFieldValueEnter(); });
        PasswordInput.onSelect.AddListener((text) => { AudioManager.instance.PlaySound("UI"); OnInputFieldValueEnter(); });
        showPassword.isOn = false;
        showPassword.onValueChanged.AddListener((show) =>
        {
            AudioManager.instance.PlaySound("UI");
            PasswordInput.contentType = show ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
            PasswordInput.ForceLabelUpdate();
        });
    }
    #endregion


    #region WELCOME PANEL
    private void OnPressedLogIn()
    {
        UIManager.Instance.ChangePanel(() =>
         {
             welcomePanel.SetActive(false);
             StartCoroutine(nameof(ShowFakeLoadingScreen));
         });
    }
    #endregion


    #region LOGIN
    private void ValidateCredentials(string username, string password)
    {
        string message =
            string.IsNullOrEmpty(username) && string.IsNullOrEmpty(password) ? "Enter user name and password" :
            string.IsNullOrEmpty(username) ? "Invalid user name" : (!string.IsNullOrEmpty(username) && username.Length < 10) ? "Require 10 digit nummber" :
            string.IsNullOrEmpty(password) ? "Invalid password" :
            null;

        if (!string.IsNullOrEmpty(message))
        {
            ShowError(message);
        }
        else
        {
            UIManager.Instance.ChangePanel(() =>
            {
                loginPanel.SetActive(false);
                StartCoroutine(nameof(LoadSceneAsync));
            });
            Debug.Log($"Log in succeess");
        }
    }

    public void ShowError(string message)
    {
        errorMessageShow = true;
        errorMessage.DOKill();
        errorMessage.text = message;
        errorMessage.alpha = 0;
        errorMessage.gameObject.SetActive(true);

        errorMessage.DOFade(1f, 0.5f)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                errorMessage.DOFade(0f, 0.5f)
                    .SetDelay(1.5f)
                    .OnComplete(() =>
                    {
                        errorMessage.text = "";
                        errorMessage.gameObject.SetActive(false);
                        errorMessageShow = false;
                    });
            });
    }

    private void OnInputFieldValueEnter()
    {
        if (errorMessageShow)
        {
            errorMessage.DOKill();
            errorMessage.text = "";
            errorMessage.gameObject.SetActive(false);
            errorMessageShow = false;
        }
    }
    #endregion


    #region LOADING 
    public IEnumerator ShowFakeLoadingScreen()
    {
        UIManager.Instance.Options.interactable = false;
        LoadigPanel.gameObject.SetActive(true);
        var _do = DOTween.To(() => 0, x => LoadingText.text = "Loading" + new string('.', x % 4), 3, 3f).SetLoops(-1, LoopType.Restart);
        float _duration = UnityEngine.Random.Range(2, 5);
        float _elapsed = 0;
        while (_elapsed < _duration)
        {
            _elapsed += Time.deltaTime;
            loadingProcess.value = Mathf.Clamp01(_elapsed / _duration);
            yield return null;
        }

        UIManager.Instance.ChangePanel(() =>
        {
            UIManager.Instance.Options.interactable = true;
            loadingProcess.value = 1f;
            _do.Kill();
            LoadigPanel.gameObject.SetActive(false);
            loginPanel.gameObject.SetActive(true);
        });
    }

    private IEnumerator LoadSceneAsync()
    {
        loadingProcess.value = 0f;
        UIManager.Instance.Options.interactable = false;
        LoadigPanel.gameObject.SetActive(true);

        var _do = DOTween.To(() => 0, x => LoadingText.text = "Loading" + new string('.', x % 4), 3, 3f).SetLoops(-1, LoopType.Restart);

        AsyncOperation operation = SceneManager.LoadSceneAsync("GamePlay");
        operation.allowSceneActivation = false;

        Debug.Log($"111 loading value {loadingProcess.value}");
        float fakeProgress = 0f;
        while (!operation.isDone)
        {
            float realProgress = Mathf.Clamp01(operation.progress / 0.9f);
            if (fakeProgress < realProgress)
            {
                fakeProgress += Time.deltaTime;
                fakeProgress = Mathf.Min(fakeProgress, realProgress);
            }

            loadingProcess.value = fakeProgress;

            if (operation.progress >= 0.9f && fakeProgress >= 1f)
            {
                UIManager.Instance.Options.interactable = true;
                LoadigPanel.SetActive(false);
                yield return new WaitForSeconds(0.5f);
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
        
    }
    #endregion


    #region THEME FUNCTION
    [ContextMenu("UpdateTheme")]
    public void UpdateTheme()
    {
        ApplyTheme(1);
    }
    public void ApplyTheme(int _index)
    {
        ThemeData.Theme _data = new();
        _data = MyThemeData.GetTheme(_index);
        if (MyThemeAssets.CurrentThemeIndex != _index || true)
        {
            Debug.Log($"Theme apply");
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
    #endregion
}

[System.Serializable]
public class ThemeAssets
{
    public List<Image> AllPanelBG = new();
    public List<Image> AllBG = new();
    public List<TMP_Text> AllTexts = new();
    public List<Image> AllButtons = new();
    public List<TMP_Text> AllButtonsText = new();
    public List<Image> AllIcons = new();
    public int CurrentThemeIndex = -1;
}
