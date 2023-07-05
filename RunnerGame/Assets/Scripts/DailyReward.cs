using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;
    public long rewardGivingTimeTicks;
    public GameObject rewardMenu;
    public Text remainingTimeText;

    public void InitializeDailyReward()
    {
        if(PlayerPrefs.HasKey("lastdailyreward"))
        {
            rewardGivingTimeTicks=long.Parse(PlayerPrefs.GetString("lastdailyreward"))+864000000000;
            long currentTime=System.DateTime.Now.Ticks;
            if(currentTime>=rewardGivingTimeTicks)
            {
                GiveReward();
            }
        }
        else
        {
            GiveReward();
        }
        initialized=true;

    }
    public void GiveReward()
    {
        LevelController.Current.GiveMoneyPlayer(100);
        rewardMenu.SetActive(true);
        PlayerPrefs.SetString("lastdailyreward",System.DateTime.Now.Ticks.ToString());
        rewardGivingTimeTicks=long.Parse(PlayerPrefs.GetString("lastdailyreward"))+864000000000;
    }
    void Update()
    {
        if(initialized)
        {
            if(LevelController.Current.startMenu.activeInHierarchy)
            {
                long currentTime=System.DateTime.Now.Ticks;
                long remainingTime=rewardGivingTimeTicks-currentTime;
                if(remainingTime<=0)
                {
                    GiveReward();
                }
                else
                {
                    System.TimeSpan timeSpan=System.TimeSpan.FromTicks(remainingTime);
                    remainingTimeText.text=string.Format("{0}:{1}:{2}",timeSpan.Hours.ToString("D2"),timeSpan.Minutes.ToString("D2"),timeSpan.Seconds.ToString("D2"));
                }
            }
        }
    }

    public void TapToTurnButton()
    {
        rewardMenu.SetActive(false);
    }
}
