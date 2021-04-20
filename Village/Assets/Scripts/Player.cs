using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    Genome genome;
    Rigidbody2D rb;
    SpriteRenderer sr;

    // // // //

    void Start() {
        genome = new Genome(new Genome(0), new Genome(1));
        
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        gameObject.name = genome.firstName;
        transform.localScale = genome.scale;
        sr = GetComponent<SpriteRenderer>();
        sr.color = genome.color;
    }

    void Update() {
        ExitApplication();
        //sr.color = Stat.AvgColor();
    }

    void FixedUpdate() {
        Move();
    }

    //

    void ExitApplication() {
        if (Time.deltaTime > 0.5 || Stat.People.Count == 0) { UnityEditor.EditorApplication.isPlaying = false; }
    }

    // movement

    public void Move() {
        if (Input.GetKey(KeyCode.W)) {
            transform.position += new Vector3(0, WorldControl.speed*genome.speed/100);
        } // up
        if (Input.GetKey(KeyCode.S)) {
            transform.position += new Vector3(0, -WorldControl.speed*genome.speed/100);
        } // down
        if (Input.GetKey(KeyCode.D)) {
            transform.position += new Vector3(WorldControl.speed*genome.speed/100, 0);
        } // right
        if (Input.GetKey(KeyCode.A)) {
            transform.position += new Vector3(-WorldControl.speed*genome.speed/100, 0);
        } // left
    }

    // collisions

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Human") {
            Display.contactGenome = other.gameObject.GetComponent<NPC>().genome;
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (other.gameObject.tag == "Human") {
            Display.contactGenome = null;
        }
    }

}
