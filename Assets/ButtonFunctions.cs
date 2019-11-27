using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    public InputField editField;
    public EditPlayerName playerPrefab;
    public Transform playerParent;
    [Header("Audio")]
    public AudioClip farkleAudio;
    public AudioClip farklePenaltyAudio;
    public AudioClip bankAudio;
    public AudioClip clickAudio;
    public AudioSource audioSource;

    private EditPlayerName currentPlayer;

    private bool winState = false;
    private List<EditPlayerName> players = new List<EditPlayerName>();

    private void TogglePlayer()
    {
        int nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % players.Count;
        currentPlayer = players[nextPlayerIndex];

        for (int i = 0; i < players.Count; i++)
        {
            if (i == nextPlayerIndex)
            {
                players[i].name.color = Color.blue;
            }
            else
            {
                players[i].name.color = Color.black;
            }
        }    
    }

    void Start()
    {
        AddPlayer("Player 1");
        AddPlayer("Player 2");
    }

    private void Reset()
    {
        Clear();

        currentPlayer = players[0];

        for (int i = 0; i < players.Count; i++)
        {

            if (i == 0)
            {
                players[i].name.color = Color.blue;
            }
            else
            {
                players[i].name.color = Color.black;
            }

            players[i].scoreText.text = "0";
            players[i].score = 0;
            players[i].fill.fillAmount = 0f;
        }
        winState = false;
    }

    public void AddPlayer(string initialName)
    {
        EditPlayerName newPlayer = Instantiate(playerPrefab, playerParent);
        newPlayer.name.text = initialName;
        players.Add(newPlayer);
        Reset();
    }

    public void RemovePlayer()
    {
        if (players.Count == 0)
            return;

        EditPlayerName playerToRemove = players[players.Count - 1];
        players.Remove(playerToRemove);
        Destroy(playerToRemove.gameObject);
    }

    public void Farkle()
    {
        if (winState) 
        {
            Reset();
            return;
        }

        currentPlayer.consecutiveFarkles++;
        if (currentPlayer.consecutiveFarkles > 2)
        {
            currentPlayer.consecutiveFarkles = 0;
            FarklePenalty();
        }
        else
        {
            audioSource.PlayOneShot(farkleAudio);
        }

        CheckForWin();
        TogglePlayer();
    }

    private void FarklePenalty()
    {
        SetScore(currentPlayer.score - 1000); 
        audioSource.PlayOneShot(farklePenaltyAudio);
    }

    public void Bank()
    {
        if (winState) 
        {
            Reset();
            return;
        }

        int toAdd;
        int.TryParse(editField.text, out toAdd);
        if (toAdd == 0) return;
        int sum = (currentPlayer.score + toAdd);
        SetScore(sum);
        StartCoroutine(GradualFill(currentPlayer.fill, sum));
        audioSource.PlayOneShot(bankAudio);
        Clear();

        currentPlayer.consecutiveFarkles = 0;

        CheckForWin();

        TogglePlayer();
    }

    private void CheckForWin()
    {
        EditPlayerName potentialWinner = null;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].score >= 10000)
            {
                if (potentialWinner == null || players[i].score > potentialWinner.score)
                {
                    potentialWinner = players[i];
                }
            }
        }

        // Nobody's in a potential win state - bail.
        if (potentialWinner == null)
        {
            return;
        }

        currentPlayer.hasLost = (currentPlayer.score < potentialWinner.score);

        int loserCount = 0;
        foreach (EditPlayerName player in players)
        {
            if (player.hasLost)
            {
                loserCount++;
            }
        }
        if (loserCount == players.Count - 1)
        {
            editField.text = potentialWinner.name.text + " Wins!";
            winState = true;
        }
    }

    private void SetScore(int newScore)
    {
        currentPlayer.score = newScore;
        currentPlayer.scoreText.text = currentPlayer.score.ToString();
    }

    public void AppendDigits(string digits)
    {
        audioSource.PlayOneShot(clickAudio);
        editField.text = editField.text + digits;
    }
    public void Backspace()
    {
        audioSource.PlayOneShot(clickAudio);
        if (editField.text.Length > 0)
            editField.text = editField.text.Substring(0, editField.text.Length - 1);
    }
    public void Clear()
    {
        audioSource.PlayOneShot(clickAudio);
        editField.text = "";
    }


    private IEnumerator GradualFill(Image filler, int score)
    {
        float initialFill = filler.fillAmount;
        float targetFill = Mathf.InverseLerp(0, 10000, score);
        float duration = 1f;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            filler.fillAmount = Mathf.Lerp(initialFill, targetFill, Mathf.InverseLerp(0f, duration, elapsedTime));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        filler.fillAmount = targetFill;
    }
}
