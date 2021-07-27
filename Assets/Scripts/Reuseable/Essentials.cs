
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Essentials : MonoBehaviour
{
    public static void initAndShuffleList(List<Point2D> tileList)
    {
        for (int i = 0; i < tileList.Count; i++)
        {
            Point2D temp = tileList[i];
            int r = Random.Range(i, tileList.Count);
            tileList[i] = tileList[r];
            tileList[r] = temp;
        }
    }

    public static void ShuffleArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            T temp = array[i];
            int r = Random.Range(i, array.Length);
            array[i] = array[r];
            array[r] = temp;
        }
    }
    public static void InitAndShuffleArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = i;
        }
        for (int i = 0; i < array.Length; i++)
        {
            int temp = array[i];
            int r = UnityEngine.Random.Range(i, array.Length);
            array[i] = array[r];
            array[r] = temp;
        }
    }

    public static void MakeTint(Image image,float colorMultiplier) {
        Color imageColor = image.color;
        image.color =
            new Color(imageColor.r * colorMultiplier, imageColor.g * colorMultiplier, 
                imageColor.b * colorMultiplier, 1);
    }
}
