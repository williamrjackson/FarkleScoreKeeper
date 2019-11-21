using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour
{
    public Text name;
    public InputField input;
    public Image fill;
    public Text scoreText;
    public int consecutiveFarkles = 0;
    public int score = 0;
    public bool hasLost = false;


    void Start()
    {
        input.onEndEdit.AddListener(NameEdited);
    }

    public void EditName()
    {
        input.ActivateInputField();
    }

    private void NameEdited(string newName)
    {
        name.text = newName;
    }
}
