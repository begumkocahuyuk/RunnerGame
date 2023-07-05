using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current;
    public bool gameActive=false;

    public GameObject startMenu,gameMenu,gameoverMenu,finishMenu;
    public Text scoreText,finishScoreText,currentLevelText,nextLevelText,startingMenuMoneyText,gameOverMenuMoneyText,finishGameMenuMoneyText;
    public Slider levelProgressBar;
    public float maxDistance;
    public GameObject finishLine;

    int currentLevel;
    int score;

    public AudioSource gameMusicAudioSource;
    public AudioClip victoryAudioClip,gameOverAudioClip;

    public DailyReward dailyReward;

    void Start()
    {
        GameObject[] parentsInScene=this.gameObject.scene.GetRootGameObjects();
        foreach(GameObject parent in parentsInScene)
        {
            TextObject[] textObjectsInParent = parent.GetComponentsInChildren<TextObject>(true);
            foreach(TextObject textObject in textObjectsInParent)
            {
                textObject.InitTextObject();
            }
        }
        Current=this;
        //Hangi Levelde kaldığını  kontrol etme
        currentLevel=PlayerPrefs.GetInt("currentLevel");
        PlayerController.Current=GameObject.FindObjectOfType<PlayerController>();
        GameObject.FindObjectOfType<MarketController>().InitializeMarketController();
        dailyReward.InitializeDailyReward();
        currentLevelText.text=(currentLevel+1).ToString();
        nextLevelText.text=(currentLevel+2).ToString();
        UpdateMoneyText();

        gameMusicAudioSource=Camera.main.GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if(gameActive)
        {
            PlayerController player=PlayerController.Current;
            float distance=finishLine.transform.position.z-PlayerController.Current.transform.position.z;
            levelProgressBar.value=1-(distance/maxDistance);
        }
    }
    public void StartLevel()
    {
        maxDistance=finishLine.transform.position.z-PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeSpeed(PlayerController.Current.runningSpeed);
        startMenu.SetActive(false);
        PlayerController.Current.animator.SetBool("running",true);
        gameMenu.SetActive(true);
        gameActive=true;
    }

    public void RestartLevel()
    {
        LevelLoader.Current.ChangeLevel(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        LevelLoader.Current.ChangeLevel("Level "+(currentLevel+1));
    }

    public void GameOver()
    {
        UpdateMoneyText();
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(gameOverAudioClip);
        gameMenu.SetActive(false);
        gameoverMenu.SetActive(true);
        gameActive=false;
    }
    public void FinishGame()
    {
        GiveMoneyPlayer(score);
        gameMusicAudioSource.Stop();
        gameMusicAudioSource.PlayOneShot(victoryAudioClip);
        PlayerPrefs.SetInt("currentLevel",currentLevel+1);
        finishScoreText.text=score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive=false;
    }
    public void ChangeScore(int increment)
    {
        score +=increment;
        scoreText.text=score.ToString();
    }
    public void UpdateMoneyText()
    {
        int money=PlayerPrefs.GetInt("money");
        startingMenuMoneyText.text=money.ToString();
        gameOverMenuMoneyText.text=money.ToString();
        finishGameMenuMoneyText.text=money.ToString();

    }
    public void GiveMoneyPlayer(int increment)
    {
        int money=PlayerPrefs.GetInt("money");
        money=Mathf.Max(money+increment);
        PlayerPrefs.SetInt("money",money);
        UpdateMoneyText();
    }
}
