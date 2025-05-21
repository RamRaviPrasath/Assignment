using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region VARIABLES
    public static GameManager instance;

    [Header("Script Reference")]
    public ObjectPooling ObjectPooling;

    [Header("Timer")]
    public TMP_Text Timer;
    private float TotalTime = 30;
    public float CurrentTime;

    [Header("Point")]
    public TMP_Text PointsText;
    public float Points;

    [Space(20)]
    [Header("Theme")]
    public ThemeAssets MyThemeAssets;
    public ThemeData MyThemeData;

    [Header("Result")]
    public GameObject ResultPanel;
    public TMP_Text YourPoints;
    public Button PlayAgain;
    public Button Exit;
    #endregion


    #region UNITY FUNCTIONS
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        PlayAgain.onClick.AddListener(() =>
        {
            AudioManager.instance.PlaySound("UI");
            ResultPanel.SetActive(false);
            StopCoroutine(nameof(StartTimerCountDown));
            StartCoroutine(nameof(StartTimerCountDown));
        });

        Exit.onClick.AddListener(() => { AudioManager.instance.PlaySound("UI"); UIManager.Instance.AlertPanel.gameObject.SetActive(true); }); 
        StopCoroutine(nameof(StartTimerCountDown));
        StartCoroutine(nameof(StartTimerCountDown));
        UIManager.Instance.ForceUpdateTheme();
    }
    #endregion


    #region TIMER
    private IEnumerator StartTimerCountDown()
    {
        CurrentTime = TotalTime;
        Points = 0;
        UpdatePoints();
        yield return new WaitForSeconds(1f);
        StartCoroutine(ObjectPooling.StartShowingCoins());
        while (CurrentTime >= 0)
        {
            ShowTimer(CurrentTime);
            yield return new WaitForSeconds(1f);
            CurrentTime -= 1;
        }

        ShowResult();
        Debug.Log($"GameOver");

    }

    private void ShowTimer(float _time)
    {
        Timer.text = $"{Mathf.CeilToInt(_time)}";
    }
    #endregion


    #region GAMEPLAY
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Coin"))
                {
                    Debug.Log("Coin clicked!");
                    AudioManager.instance.PlaySound("Coin");
                    Points += 1;
                    UpdatePoints();
                    hit.collider.transform.parent.gameObject.SetActive(false);
                }
            }
        }
    }

    private void UpdatePoints()
    {
        PointsText.text = $"Point: {Points}";
    }
    #endregion


    #region SHOW RESULT
    private void ShowResult()
    {
        ResultPanel.gameObject.SetActive(true);
        YourPoints.text = $"Points: {Points}";
    }
    #endregion


    #region APPLY THEME
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
