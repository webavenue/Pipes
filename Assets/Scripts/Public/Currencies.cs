using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Currencies : MonoBehaviour
{
    public static Currencies instance;

    public GameObject updateParticle;
    public Sprite heartIcon;
    public Sprite infiniteHeartIcon;

    private Coroutine heartTimerCoroutine;

    private static readonly string textkey_full = "Full";

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateHeartsNumInDatabase()
    {
        if (Manager.instance.isInfiniteHeart)
        {
            int infiniteHeartSeconds = (int)(DateTime.Now - Manager.instance.heartsRefilledTime).TotalSeconds;
            if (infiniteHeartSeconds >= Manager.instance.infiniteHeartMinute * 60)
            {
                Manager.instance.isInfiniteHeart = false;
                Manager.instance.hearts = 5;
                Manager.instance.heartsRefilledTime = DateTime.MinValue;
                Manager.instance.Save(false);
            }
        }
        else
        {
            if (Manager.instance.heartsRefilledTime == DateTime.MinValue)
            {
                Manager.instance.hearts = 5;
                Manager.instance.Save(false);
            }
            else
            {
                // calculate 30 min for each new heart.
                int addedHearts = (int)(DateTime.Now - Manager.instance.heartsRefilledTime).TotalSeconds / 1800;
                Manager.instance.hearts += addedHearts;
                if (Manager.instance.hearts >= 5)
                {
                    Manager.instance.hearts = 5;
                    Manager.instance.heartsRefilledTime = DateTime.MinValue;
                }
                else
                {
                    Manager.instance.heartsRefilledTime =
                        Manager.instance.heartsRefilledTime.Add(new TimeSpan(0, 0, 1800 * addedHearts));
                }
                Manager.instance.Save(false);
            }
        }
    }

    public void InitHeartValues(Text HeartsNumText, Text HeartsTimerText, Image heartImage)
    {
        int defaultHearts = 5;
        if (!Manager.instance.isInfiniteHeart && Manager.instance.hearts == defaultHearts)
        {
            //heartImage.sprite = heartSprite;
            HeartsNumText.gameObject.SetActive(true);
            HeartsNumText.text = defaultHearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
            HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
        }
        else
        {
            if (heartTimerCoroutine != null)
                StopCoroutine(heartTimerCoroutine);
            heartTimerCoroutine = StartCoroutine(NextHeartTimer(HeartsNumText, HeartsTimerText, heartImage));
        }
    }

    public void StopHeartTimer()
    {
        if (heartTimerCoroutine != null)
            StopCoroutine(heartTimerCoroutine);
    }

    private IEnumerator NextHeartTimer(Text HeartsNumText, Text HeartsTimerText, Image heartImage)
    {
        int infiniteHeartDays = 0;
        TimeSpan heartTime = DateTime.Now - Manager.instance.heartsRefilledTime;
        if (Manager.instance.isInfiniteHeart)
        {
            HeartsNumText.gameObject.SetActive(false);
            heartImage.sprite = infiniteHeartIcon;
            //HeartsNumText.text = "∞";
            heartTime = new TimeSpan(0, Manager.instance.infiniteHeartMinute, 0) - heartTime;
            infiniteHeartDays = heartTime.Days;
        }
        else
        {
            HeartsNumText.gameObject.SetActive(true);
            heartImage.sprite = heartIcon;
            heartTime = new TimeSpan(0, 30, 0) - heartTime;
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
        }
        do
        {
            if (Manager.instance.isInfiniteHeart && Manager.instance.infiniteHeartMinute > 60)
            {
                HeartsTimerText.text = heartTime.ToString(@"hh\:mm\:ss");
                HeartsTimerText.text =
                    (int.Parse(HeartsTimerText.text.Substring(0, 2)) + infiniteHeartDays * 24).ToString() +
                        HeartsTimerText.text.Substring(2, 6);
                if (LanguageManager.instance.isLanguageRTL)
                    HeartsTimerText.text = LanguageManager.arabicFix(HeartsTimerText.text);
            }
            else
            {
                HeartsTimerText.text = heartTime.ToString(@"hh\:mm\:ss").Substring(3, 5);
                if (LanguageManager.instance.isLanguageRTL)
                    HeartsTimerText.text = LanguageManager.arabicFix(HeartsTimerText.text);
            }
            yield return new WaitForSeconds(1);
            heartTime = heartTime.Subtract(new TimeSpan(0, 0, 1));
        } while (heartTime.TotalSeconds >= 0);
        if (Manager.instance.isInfiniteHeart)
        {
            Manager.instance.isInfiniteHeart = false;
            Manager.instance.hearts = 5;
            //heartImage.sprite = heartSprite;
            HeartsNumText.gameObject.SetActive(true);
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
            StartCoroutine(Transitions.stamp(HeartsNumText.gameObject, new Vector3(3f, 3f, 1), 0.2f));
            HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
            Manager.instance.heartsRefilledTime = DateTime.MinValue;
        }
        else
        {
            Manager.instance.hearts++;
            HeartsNumText.text = Manager.instance.hearts.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                HeartsNumText.text = LanguageManager.arabicFix(HeartsNumText.text);
            StartCoroutine(Transitions.stamp(HeartsNumText.gameObject, new Vector3(3f, 3f, 1), 0.2f));
            if (Manager.instance.hearts == 5)
            {
                HeartsTimerText.text = LanguageManager.instance.GetTheTextByKey(textkey_full);
                Manager.instance.heartsRefilledTime = DateTime.MinValue;
            }
            else
            {
                Manager.instance.heartsRefilledTime =
                    Manager.instance.heartsRefilledTime.Add(new TimeSpan(0, 0, 1800));
                heartTimerCoroutine = StartCoroutine(NextHeartTimer(HeartsNumText, HeartsTimerText, heartImage));
            }
        }
        Manager.instance.Save(false);
    }

    public void UpdateCoins(Text coinText,int addedCoins)
    {
        Manager.instance.coins += addedCoins;
        Quests.CheckQuestType(QuestType.EarnCoins, true, addedCoins, false, false);
        if (coinText)
        {
            GameObject coin = coinText.transform.parent.gameObject;
            coinText.text = Manager.instance.coins.ToString();
            if (LanguageManager.instance.isLanguageRTL)
                coinText.text = LanguageManager.arabicFix(coinText.text);
            StartCoroutine(Transitions.stamp(coin, new Vector3(2f, 2f, 2f), 0.2f));
            Vector3 coinImagePos = coin.transform.GetChild(0).position;
            var particleMain = Instantiate(updateParticle, coinImagePos, Quaternion.identity, coin.transform).GetComponent<ParticleSystem>().main;
            particleMain.loop = false;
        }
    }

    public void UpdateInfiniteHeart(Text heartsNumText,Text heartsTimeText, Image heartImage, int addedHeartTime) {
        if (Manager.instance.isInfiniteHeart)
        {
            Manager.instance.infiniteHeartMinute += addedHeartTime;
        }
        else
        {
            Manager.instance.isInfiniteHeart = true;
            Manager.instance.heartsRefilledTime = DateTime.Now;
            Manager.instance.infiniteHeartMinute = addedHeartTime;
        }
        Quests.CheckQuestType(QuestType.EarnInfiniteHearts, true, addedHeartTime, false, false);
        if (heartsNumText && heartsTimeText)
        {
            GameObject heart = heartsTimeText.transform.parent.gameObject;
            instance.StopHeartTimer();
            instance.InitHeartValues(heartsNumText, heartsTimeText, heartImage);
            StartCoroutine(Transitions.stamp(heart, new Vector3(2f, 2f, 2f), 0.2f));
            Vector3 heartImagePos = heart.transform.GetChild(0).position;
            var particleMain = Instantiate(updateParticle, heartImagePos, Quaternion.identity, heart.transform).GetComponent<ParticleSystem>().main;
            particleMain.loop = false;
        }
    }

}
