using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {

    public Genome genome { get; set; }

    // times
    float t = 0;
    public float lifeTime = 0;
    public int saplingTime;
    float seedDate = 0;
    public int seedCount = 0;
    private int seedLimit = 2;
    public int lifeLength;

    // states
    bool growing = false;
    public bool fertile {
        get { return lifeTime > saplingTime && seedCount < seedLimit && Stat.Trees.Count < Stat.MaxPop && !growing && lifeTime - seedDate > WorldControl.intercourseCD; }
    }

    // classes
    Rigidbody2D rb;
    Genome mateGenome;
    WorldControl wc;
    Transform targetTrans;
    public GameObject treePrefab;
    public static Transform npcParent { get; set; }

    // // // //

    void Start() {
        gameObject.name = genome.firstName;
        transform.localScale = genome.scale;
        GetComponent<SpriteRenderer>().color = genome.color;
        lifeTime = 0;
        seedCount = 0;
        saplingTime = Stat.RandInt(8,16);

        wc = GameObject.Find("GameWorld").GetComponent<WorldControl>();
        lifeLength = Stat.RandNorm(wc.lifeLengthAvg-50, wc.lifeLengthAvg+50, wc.lifeLengthAvg, 5);
        Stat.Trees.Add(gameObject);

        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (npcParent == null) { npcParent = GameObject.Find("Entities/NPC:s").transform; }
    }

    void Update() {
        lifeTime += Time.deltaTime * WorldControl.speed;
        
        if (fertile) {
            seedDate = lifeTime;
            Seed();
        }
    }

    void FixedUpdate() {
        // grow
        if (transform.localScale.x < genome.scale.x) {
            transform.localScale *= 1.0001f * WorldControl.speed; // inte z, hur?
            growing = true;
        }
        else {
            growing = false; 
            transform.localScale = genome.scale; 
        }

        // die
        if (lifeTime > lifeLength || seedCount >= seedLimit) {
            Stat.People.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    //

    void Seed() {
        for (int i = 0; i < Stat.RandInt(0,2); i++) {
            GameObject seed = Instantiate(
                treePrefab,
                transform.position + Stat.ToVector3(Stat.RandVector2Circle(1, 8), 0),
                Quaternion.identity,
                npcParent);

            seed.GetComponent<Tree>().genome = new Genome(genome, genome);
            seedCount++;
            //seed.transform.localScale = new Vector2(0,0);
        }
    }

}
