using UnityEngine;
using System.Collections;

public class TempStaticMemory {

    
    public static bool isGamePlaying= true;
    public static bool isInstallPrepare = false;

    public static int gold = 0;
    public static int enemykill;
    public static int openStageCount = 0;
    
	public static string initStageLevel = "1000000000";
    public static int playCount = 0;

    public static string AliasData = "111_10000";

    public static bool isOpenPopUp = false;

    public static bool isTutoProcessOn = false;
    
    public static int gateLevel =0;
    public static int gameCount = 0;

    public static void GamePause()
    {
        Time.timeScale = 0.0f;
    }
    public static void GameResume()
    {
        Time.timeScale = 1.0f;
    }

    

}
