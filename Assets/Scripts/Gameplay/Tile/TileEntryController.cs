using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileEntryController : MonoBehaviour
{
    [HideInInspector]
    public Animation anim;
    [HideInInspector]
    public TrailRenderer[] trails;
    [HideInInspector]
    public int particleCount;

    private AnimationState animClip; // show which animation should be played!
    private LevelManger fatherScript;
    private Vector3[] particleInitPos;
    private string tileName;
    private string entryDefaultIndex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitEntryParticlePos()
    {
        if (transform.Find("Map"))
        {
            print("hooooooy!");
        }
        fatherScript = GameObject.Find("Map").GetComponent<LevelManger>();
        particleCount = transform.childCount;
        particleInitPos = new Vector3[particleCount];
        trails = new TrailRenderer[particleCount];
        for (int i = 0; i < particleCount; i++)
        {
            Transform child = transform.GetChild(i);
            particleInitPos[i] = child.localPosition;
            trails[i] = child.GetComponent<TrailRenderer>();
        }
        anim = GetComponent<Animation>();
    }

    public void InitAnimation(object branchType)
    {
        char[] separator = { '(' };
        tileName = transform.parent.gameObject.name.Split(separator)[0];
        separator[0] = 'y';
        entryDefaultIndex = gameObject.name.Split(separator)[1];
        animClip = anim[tileName + "-" + entryDefaultIndex];
    }

    public void SetEnabledTrail(bool show)
    {
        for (int i = 0; i < particleCount; i++)
        {
            trails[i].enabled = show;
            trails[i].emitting = show;
        }
    }

    public void ClearTrail()
    {
        for (int i = 0; i < particleCount; i++)
        {
            trails[i].Clear();
            trails[i].startWidth = 0;
        }
    }

    public void EndOfAnimation()
    {
        SetEnabledTrail(false);
        for (int i = 0; i < particleCount; i++)
        {
            trails[i].transform.localPosition = particleInitPos[i];
        }
        SetEnabledTrail(true);
    }

    public void Fill()
    {
        animClip.speed = fatherScript.animSpeed;
        anim.Play();
        for (int i = 0; i < particleCount; i++)
        {
            StartCoroutine(FillEntry(i));
        }
    }

    private IEnumerator FillEntry(int index)
    {
        float currentTime = 0f;
        do
        {
            trails[index].startWidth = Mathf.Lerp(0, fatherScript.trailWidth, currentTime / fatherScript.timeOfFillAndEmpty);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= fatherScript.timeOfFillAndEmpty);
        trails[index].startWidth = fatherScript.trailWidth;
    }

    public IEnumerator Empty()
    {
        for (int i = 0; i < particleCount; i++)
        {
            StartCoroutine(EmptyEntry(i));
        }
        yield return new WaitForSeconds(fatherScript.timeOfFillAndEmpty);
        ClearTrail();
    }

    private IEnumerator EmptyEntry(int index)
    {
        float currentTime = 0f;
        do
        {
            trails[index].startWidth = Mathf.Lerp(fatherScript.trailWidth, 0, currentTime / fatherScript.timeOfFillAndEmpty);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= fatherScript.timeOfFillAndEmpty);
        trails[index].startWidth = 0;
    }
}
