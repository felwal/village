using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ancestree : MonoBehaviour {

    // properties
    public static Genome SubjectGenome { get; set; }
    public static Dictionary<string, GameObject> Bodies { get; set; } = new Dictionary<string, GameObject>();

    // frac
    public static Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>();

    // settings
    public Mode mode = Mode.AncCirc; // 1 des, -1 anc, 0 desanc, 2 global
    public bool testing;

    public enum Mode {
        AncCirc,
        AncFrac,
        DesAnc
    }

    // consts circ
    public static int genMax = 12; // testvalue 10
    const int genMin = 3; // 3 (4 funkar ej)
    const float ΔL = 2 * Mathf.PI; // ΔR * PI (?)
    public static float ΔR;

    // consts frac
    public float l0 = Mathf.Sqrt(8) + Mathf.Sqrt(2); // 4
    public const float size0 = 4;

    Transform[] genParents;
    public GameObject bodyPrefab;
    public GameObject emptyPrefab;

    // // // //

    void Start() {
        ΔR = GenR(genMin-1) / 2; // = 4
        if (!testing) { genMax = SubjectGenome.generation; }

        genParents = new Transform[genMax];

        switch (mode) {
            case Mode.DesAnc: SpawnDesAnc(); break;
            case Mode.AncCirc: SpawnAncestorsCirc(); break;
            case Mode.AncFrac: SpawnAncestorsFrac(); break;
            default: break;
        }
    }
    
    void FixedUpdate() {
        switch (mode) {
            case Mode.AncCirc: MoveAncestorsCirc(); break;
        }
    }

    // anc circle -1

    void SpawnAncestorsCirc() {
        for (int gen = 0; gen < genMax; gen++){

            genParents[gen] = InstantiateEmpty(transform, "gen " + gen).transform;
            float genPop = GenPop(gen);

            // angle
            float genR = GenR(gen);
            if (gen == 0) { genR = 0; }
            float angleDif = 2 * Mathf.PI / genPop;
            
            for (int n = 0; n < genPop; n++){
                float angle = n * angleDif + angleDif / 2;

                // gameobject
                string key = GetKey(gen, n);
                Bodies[key] = Instantiate(
                    bodyPrefab,
                    Stat.VectorByAngle(genR, angle),
                    Quaternion.identity,
                    genParents[gen]);
                Bodies[key].name = key;

                // infuse chars
                if (!testing) {
                    Bodies[key].GetComponent<Point>().genome = SubjectGenome.GetAncestor(key);
                }
            }
        }
    }
    
    void MoveAncestorsCirc() {
        // sqr?
        float camMag = TreeCam.Mag;
        for(int gen = genMin; gen < genMax; gen++) {

            float genR = GenR(gen);
            float genP = ΔR * gen;
            float genMagMax = GenMagMax(gen);
            float genAmp = genMagMax / 2;

            // magnitude
            float genMag;
            if (camMag < genP / 2) { genMag = genAmp * Mathf.Sin((camMag * 2 * Mathf.PI / genP) - Mathf.PI / 2) + genAmp; }
            else { genMag = genMagMax; }
            
            // angle
            float camAngle = 0;
            float genAngle = 0;
            if (camMag != 0) {
                camAngle = Mathf.Acos(TreeCam.trans.position.x / camMag);
                genAngle = camAngle + Mathf.PI;
            }
            if (TreeCam.trans.position.y < 0) { genAngle = -genAngle; }
            
            genParents[gen].position = Stat.VectorByAngle(genMag, genAngle);
            
        }
    }
    
    // anc fractal -2

    void SpawnAncestorsFrac() {
        // calc
        Vector2 childPos = new Vector2(0,0);
        positions.Add("", childPos);
        CalcParentsFrac("", childPos, l0, -1);
        
        // instantiate
        for (int gen = 0; gen < genMax; gen++) {
            genParents[gen] = InstantiateEmpty(transform, "gen " + gen).transform;

            for (int n = 0; n < GenPop(gen); n++) {

                string key = GetKey(gen, n);
                Vector2 pos = positions[key];
                float size = size0 / GenPop(gen);

                GameObject body = bodyPrefab;
                body.transform.localScale = new Vector3(size, size, 1);
                Bodies[key] = Instantiate(
                    body,
                    pos,
                    Quaternion.Euler(0,0,45),
                    genParents[gen]);
                Bodies[key].name = key;

                // infuse chars
                if (!testing) {
                    Bodies[key].GetComponent<Point>().genome = SubjectGenome.GetAncestor(key);
                }

            }
        }

        bodyPrefab.transform.localScale = new Vector3(1,1,1);
    }
    
    void CalcParentsFrac(string childKey, Vector2 childPos, float l, int dir) {
        if (childKey.Length < genMax) {

            string motherKey = childKey + "0";
            string fatherKey = childKey + "1";
            //int motherN = Stat.KeyToInt(motherKey);
            //int fatherN = Stat.KeyToInt(fatherKey);
            int gen = motherKey.Length;

            Vector2 dis;
            if (gen % 2 == 0) { dis = new Vector2(1,0); } // even -> change x
            else { dir *= -1; dis = new Vector2(0,1); } // odd -> change y & dir 
            dis *= dir * l;

            Vector2 motherPos = childPos + dis;
            Vector2 fatherPos = childPos - dis;
            positions.Add(motherKey, motherPos);
            positions.Add(fatherKey, fatherPos);

            CalcParentsFrac(motherKey, motherPos, l/2, dir);
            CalcParentsFrac(fatherKey, fatherPos, l/2, -dir);
        }
    }

    // circle fractal 0

    void SpawnDesAnc() {
        Dictionary<string, float> angles = new Dictionary<string, float>();

        const int ancMax = 12;
        const int desMax = 0;
        Transform[] ancParents = new Transform[ancMax];
        Transform[] desParent = new Transform[desMax];

        Vector2 origin = new Vector2(0,0);
        positions.Add("", origin);
        angles.Add("", (float)Math.PI*3/2);
        
        float r0 = 10f;

        Transform originParent = InstantiateEmpty(transform, "gen " + 0).transform;
        GameObject originBody = bodyPrefab;
        originBody.transform.localScale = new Vector3(size0, size0, 1);
        Bodies[""] = Instantiate(
            originBody,
            origin,
            Quaternion.identity,
            originParent);
        Bodies[""].name = "";

        // anc
        for (int gen = 1; gen <= ancMax; gen++) {

            ancParents[gen-1] = InstantiateEmpty(transform, "gen " + (-gen)).transform;
            float genPop = GenPop(gen);
            float angleDif = Mathf.PI / 3;
            float r = r0 * (float)Math.Pow(angleDif/2, gen);
            float size = size0 / genPop;

            for (int n = 0; n < genPop; n++){

                string key = GetKey(gen, n);
                string childKey = GetChildKey(key);
                //float angle = Mathf.PI*3/2 + gen*(angleDif*(n%2+1)-Mathf.PI/2);
                float angle = angles[childKey] - Mathf.PI/2 + angleDif*(n%2+1);
                angles.Add(key, angle);

                Vector2 pos = positions[childKey] + Stat.ToVector2(Stat.VectorByAngle(r, angle));
                positions.Add(key, pos);

                // gameobject
                GameObject body = bodyPrefab;
                body.transform.localScale = new Vector3(size, size, 1);
                Bodies[key] = Instantiate(
                    body,
                    pos,
                    Quaternion.identity,
                    ancParents[gen-1]);
                Bodies[key].name = key;

                // infuse chars
                if (!testing) {
                    Bodies[key].GetComponent<Point>().genome = SubjectGenome.GetAncestor(key);
                }

            }

        }

        // des
        for (int gen = 1; gen <= desMax; gen++) {
            
            desParent[gen] = InstantiateEmpty(transform, "gen " + (gen+1)).transform;


        }
    }

    // tools

    public static int GenPop(int gen) {
        return (int)Mathf.Pow(2, gen);
    }
    
    public static float GenR(int gen) {
        return (GenPop(gen) * ΔL) / (2 * Mathf.PI); // ur omkrets
    }
    
    public static float GenMagMax(int gen) {
        return GenR(gen) - ΔR * gen; // = genR - genP (något samband?)
    }

    // other

    static string GetKey(int gen, int n) {
        string key = Convert.ToString(n, 2);
        while (key.Length < gen) { key = "0" + key; }
        if (gen == 0) { key = ""; }
        return key;
    }
    
    static string GetChildKey(string parentKey) {
        return parentKey.Substring(0, parentKey.Length-1);
    }
    
    static void CopyChars(Genome original, Genome copy) {
        // constructor / struct?

        copy.color = original.color;
        copy.firstName = original.firstName;
        copy.lastName = original.lastName;
        copy.sex = original.sex;
        copy.speed = original.speed;
        copy.stamina = original.stamina;
        copy.scale = original.scale;
        copy.generation = original.generation;
        copy.parentsName = original.parentsName;
    }
    
    GameObject InstantiateEmpty(Transform parent, string name) {
        GameObject obj = Instantiate(
            emptyPrefab,
            new Vector3(0, 0, 0),
            Quaternion.identity,
            parent);
        obj.name = name;
        return obj;
    }

}
