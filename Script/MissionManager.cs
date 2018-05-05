using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour {
    public GameObject[] mission;

    private GameObject currentMission;
    private int missionClear;
    public static int matchCount = 0;

    void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            currentMission = Instantiate(mission[0]);
            missionClear = currentMission.transform.childCount;
        }

        if(missionClear!=0 && matchCount == missionClear)
        {
            print("Mission Clear!!!");
            missionClear = 0;
        }
	}
}
