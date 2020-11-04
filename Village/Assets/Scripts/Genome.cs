using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Genome {

    // house
    public Language lang;
    public string firstName;
    public string lastName;
    public string[] parentsName = new string[2]; // temp
    public int generation = 1;
 
    public Dictionary<string, Genome> ancestors = new Dictionary<string, Genome>(); // -> parents
    public Dictionary<string, Genome> descendants = new Dictionary<string, Genome>(); // -> children

    public List<Genome> parents = new List<Genome>();
    public List<Genome> children = new List<Genome>();

    // abstract
    public Vector3 scale;
    public Color color;
    
    // primary
    public int sex;
    public float speed;
    public float stamina;
    //int strength;
    //int hitpoints;

    public Genome(Genome P0, Genome P1) {
        
        parents.Add(P0);
        parents.Add(P1);
        Genome G00, G01, G10, G11;

        G00 = P0.parents.Count >= 2 ? P0.parents[0] : P0;
        G01 = P0.parents.Count >= 2 ? P0.parents[1] : P0;
        G10 = P1.parents.Count >= 2 ? P1.parents[0] : P1;
        G11 = P1.parents.Count >= 2 ? P1.parents[1] : P1;

        //parent1.children.Add(childGenome);
        //parent2.children.Add(childGenome);

        generation = P0.generation + 1;
        sex = Stat.IntFlip();
        lang = Stat.Coinflip() ? P0.lang : P1.lang;
        firstName = Ling.Name(lang);
        lastName = Stat.Coinflip() ? P0.lastName : P1.lastName;
        speed = Stat.Coinflip() ? P0.speed : P1.speed;
        stamina = 12 - speed;
        scale = Stat.Mutate() ? Stat.ToVector3(Stat.RandExp(1, 1.5f, false), 1) : (Stat.Coinflip() ? P0.scale : P1.scale);

        color = Stat.Mutate() ? Stat.RandColor() : Stat.DominantColor(
            Stat.Coinflip() ? G00.color : G01.color, 
            Stat.Coinflip() ? G10.color : G11.color);

    } 
    public Genome(int sex = -1) {

        generation = 1;
        this.sex = sex != 0 || sex != 1 ? Stat.IntFlip() : sex;
        lang = Stat.RandLang();
        firstName = Ling.Name(lang);
        lastName = Ling.Name(lang);
        speed = Stat.RandInt(1, 11);
        stamina = 12 - speed;
        color = Stat.RandColor();
        scale = Stat.ToVector3(Stat.RandExp(1, 1.5f, false), 1);

    }

    void Awake() {
        //ancestors[""] = this;
    }

    public Genome GetAncestor(string key) {
        return key.Length == 0 ? this : parents[int.Parse(key.Substring(0, 1))].GetAncestor(key.Substring(1));
    }

}
