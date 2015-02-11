using UnityEngine;
using System.Collections;

public class LevelNames {
    static public string[] Names;

    static LevelNames()
    {
        Names = new string[7];

        Names[0] = "Free Flight";
        Names[1] = "Too Easy";
        Names[2] = "Straight Line";
        Names[3] = "Air Columns";
        Names[4] = "Wind Boost";
        Names[5] = "Rocket Man";
    }
}
