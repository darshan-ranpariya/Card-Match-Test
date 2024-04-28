using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager inst;
    [Header("Panels")]
    public Panel startPanel;
    public Panel gridPanel;
    public Panel gamePlayPanel;
    public Panel gameOverPanel;
    public Panel pausePanel;
    public Panel winPanel;
    [Header("UI Elements")]
    public GameObject loader;
    public uString gameOverMsg;
    public uString gameWinMsg;
    //public int winScore;
    public uNumber currScore;
    public uNumber highScore;
    public Image fillImg;
    public UISwitch[] audioToggles;
    public float totalTime;
    internal float currTime;
    [Header("Others")]
    public uString timeString;
    Coroutine timer;
    private void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        currScore.Value = 0;
        highScore.Value = PlayerPrefs.GetInt("HighScore", 0);
        foreach (var sw in audioToggles)
        {
            sw.Set(AudioPlayer.effectsOn);
        }
    }

    void Start()
    {
        if(PlayerPrefs.GetInt(Utility.StageDataSavePrefKey, 0) != 0)
        {
            new Delayed.Action(ItemsHandler.inst.LoadGameSavedData, Time.deltaTime * 2);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (gamePlayPanel.gameObject.activeInHierarchy && !pausePanel.gameObject.activeInHierarchy)
            {
                ItemsHandler.inst.SaveStageData();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (gamePlayPanel.gameObject.activeInHierarchy && !pausePanel.gameObject.activeInHierarchy)
        {
            ItemsHandler.inst.SaveStageData();
        }
    }

    void ResetGame()
    {
        gameOverPanel.gameObject.SetActive(false);
        currTime = totalTime;
        currScore.Value = 0;
        if(timer != null) StopCoroutine(timer);
    }


    #region ClickMethods
    public void StartBtnClick()
    {
        if (pausePanel.gameObject.activeInHierarchy) pausePanel.gameObject.SetActive(false);
        gridPanel.Activate();
    }

    public void StartGame()
    {
        gamePlayPanel.Activate();
        timer = StartCoroutine(StartTime());
    }

    public void PauseBtnClick()
    {
        StopCoroutine(timer);
        pausePanel.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeBtnClick()
    {
        Time.timeScale = 1;
        pausePanel.gameObject.SetActive(false);
        timer = StartCoroutine(StartTime());
    }

    public void HomeBtnClick()
    {
        ResetGame();
        if (pausePanel.gameObject.activeInHierarchy) pausePanel.gameObject.SetActive(false);
        startPanel.Activate();
        //SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartBtnClick()
    {
        ResetGame();
        StartBtnClick();
    }

    public void WinCloseBtnClick()
    {
        winPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }
    public void SoundToggle()
    {
        AudioPlayer.effectsOn = !AudioPlayer.effectsOn;
    }

    #endregion
    IEnumerator StartTime()
    {
        while (currTime > 0)
        {
            int t = (int)Mathf.Clamp(currTime, 0, totalTime);
            timeString.Value = t.ToString();
            currTime = Mathf.Clamp(currTime, 0, totalTime);
            yield return new WaitForSeconds(1f);
            currTime--;
            //print("currTime : " + currTime);
            t = (int)Mathf.Clamp(currTime, 0, totalTime);
            timeString.Value = t.ToString();
            //fillImg.fillAmount = currTime / totalTime;
        }
        if (currTime <= 0)
        {
            OnGameOver();
        }
    }
    private void OnGameOver()
    {
        if (highScore.Value < currScore.Value)
        {
            highScore.Value = currScore.Value;
            PlayerPrefs.SetInt("HighScore", highScore.ValueAsInt);
        }
        AudioPlayer.PlaySFX("GameOver");
    }

    public void UpdateScore(int score)
    {
        currScore.Value += score;
        currScore.Value = currScore.Value >= 0 ? currScore.Value : 0;
        if(currScore.Value > highScore.Value)
        {
            highScore.Value = currScore.Value;
            PlayerPrefs.SetInt("HighScore", highScore.ValueAsInt);
        }
    }
}
