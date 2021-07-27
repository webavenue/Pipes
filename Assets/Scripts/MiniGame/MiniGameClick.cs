using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameClick : MonoBehaviour
{
    [HideInInspector]
    public MiniGame fatherScript;

    public void OnMouseDown()
    {
        if (!fatherScript.isEnd)
        {
            Sounds.instance.PlayShapeDestroy();
            VibrationManager.instance.VibrateSuccess();
            shapeProcess();
        }
    }

    public void shapeProcess()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        //ParticleManager.instance.playSuccessDestroyEffect(transform.position, GetComponent<SpriteRenderer>().color);
        StartCoroutine(Transitions.scaleDownAndDestroy(gameObject, Vector3.zero, 0.2f));
        fatherScript.numberOfUndestroyedShapes--;
        if (fatherScript.numberOfUndestroyedShapes == 0)
        {
            fatherScript.RevealCard();
        }
    }
}
