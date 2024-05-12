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
    public Button piggyBackButton;

    private EditPlayerName currentPlayer;

    private bool winState = false;
    private List<EditPlayerName> players = new List<EditPlayerName>();
    private Stack<int> scoreStack = new Stack<int>();
    private int piggyBackAmount = 0;
    private void NextPlayer()
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
        scoreStack.Clear();
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

    public void Undo()
    {
        if (scoreStack.Count == 0)
        {
            editField.text = "";
            return;
        }
        string lastScore = scoreStack.Pop().ToString();

        editField.text = (lastScore == "0") ? "" : lastScore;
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
        Clear();
        NextPlayer();
    }
    public void Piggyback()
    {
        AddToCurrentScore(piggyBackAmount);
        piggyBackAmount = 0;
    }

    private void FarklePenalty()
    {
        SetScore(currentPlayer.score - Settings.farklePenalty.Value); 
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

        NextPlayer();
        if (Settings.piggyBack.Value == 1)
        {
            piggyBackAmount = toAdd;
            piggyBackButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"Piggyback {piggyBackAmount}?";
            piggyBackButton.gameObject.SetActive(true);
        }
    }

    private void CheckForWin()
    {
        EditPlayerName potentialWinner = null;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].score >= Settings.winningScore.Value)
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

    public void ThreeOfAKind(int value)
    {
        switch (value)
        {
            case 1:
                AddToCurrentScore(Settings.threeOnes.Value);
                break;
            case 2:
                AddToCurrentScore(Settings.threeTwos.Value);
                break;
            case 3:
                AddToCurrentScore(Settings.threeThrees.Value);
                break;
            case 4:
                AddToCurrentScore(Settings.threeFours.Value);
                break;
            case 5:
                AddToCurrentScore(Settings.threeFives.Value);
                break;
            case 6:
                AddToCurrentScore(Settings.threeSixes.Value);
                break;
        }
    }
    public void FourOfaKind(int value)
    {
        if (Settings.fourOfAKindDbl.Value == 1)
        {
            switch (value)
            {
                case 1:
                    AddToCurrentScore(Settings.threeOnes.Value * 2);
                    break;
                case 2:
                    AddToCurrentScore(Settings.threeTwos.Value * 2);
                    break;
                case 3:
                    AddToCurrentScore(Settings.threeThrees.Value * 2);
                    break;
                case 4:
                    AddToCurrentScore(Settings.threeFours.Value * 2);
                    break;
                case 5:
                    AddToCurrentScore(Settings.threeFives.Value * 2);
                    break;
                case 6:
                    AddToCurrentScore(Settings.threeSixes.Value * 2);
                    break;
            }
        }
        else
        {
            AddToCurrentScore(Settings.fourOfAKind.Value);
        }
    }
    public void FiveOfaKind(int value)
    {
        if (Settings.fiveOfAKindDbl.Value == 1)
        {
            switch (value)
            {
                case 1:
                    AddToCurrentScore(Settings.threeOnes.Value * 4);
                    break;
                case 2:
                    AddToCurrentScore(Settings.threeTwos.Value * 4);
                    break;
                case 3:
                    AddToCurrentScore(Settings.threeThrees.Value * 4);
                    break;
                case 4:
                    AddToCurrentScore(Settings.threeFours.Value * 4);
                    break;
                case 5:
                    AddToCurrentScore(Settings.threeFives.Value * 4);
                    break;
                case 6:
                    AddToCurrentScore(Settings.threeSixes.Value * 4);
                    break;
            }
        }
        else
        {
            AddToCurrentScore(Settings.fiveOfAKind.Value);
        }
    }
    public void SixOfaKind()
    {
        AddToCurrentScore(Settings.sixOfAKind.Value);
    }
    public void Straight()
    {
        AddToCurrentScore(Settings.straight.Value);
    }
    public void ThreePairs()
    {
        AddToCurrentScore(Settings.threePairs.Value);
    }
    public void FourAndPair()
    {
        AddToCurrentScore(Settings.fourAndPair.Value);
    }
    public void TwoTriplets()
    {
        AddToCurrentScore(Settings.twoTriplets.Value);
    }

    private void AddToCurrentScore(int increase)
    {
        int currentScore = 0;
        if (piggyBackButton.gameObject.activeSelf)
        {
            piggyBackButton.gameObject.SetActive(false);
        }
        int.TryParse(editField.text, out currentScore);
        int newScore = currentScore + increase;
        editField.text = newScore.ToString();
        scoreStack.Push(currentScore);
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
        scoreStack.Clear();
    }


    private IEnumerator GradualFill(Image filler, int score)
    {
        float initialFill = filler.fillAmount;
        float targetFill = Mathf.InverseLerp(0, Settings.winningScore.Value, score);
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
