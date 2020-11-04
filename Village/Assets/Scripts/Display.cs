using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Display : MonoBehaviour {

    // world
    public Text maxPopDisplay;
    public Text popDisplay;
    public Text gSpeedDisplay;

    // conacts
    public Text nameDisplay;
    public Text genDisplay;
    public Text speedDisplay;
    public Text staminaDisplay;
    public Button enterTree;

    WorldControl wc;
    public static Genome contactGenome;

    // // // //

    void Start() {

        wc = GameObject.Find("GameWorld").GetComponent<WorldControl>();

    }
    void Update() {
        
        UpdateDisplays();

    } 
    void UpdateDisplays() {

        // world
        maxPopDisplay.text = "Max pop: " + Stat.MaxPop;
        popDisplay.text = "Pop: " + Stat.People.Count;
        gSpeedDisplay.text = WorldControl.speed + "x";

        // contact
        if (contactGenome != null) {
            enterTree.interactable = true;
            nameDisplay.text = contactGenome.firstName + " " + contactGenome.lastName;
            genDisplay.text = "Gen: " + contactGenome.generation + "";
            speedDisplay.text = "speed: " + contactGenome.speed;
            staminaDisplay.text = "stamina: " + contactGenome.stamina + "";
        }
        else {
            enterTree.interactable = false;
            nameDisplay.text = "";
            genDisplay.text = "";
            speedDisplay.text = "";
            staminaDisplay.text = "";
        }

    }
    
    // buttons
    public void LoadTree() {

        Ancestree.SubjectGenome = contactGenome;
        SceneManager.LoadScene("Ancestree");

    }


}
