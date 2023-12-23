using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour
{
    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _pauseBtn;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _repeatBtn;

    private void Awake()
    {
        SetFrameRate();
        RegisterBtns();
    }

    private void SetFrameRate()
    {
        Application.targetFrameRate = 180;
    }

    private void RegisterBtns()
    {
        _backBtn.onClick.AddListener(() => Back());
        _pauseBtn.onClick.AddListener(() => Pause());
        _continueBtn.onClick.AddListener(() => Continue());
        _repeatBtn.onClick.AddListener(() => Repeat());

        _pauseBtn.gameObject.SetActive(true);
        _continueBtn.gameObject.SetActive(false);
        SetTimeScale(1.0f);
    }

    private void Back()
    {
        SceneManager.LoadScene("menu");
        Debug.Log($"載入場景");
    }

    private void Pause()
    {
        _pauseBtn.gameObject.SetActive(false);
        _continueBtn.gameObject.SetActive(true);
        SetTimeScale(0.0f);
    }

    private void Continue()
    {
        _pauseBtn.gameObject.SetActive(true);
        _continueBtn.gameObject.SetActive(false);
        SetTimeScale(1.0f);
    }

    private void SetTimeScale(float speed)
    {
        Time.timeScale = speed;
    }

    private void Repeat()
    {
        SceneManager.LoadScene("main");
        Debug.Log($"載入場景");
    }
}
