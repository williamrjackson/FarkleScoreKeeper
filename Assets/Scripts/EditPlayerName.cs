using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditPlayerName : MonoBehaviour
{
    public Text playerName;
    public InputField input;
    public Image fill;
    public Image currentTurnIndicator;
    public Color currentPlayerColor = Color.blue;
    public Text scoreText;
    public int consecutiveFarkles = 0;
    public int score = 0;
    public bool hasLost = false;
    private bool _isCurrentPlayer = false;
    public bool IsCurrentPlayer
    {
        get
        {
            return _isCurrentPlayer;
        }
        set
        {
            _isCurrentPlayer = value;
            currentTurnIndicator.enabled = value;
            playerName.color = value ? currentPlayerColor : Color.black;
        }
    }


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
        playerName.text = newName;
    }
}
