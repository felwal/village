using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeCam : MonoBehaviour {

    // target
    public static string targetKey = "";
    public static string lastTargetKey = "";
    public static List<string> targetKeyQueue = new List<string>();
    public static Vector2 targetPosCirc;
    public static Vector2 lastLocalPos;
    public static int lastTargetGenPop = 1;

    // properites
    public static float Mag {
        get { return Stat.Magnitude2D(trans.position); }
    }

    public static Transform trans;
    public static Transform gameWorld;
    Transform people;
    Ancestree tree;

    // // // //

    void Start() {
        gameWorld = GameObject.Find("GameWorld").transform;
        people = GameObject.Find("GameWorld/People").transform;
        tree = people.GetComponent<Ancestree>();
        trans = transform;

        targetPosCirc = CalcPosCirc();
    }
    
    void FixedUpdate() {
        switch (tree.mode) {
            case Ancestree.Mode.AncCirc: HomeCirc(); break;
            case Ancestree.Mode.AncFrac: HomeFrac(); break;
        }

        MoveMan();
        ZoomMan();
    }

    // movements

    void HomeFrac() {
        if (targetKeyQueue.Count > 0) {

            Vector2 targetLocalPos = Ancestree.positions[targetKeyQueue[0]];
            int targetGenPop = Ancestree.GenPop(targetKeyQueue[0].Length);
            Vector2 localPos = Stat.ToVector2(transform.localPosition);
            Vector3 posDif = targetLocalPos - localPos;
            float magDifSqr = Stat.MagnitudeSqr2D(posDif);

            if (magDifSqr > 0.01f / targetGenPop / targetGenPop) {
                float v = 0.2f / targetGenPop;
                float distRatio = Stat.Magnitude2D(lastLocalPos - localPos) / Stat.Magnitude2D(lastLocalPos - targetLocalPos); // sqr?

                transform.localPosition += v * Stat.TrigOne(posDif); // move camera
                float worldSize = lastTargetGenPop + distRatio * (targetGenPop - lastTargetGenPop);
                gameWorld.localScale = new Vector3(worldSize, worldSize, 1);
            }
            else {
                transform.localPosition = Stat.ToVector3(targetLocalPos, -10);
                lastLocalPos = targetLocalPos;
                lastTargetGenPop = targetGenPop;
                lastTargetKey = targetKeyQueue[0];
                targetKeyQueue.RemoveAt(0);
            }
        }
    }
    
    void HomeCirc() {
        Vector2 pos = Stat.ToVector2(transform.position);

        float v = 0.05f;
        float magDifSqr = Stat.MagnitudeSqr2D(targetPosCirc - pos);

        if (magDifSqr > 0.001f) {
            transform.position += v * Stat.TrigOne(targetPosCirc - pos);
        }
        else {
            transform.position = Stat.ToVector3(targetPosCirc, -10);
        }
    }

    void MoveMan() {
        float v = 0.1f; //0.01f * Mathf.Abs(transform.position.z);

        if (Input.GetKey(KeyCode.W)) {
            transform.position += new Vector3(0, v);
        } // up
        if (Input.GetKey(KeyCode.S)) {
            transform.position += new Vector3(0, -v);
        } // down
        if (Input.GetKey(KeyCode.D)) {
            transform.position += new Vector3(v, 0);
        } // right
        if (Input.GetKey(KeyCode.A)) {
            transform.position += new Vector3(-v, 0);
        } // left
    }
    
    void ZoomMan() {
        float v = 0.02f * Mathf.Abs(transform.position.z);

        // zoom in 
        // && transform.position.z < -10
        if (Input.GetKey(KeyCode.E)) {
            transform.position += new Vector3(0, 0, v);
        }
        // zoom out
        if (Input.GetKey(KeyCode.Q)) {
            transform.position += new Vector3(0, 0, -v);
        }
    }

    //

    public static Vector3 CalcPosCirc() {
        int gen = targetKey.Length;
        if (gen == 0) { return new Vector3(0, 0); }

        else {
            float camMagMax = Ancestree.ΔR * gen;
            float genMagMax = Ancestree.GenMagMax(gen);
        
            float n = Stat.KeyToInt(targetKey);
            float genPop = Ancestree.GenPop(gen);
            float genR = Ancestree.GenR(gen);
            float angleDif = 2 * Mathf.PI / genPop;
            float angle = n * angleDif + angleDif / 2;

            Vector3 targetPosLocal = Stat.VectorByAngle(genR, angle);
            Vector3 targetPos = targetPosLocal / (1 + genMagMax / camMagMax);

            return targetPos;
        }
    }

}
