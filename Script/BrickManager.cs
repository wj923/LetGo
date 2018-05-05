using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour{
    public GameObject[] bricks;
    public Material[] colors;
    private static GameObject[] BRICKS;
    private static Material[] COLORS;

    public static List<Bricks> brickList = new List<Bricks>();

    private void Start()
    {
        BRICKS = bricks;
        COLORS = colors;
    }

    public static GameObject getBrick(int type)
    {
        switch (type)
        {
            case 0 : return BRICKS[0];
            case 1 : return BRICKS[1];
            case 2 : return BRICKS[2];
            case 3 : return BRICKS[3];
            case 4 : return BRICKS[4];
            case 5 : return BRICKS[5];
            case 6 : return BRICKS[6];
            case 7 : return BRICKS[7];
            case 8 : return BRICKS[8];
            default : return null;
        }
    }

    public static Material getColor(string color)
    {
        switch (color)
        {
            case "Red": return BrickManager.COLORS[0];
            case "Yellow": return BrickManager.COLORS[1];
            case "Green": return BrickManager.COLORS[2];
            case "Blue": return BrickManager.COLORS[3];
            default: return null;
        }
    }
}
