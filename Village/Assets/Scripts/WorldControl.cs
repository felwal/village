using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldControl : MonoBehaviour {

    public static int speed = 1;
    public int lifeLengthAvg = 50; // 150
    public int growTime = 4;
    public static float intercourseCD = 4;
    public float pregnancyTime = 9;
    public static int childCap = 10; // 4
    public int walkspeed = 4;
    public static float mutatationRisk = 0.2f;

    public int genGoal = 10;

    public GameObject npcPrefab;
    public GameObject treePrefab;
    public Transform npcParent;

    // // // //

    void Start() {
        int ancestorsCount = Stat.RandInt(5, 10); // 4, 8
        for (int a = 0; a < ancestorsCount; a++) {
            Stat.RandNpc(npcPrefab, npcParent);
            //Stat.RandTree(treePrefab, npcParent);
        }
    }

    // buttons

    public void ChangeSpeed() {
        if (speed < 8) {
            speed *= 2;
        }
        else { speed = 1; }
    }

}
