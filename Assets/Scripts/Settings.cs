using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class Settings : MonoBehaviour
{
    public Setting[] gameSettings;

    public static Settings Instance;
    
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
        foreach (Setting setting in gameSettings)
        {
            setting.Init();
        }
    }
    public static int GetSetting(string name)
    {
        Setting setting = System.Array.Find(Instance.gameSettings, s => s.Name == name);
        if (setting == null)
        {
            Debug.LogWarning("Setting " + name + " not found.");
            return 0;
        }
        return setting.Value;
    }
    public void GuiFromSettings()
    {
        foreach (Setting setting in gameSettings)
        {
            setting.UpdateUI();
        }
    }
    
    public void SettingsFromGui()
    {
        foreach (Setting setting in gameSettings)
        {
            setting.ApplyUI();
        }
    }
    public void ResetSettings()
    {
        foreach (Setting setting in gameSettings)
        {
            setting.Reset();
        }
        GuiFromSettings();
    }

    [System.Serializable]
    public class Setting 
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private int _defaultValue;
        [SerializeField]
        private bool _isToggle;
        public bool IsToggle { get { return _isToggle; } }
        [SerializeField]
        [AllowNesting]
        [HideIf("IsToggle")]
        private InputField _inputField;
        [SerializeField]
        [AllowNesting]
        [ShowIf("IsToggle")]
        private Toggle _toggle;
        [SerializeField]
        [AllowNesting]
        [ShowIf("IsToggle")]
        private ControlInteractivity _controlOther;
        public bool HideControlOther { get { return _controlOther == ControlInteractivity.Off || !_isToggle; } }
        [SerializeField]
        [AllowNesting]
        [HideIf("HideControlOther")]
        private Selectable _control;
        private int _value;
        public int Value
        {
            get { return _value; }
            set 
            { 
                _value = value; 
                PlayerPrefs.SetInt(_name, _value);
                UpdateUI();
            }
        }
        public void Reset()
        {
            Value = _defaultValue;
        }
        public string Name { get { return _name; } }
        private static List<Setting> _settings = new List<Setting>();
        public void Init()
        {
            if (_settings.Find(s => s.Name == _name) != null)
            {
                Debug.LogWarning("Setting " + _name + " already exists. Skipping.");
                return;
            }
            _value = PlayerPrefs.GetInt(_name, _defaultValue);
            if (this._isToggle && this._toggle != null)
            {
                _toggle.onValueChanged.AddListener((v) => Settings.Instance.SettingsFromGui());
            }
            else
            {
                _inputField.onEndEdit.AddListener((string valUpdate) => { Value = int.Parse(valUpdate); });
            }
            _settings.Add(this);
        }
        public void UpdateUI()
        {
            if (_isToggle && _toggle != null)
            {
                _toggle.SetIsOnWithoutNotify(Value == 1);
                if (_control != null && _controlOther != ControlInteractivity.Off)
                {
                    if (_controlOther == ControlInteractivity.Enable)
                    {
                        _control.interactable = Value == 1;
                    }
                    else if (_controlOther == ControlInteractivity.Disable)
                    {
                        _control.interactable = Value == 0;
                    }
                }
            }
            else if (_inputField != null)
            {
                _inputField.SetTextWithoutNotify(Value.ToString());
            }
        }
        public void ApplyUI()
        {
            if (_isToggle)
            {
                Value = (_toggle != null && _toggle.isOn) ? 1 : 0;
            }
            else if (_inputField != null)
            {
                Value = int.Parse(_inputField.text);
            }

        }
        public enum ControlInteractivity
        {
            Off,
            Enable,
            Disable
        }
    }
}
