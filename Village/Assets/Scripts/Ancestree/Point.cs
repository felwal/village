using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour {

    //int gen = 0;

    public Genome genome { get; set; }
    Ancestree tree;
    
    //public Transform people;
    //public GameObject parentsPrefab;

    // // // //

    void Start() {
        tree = GameObject.Find("GameWorld/People").GetComponent<Ancestree>();
        //genome = GetComponent<Genome>();

        if (!tree.testing) {
            gameObject.name = genome.firstName;
            GetComponent<SpriteRenderer>().color = genome.color;
            //transform.localScale = chars.scale;
        }
    }

}
