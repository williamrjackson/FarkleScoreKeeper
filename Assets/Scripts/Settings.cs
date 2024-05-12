using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public TMP_InputField threeOnesField;
    public TMP_InputField threeTwosField;
    public TMP_InputField threeThreesField;
    public TMP_InputField threeFoursField;
    public TMP_InputField threeFivesField;
    public TMP_InputField threeSixesField;
    public TMP_InputField fourOfAKindField;
    public TMP_InputField fiveOfAKindField;
    public TMP_InputField sixOfAKindField;
    public TMP_InputField straightField;
    public TMP_InputField threePairsField;
    public TMP_InputField fourAndPairField;
    public TMP_InputField twoTripletsField;
    public TMP_InputField farklePenaltyField;
    public TMP_InputField winningScoreField;
    public Toggle piggyBackToggle;
    public Toggle fourOfAKindDblToggle;
    public Toggle fiveOfAKindDblToggle;
    
    public static Settings Instance;
    public static Setting threeOnes;
    public static Setting threeTwos;
    public static Setting threeThrees;
    public static Setting threeFours;
    public static Setting threeFives;
    public static Setting threeSixes;
    public static Setting fourOfAKind;
    public static Setting fiveOfAKind;
    public static Setting sixOfAKind;
    public static Setting straight;
    public static Setting threePairs;
    public static Setting fourAndPair;
    public static Setting twoTriplets;
    public static Setting piggyBack;
    public static Setting fourOfAKindDbl;
    public static Setting fiveOfAKindDbl;
    public static Setting farklePenalty;
    public static Setting winningScore;
    
    void Awake ()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple Settings instantiated. Component removed from " + gameObject.name + ". Instance already found on " + Instance.gameObject.name + "!");
            Destroy(this);
        }
    }
    void Start()
    {
        threeOnes = new Setting("ThreeOnes", 300);
        threeTwos = new Setting("ThreeTwos", 200);
        threeThrees = new Setting("ThreeThrees", 300);
        threeFours = new Setting("ThreeFours", 400);
        threeFives = new Setting("ThreeFives", 500);
        threeSixes = new Setting("ThreeSixes", 600);
        fourOfAKind = new Setting("FourOfaKind", 1000);
        fiveOfAKind = new Setting("FiveOfaKind", 2000);
        sixOfAKind = new Setting("SixOfaKind", 3000);
        straight = new Setting("Straight", 1500);
        threePairs = new Setting("ThreePairs", 1500);
        fourAndPair = new Setting("FourAndPair", 1500);
        twoTriplets = new Setting("TwoTriplets", 2500);
        piggyBack = new Setting("PiggyBack", 0);
        fourOfAKindDbl = new Setting("FourOfaKindDouble", 0);
        fiveOfAKindDbl = new Setting("FiveOfaKindDouble", 0);
        farklePenalty = new Setting("FarklePenalty", 500);
        winningScore = new Setting("WinningScore", 10000);
        UpdateSettingsGUI();
    }

    public void UpdateSettingsGUI()
    {
        threeOnesField.text = threeOnes.Value.ToString();
        threeTwosField.text = threeTwos.Value.ToString();
        threeThreesField.text = threeThrees.Value.ToString();
        threeFoursField.text = threeFours.Value.ToString();
        threeFivesField.text = threeFives.Value.ToString();
        threeSixesField.text = threeSixes.Value.ToString();
        fourOfAKindField.text = fourOfAKind.Value.ToString();
        fourOfAKindField.interactable = fourOfAKindDbl.Value == 0;
        fiveOfAKindField.text = fiveOfAKind.Value.ToString();
        fiveOfAKindField.interactable = fiveOfAKindDbl.Value == 0;
        sixOfAKindField.text = sixOfAKind.Value.ToString();
        straightField.text = straight.Value.ToString();
        threePairsField.text = threePairs.Value.ToString();
        fourAndPairField.text = fourAndPair.Value.ToString();
        twoTripletsField.text = twoTriplets.Value.ToString();
        farklePenaltyField.text = farklePenalty.Value.ToString();
        winningScoreField.text = winningScore.Value.ToString();
        piggyBackToggle.isOn = piggyBack.Value == 1;
        fourOfAKindDblToggle.isOn = fourOfAKindDbl.Value == 1;
        fiveOfAKindDblToggle.isOn = fiveOfAKindDbl.Value == 1;
    }
    public void SaveSettingsFromGUI()
    {
        threeOnes.Value = int.Parse(threeOnesField.text);
        threeTwos.Value = int.Parse(threeTwosField.text);
        threeThrees.Value = int.Parse(threeThreesField.text);
        threeFours.Value = int.Parse(threeFoursField.text);
        threeFives.Value = int.Parse(threeFivesField.text);
        threeSixes.Value = int.Parse(threeSixesField.text);
        fourOfAKind.Value = int.Parse(fourOfAKindField.text);
        fiveOfAKind.Value = int.Parse(fiveOfAKindField.text);
        sixOfAKind.Value = int.Parse(sixOfAKindField.text);
        straight.Value = int.Parse(straightField.text);
        threePairs.Value = int.Parse(threePairsField.text);
        fourAndPair.Value = int.Parse(fourAndPairField.text);
        twoTriplets.Value = int.Parse(twoTripletsField.text);
        farklePenalty.Value = int.Parse(farklePenaltyField.text);
        winningScore.Value = int.Parse(winningScoreField.text);
        piggyBack.Value = piggyBackToggle.isOn ? 1 : 0;
        fourOfAKindDbl.Value = fourOfAKindDblToggle.isOn ? 1 : 0;
        fiveOfAKindDbl.Value = fiveOfAKindDblToggle.isOn ? 1 : 0;
        UpdateSettingsGUI();
    }
    public void ResetSettings()
    {
        foreach (Setting setting in Setting.Settings)
        {
            setting.Reset();
        }
        UpdateSettingsGUI();
    }

    public class Setting 
    {
        private string _name;
        private int _value;
        private int _defaultValue;
        public int Value
        {
            get { return _value; }
            set 
            { 
                PlayerPrefs.SetInt(_name, value);
                this._value = value; 
            }
        }
        public void Reset()
        {
            Value = _defaultValue;
        }
        public string Name { get { return _name; } }
        private static List<Setting> settings = new List<Setting>();
        public static Setting[] Settings { get { return settings.ToArray(); } }
        public Setting(string name, int defaultValue)
        {
            this._name = name;
            this._defaultValue = defaultValue;
            Value = PlayerPrefs.GetInt(name, defaultValue);
            settings.Add(this);
        }
    }
}
