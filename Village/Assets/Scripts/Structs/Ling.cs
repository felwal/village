using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ling {

    public static Language shfe = new Shfe();
    public static Language zhroom = new Zhroom();
    public static Language english = new English();
    public static Language swedish = new Swedish();

    public static Language[] langs = { shfe, zhroom, english, swedish };

    /*public static Language GetLang(Genome.Race race) {
        switch(race) {
            case Genome.Race.Zhroom: return zhroom;
            case Genome.Race.Three: return shfe;
            case Genome.Race.Human: return Stat.Coinflip() ? english : swedish;
            default: return english;
        }
    }*/

    public static readonly string[,,] consonants = {
            // bilabial    labiodental  dental       alveolar     postalveolar retrofelx    palatal      velar        uvular       pharyngeal   glottal      labial-velar
            { {"p","b"},   {"p̪","b̪"},   {" "," "},   {"t","d"},   {" "," "},   {"ʈ","ɖ"},   {"c","ɟ"},   {"k","g"},   {"q","ɢ"},   {"ʡ"," "},   {"ʔ"," "},   {" "," "} }, // plosive
            { {"m̥","m"},   {" ","ɱ"},   {" "," "},   {"n̥","n"},   {" "," "},   {"ɳ̊","ɳ"},   {"ɲ̊","ɲ"},   {"ŋ̊","ŋ"},   {" ","ɴ"},   {" "," "},   {" "," "},   {" "," "} }, // nasal
            { {"ʙ̥","ʙ"},   {" "," "},   {" "," "},   {"r̥","r"},   {" "," "},   {"ɽ̊r̥","ɽr"}, {" "," "},   {" "," "},   {"ʀ̥","ʀ"},   {"ʜ","ʢ"},   {" "," "},   {" "," "} }, // trill
            { {" ","ⱱ̟"},   {" ","ⱱ"},   {" "," "},   {"ɾ̥","ɾ"},   {" "," "},   {"ɽ̊","ɽ"},   {" "," "},   {" "," "},   {" ","ɢ̆"},   {" ","ʡ̆"},   {" "," "},   {" "," "} }, // tap or flap
            { {"ɸ","β"},   {"f","v"},   {"θ","ð"},   {"s","z"},   {"ʃ","ʒ"},   {"ʂ","ʐ"},   {"ɕ","ʑ"},   {"x","ɣ"},   {"χ","ʁ"},   {"ħ","ʕ"},   {"h","ɦ"},   {" "," "} }, // fricative
            { {" "," "},   {" "," "},   {" "," "},   {"ɬ","ɮ"},   {" "," "},   {"ɭ̊˔","ɭ˔"}, {"ʎ̝̊","ʎ̝"},   {"ʟ̝̊","ʟ̝"},   {" "," "},   {" "," "},   {" "," "},   {" "," "} }, // lateral fricative
            { {" "," "},   {"ʋ̥","ʋ"},   {" "," "},   {"ɹ̥","ɹ"},   {" "," "},   {"ɻ̊","ɻ"},   {"j̊","j"},   {"ɰ̊","ɰ"},   {" "," "},   {" "," "},   {" ","ʔ̞"},   {"ʍ","w"} }, // approximant
            { {" "," "},   {" "," "},   {" "," "},   {"l̥","l"},   {" "," "},   {"ɭ̊","ɭ"},   {"ʎ̥","ʎ"},   {"ʟ̥","ʟ"},   {" ","ʟ̠"},   {" "," "},   {" "," "},   {" "," "} }  // lateral approximant
    };
    public static readonly string[] consonantsUnlabled = Stat.FlattenArray(consonants);

    public static readonly List<string> rhotics = new List<string> {
        "r","ɾ","ɹ","ɻ","ʀ","ʁ","ɽ","ɺ"
    };
    public static readonly List<string> laterals = new List<string> {
        "l̪","l","l̥","ɭ","ʎ","ʎ̥","ʟ","ʟ̠","ɬ̪","ɮ̪","ɬ","ɮ","ɭ̊","ꞎ","dɮ","ɺ"
    };
    public static readonly List<string> affricates = new List<string> {
        "dʒ","tʃ"
    };
    public static readonly List<string> obstruents = GetObstruents();
    public static readonly List<string> sonorants = GetSonorants();
    public static readonly List<string> liquids = GetLiquids();
    public static readonly List<string> glides = GetGlides();
    
    // // // //

    // categories

    private static List<string> GetObstruents() {
        List<string> obstruents = new List<string>();

        obstruents.AddRange(GetConsonantsByManner(Manner.Plosive));
        obstruents.AddRange(GetConsonantsByManner(Manner.Fricative));
        obstruents.AddRange(affricates);

        return obstruents;
    }
    
    private static List<string> GetSonorants() {
        List<string> sonorants = new List<string>();
        
        sonorants.AddRange(GetConsonantsByManner(Manner.Approximant));
        sonorants.AddRange(GetConsonantsByManner(Manner.Nasal));
        sonorants.AddRange(GetConsonantsByManner(Manner.TapFlap));
        sonorants.AddRange(GetConsonantsByManner(Manner.Trill));

        return sonorants;
    }
    
    private static List<string> GetLiquids() {
        List<string> liquids = new List<string>();

        liquids.AddRange(GetConsonantsByManner(Manner.ApproximantLateral, true));
        liquids.AddRange(rhotics);

        return liquids;
    }
    
    private static List<string> GetGlides() {
        // säker att laterala ska inkluderas?

        List<string> glides = new List<string>();

        glides.AddRange(GetConsonantsByManner(Manner.Approximant));
        glides.AddRange(GetConsonantsByManner(Manner.ApproximantLateral));
        //glides.Add("w");

        return glides;
    }

    public static List<string> GetObstruents(bool voiced) {
        List<string> obstruents = new List<string>();

        obstruents.AddRange(GetConsonantsByManner(Manner.Plosive, voiced));
        obstruents.AddRange(GetConsonantsByManner(Manner.Fricative, voiced));
        obstruents.AddRange(affricates); // !

        return obstruents;
    }

    // manner & place

    public static List<string> GetConsonants(Manner manner, Place place) {
        List<string> cons = new List<string>();
        string con;
        if ((con = consonants[(int)manner, (int)place, 0]) != " ") { cons.Add(con); }
        if ((con = consonants[(int)manner, (int)place, 1]) != " ") { cons.Add(con); }

        return cons;
    }
    
    public static List<string> GetConsonants(bool voiced) {
        int voicedInt = Stat.BoolToInt(voiced);
        List<string> cons = new List<string>();
        for (int m = 0; m < consonants.GetLength(0); m++) {
            for (int p = 0; p < consonants.GetLength(1); p++) {

                string con;
                if ((con = consonants[m, p, voicedInt]) != " ") { cons.Add(con); }
            }

        }

        return cons;
    }
    
    public static List<string> GetConsonantsByManner(Manner manner) {
        List<string> cons = new List<string>();
        for (int p = 0; p < consonants.GetLength(1); p++) {

            string con;
            if ((con = consonants[(int)manner, p, 0]) != " ") { cons.Add(con); }
            if ((con = consonants[(int)manner, p, 1]) != " ") { cons.Add(con); }
        }

        return cons;
    }
    
    public static List<string> GetConsonantsByManner(Manner manner, bool voiced) {
        int voicedInt = Stat.BoolToInt(voiced);
        List<string> cons = new List<string>();
        for (int p = 0; p < consonants.GetLength(1); p++) {

            string con;
            if ((con = consonants[(int)manner, p, voicedInt]) != " ") { cons.Add(con); }
        }

        return cons;
    }
    
    public static List<string> GetConsonantsByPlace(Place place) {
        List<string> cons = new List<string>();
        for (int m = 0; m < consonants.GetLength(0); m++) {

            string con;
            if ((con = consonants[m, (int)place, 0]) != " ") { cons.Add(con); }
            if ((con = consonants[m, (int)place, 1]) != " ") { cons.Add(con); }
        }

        return cons;
    }
    
    public static List<string> GetConsonantsByPlace(Place place, bool voiced) {
        int voicedInt = Stat.BoolToInt(voiced);
        List<string> cons = new List<string>();
        for (int m = 0; m < consonants.GetLength(0); m++) {

            string con;
            if ((con = consonants[m, (int)place, voicedInt]) != " ") { cons.Add(con); }
        }

        return cons;
    }
    
    // sonority

    public static List<string> GetConsonants(Soronity soronity) {
        switch(soronity) {
            case Soronity.Click: break;
            case Soronity.PlosiveVoiceless: return GetConsonantsByManner(Manner.Plosive, false);
            case Soronity.PlosiveVoiced: return GetConsonantsByManner(Manner.Plosive, true);
            case Soronity.FricativeVoiceless: return GetConsonantsByManner(Manner.Fricative, false);
            case Soronity.FricativeVoiced: return GetConsonantsByManner(Manner.Fricative, true);
            case Soronity.Nasal: return GetConsonantsByManner(Manner.Nasal);
            case Soronity.Lateral: return laterals;
            case Soronity.TapFlap: return GetConsonantsByManner(Manner.TapFlap);
            case Soronity.VowelHighGlide: return glides;
            case Soronity.VowelMid: break;
            case Soronity.VowelLow: break;
        }
        return new List<string>();
    }
    
    public static List<string> GetConsonantsRangeMax(Soronity max) {
        List<string> range = new List<string>();
        for (int i = 0; i <= (int)max; i++) {
            range.AddRange(GetConsonants((Soronity)i));
        }
        return range;
    }
    
    public static List<string> GetConsonantsRangeMin(Soronity min) {
        List<string> range = new List<string>();
        for (int i = (int)min; i <= (int)Soronity.VowelLow; i++) {
            range.AddRange(GetConsonants((Soronity)i));
        }
        return range;
    }
    
    public static List<string> GetConsonantsRange(Soronity min, Soronity max) {
        List<string> range = new List<string>();
        for (int i = (int)min; i <= (int)max; i++) {
            range.AddRange(GetConsonants((Soronity)i));
        }
        return range;
    }

    //

    public enum Soronity {
        Click,
        PlosiveVoiceless,
        PlosiveVoiced,
        FricativeVoiceless,
        FricativeVoiced,
        Nasal,
        Lateral,
        TapFlap,
        VowelHighGlide,
        VowelMid,
        VowelLow
    }
    public enum Manner {
        Plosive,
        Nasal,
        Trill,
        TapFlap,
        Fricative,
        FricativeLateral,
        Approximant,
        ApproximantLateral
    }
    public enum Place {
        Bilabial,
        Labiodental,
        Dental,
        Alveolar,
        Postalveolar,
        Retroflex,
        Palatal,
        Velar,
        Uvular,
        Pharyngeal,
        Glottal,
        LabialVelar
    }

    public struct Consonant {

        public string character { get; set; }
        public Place place { get; set; }
        public Manner manner { get; set; }
        public bool voiced { get; set; }

        // // // //

        public Consonant(string consonant) {
            character = consonant;
            manner = (Manner)0;
            place = (Place)0;
            voiced = false;

            for (int m = 0; m < consonants.GetLength(0); m++) {
                for (int p = 0; p < consonants.GetLength(1); p++) {
                    for (int v = 0; v < 2; v++) {
                        if (consonants[m,p,v].Equals(consonant)) {
                            manner = (Manner)m;
                            place = (Place)p;
                            voiced = Stat.IntToBool(v);
                            return;
                        }
                    }
                }
            }
        }
        
        public Consonant(Manner manner, Place place, bool voiced) {
            this.manner = manner;
            this.place = place;
            this.voiced = voiced;
            character = consonants[(int)manner, (int)place, Stat.BoolToInt(voiced)];
        }

        //

        public Soronity GetSoronity() {
            if (manner == Manner.Plosive && !voiced) { return Soronity.PlosiveVoiceless; }
            else if (manner == Manner.Plosive && voiced) { return Soronity.PlosiveVoiced; }
            else if (manner == Manner.Fricative && !voiced) { return Soronity.FricativeVoiceless; }
            else if (manner == Manner.Fricative && voiced) { return Soronity.FricativeVoiced; }
            else if (manner == Manner.Nasal) { return Soronity.Nasal; }
            else if (laterals.Contains(character)) { return Soronity.Lateral; }
            else if (manner == Manner.TapFlap) { return Soronity.TapFlap; }
            else if (glides.Contains(character)) { return Soronity.VowelHighGlide; }

            return (Soronity)0;
        }

    }
    
    // // // //

    static int OnsetLength(Language lang) {
        return Stat.RandInt(lang.onsetMin, lang.onsetMax);
    }
    
    static int CodaLength(Language lang) {
        return Stat.RandInt(lang.codaMin, lang.codaMax);
    }
    
    static int SyllableCount(Language lang) {
        return Stat.RandNorm(lang.sylMin, lang.sylMax, lang.sylMean, lang.sylDev);
    }

    //

    static string Syllable(Language lang) {

        // nucleous
        string nucleous = RandPhoneme(lang.AllowedNucleous());

        // onset
        List<string> onset = new List<string>();
        int onsetLength = OnsetLength(lang);
        for (int i = 0; i < onsetLength; i++) {
            onset.Add(RandPhoneme(lang.AllowedOnset(onset, onsetLength)));
        }
        
        // coda
        List<string> coda = new List<string>();
        int codaLength = CodaLength(lang);
        for (int i = 0; i < codaLength; i++) {
            coda.Add(RandPhoneme(lang.AllowedCoda(coda)));
        }
        
        return lang.Romanize(onset) + lang.Romanize(nucleous) + lang.Romanize(coda);
        
    }
    
    public static string Name(Language lang) {
        
        string name = "";
        for (int i = 0; i < SyllableCount(lang); i++) { 
            name += Syllable(lang);
        }
        
        return Stat.CapitalizeFirstLetter(name);
    }

    //

    public static string RandPhoneme(List<string> phonems) {
        return Stat.RandStringFromList(phonems);
    }

}

// languages

public class Language {

    public string[] vowels { get; set; } = {
        "a","ɑ:","ʊ","u:","ɵ","ʉ:","ɔ","o:","e","ə","e:","ɪ","i:","ʏ","y:","æ","æ:","ɛ","ɛ:","œ","œ:","ø:" 
        };
    public string[] consonants { get; set; } = {
        "b","d","f","g","h","j","k","l","m","n","p","r","s","t","v","ŋ","ɧ","ɕ","ʂ","ɖ","ʈ","ɳ","ɭ" 
        };
    
    public static string[] plosives { get; } = {
        "p","t","b","d","c","k","q","g","ʈ","ɖ","ɟ","ɢ","ʔ","ʡ"
    }; // /stop
    public static string[] fricatives { get; } = {
        "s","z","f","v","ʃ","ʒ","ɕ","ʑ","ʂ","ɸ","β","θ","ð","x","ɣ","ɧ","χ","ʜ","ʢ"
    };
    public static string[] affricates { get; } = {
        "dʒ","tʃ"
    };
    public static string[] sonorants { get; } = {
        "m","n","w","j","l","r","ɹ","ŋ","ɲ" 
    };
    public static string[] glides { get; } = {
        "y","w","ʋ" 
    }; // /semivowel
    public static string[] liquids { get; } = {
        "l","r","ɹ","ʎ" 
    }; 
    
    public int onsetMin = 0;
    public int onsetMax = 3;
    public int codaMin = 0;
    public int codaMax = 3;

    public int sylMin = 1;
    public int sylMax = 4;
    public int sylMean = 2;
    public int sylDev = 1;

    // // // //

    public Language(int onsetMin, int onsetMax, int codaMin, int codaMax, int sylMin, int sylMax, int sylMean, int sylDev, string[] vowels, string[] consonants) {
        this.vowels = vowels;
        this.consonants = consonants;
        this.onsetMin = onsetMin;
        this.onsetMax = onsetMax;
        this.codaMin = codaMin;
        this.codaMax = codaMax;
        this.sylMin = sylMin;
        this.sylMax = sylMax;
        this.sylMean = sylMean;
        this.sylDev = sylDev;
    }
    
    public Language() {}

    //

    public virtual string Romanize(List<string> phonems) {   
        string letters = "";
        foreach (string phonem in phonems) {
            letters += Romanize(phonem);
        }
        
        return letters;
    }
    
    public virtual string Romanize(string phonem) {
        return phonem;
    }

    //

    public virtual List<string> AllowedOnset(List<string> preceding, int length) {
        return new List<string>(consonants);
    }
    
    public virtual List<string> AllowedNucleous() {
        return new List<string>(vowels);
    }
    
    public virtual List<string> AllowedCoda(List<string> preceding) {
        return new List<string>(consonants);
    }

    //

    public virtual List<string> Coda() {
        return new List<string>();
    }

} 

public class English : Language {

    public English() {
        vowels = new string[] {
            "æ","ɑ:","e","ə","ɪ","i:","ɔ","ɔ:","ʊ","u:","ʌ","ɜ:","eɪ","aɪ","oʊ","ɔɪ","aʊ","eə","ɪə","ʊə" 
        };
        consonants = new string[] {
            "b","d","f","g","h","j","k","l","m","n","p","ɹ","s","t","v","w","z","ʒ","ʃ","θ","ð","ŋ" // ,"dʒ" ,"tʃ"
        };
        codaMax = 4;
        sylMax = 2;
        sylMin = 1;
    }

    //

    public override string Romanize(List<string> phonemes) {  
        string letters = "";
        foreach (string phoneme in phonemes) {
            letters += Romanize(phoneme);
        }
        
        return letters;
    }
    
    public override string Romanize(string phoneme) {
        switch (phoneme) { 
            case "": break;
            case "æ": return "a";
            case "ɑ:": return "a";
            case "e": return "e";
            case "ə": return "a";
            case "ɪ": return "i";
            case "i:": return "e";
            case "ɔ": return "a";
            case "ɔ:": return "au";
            case "ʊ": return "o";
            case "u:": return "o";
            case "ʌ": return "u";
            case "ɜ:": return "i"; // ir
            case "eɪ": return "ai";
            case "aɪ": return "i";
            case "oʊ": return "o";
            case "ɔɪ": return "oi";
            case "aʊ": return "ou";
            case "eə": return "ai";
            case "ɪə": return "ee";
            case "ʊə": return "au";

            case "b": return "b";
            case "d": return "d";
            case "f": return "f";
            case "g": return "g";
            case "h": return "h";
            case "j": return "y";
            case "k": return "k";
            case "l": return "l";
            case "m": return "m";
            case "n": return "n";
            case "p": return "p";
            case "ɹ": return "r";
            case "s": return "s";
            case "t": return "t";
            case "v": return "v";
            case "w": return "w";
            case "z": return "z";
            
            case "ʒ": return "s";
            case "dʒ": return "g"; // ge
            case "ʃ": return "sh";
            case "tʃ": return "ch";
            case "θ": return "th";
            case "ð": return "th";
            case "ŋ": return "ng";

            default: return phoneme;
        }

        return "";
    }

    //

    public override List<string> AllowedOnset(List<string> preceding, int length) {
        //return OldOnset(preceding);
        int count = preceding.Count;
        List<string> onset = new List<string>(consonants);
        //if (length == 3 && count == 0) return new List<string>() {"s"};

        onset.Remove("ŋ");
        onset.Remove("ʒ");
        if (length > 1) { onset.Remove("h"); }

        if (count < length-1) {
            Stat.KeepRange(onset, Ling.obstruents);
        }
        if (count >= 1) {
            string previous = preceding[count-1];
            onset.Remove(previous);

            Stat.RemoveRange(onset, Ling.GetObstruents(true));

            if (previous != "s") {
                List<string> liquidsGlides = new List<string>(Ling.liquids);
                liquidsGlides.AddRange(Ling.glides);
                Stat.KeepRange(onset, liquidsGlides);

                Ling.Soronity sonority = new Ling.Consonant(previous).GetSoronity();
                Stat.RemoveRange(onset, Ling.GetConsonantsRangeMax(sonority));
            }

            Stat.RemoveRange(onset, Ling.affricates);
        }

        return onset;        
    }
    
    public override List<string> AllowedNucleous() {
        return new List<string>(vowels);
    }
    
    public override List<string> AllowedCoda(List<string> preceding) {
        int count = preceding.Count;
        List<string> coda = new List<string>(consonants);

        coda.Remove("h");
        Stat.RemoveRange(coda, Ling.glides);

        if (count >= 1) {
            string previous = preceding[count-1];
            Ling.Consonant con = new Ling.Consonant(previous);

            List<string> moreSonorous = Ling.GetConsonantsRangeMin(con.GetSoronity());
            //moreSonorous.Remove("s");
            Stat.RemoveRange(coda, moreSonorous);

            coda.Remove(previous);
            Stat.RemoveRange(coda, new List<string> {"r","ŋ","ʒ","ð"});
            if (!con.voiced) {
                Stat.RemoveRange(coda, Ling.GetConsonants(true));
            }
            if (Ling.GetConsonantsByManner(Ling.Manner.Nasal).Contains(previous)) {
                List<string> unhomorganicObs = Stat.RemoveRangeNew(Ling.obstruents, Ling.GetConsonantsByManner(con.manner));
                Stat.RemoveRange(coda, unhomorganicObs);
            }
        }

        for (int i = 0; i < count; i++) {
            if (Ling.obstruents.Contains(preceding[i])) {
                List<string> unsharedVoicingObs = Stat.RemoveRangeNew(Ling.obstruents, Ling.GetConsonants(new Ling.Consonant(preceding[i]).voiced));
                Stat.RemoveRange(coda, unsharedVoicingObs);
                break;
            }
        }

        return coda;
    }

    //

    public List<string> OldOnset(List<string> preceding) {
        if (preceding.Count == 0) {
            return new List<string> { "b","d","f","g","h","j","k","l","m","n","p","ɹ","s","t","v","w","z"/*,"ʒ"*/,"dʒ","ʃ","tʃ","θ","ð"/*,"ŋ"*/ };
        }
        else if (preceding.Count == 1) {
            switch (preceding[0]) {
                case "s": return new List<string> { "m","n","t","w","k","l","p","f" };
                case "θ": return new List<string> { "ɹ","w" };
                case "d": return new List<string> { "ɹ","w" };
                case "t": return new List<string> { "ɹ","w" };
                case "k": return new List<string> { "ɹ","w","l" };
                case "p": return new List<string> { "ɹ","l" };
                case "f": return new List<string> { "ɹ","l" };
                case "b": return new List<string> { "ɹ","l" };
                case "g": return new List<string> { "ɹ","l" };
                case "ʃ": return new List<string> { "ɹ" };
                default: return new List<string>();
            }
        }
        else if (preceding.Count == 2) {
            switch (preceding[0] + preceding[1]) {
                case "sp": return new List<string> { "l","ɹ" };
                case "st": return new List<string> { "ɹ" };
                case "sk": return new List<string> { "w","ɹ" };
                default: return new List<string>();
            }
        }
        return new List<string>();
    }

    public override List<string> Coda() {
        List<string> coda = new List<string>();
        int length = Stat.RandInt(codaMin, codaMax);

        for (int i = 0; i < length; i++) {
            coda.Add(Ling.RandPhoneme(AllowedCoda(coda)));
        }

        return coda;
    }

}

public class Zhroom : Swedish {

    public Zhroom() {
        vowels = new string[] {
            "a","æ","e","ɔ:","o:","ʊ","œ","ʌ","y:",  "ey","ay","ɔy","aʊ","ʊə"
        };
        consonants = new string[] {
            "b","d","g","ɦ","j","l","m","n","ɹ","v","w","z","ʒ","ð","ŋ",/*"ɳ","ɖ","ɭ",*/"dʒ"
        };
    }

    //

    public override string Romanize(string phoneme) {
        switch (phoneme) {
            case "": break;
            case "a": return "a";
            case "e": return "e";
            case "ɔ:": return "au";
            case "ʊ": return "o";
            case "ʌ": return "u";
            case "y:": return "y";

            case "æ": return "æ";
            case "œ": return "œ";
            case "o:": return "oo";

            case "ey": return "ey";
            case "ay": return "ay";
            case "ɔy": return "oy";
            case "aʊ": return "ou";
            case "ʊə": return "au";

            case "b": return "b";
            case "d": return "d";
            case "g": return "g";
            case "ɦ": return "ɦ";
            case "j": return "y";
            case "l": return "l";
            case "n": return "n";
            case "ɹ": return "r";
            case "v": return "v";
            case "w": return "w";
            case "z": return "z";
            case "m": return "m";

            case "ʒ": return "ʑ";
            case "ð": return "dɦ";
            case "ŋ": return "ng";
            case "ɳ": return "rn";
            case "ɖ": return "rd";
            case "ɭ": return "rl";
            case "dʒ": return "dʑ";

            default: return phoneme;
        }

        return "";
    }

}

public class Shfe : Swedish {

    public Shfe() {
        vowels = new string[] {
            "æ","e","i",  "æi","ei","eæ"
        };
        consonants = new string[] {
            "ɸ","f","θ","s","ʃ","h","n̥","ʍ"
        };
        onsetMax = 2;
        codaMax = 2;
        sylMax = 1;
        sylMean = 1;
    }

    //

    public override string Romanize(string phoneme) {
        switch (phoneme) {
            case "": break;
            case "e": return "ë";
            case "i": return "i";
            case "æ": return "ä";

            case "æi": return "äi";
            case "ei": return "ëi";
            case "eæ": return "ëä";

            case "ɸ": return "φ";
            case "f": return "f";
            case "θ": return "th";
            case "s": return "s";
            case "ʃ": return "sh";
            case "h": return "h";
            case "n̥": return "ñ";
            case "ʍ": return "w";

            default: return phoneme;
        }

        return "";
    }

    //

    public override List<string> AllowedOnset(List<string> preceding, int length) {
        int count = preceding.Count;
        List<string> onset = new List<string>(consonants);

        if (length > 1) onset.Remove("h");

        if (count >= 1) {
            string previous = preceding[count-1];
            onset.Remove(previous);

            List<string> lessSonorous = Ling.GetConsonantsRangeMax(new Ling.Consonant(previous).GetSoronity());
            Stat.RemoveRange(onset, lessSonorous);

            if (previous == "ɸ") onset.Remove("ʍ");
            if (previous == "f") onset.Remove("s");
            if (previous == "θ") onset.Remove("ʃ");
            //if (previous == "ʃ") onset.Remove("θ"); //
            if (previous == "h") onset.Remove("n̥");
            if (previous == "n̥") onset.Remove("");
            if (previous == "ʍ") { onset.Remove("n̥"); onset.Remove("s"); onset.Remove("f"); onset.Remove("ɸ"); }
            
        }

        return onset;        
    }
    
    public override List<string> AllowedNucleous() {
        return new List<string>(vowels);
    }
    
    public override List<string> AllowedCoda(List<string> preceding) {
        int count = preceding.Count;
        List<string> coda = new List<string>(consonants);

        coda.Remove("h"); coda.Remove("ʍ");

        if (count >= 1) {
            string previous = preceding[count-1];
            coda.Remove(previous);

            List<string> moreSonorous = Ling.GetConsonantsRangeMin(new Ling.Consonant(previous).GetSoronity());
            Stat.RemoveRange(coda, moreSonorous);

            if (previous == "ɸ")
            if (previous == "f")
            if (previous == "θ")
            if (previous == "s") coda.Remove("ʃ");
            if (previous == "ʃ") coda.Remove("s");
            //if (previous == "n̥") coda.Remove("θ");
        }

        return coda;
    }

}

public class Swedish : Language {

    public Swedish() {
        vowels = new string[] {
            "a","ɑ:","ʊ","u:","ɵ","ʉ:","ɔ","o:","e","ə","e:","ɪ","i:","ʏ","y:","æ","æ:","ɛ","ɛ:","œ","œ:","ø:" 
        };
        consonants = new string[] {
            "b","d","f","g","h","j","k","l","m","n","p","r","s","t","v","ŋ","ɧ","ɕ","ʂ","ɖ","ʈ","ɳ","ɭ" 
        };
        onsetMin = 0;
        onsetMax = 3;
        codaMin = 0;
        codaMax = 3;
        sylMin = 1;
        sylMax = 2;
    }

    //

    public override string Romanize(List<string> phonemes) { 
        string letters = "";
        foreach (string phoneme in phonemes) {
            letters += Romanize(phoneme);
        }
        
        return letters;
    }
    
    public override string Romanize(string phoneme) {
        string letter = "";
        switch (phoneme) {
            case "": break;
            case "a": letter += "a"; break;
            case "ɑ:": letter += "a"; break;
            case "ʊ": letter += "o"; break;
            case "u:": letter += "o"; break;
            case "ɵ": letter += "u"; break;
            case "ʉ:": letter += "u"; break;
            case "ɔ": letter += "å"; break;
            case "o:": letter += "å"; break;
            case "e": letter += "e"; break;
            case "ə": letter += "e"; break;
            case "e:": letter += "e"; break;
            case "ɪ": letter += "i"; break;
            case "i:": letter += "i"; break;
            case "ʏ": letter += "y"; break;
            case "y:": letter += "y"; break;
            case "æ": letter += "ä"; break;
            case "æ:": letter += "ä"; break;
            case "ɛ": letter += "ä"; break;
            case "ɛ:": letter += "ä"; break;
            case "œ": letter += "ö"; break;
            case "œ:": letter += "ö"; break;
            case "ø:": letter += "ö"; break;

            case "b": letter += "b"; break;
            case "d": letter += "d"; break;
            case "f": letter += "f"; break;
            case "g": letter += "g"; break;
            case "h": letter += "h"; break;
            case "j": letter += "j"; break;
            case "k": letter += "k"; break;
            case "l": letter += "l"; break;
            case "m": letter += "m"; break;
            case "n": letter += "n"; break;
            case "p": letter += "p"; break;
            case "r": letter += "r"; break;
            case "s": letter += "s"; break;
            case "t": letter += "t"; break;
            case "v": letter += "v"; break;
            case "ŋ": letter += "ng"; break;
            case "ɧ": letter += "sj"; break;
            case "ɕ": letter += "tj"; break;
            case "ʂ": letter += "rs"; break;
            case "ɖ": letter += "rd"; break;
            case "ʈ": letter += "rt"; break;
            case "ɳ": letter += "rn"; break;
            case "ɭ": letter += "rl"; break;

            default: letter += phoneme; break;
        }

        return letter;
    }

    //

    public override List<string> AllowedOnset(List<string> preceding, int length) {
        int count = preceding.Count;
        List<string> onset = new List<string>(consonants);

        Stat.RemoveRange(onset, new List<string> {"ŋ","ɧ","ɕ","ʂ","ɖ","ʈ","ɳ","ɭ"});
        if (length > 1) { onset.Remove("h"); }

        if (count < length-1) {
            Stat.KeepRange(onset, Ling.obstruents);
        }
        if (count >= 1) {
            string previous = preceding[count-1];
            onset.Remove(previous);

            Stat.RemoveRange(onset, Ling.GetObstruents(true));

            if (previous != "s") {
                List<string> liquidsGlides = new List<string>(Ling.liquids);
                liquidsGlides.AddRange(Ling.glides);
                Stat.KeepRange(onset, liquidsGlides);

                Ling.Soronity sonority = new Ling.Consonant(previous).GetSoronity();
                Stat.RemoveRange(onset, Ling.GetConsonantsRangeMax(sonority));
            }

            Stat.RemoveRange(onset, Ling.affricates);
        }

        return onset;        
    }
    
    public override List<string> AllowedNucleous() {
        return new List<string>(vowels);
    }
    
    public override List<string> AllowedCoda(List<string> preceding) {
        int count = preceding.Count;
        List<string> coda = new List<string>(consonants);

        coda.Remove("h");
        Stat.RemoveRange(coda, Ling.glides);

        if (count >= 1) {
            string previous = preceding[count-1];
            Ling.Consonant con = new Ling.Consonant(previous);
            coda.Remove(previous);

            List<string> moreSonorous = Ling.GetConsonantsRangeMin(con.GetSoronity());
            //moreSonorous.Remove("s");
            Stat.RemoveRange(coda, moreSonorous);

            Stat.RemoveRange(coda, new List<string> {"r","ŋ"});
            if (!con.voiced) {
                Stat.RemoveRange(coda, Ling.GetConsonants(true));
            }
            if (Ling.GetConsonantsByManner(Ling.Manner.Nasal).Contains(previous)) {
                List<string> unhomorganicObs = Stat.RemoveRangeNew(Ling.obstruents, Ling.GetConsonantsByManner(con.manner));
                Stat.RemoveRange(coda, unhomorganicObs);
            }
        }

        for (int i = 0; i < count; i++) {
            if (Ling.obstruents.Contains(preceding[i])) {
                List<string> unsharedVoicingObs = Stat.RemoveRangeNew(Ling.obstruents, Ling.GetConsonants(new Ling.Consonant(preceding[i]).voiced));
                Stat.RemoveRange(coda, unsharedVoicingObs);
                break;
            }
        }

        return coda;
    }

}

public class HawaiianL : Language {

    public HawaiianL() {
        vowels = new string[] {
            "a","ā","e","ē","i","ī","o","ō","u","ū","ai","ae","ao","au","ei","eu","iu","oe","oi","ou","ui" 
        };
        consonants = new string[] {
            "p","k","ʻ","h","m","n","l","w" 
        };
        onsetMin = 0;
        onsetMax = 1;
        codaMin = 0;
        codaMax = 0;
        sylMin = 2;
        sylMax = 3;
    }

}
