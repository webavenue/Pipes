using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public static Sounds instance;

    public float DefaultVolume;
    public float fadeTime;
    public AudioClip capture;
    public AudioClip levelStart;
    public AudioClip levelComplete;
    public AudioClip[] giveStar;
    public AudioClip[] destFill;
    public AudioClip[] wellFill;
    public AudioClip[] rotatePipes;
    public AudioClip[] error;
    public AudioClip[] UI;
    public AudioClip settingButtonUI;
    public AudioClip waterFlow;
    public AudioClip plantGrow;
    public AudioClip[] rotateWells;
    public AudioClip[] rotateDestination;
    public AudioClip plantFade;
    public AudioClip levelEnd;
    public AudioClip starProgressBar;
    public AudioClip chestOpening;
    public AudioClip chestItems;
    public AudioClip dailyChallengeDone;
    public AudioClip giveACard;
    public AudioClip showPopUp;
    public AudioClip earnCoin;
    public AudioClip chestPrepare;
    public AudioClip hintUse;
    public AudioClip infiniteMovesUse;
    public AudioClip undo;
    public AudioClip[] shapeDestroy; // tap on all shapes!
    public AudioClip nextLevel;
    public AudioClip nextChapter;
    public AudioClip wellEmpty;
    public AudioClip extraMovesRewarded;
    public AudioClip poof3Seconds;

    [HideInInspector]
    public AudioSource _audioSource;

    private int giveStarIndex;
    private int destFillIndex;
    private int wellFillIndex;
    private int rotatePipesIndex;
    private int errorIndex;
    private int UIIndex;
    private int rotateWellsIndex;
    private int rotateDestinationIndex;
    private int shapeDestroyIndex;

    // Start is called before the first frame update
    void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
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

    public void StopTheSound()
    {
        if (_audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
    }

    public void PlayCapture()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = capture;
            _audioSource.Play();
        }
    }

    public void PlayLevelStart() {
        if (Manager.instance.isSoundEnabled)
        {
            StartCoroutine(Music.FadeInMusic(_audioSource, DefaultVolume, fadeTime));
            _audioSource.clip = levelStart;
            _audioSource.Play();
        }
    }

    public void PlayLevelComplete()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = levelComplete;
            _audioSource.Play();
        }
    }

    public void PlayGiveStar()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = giveStar[giveStarIndex];
            _audioSource.Play();
            giveStarIndex++;
            giveStarIndex %= giveStar.Length;
        }
    }

    public void PlayDestFill()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = destFill[destFillIndex];
            _audioSource.Play();
            destFillIndex++;
            destFillIndex %= destFill.Length;
        }
    }

    public void PlayWellFill()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = wellFill[wellFillIndex];
            _audioSource.Play();
            wellFillIndex++;
            wellFillIndex %= wellFill.Length;
        }
    }

    public void PlayRotatePipes()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = rotatePipes[rotatePipesIndex];
            _audioSource.Play();
            rotatePipesIndex++;
            rotatePipesIndex %= rotatePipes.Length;
        }
    }

    public void PlayError()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = error[errorIndex];
            _audioSource.Play();
            errorIndex++;
            errorIndex %= error.Length;
        }
    }

    public void PlayUIInteraction()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = UI[UIIndex];
            _audioSource.Play();
            UIIndex++;
            UIIndex %= UI.Length;
        }
    }

    public void PlaySettingButtonUI()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = settingButtonUI;
            _audioSource.Play();
        }
    }

    public void PlayWaterFlow()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = waterFlow;
            _audioSource.Play();
        }
    }

    public void PlayPlantGrow()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = plantGrow;
            _audioSource.Play();
        }
    }

    public void PlayRotateWells()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = rotateWells[rotateWellsIndex];
            _audioSource.Play();
            rotateWellsIndex++;
            rotateWellsIndex %= rotateWells.Length;
        }
    }

    public void PlayRotateDestination()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = rotateDestination[rotateDestinationIndex];
            _audioSource.Play();
            rotateDestinationIndex++;
            rotateDestinationIndex %= rotateDestination.Length;
        }
    }

    public void PlayPlantFade()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = plantFade;
            _audioSource.Play();
        }
    }

    public void PlayLevelEnd()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = levelEnd;
            _audioSource.Play();
        }
    }

    public void PlayStarProgressBar()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = starProgressBar;
            _audioSource.Play();
        }
    }

    public void PlayChestOpening()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = chestOpening;
            _audioSource.Play();
        }
    }

    public void PlayChestItems()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = chestItems;
            _audioSource.Play();
        }
    }

    public void PlayDailyChallengeDone()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = dailyChallengeDone;
            _audioSource.Play();
        }
    }

    public void PlayGiveACard()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = giveACard;
            _audioSource.Play();
        }
    }

    public void PlayShowPopUp()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = showPopUp;
            _audioSource.Play();
        }
    }

    public void PlayEarnCoin()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = earnCoin;
            _audioSource.Play();
        }
    }

    public void PlayChestPrepare()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = chestPrepare;
            _audioSource.Play();
        }
    }

    public void PlayHintUse()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = hintUse;
            _audioSource.Play();
        }
    }

    public void PlayInfiniteMovesUse()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = infiniteMovesUse;
            _audioSource.Play();
        }
    }

    public void PlayUndo()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = undo;
            _audioSource.Play();
        }
    }

    public void PlayShapeDestroy()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = shapeDestroy[shapeDestroyIndex];
            _audioSource.Play();
            shapeDestroyIndex++;
            shapeDestroyIndex %= shapeDestroy.Length;
        }
    }

    public void PlayNextLevel()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = nextLevel;
            _audioSource.Play();
        }
    }

    public void PlayNextChapter()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = nextChapter;
            _audioSource.Play();
        }
    }

    public void PlayWellEmpty()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = wellEmpty;
            _audioSource.Play();
        }
    }

    public void PlayAfterVideoAdReward()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = extraMovesRewarded;
            _audioSource.Play();
        }
    }

    public void PlayPoof3Seconds()
    {
        if (Manager.instance.isSoundEnabled)
        {
            _audioSource.clip = poof3Seconds;
            _audioSource.Play();
        }
    }

}
