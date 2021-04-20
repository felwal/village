using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TreeDisplay : MonoBehaviour {

    // texts
    public Text nameDisplay;
    public Text genDisplay;
    public Text speedDisplay;
    public Text staminaDisplay;
    public Text keyDisplay;
    public Text relDisplay;
    public Text relShortDisplay;

    Ancestree tree;
    TreeCam cam;

    // // // //

    void Start() {
        tree = GameObject.Find("GameWorld/People").GetComponent<Ancestree>();
        //cam = GameObject.Find("GameWorld/Main Camera").GetComponent<TreeCam>();
        UpdateDisplays();
    }
    
    void UpdateDisplays() {
        keyDisplay.text = TreeCam.targetKey + "_";
        relDisplay.text = Stat.KeyToRel(TreeCam.targetKey);
        relShortDisplay.text = Stat.KeyToRelShort(TreeCam.targetKey);

        if (!tree.testing) {
            nameDisplay.text = Ancestree.SubjectGenome.GetAncestor(TreeCam.targetKey).firstName + " " + Ancestree.SubjectGenome.GetAncestor(TreeCam.targetKey).lastName;
            genDisplay.text = "Gen: " + Ancestree.SubjectGenome.GetAncestor(TreeCam.targetKey).generation;
            speedDisplay.text = "speed: " + Ancestree.SubjectGenome.GetAncestor(TreeCam.targetKey).speed;
            staminaDisplay.text = "stamina: " + Ancestree.SubjectGenome.GetAncestor(TreeCam.targetKey).stamina;
        }
    }

    // buttons
    
    public void SetKey(string target) {
        // key
        if (target == "mother" && TreeCam.targetKey.Length < Ancestree.genMax-1) {
            TreeCam.targetKey += "0";
        }
        else if (target == "father" && TreeCam.targetKey.Length < Ancestree.genMax-1) {
            TreeCam.targetKey += "1";
        }
        else if (target == "child" && TreeCam.targetKey.Length > 0) {
            TreeCam.targetKey = TreeCam.targetKey.Substring(0, TreeCam.targetKey.Length - 1);
        }     

        // target
        if (tree.mode == Ancestree.Mode.AncCirc) {
            TreeCam.targetPosCirc = TreeCam.CalcPosCirc();
        }
        else if (tree.mode == Ancestree.Mode.AncFrac) {
            //TreeCam.targetKeyQueue.Add(TreeCam.targetKey); // sätt som alla mellan cam och target?
            UpdateTargetKey();
        }

        UpdateDisplays();
    }

    public void LoadOverworld() {
        SceneManager.LoadScene("Overworld");
    }

    //
    
    void UpdateTargetKey() {
        // hoppar över en om går bak 2 steg på vägen
        // går inte bak en om går bak 1 steg på vägen

        // clear queue
        if (TreeCam.targetKeyQueue.Count > 1) {
            TreeCam.targetKeyQueue.RemoveRange(1, TreeCam.targetKeyQueue.Count-1);
        }

        // get lastCommonIndex
        int lci = -1; 
        for (int i = 0; i < Smallest(TreeCam.targetKey.Length, TreeCam.lastTargetKey.Length); i++) {
            if(TreeCam.lastTargetKey[i] == TreeCam.targetKey[i]) {
                lci = i;
            }
            else { break; }
        }

        // move up
        for (int i = TreeCam.lastTargetKey.Length-2; i >= lci; i--) {
            TreeCam.targetKeyQueue.Add(TreeCam.lastTargetKey.Substring(0,i+1));
        }

        // move down
        for (int i = lci+1; i < TreeCam.targetKey.Length; i++) {
            TreeCam.targetKeyQueue.Add(TreeCam.targetKey.Substring(0,i+1));
        }
    }
    
    int Smallest(int a, int b) {
        if (a < b) { return a; }
        else if (b < a) { return b; }
        else { return a; }
    }

}
