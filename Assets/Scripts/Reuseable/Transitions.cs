using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Transitions : MonoBehaviour
{

    public static IEnumerator moveToPosition(GameObject obj, Vector3 finalPosition, float totalTime)
    {
        Transform objTransform = obj.GetComponent<Transform>();
        Vector3 startPosition = objTransform.position;
        float currentTime = 0f;
        do
        {
            objTransform.position = Vector3.Lerp(startPosition, finalPosition, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            objTransform.position = finalPosition;
    }
    
    public static IEnumerator moveToPositionUI(GameObject obj, Vector3 finalPosition, float totalTime)
    {
        Vector3 startPosition = obj.GetComponent<RectTransform>().localPosition;
        float currentTime = 0f;
        do
        {
            obj.GetComponent<RectTransform>().localPosition = Vector3.Lerp(startPosition, finalPosition, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.GetComponent<RectTransform>().localPosition = finalPosition;
    }

    public static IEnumerator moveToPositionOnlyX(GameObject obj, float finalX, float totalTime)
    {
        Vector3 startPosition = obj.transform.position;
        float diffX = finalX - startPosition.x;
        float currentTime = 0f;
        do
        {
            obj.GetComponent<Transform>().position = new Vector3(startPosition.x + (currentTime / totalTime) * diffX, obj.transform.position.y, obj.transform.position.z);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.GetComponent<Transform>().position = new Vector3(finalX, obj.transform.position.y, obj.transform.position.z);
    }

    public static IEnumerator moveToPositionAndDestroy(GameObject obj, Vector3 finalPosition, float totalTime)
    {
        Vector3 startPosition = obj.GetComponent<Transform>().position;
        float currentTime = 0f;
        do
        {
            obj.GetComponent<Transform>().position = Vector3.Lerp(startPosition, finalPosition, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.GetComponent<Transform>().position = finalPosition;
        Destroy(obj);
    }

    public static IEnumerator fadeInSprite(GameObject obj, float finalAlpha, float totalTime)
    {
        obj.SetActive(true);
        float r = obj.GetComponent<SpriteRenderer>().color.r;
        float g = obj.GetComponent<SpriteRenderer>().color.g;
        float b = obj.GetComponent<SpriteRenderer>().color.b;

        Vector4 startVec = new Vector4(r, g, b, 0);
        Vector4 destinationVec = new Vector4(r, g, b, finalAlpha);

        float currentTime = 0f;

        do
        {
            obj.GetComponent<SpriteRenderer>().color = Vector4.Lerp(startVec, destinationVec, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.GetComponent<SpriteRenderer>().color = destinationVec;
    }

    public static IEnumerator fadeOutSprite(GameObject obj, float startingAlpha, float totalTime)
    {
        float r = obj.GetComponent<SpriteRenderer>().color.r;
        float g = obj.GetComponent<SpriteRenderer>().color.g;
        float b = obj.GetComponent<SpriteRenderer>().color.b;

        Vector4 startVec = new Vector4(r, g, b, startingAlpha);
        Vector4 destinationVec = new Vector4(r, g, b, 0);

        float currentTime = 0f;

        do
        {
            obj.GetComponent<SpriteRenderer>().color = Vector4.Lerp(startVec, destinationVec, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if (obj)
        {
            obj.GetComponent<SpriteRenderer>().color = destinationVec;
            obj.SetActive(false);
        }
    }

    public static IEnumerator fadeIn(GameObject obj, float finalAlpha,float totalTime)
    {
        obj.SetActive(true);
        float r = obj.GetComponent<Image>().color.r;
        float g = obj.GetComponent<Image>().color.g;
        float b = obj.GetComponent<Image>().color.b;

        Vector4 startVec = new Vector4(r,g,b,0);
        Vector4 destinationVec = new Vector4(r,g,b,finalAlpha);

        float currentTime = 0f;
        
        do{
            obj.GetComponent<Image>().color = Vector4.Lerp(startVec,destinationVec,currentTime/totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        }while(currentTime <= totalTime && obj);
        if(obj)
            obj.GetComponent<Image>().color = destinationVec;
    }

    public static IEnumerator fadeOut(GameObject obj, float startingAlpha,float totalTime)
    {
        float r = obj.GetComponent<Image>().color.r;
        float g = obj.GetComponent<Image>().color.g;
        float b = obj.GetComponent<Image>().color.b;

        Vector4 startVec = new Vector4(r,g,b,startingAlpha);
        Vector4 destinationVec = new Vector4(r,g,b,0);

        float currentTime = 0f;
        do{
            obj.GetComponent<Image>().color = Vector4.Lerp(startVec,destinationVec,currentTime/totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        }while(currentTime <= totalTime && obj);
        if (obj)
        {
            obj.GetComponent<Image>().color = destinationVec;
            obj.SetActive(false);
        }
    }

    public static IEnumerator fadeInText(Text obj, float finalAlpha, float totalTime)
    {
        float r = obj.GetComponent<Text>().color.r;
        float g = obj.GetComponent<Text>().color.g;
        float b = obj.GetComponent<Text>().color.b;

        Vector4 startVec = new Vector4(r, g, b, 0);
        Vector4 destinationVec = new Vector4(r, g, b, finalAlpha);

        float currentTime = 0f;

        do
        {
            obj.GetComponent<Text>().color = Vector4.Lerp(startVec, destinationVec, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.GetComponent<Text>().color = destinationVec;
    }

    public static IEnumerator fadeOutText(Text obj, float startingAlpha, float totalTime)
    {
        float r = obj.GetComponent<Text>().color.r;
        float g = obj.GetComponent<Text>().color.g;
        float b = obj.GetComponent<Text>().color.b;

        Vector4 startVec = new Vector4(r, g, b, startingAlpha);
        Vector4 destinationVec = new Vector4(r, g, b, 0);

        float currentTime = 0f;
        do
        {
            obj.GetComponent<Text>().color = Vector4.Lerp(startVec, destinationVec, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.GetComponent<Text>().color = destinationVec;
    }

    public static IEnumerator FillBarByScale(RectTransform greenBar,float addedScale,float totalTime) {
        Vector2 startScale = greenBar.localScale;
        Vector2 destinationScale = new Vector3(startScale.x + addedScale,startScale.y,1);
        float currentTime = 0.0f;

        do
        {
            greenBar.localScale = Vector2.Lerp(startScale, destinationScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime);
        greenBar.localScale = destinationScale;
    }

    public static IEnumerator setScale(GameObject obj, Vector3 destinationScale, float totalTime){
        Vector3 startScale = obj.transform.localScale;
        float currentTime = 0.0f;
        
        do
        {
            obj.transform.localScale = Vector3.Lerp(startScale, destinationScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.transform.localScale = destinationScale;
    }

    public static IEnumerator scaleUp(GameObject obj, Vector3 startScale, float totalTime)
    {
        obj.SetActive(true);
        Vector3 destinationScale = obj.transform.localScale;
        float currentTime = 0.0f;
        
        do
        {
            obj.transform.localScale = Vector3.Lerp(startScale, destinationScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.transform.localScale = destinationScale;
    }

    public static IEnumerator scaleDownAndDestroy(GameObject obj, Vector3 destinationScale, float totalTime){
        Vector3 startVec = obj.transform.localScale;
        float currentTime = 0.0f;

        do{
            obj.transform.localScale = Vector3.Lerp(startVec, destinationScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        }while(currentTime <= totalTime && obj);
        if (obj)
        {
            obj.transform.localScale = destinationScale;
            obj.SetActive(false);
            Destroy(obj);
        }
    }

    public static IEnumerator stamp(GameObject obj, Vector3 startVec, float totalTime)
    {
        obj.SetActive(true);
        float loweringPercent = 0.4f;//How Much it is lowered in scale
        Vector3 destinationScale = obj.transform.localScale;
        Vector3 lowerDestinationScale = new Vector3(obj.transform.localScale.x * loweringPercent,
                                                    obj.transform.localScale.y * loweringPercent,
                                                    obj.transform.localScale.z);
        float currentTime = 0.0f;
        do
        {
            obj.transform.localScale = Vector3.Lerp(startVec, lowerDestinationScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            startVec = obj.transform.localScale;
        currentTime = 0.0f;
        totalTime /= 2;
        do
        {
            obj.transform.localScale = Vector3.Lerp(startVec, destinationScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.transform.localScale = destinationScale;
    }

    public static IEnumerator stampAndDestroy(GameObject obj, Vector3 startVec, float totalTime, float destroyDelay)
    {
        obj.SetActive(true);
        Vector3 destinationScale = obj.transform.localScale;
        float currentTime = 0.0f;

        do
        {
            obj.transform.localScale = Vector3.Lerp(startVec, destinationScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if (obj)
        {
            obj.transform.localScale = destinationScale;
            yield return new WaitForSeconds(destroyDelay);
            Destroy(obj);
        }
        else {
            yield return null;
        }
    }

    public static IEnumerator moveDescriptionObj(GameObject obj, Vector3 newpos, Color newColor, float moveDelayTime, float travelTime)
    {
        Text objText = obj.GetComponent<Text>();

        Vector4 sourceColor = new Vector4(objText.color.r, objText.color.g, objText.color.b, objText.color.a);
        Vector4 destinationColor = new Vector4(newColor.r, newColor.g, newColor.b, newColor.a);
        Vector3 sourcePosition = obj.transform.position;
        Vector3 destinationPosition = newpos;

        yield return new WaitForSeconds(moveDelayTime);
        float currentTime = 0.0f;
        do
        {
            float timeFraction = currentTime / travelTime;
            obj.transform.position = Vector3.Lerp(sourcePosition, destinationPosition, timeFraction);
            objText.color = Vector4.Lerp(sourceColor, destinationColor, timeFraction);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= travelTime && obj);
        if (obj)
        {
            obj.transform.position = destinationPosition;
            objText.color = destinationColor;
        }
    }

    public static IEnumerator spin(GameObject obj, bool clockwise, int numOfSpins, float totalTime)
    {
        Vector3 startingAngle = obj.transform.rotation.eulerAngles;
        Vector3 finalAngle = new Vector3(startingAngle.x, startingAngle.y,
            (clockwise ? startingAngle.z - 359 : startingAngle.z + 359));
        float currentTime = 0.0f;
        float oneSpinTime = totalTime / numOfSpins;
        for (int i = 0; i < numOfSpins; i++)
        {
            do
            {
                obj.transform.rotation = Quaternion.Euler(Vector3.Lerp(startingAngle, finalAngle, currentTime / oneSpinTime));
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= oneSpinTime && obj);
            if(obj)
                obj.transform.rotation = Quaternion.Euler(startingAngle);//because we want it to remain unchanged at the end
        }
    }

    public static IEnumerator spinLevelNumber(GameObject objIcon, GameObject objNum, bool clockwise, int numOfSpins, float totalTime)
    {
        objIcon.SetActive(true);
        objNum.SetActive(false);
        Vector3 startingAngle = objIcon.transform.rotation.eulerAngles;
        Vector3 finalAngle = new Vector3(startingAngle.x, startingAngle.y, 
            (clockwise ? startingAngle.z - 359 : startingAngle.z + 359));
        float currentTime = 0.0f;
        float oneSpinTime = totalTime / numOfSpins;
        for (int i = 0; i < numOfSpins; i++)
        {
            do
            {
                objIcon.transform.rotation = Quaternion.Euler(Vector3.Lerp(startingAngle, finalAngle, currentTime / oneSpinTime));
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= oneSpinTime);
            objIcon.transform.rotation = Quaternion.Euler(startingAngle);//because we want it to remain unchanged at the end
        }
        objIcon.SetActive(false);
        objNum.SetActive(true);
    }

    public static IEnumerator spinMusic(GameObject objMusic, Sprite nextSprite, Color nextColor, bool clockwise, float totalTime)
    {
        Vector3 startingAngle = new Vector3(0f, 0f, 0f);
        Vector3 finalAngle = new Vector3(startingAngle.x, startingAngle.y,
            (clockwise ? startingAngle.z - 180 : startingAngle.z + 180));
        float currentTime = 0.0f;
        do
        {
            objMusic.transform.rotation = Quaternion.Euler(Vector3.Lerp(startingAngle, finalAngle, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime);
        objMusic.transform.rotation = Quaternion.Euler(finalAngle);
        objMusic.GetComponent<Image>().sprite = nextSprite;
        objMusic.GetComponent<Image>().color = nextColor;
        startingAngle = finalAngle;
        finalAngle = new Vector3(startingAngle.x, startingAngle.y,
            (clockwise ? startingAngle.z - 180 : startingAngle.z + 180));
        currentTime = 0.0f;
        do
        {
            objMusic.transform.rotation = Quaternion.Euler(Vector3.Lerp(startingAngle, finalAngle, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime);
        objMusic.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    public static IEnumerator rotateZ(GameObject obj, float degree,bool clockwise,float totalTime) {
        Vector3 startingAngle = new Vector3(obj.transform.rotation.eulerAngles.x,
            obj.transform.rotation.eulerAngles.y,obj.transform.rotation.eulerAngles.z%360);
        Vector3 finalAngle = new Vector3(startingAngle.x, startingAngle.y,
            (clockwise ? startingAngle.z - degree : startingAngle.z + degree));
        float currentTime = 0.0f;
        do
        {
            obj.transform.rotation = Quaternion.Euler(Vector3.Lerp(startingAngle, finalAngle, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if(obj)
            obj.transform.rotation = Quaternion.Euler(finalAngle);//because we want it to remain unchanged at the end
    }

    public static IEnumerator RotateY(GameObject obj, float degree, bool clockwise, float totalTime)
    {
        Vector3 startingAngle = new Vector3(obj.transform.rotation.eulerAngles.x,
            obj.transform.rotation.eulerAngles.y % 360, obj.transform.rotation.eulerAngles.z);
        Vector3 finalAngle = new Vector3(startingAngle.x, (clockwise ? startingAngle.y - degree : startingAngle.y + degree), startingAngle.z);
        float currentTime = 0.0f;
        do
        {
            obj.transform.rotation = Quaternion.Euler(Vector3.Lerp(startingAngle, finalAngle, currentTime / totalTime));
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && obj);
        if (obj)
            obj.transform.rotation = Quaternion.Euler(finalAngle);//because we want it to remain unchanged at the end
    }

    public static IEnumerator ErrorRotateZ(GameObject obj, float Errordegree, bool clockwise, float errorRotateTime, int rotateTimes) {
        Vector3 startingAngle = new Vector3(obj.transform.rotation.eulerAngles.x,
            obj.transform.rotation.eulerAngles.y, obj.transform.rotation.eulerAngles.z % 360);
        Vector3 finalAngle1 = new Vector3(startingAngle.x, startingAngle.y,
            (clockwise ? startingAngle.z - Errordegree : startingAngle.z + Errordegree));
        Vector3 finalAngle2 = new Vector3(startingAngle.x, startingAngle.y,
            (!clockwise ? startingAngle.z - Errordegree : startingAngle.z + Errordegree));
        Vector3 finalAngle = startingAngle;
        for (int i = 0; i < rotateTimes; i++)
        {
            Vector3 currentAngle = finalAngle;
            if (i == rotateTimes - 1)
                finalAngle = startingAngle;
            else if (i % 2 == 0)
                finalAngle = finalAngle1;
            else
                finalAngle = finalAngle2;
            float currentTime = 0.0f;
            do
            {
                obj.transform.rotation = Quaternion.Euler(Vector3.Lerp(currentAngle, finalAngle, currentTime / errorRotateTime));
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= errorRotateTime && obj);
            if(obj)
                obj.transform.rotation = Quaternion.Euler(finalAngle);//because we want it to remain unchanged at the end
        }
    }

    // the vector it rotates around aroundVector.
    public static IEnumerator rotate3D(GameObject obj,Vector3 aroundVector,float degree,float totalTime) {
        float currentDegree = 0; // amount of degree until now  
        Transform objTransform = obj.transform;
        do
        {
            objTransform.RotateAround(objTransform.position, aroundVector, (Time.deltaTime / totalTime) * degree);
            currentDegree += (Time.deltaTime / totalTime) * degree;
            yield return null;
        } while (Mathf.Abs(currentDegree) < Mathf.Abs(degree));
        objTransform.RotateAround(objTransform.position, aroundVector, degree - currentDegree);
    }

    public static IEnumerator scaleXandYSeperately(GameObject obj, Vector3 startingScale,bool xIsFirst, float totalTime)
    {
        Vector3 theFinalScale = obj.transform.localScale;
        obj.transform.localScale = startingScale;
        Vector3 finalScale = startingScale;
        if (xIsFirst)
        {
            finalScale.x = theFinalScale.x;
        }
        else
        {
            finalScale.y = theFinalScale.y;
        }
        float halfTime = totalTime / 2;
        float currentTime = 0.0f;
        do
        {
            obj.transform.localScale = Vector3.Lerp(startingScale, finalScale, currentTime / halfTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= halfTime);
        obj.transform.localScale = finalScale;
        startingScale = finalScale;
        currentTime = 0.0f;
        do
        {
            obj.transform.localScale = Vector3.Lerp(startingScale, theFinalScale, currentTime / halfTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= halfTime);
        obj.transform.localScale = theFinalScale;
    }

    public static IEnumerator changeColor(SpriteRenderer SR, Color desColor, float totalTime)
    {
        Color startColor = SR.color;
        float currentTime = 0.0f;
        do
        {
            SR.color = Color.Lerp(startColor, desColor, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && SR);
        if(SR)
            SR.color = desColor;
    }

    public static IEnumerator changeColor(Image img, Color desColor, float totalTime)
    {
        Color startColor = img.color;
        float currentTime = 0.0f;
        do
        {
            img.color = Color.Lerp(startColor, desColor, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime && img);
        if(img)
            img.color = desColor;
    }

    public static IEnumerator beatObject(GameObject obj, float scalerMultiplier, int beatCount, float beatTime)
    {
        // beatCount consist of a scale up and a scale down ! 
        Vector3 sourceScale = obj.transform.localScale;
        Vector3 destinationScale = new Vector3(sourceScale.x * scalerMultiplier, sourceScale.y * scalerMultiplier, sourceScale.z * scalerMultiplier);

        for (int i = 0; i < beatCount * 2; i++)
        {
            float currentTime = 0.0f;
            do
            {
                obj.transform.localScale = Vector3.Lerp(sourceScale, destinationScale, currentTime / beatTime);
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= beatTime && obj);
            if(obj)
                obj.transform.localScale = destinationScale;
            Vector3 temp = sourceScale;
            sourceScale = destinationScale;
            destinationScale = temp;
        }
    }

    public static IEnumerator typeTheText(Text textObj, float letterTime)
    {
        if (LanguageManager.instance.isLanguageRTL)
        {
            char[] theDescChars = textObj.text.ToCharArray();
            int numOfLetters = theDescChars.Length;
            if (textObj.text.Contains("\n"))
            {
                int enterIndex = textObj.text.IndexOf("\n");
                textObj.text = "";
                for (int i = enterIndex - 1; i >= 0; i--)
                {
                    textObj.text = theDescChars[i] + textObj.text;
                    yield return new WaitForSecondsRealtime(letterTime);
                }
                textObj.text = textObj.text + "\n";
                for (int i = numOfLetters - 1; i > enterIndex; i--)
                {
                    textObj.text = textObj.text.Insert(enterIndex + 1, theDescChars[i].ToString());
                    yield return new WaitForSecondsRealtime(letterTime);
                }
            }
            else
            {
                textObj.text = "";
                for (int i = 0; i < numOfLetters; i++)
                {
                    textObj.text = theDescChars[numOfLetters - i - 1] + textObj.text;
                    yield return new WaitForSecondsRealtime(letterTime);
                }
            }
        }
        else
        {
            char[] theDescChars = textObj.text.ToCharArray();
            int numOfLetters = theDescChars.Length;
            textObj.text = "";
            for (int i = 0; i < numOfLetters; i++)
            {
                textObj.text += theDescChars[i];
                yield return new WaitForSecondsRealtime(letterTime);
            }
        }
    }

    public static IEnumerator FillCircularImage(Image image, float totalTime, float startingFill, float finalFill)
    {
        // total time is time for filling the hole of image.
        float timeForFilling = Mathf.Abs(finalFill - startingFill) * totalTime;
        bool increasingFill = (finalFill > startingFill);

        image.fillAmount = startingFill;
        if (increasingFill)
        {
            float currentTime = timeForFilling;
            do
            {
                image.fillAmount = startingFill + (1 - (currentTime / timeForFilling)) * (finalFill - startingFill);
                currentTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (currentTime >= 0);
        }
        else
        {
            float currentTime = timeForFilling;
            do
            {
                image.fillAmount = (currentTime / timeForFilling) * (startingFill - finalFill) + finalFill;
                currentTime -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            } while (currentTime >= 0);
        }
        image.fillAmount = finalFill;
    }

    public static IEnumerator destroyWithDelay(GameObject go, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (go)
            Destroy(go);
    }

    public static IEnumerator turnOffWithDelay(GameObject go, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (go)
            go.SetActive(false);
    }
    
    public static IEnumerator turnOnImageWithDelay(GameObject obj, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        if (obj)
        {
            Image imgComponent = obj.GetComponent<Image>();
            imgComponent.enabled = true;
            float r = imgComponent.color.r;
            float g = imgComponent.color.g;
            float b = imgComponent.color.b;
            imgComponent.color = new Vector4(r, g, b, 1f);
            obj.SetActive(true);
        }
    }

    public static IEnumerator ChangeScaleImage(GameObject obj, float newWidth, float newHeight, float totalTime)
    {
        if (obj)
        {
            RectTransform rect = obj.GetComponent<RectTransform>();
            Vector2 startScale = rect.sizeDelta;
            Vector2 destinationScale = new Vector2(newWidth, newHeight);
            float currentTime = 0.0f;

            do
            {
                rect.sizeDelta = Vector2.Lerp(startScale, destinationScale, currentTime / totalTime);
                currentTime += Time.deltaTime;
                yield return null;
            } while (currentTime <= totalTime);
            rect.sizeDelta = destinationScale;
        }
    }

    public static IEnumerator PanelAct(GameObject circle, Vector3 finalScale, float totalTime)
    {
        circle.SetActive(true);
        Vector3 startScale = circle.GetComponent<RectTransform>().localScale;
        float currentTime = 0f;
        do
        {
            circle.GetComponent<RectTransform>().localScale = Vector3.Lerp(startScale, finalScale, currentTime / totalTime);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= totalTime);
        circle.SetActive(false);
        circle.GetComponent<RectTransform>().localScale = startScale;
    }

}
