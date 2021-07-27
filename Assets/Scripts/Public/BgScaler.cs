using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgScaler : MonoBehaviour
{
    public GameObject Back2;
    public GameObject Back3;
    private SpriteRenderer background;
    private SpriteRenderer secondBackground;
    private SpriteRenderer thirdBackground;

    [HideInInspector]
    public static Vector2 topRightCorner = new Vector2(2.8f, 5f);
    public static Vector2 botLeftCorner = new Vector2(-2.8f, -5f);

    // Start is called before the first frame update
    void Start()
    {


        float cameraY = Camera.main.orthographicSize * 2f;
        float cameraX = cameraY / Screen.height * Screen.width;



        topRightCorner = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        botLeftCorner = new Vector2(topRightCorner.x * -1f, topRightCorner.y * -1f);


        background = GetComponent<SpriteRenderer>();

        background.transform.localScale = new Vector3(cameraX / background.sprite.bounds.size.x * 1.05f, cameraY / background.sprite.bounds.size.y * 1.05f, 1);
        background.transform.position = Camera.main.transform.position + new Vector3(0, 0, 100);

        if (Back2 != null)
        {
            secondBackground = Back2.GetComponent<SpriteRenderer>();
            if (secondBackground != null)
            {
                secondBackground.transform.localScale = new Vector3(cameraX / secondBackground.sprite.bounds.size.x * 1.05f, cameraY / secondBackground.sprite.bounds.size.y * 1.05f, 1);
                secondBackground.transform.position = Camera.main.transform.position + new Vector3(0, 0, 100);
            }
        }
        if (Back3 != null)
        {
            thirdBackground = Back3.GetComponent<SpriteRenderer>();
            if (thirdBackground != null)
            {
                thirdBackground.transform.localScale = new Vector3(cameraX / thirdBackground.sprite.bounds.size.x * 1.05f, cameraY / thirdBackground.sprite.bounds.size.y * 1.05f, 1);
                thirdBackground.transform.position = Camera.main.transform.position + new Vector3(0, 0, 100);
            }
        }
    }
}
