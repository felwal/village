using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPC : MonoBehaviour {

    public Genome genome { get; set; }

    // times
    float t = 0;
    float idleTime = 0;
    public float lifeTime = 0;
    float lastIntercourse = 0;
    float impregnationDate = 0;
    public int childCount = 0;
    public int lifeLength;

    // properties
    public static Transform npcParent { get; set; }

    // states
    bool resting = true;
    bool charging = false;
    bool growing = false;
    public bool pregnant = false;
    //bool compatable;
    //bool related;
    //bool willing = true;
    public bool receptive {
        get { return childCount < WorldControl.childCap && !growing && !pregnant && lifeTime - lastIntercourse > WorldControl.intercourseCD; }
    }

    // relative
    float angle;
    float speedRel;
    float staminaRel;
    
    // classes
    Rigidbody2D rb;
    Genome mateGenome;
    WorldControl wc;
    Transform targetTrans;
    public GameObject npcPrefab;

    public enum State {
        Sleeping,
        Idling,
        Moving
    }
    public State state = State.Idling;

    // // // //
   
    void Awake() {

       //genome = GetComponent<Genome>();
       //chars.scale = transform.localScale;
    
    }
    void Start() {

        gameObject.name = genome.firstName;
        transform.localScale = genome.scale;
        GetComponent<SpriteRenderer>().color = genome.color;
        lifeTime = 0;
        childCount = 0;

        wc = GameObject.Find("GameWorld").GetComponent<WorldControl>();
        lifeLength = Stat.RandNorm(wc.lifeLengthAvg-50, wc.lifeLengthAvg+50, wc.lifeLengthAvg, 5);
        Stat.People.Add(gameObject);

        rb = GetComponent<Rigidbody2D>();
        //rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        if (npcParent == null) { npcParent = GameObject.Find("Entities/NPC:s").transform; }
        //chars.color = GetComponent<SpriteRenderer>().color;

        angle = Stat.RandAngle();
        staminaRel = genome.stamina;
        speedRel = genome.speed;
        
    }
    void Update() {

        lifeTime += Time.deltaTime * WorldControl.speed;
        
        //receptive = childCount < wc.childCap && !growing && !pregnant && lifeTime - lastIntercourse > wc.intercourseCD;

        // enter tree
        if (wc.genGoal > 1 && genome.generation == wc.genGoal) {
            Ancestree.SubjectGenome = genome;
            SceneManager.LoadScene("Ancestree");
        }

    }
    void FixedUpdate() {

        if (targetTrans != null) { state = State.Moving; }
        else { state = State.Idling; }

        switch(state) {
            case State.Moving: Move(); break;
            case State.Idling: Idle(); break;
        }

        // grow
        if (transform.localScale.x < genome.scale.x) {
            transform.localScale *= 1.001f * WorldControl.speed; // inte z, hur?
            growing = true;
        }
        else {
            growing = false; 
            transform.localScale = genome.scale; 
        }

        // pregnant
        if (pregnant) {
            transform.localScale *= 1.001f;

            if (lifeTime - impregnationDate > wc.pregnancyTime) {
                pregnant = false;
                transform.localScale = genome.scale;
                Birth();
            }
        }

        // die
        if (lifeTime > lifeLength || childCount >= WorldControl.childCap) {
            Stat.People.Remove(gameObject);
            Destroy(gameObject);
        }

        
    }


    // movement
    void Move() {

        Home(targetTrans.position);
        if (!targetTrans.gameObject.GetComponent<NPC>().receptive) {
            targetTrans = null;
        }

        // idle & search mate
        /*else {
            t += Time.fixedDeltaTime * WorldControl.speed;
            Idle();
            if (receptive && t > 2) { 
                t = 0; 
                targetTrans = GetClosestMate(); 
            }
        }*/

    }
    void Idle() {

        idleTime += Time.fixedDeltaTime * WorldControl.speed;

        if (idleTime > Stat.RandFloat(1, 3)) {
            targetTrans = receptive ? GetClosestMate() : null;
            resting = Stat.Coinflip();
            angle = Stat.MagnitudeSqr2D(transform.position) > 625 ? Stat.AngleByVector(-transform.position) : Stat.RandAngle();
            idleTime = 0;
        }

        if (!resting) {
            Vector3 v = WorldControl.speed * speedRel / 2 / 100 * Stat.TrigOneByAngle(angle);
            transform.position += v;
        }

    }
    void Home(Vector3 target) {

        // speed & charge
        if (!charging && staminaRel > 0) {
            staminaRel -= 0.02f;
            speedRel = Accelerate(speedRel, genome.speed); // accelerate
        }
        else {
            if (staminaRel < genome.stamina) {
                staminaRel += 0.02f;
                charging = true;
            }
            else { charging = false; }
            speedRel = Accelerate(speedRel, genome.speed / 4); // retardate
        }

        Vector3 v = WorldControl.speed * speedRel / 100 * Stat.TrigOne(target - transform.position);
        transform.position += v;

    }
    float Accelerate(float v, float v1) {

        // if v != v1
        if (Mathf.Abs(v1 - v) > 0.1f) {
            v += (v1 - v) / 20;
        }
        else { v = v1; }

        return v;
    }

    // reproduction
    Transform GetClosestMate() {

        Transform closestMate = null;
        float magDifSqrShortest = Mathf.Infinity;

        foreach (GameObject person in Stat.People) {
            if (Compatable(person.GetComponent<NPC>())) { 

                float magDifSqr = (person.transform.position - transform.position).sqrMagnitude;
                if (magDifSqr < magDifSqrShortest) {
                    magDifSqrShortest = magDifSqr;
                    closestMate = person.transform;
                }
            }
        }

        return closestMate;
    }  
    void Birth() {

        GameObject child = Instantiate(
            npcPrefab,
            transform.position,
            Quaternion.identity,
            npcParent);

        //Dna(child);
        //child.AddComponent<Genome>();
        child.GetComponent<NPC>().genome = new Genome(genome, mateGenome);

    }
    void Dna(GameObject child) {
        
        Genome childGenome = child.GetComponent<Genome>();

        // temp
        /*childGenome.parentsName[genome.sex] = genome.firstName;
        childGenome.parentsName[mateGenome.sex] = mateGenome.firstName;

        childGenome.parents.Insert(genome.sex, genome);
        childGenome.parents.Insert(mateGenome.sex, mateGenome);
        //genome.children.Add(childGenome);
        //mateGenome.children.Add(childGenome);

        childGenome.generation = genome.generation + 1;
        childGenome.sex = Stat.IntFlip();
        childGenome.firstName = Ling.Name(new English());
        childGenome.lastName = Stat.Coinflip() ? genome.lastName : mateGenome.lastName;
        childGenome.speed = Stat.Coinflip() ? genome.speed : mateGenome.speed;
        childGenome.stamina = Stat.Coinflip() ? genome.stamina : mateGenome.stamina;
        childGenome.scale = Stat.Mutate() ? Stat.ToVector3(Stat.RandExp(1, 2, false), 1) : (Stat.Coinflip() ? genome.scale : mateGenome.scale);
        childGenome.color = Stat.Mutate() ? Stat.RandColor() : Stat.DominantColor(genome.color, mateGenome.color);*/


        // initial values
        //child.GetComponent<NPC>().lifeTime = 0;
        //child.GetComponent<NPC>().childCount = 0;
        //SetAncestorGenomes(childGenome, mateGenome);

    }
    bool Compatable(NPC mate) {

        bool age = Mathf.Abs(lifeTime - mate.lifeTime) < 50;
        bool gen = genome.generation == mate.genome.generation;
        bool sex = genome.sex != mate.genome.sex;
        bool receptive = this.receptive && mate.receptive;

        return gen && sex && receptive;
    }

    void SetAncestorGenomes(Genome childGenome, Genome mateGenome) {

        childGenome.ancestors[genome.sex + ""] = genome;
        childGenome.ancestors[mateGenome.sex + ""] = mateGenome;

        foreach (string key in genome.ancestors.Keys) {
            childGenome.ancestors[genome.sex + key] = genome.ancestors[key];
        }
        foreach (string key in mateGenome.ancestors.Keys) {
            childGenome.ancestors[mateGenome.sex + key] = mateGenome.ancestors[key];
        }

    }

    private void OnCollisionEnter2D(Collision2D other) {
        
        if (other.gameObject.tag == "Solid") {
            //angle += Mathf.PI; // bättre sätt att resetta?
        }

        else if (other.gameObject.tag == "Human") {
            //resting = true;

            // arrived - reset target
            if (other.gameObject.transform == targetTrans) {
                targetTrans = null;
            } 

            // intercourse
            if (Compatable(other.gameObject.GetComponent<NPC>())) {
                angle = Stat.AngleByVector(other.transform.position - transform.position) + Mathf.PI;
                lastIntercourse = lifeTime;
                childCount++;
                // inpregnated
                if (Stat.People.Count < Stat.MaxPop && genome.sex == 0) {
                    mateGenome = other.gameObject.GetComponent<NPC>().genome;
                    impregnationDate = lifeTime;
                    pregnant = true;
                }

            }

        }

    }

}

// idle funkar inte?

// koka ihop dna vid intercourse

// sök bara upp kvinnor som inte är relaterade

// home med sphere
// homeSmart

// ett bra system för när och hur de ska gå respektive vila

// grow inte z