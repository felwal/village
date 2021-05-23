using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Stat {

    // properties
    public static int MaxPop { get; } = 100;
    public static List<GameObject> People { get; set; } = new List<GameObject>();
    public static List<GameObject> Trees { get; set; } = new List<GameObject>();

    // // // //

    // randoms

    public static bool Coinflip() {
        return RandInt(0, 1) == 1;
    }

    public static int IntFlip() {
        return RandInt(0, 1);
    }

    public static bool Chance(float chance) {
        return Rand() < chance;
    }

    public static float Rand() {
        return UnityEngine.Random.Range(0f, 1f);
    }

    public static float RandFloat(float min, float max) {
        return UnityEngine.Random.Range(min, max);
    }

    public static int RandInt(int min, int max) {
        return UnityEngine.Random.Range(min, max + 1);
    }

    public static float RandAngle() {
        return RandFloat(0, 2 * Mathf.PI);
    }

    public static int RandNorm(int a, int b, int mean, float deviation) {
        // normalfördelning
        // min, max, medelvärde, standardavvikelse

        float min = 0;
        float rand = Rand();
        float[] p = new float[b - a + 1]; // probability

        for (int x = a; x <= b; x++) {

            // get probability med täthetsfunktion
            p[x - a] = ((1 / (deviation * Mathf.Sqrt(2 * Mathf.PI))) * Mathf.Exp(-(Mathf.Pow(x - mean, 2) / (2 * Mathf.Pow(deviation, 2)))));

            // choose int with rand
            if (rand >= min && rand < min + p[x - a]) { return x; }
            min += p[x - a];
        }

        return mean; // backup
    }

    public static int RandExp(int amount) {
        // exponentialfördelning

        float λ = 3.9f / amount; // högsta värde (intensitet), anpassat
        float min = 0;
        float rand = Rand();
        float[] p = new float[amount]; // probability

        for (int x = 0; x < amount; x++) {

            // get probability med täthetsfunktion
            p[x] = λ * Mathf.Exp(-λ * x);

            // choose int with rand
            if (rand >= min && rand < min + p[x]) {
                return x;
            }
            min += p[x];
        }

        return 0; // backup
    }

    public static float RandExp(float min, float max, bool truncate) {
        float λ = 3.9f / (max - min);
        float p = RandFloat(0, λ);
        float x = -Mathf.Log(p/λ)/λ + min; // täthetsfunktionen

        return truncate && x > max ? max : x;
    }

    public static string RandName(int a, int b, int mean, int deviation, int sex) {
        string pChar =       "etaoinshrdlcumwfgypåäbvkjxöqz";
        string pCharFirstM = "jacmledbkrsgtnåähwiopzfövyxqu";
        string pCharFirstF = "ameslkcjrhbnigdpvtzäofwyqxuäö";
        //string pCharLastM = "";
        //string pCharLastF = "";

        string name = "";
        int length = RandNorm(a, b, mean, deviation);

        if (sex == 1) {
            for (int c = 0; c < length; c++) {
                if (c == 0) {
                    name += pCharFirstM[RandExp(pChar.Length)].ToString().ToUpper();
                }
                else if (c == length - 1) {
                    if (Rand() < 0.33f) {
                        name += 'n';
                    }
                    else {
                        name += pChar[RandExp(pChar.Length)];
                    }
                }
                else {
                    name += pChar[RandExp(pChar.Length)];
                }
            }
        }
        else {
            for (int c = 0; c < length; c++) {
                if (c == 0) {
                    name += pCharFirstF[RandExp(pChar.Length)].ToString().ToUpper();
                }
                else if (c == length - 1) {
                    if (Rand() < 0.6f) {
                        name += 'a';
                    }
                    else {
                        name += pChar[RandExp(pChar.Length)];
                    }
                }
                else {
                    name += pChar[RandExp(pChar.Length)];
                }
            }
        }

        return name;
    }

    public static Language RandLang() {
        return Ling.langs[RandInt(0, Ling.langs.Length-1)];
    }

    public static bool Mutate() {
        return Chance(WorldControl.mutatationRisk);
    }

    public static Color RandColor() {
        return new Color(Rand(), Rand(), Rand(), 1);
    }

    public static Vector2 RandVector2(int xMin, int xMax, int yMin, int yMax) {
        return new Vector2(RandFloat(xMin, xMax), RandFloat(yMin, yMax));
    }

    public static Vector2 RandVector2(int xyMin, int xyMax) {
        float xy = RandFloat(xyMin, xyMax);
        return new Vector2(xy, xy);
    }

    public static Vector2 RandVector2Circle(int rMin, int rMax) {
        return VectorByAngle(RandFloat(rMin, rMax), RandAngle());
    }

    public static GameObject RandNpc(GameObject npcPrefab, Transform parent) {
        GameObject npc = GameObject.Instantiate(
            npcPrefab,
            ToVector3(RandVector2(-25, 25, -25, 25), -5),
            Quaternion.identity,
            parent);

        npc.GetComponent<NPC>().genome = new Genome(new Genome(0), new Genome(1));//npc.GetComponent<Genome>();
        return npc;
    }

    public static GameObject RandTree(GameObject treePrefab, Transform parent) {
        GameObject tree = GameObject.Instantiate(
            treePrefab,
            ToVector3(RandVector2(-25, 25, -25, 25), -5),
            Quaternion.identity,
            parent);

        tree.GetComponent<Tree>().genome = new Genome(new Genome(0), new Genome(1));//npc.GetComponent<Genome>();
        return tree;
    }

    // string

    public static string RandCharFromString(string str) {
        return str[RandInt(0, str.Length-1)] + "";
    }

    public static string RandStringFromList(List<string> str) {
        if (str.Count == 0) { return ""; }
        return str[RandInt(0, str.Count - 1)];
    }

    public static string CapitalizeFirstLetter(string s) {
        if (string.IsNullOrEmpty(s)) { return string.Empty; }
        char[] a = s.ToCharArray();
        a[0] = char.ToUpper(a[0]);

        return new string(a);
    }

    public static Boolean StringArrayContains(string[] str, string contains) {
        for (int i = 0; i < str.Length; i++) {
            if (str[i].Equals(contains)) { return true; }
        }

        return false;
    }

    // array

    public static string[] FlattenArray(string[,,] multi3D) {
        List<string> flattened = new List<string>();
        for (int i = 0; i < multi3D.GetLength(0); i++) {
            for (int j = 0; j < multi3D.GetLength(1); j++) {
                for (int k = 0; k < multi3D.GetLength(2); k++) {
                    flattened.Add(multi3D[i,j,k]);
                }
            }
        }

        return flattened.ToArray();
    }

    public static string[] MergeArrays(string[] a1, string[] a2) {
        /*string[] merged = new string[a1.Length + a2.Length];
        Array.Copy(a1, merged, a1.Length);
        Array.Copy(a2, 0, merged, a1.Length, a2.Length);

        return merged;*/

        List<string> list = new List<string>(a1);
        for (int i = 0; i < a2.Length; i++) {
            if (!list.Contains(a2[i])) { list.Add(a2[i]); }
        }

        return list.ToArray();
    }

    public static string[] MergeArrays(string[] a1, string[] a2, string[] a3 ) {
        return MergeArrays(MergeArrays(a1, a2), a3);
    }

    public static string[] MergeArrays(string[] a1, string[] a2, string[] a3, string[] a4 ) {
        return MergeArrays(MergeArrays(MergeArrays(a1, a2), a3), a4);
    }

    public static string[] SubtractArrays(string[] original, string[] remove) {
        List<string> list = new List<string>();
        for (int o = 0; o < original.Length; o++) {
            for (int r = 0; r < remove.Length; r++) {
                if (original[o] == remove[r]) { continue; }
                list.Add(original[o]);
            }
        }

        return list.ToArray();
    }

    public static string[] SubtractArraysKeep(string[] original, string[] keep) {
        List<string> list = new List<string>();
        for (int o = 0; o < original.Length; o++) {
            for (int k = 0; k < keep.Length; k++) {
                if (original[o] != keep[k]) { continue; }
                list.Add(original[o]);
            }
        }

        return list.ToArray();
    }

    // list

    public static void RemoveRange(List<string> original, List<string> remove) {
        foreach (string s in remove) {
            if (original.Contains(s)) { original.Remove(s); }
        }
    }

    public static void KeepRange(List<string> original, List<string> keep) {
        List<string> toRemove = new List<string>();
        foreach (string s in original) {
            if (!keep.Contains(s)) { toRemove.Add(s); }
        }
        RemoveRange(original, toRemove);
    }

    public static List<string> RemoveRangeNew(List<string> original, List<string> remove) {
        List<string> newList = new List<string>(original);
        foreach (string s in remove) {
            if (newList.Contains(s)) { newList.Remove(s); }
        }
        return newList;
    }

    public static List<string> KeepRangeNew(List<string> original, List<string> keep) {
        List<string> newList = new List<string>(original);
        foreach (string s in newList) {
            if (!keep.Contains(s)) { newList.Remove(s); }
        }
        return newList;
    }

    // maths

    public static float Sqr(float baseValue){
        //return Mathf.Pow(baseValue, 2);
        return baseValue * baseValue;
    }

    public static float Magnitude2D(Vector3 vector) {
        return Mathf.Sqrt(Sqr(vector.x) + Sqr(vector.y));
    }

    public static float MagnitudeSqr2D(Vector3 vector) {
        return Stat.Sqr(vector.x) + Stat.Sqr(vector.y);
    }

    public static bool IsEven(int i) {
        if (i % 2 == 0) { return true; }
        return false;
    }

    public static float AngleByVector(Vector3 vector) {
        float angle = Mathf.Acos(vector.x / Magnitude2D(vector));
        if (vector.y < 0) { angle = -angle; }

        return angle;
    }

    public static Vector3 VectorByAngle(float r, float angle) {
        return r * new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
    }

    public static Vector3 TrigOne(Vector3 vectorDif) {
        // target - trans

        float angle = AngleByVector(vectorDif); // arg(v)
        float vx = Mathf.Pow(Mathf.Cos(angle), 2); // trigonometriska ettan
        float vy = Mathf.Pow(Mathf.Sin(angle), 2);

        // check quadrant
        if (Mathf.Cos(angle) < 0) { vx = -vx; }
        if (Mathf.Sin(angle) < 0) { vy = -vy; }

        return new Vector3(vx, vy);
    }

    public static Vector3 TrigOneByAngle(float angle) {
        float vx = Mathf.Pow(Mathf.Cos(angle), 2); // trigonometriska ettan
        float vy = Mathf.Pow(Mathf.Sin(angle), 2);

        // check quadrant
        if (Mathf.Cos(angle) < 0) { vx = -vx; }
        if (Mathf.Sin(angle) < 0) { vy = -vy; }

        return new Vector3(vx, vy);
    }

    public static bool IntToBool(int i) {
        if (i == 0) { return false; }
        return true;
    }

    public static int BoolToInt(bool b) {
        if (b == true) { return 1; }
        return 0;
    }

    // vectors

    public static Vector3 ToVector3(Vector2 vector, float z) {
        return new Vector3(vector.x, vector.y, z);
    }

    public static Vector3 ToVector3(float xy, float z) {
        return new Vector3(xy, xy, z);
    }

    public static Vector2 ToVector2(Vector3 vector) {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector2 ToVector2(float xy) {
        return new Vector2(xy, xy);
    }

    // colors

    public static Color AvgColor() {
        Color totalColor = new Color();
        foreach (GameObject person in People) {
            totalColor += person.GetComponent<NPC>().genome.color;
        }

        return totalColor / People.Count;
    }

    public static Color DominantColor(Color c1, Color c2) {
        Color.RGBToHSV(c1, out float h1, out float s1, out float v1);
        Color.RGBToHSV(c2, out float h2, out float s2, out float v2);

        float dif = h2 - h1;
        dif = Mathf.Abs(dif) > 0.5 ? -dif : dif;
        return dif > 0 ? c2 : c1;
    }

    // key

    public static string KeyToRel(string key) {
        string keyConverted = "";
        for (int c = 0; c < key.Length; c++) {
            switch (key[c]) {
                case '0':
                keyConverted += "mor";
                break;
                case '1':
                keyConverted += "far";
                break;
            }
        }
        return keyConverted;
    }

    public static string KeyToRelShort(string key) {
        string keyConverted = "";
        if (key.Length > 2) {
            for (int c = 0; c < key.Length-2; c++) { keyConverted += "G"; }
            keyConverted += "-" + KeyToRel(key.Substring(key.Length-2));
        }
        else {
            keyConverted += KeyToRel(key);
        }
        return keyConverted;
    }

    public static int KeyToInt(string key) {
        if (key == "") { return 0; }
        int keyConverted = Convert.ToInt32(key, 2);
        return keyConverted;
    }

    public static int KeySex(string key) {
        if (key.Length == 0) { return -1; }
        return key[key.Length-1];
    }

}
