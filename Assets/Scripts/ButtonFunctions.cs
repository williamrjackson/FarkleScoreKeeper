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
    public Button undoButton;

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
            players[i].IsCurrentPlayer = i == nextPlayerIndex;
        }    
        Clear();
    }

    void Start()
    {
        AddPlayer("Player 1");
        AddPlayer("Player 2");
        editField.onValueChanged.AddListener((string s) => 
        {
            if (int.TryParse(s, out int num))
            {
                scoreStack.Push(num);
            }
            undoButton.gameObject.SetActive(true);
        });
    }

    private void Reset()
    {
        Clear();

        currentPlayer = players[0];

        for (int i = 0; i < players.Count; i++)
        {
            players[i].IsCurrentPlayer = i == 0;
            players[i].scoreText.text = "0";
            players[i].score = 0;
            players[i].fill.fillAmount = 0f;
        }
        winState = false;
    }

    public void Undo()
    {
        if (scoreStack.Count > 0 && editField.text == scoreStack.Peek().ToString())
        {
            scoreStack.Pop();
        }
        if (scoreStack.Count == 0)
        {
            editField.SetTextWithoutNotify("");
            undoButton.gameObject.SetActive(false);
            return;
        }
        string lastScore = scoreStack.Pop().ToString();

        if (lastScore == "0")
        {
            editField.SetTextWithoutNotify("");
            undoButton.gameObject.SetActive(false);
            return;
        } 
        editField.SetTextWithoutNotify(lastScore);
    }

    public void AddPlayer(string initialName)
    {
        EditPlayerName newPlayer = Instantiate(playerPrefab, playerParent);
        newPlayer.playerName.text = initialName;
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
        int penalty = Settings.GetSetting("farklePenalty");
        if (penalty == 0) return;
        int updatedScore = currentPlayer.score - penalty;
        SetScore(updatedScore); 
        StartCoroutine(GradualFill(currentPlayer.fill, updatedScore));
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
        if (toAdd < Settings.GetSetting("bankMinimum")) 
        {
            Message.Show($"Banked score must be at least {Settings.GetSetting("bankMinimum")} !", Message.MessageType.Urgent, 2f);
            return;
        }

        int sum = (currentPlayer.score + toAdd);
        SetScore(sum);
        StartCoroutine(GradualFill(currentPlayer.fill, sum));
        audioSource.PlayOneShot(bankAudio);
        Clear();

        currentPlayer.consecutiveFarkles = 0;

        CheckForWin();

        NextPlayer();
        if (Settings.GetSetting("piggyBack") == 1)
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
            if (players[i].score >= Settings.GetSetting("winningScore"))
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
            Message.Show(potentialWinner.playerName.text + " Wins!", Message.MessageType.Info, 3f);
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
                AddToCurrentScore(Settings.GetSetting("threeOnes"));
                break;
            case 2:
                AddToCurrentScore(Settings.GetSetting("threeTwos"));
                break;
            case 3:
                AddToCurrentScore(Settings.GetSetting("threeThrees"));
                break;
            case 4:
                AddToCurrentScore(Settings.GetSetting("threeFours"));
                break;
            case 5:
                AddToCurrentScore(Settings.GetSetting("threeFives"));
                break;
            case 6:
                AddToCurrentScore(Settings.GetSetting("threeSixes"));
                break;
        }
    }
    public void FourOfaKind(int value)
    {
        if (Settings.GetSetting("fourOfAKindDbl") == 1)
        {
            switch (value)
            {
                case 1:
                    AddToCurrentScore(Settings.GetSetting("threeOnes") * 2);
                    break;
                case 2:
                    AddToCurrentScore(Settings.GetSetting("threeTwos") * 2);
                    break;
                case 3:
                    AddToCurrentScore(Settings.GetSetting("threeThrees") * 2);
                    break;
                case 4:
                    AddToCurrentScore(Settings.GetSetting("threeFours") * 2);
                    break;
                case 5:
                    AddToCurrentScore(Settings.GetSetting("threeFives") * 2);
                    break;
                case 6:
                    AddToCurrentScore(Settings.GetSetting("threeSixes") * 2);
                    break;
            }
        }
        else
        {
            AddToCurrentScore(Settings.GetSetting("fourOfAKind"));
        }
    }
    public void FiveOfaKind(int value)
    {
        if (Settings.GetSetting("fiveOfAKindDbl") == 1)
        {
            switch (value)
            {
                case 1:
                    AddToCurrentScore(Settings.GetSetting("threeOnes") * 4);
                    break;
                case 2:
                    AddToCurrentScore(Settings.GetSetting("threeTwos") * 4);
                    break;
                case 3:
                    AddToCurrentScore(Settings.GetSetting("threeThrees") * 4);
                    break;
                case 4:
                    AddToCurrentScore(Settings.GetSetting("threeFours") * 4);
                    break;
                case 5:
                    AddToCurrentScore(Settings.GetSetting("threeFives") * 4);
                    break;
                case 6:
                    AddToCurrentScore(Settings.GetSetting("threeSixes") * 4);
                    break;
            }
        }
        else
        {
            AddToCurrentScore(Settings.GetSetting("fiveOfAKind"));
        }
    }
    public void SixOfaKind()
    {
        AddToCurrentScore(Settings.GetSetting("sixOfAKind"));
    }
    public void Straight()
    {
        AddToCurrentScore(Settings.GetSetting("straight"));
    }
    public void ThreePairs()
    {
        AddToCurrentScore(Settings.GetSetting("threePairs"));
    }
    public void FourAndPair()
    {
        AddToCurrentScore(Settings.GetSetting("fourAndPair"));
    }
    public void TwoTriplets()
    {
        AddToCurrentScore(Settings.GetSetting("twoTriplets"));
    }

    private void AddToCurrentScore(int increase)
    {
        if (piggyBackButton.gameObject.activeSelf)
        {
            piggyBackButton.gameObject.SetActive(false);
        }
        int.TryParse(editField.text, out int currentScore);
        int newScore = currentScore + increase;
        editField.text = newScore.ToString();
        if (scoreStack.Count > 0) undoButton.gameObject.SetActive(true);
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
        editField.SetTextWithoutNotify("");
        scoreStack.Clear();
        undoButton.gameObject.SetActive(false);
    }


    private IEnumerator GradualFill(Image filler, int score)
    {
        float initialFill = filler.fillAmount;
        float targetFill = Mathf.InverseLerp(0, Settings.GetSetting("winningScore"), score);
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
