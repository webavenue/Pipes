using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZColor : MonoBehaviour
{
    public static readonly Color OrangeLeaf = HexColorBuilder("#D4696F");
    public static readonly Color WaterBlue = HexColorBuilder("#4AB0CC");
    public static readonly Color NoWaterBlue = HexColorBuilder("#15617D");

    private static Color ColorBuilder(float r, float g, float b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    private static Color ColorBuilder(float r, float g, float b, float a)
    {
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }
    private static Color HexColorBuilder(string hexCode)
    {
        Color theColor;
        int r, g, b;
        r = 0;
        g = 0;
        b = 0;
        if ((hexCode.StartsWith("#")) && (hexCode.Length == 7))
        {
            r = int.Parse(hexCode.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
            g = int.Parse(hexCode.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
            b = int.Parse(hexCode.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
            theColor = ColorBuilder(r, g, b);
        }
        else
        {
            theColor = Color.white;
        }
        return theColor;
    }
}
