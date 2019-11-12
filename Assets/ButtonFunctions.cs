using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
    public InputField editField;

    [Header("Player One")]
    public Image p1Fill;
    public Text p1ScoreText;
    public Text p1Name;
    [Header("Player Two")]
    public Image p2Fill;
    public Text p2ScoreText;
    public Text p2Name;

    private Image currentFill;
    private Text currentScoreText;
    private int currentScore;
    private Text currentName;

    private int p1ConsecutiveFarkles = 0;
    private int p2ConsecutiveFarkles = 0;
    private int p1Score = 0;
    private int p2Score = 0;

    private bool winState = false;
    private bool player1Turn = true;
    private void TogglePlayer()
    {
        player1Turn = !player1Turn;
        if (player1Turn)
        {
            p2Name.color = Color.black;
            p1Name.color = Color.blue;
            currentFill = p1Fill;
            currentName = p1Name;
            currentScoreText = p1ScoreText;
            currentScore = p1Score;
        }
        else
        {
            p1Name.color = Color.black;
            p2Name.color = Color.blue;
            currentFill = p2Fill;
            currentName = p2Name;
            currentScoreText = p2ScoreText;
            currentScore = p2Score;
        }        
    }

    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        Clear();

        p1ScoreText.text = "0";
        p2ScoreText.text = "0";
        p1Score = 0;
        p2Score = 0;
        
        p1Name.color = Color.blue;
        p2Name.color = Color.black;
        currentFill = p1Fill;
        currentName = p1Name;
        currentScoreText = p1ScoreText;
        currentScore = 0;
        player1Turn = true;

        p2Fill.fillAmount = 0f;
        p1Fill.fillAmount = 0f;

        winState = false;
    }
    public void Farkle()
    {
        if (winState) 
        {
            Reset();
            return;
        }

        if (player1Turn)
        {
            p1ConsecutiveFarkles++;
            if (p1ConsecutiveFarkles > 2)
            {
                p1ConsecutiveFarkles = 0;
                FarklePenalty();
            }
        }
        else
        {
            p2ConsecutiveFarkles++;
            if (p2ConsecutiveFarkles > 2)
            {
                p2ConsecutiveFarkles = 0;
                FarklePenalty();
            }
        }
        CheckForWin();
        TogglePlayer();
    }

    private void FarklePenalty()
    {
        SetScore(currentScore - 1000); 
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
        int sum = (currentScore + toAdd);
        SetScore(sum);
        StartCoroutine(GradualFill(currentFill, sum));
        Clear();
        if (player1Turn)
        {
            p1ConsecutiveFarkles = 0;
        }
        else
        {
            p2ConsecutiveFarkles = 0;
        }

        CheckForWin();

        TogglePlayer();
    }

    private void CheckForWin()
    {
        if (p1Score < 10000 && p2Score < 10000)
        {
            return;
        }
        
        if (player1Turn)
        {
            if (p2Score > p1Score)
            {
                editField.text = p2Name.text + " Wins!";
                winState = true;
            }
        }
        else
        {
            if (p1Score > p2Score)
            {
                editField.text = p1Name.text + " Wins!";
                winState = true;
            }
        }
    }

    private void SetScore(int newScore)
    {
        currentScore = newScore;
        if(player1Turn)
        {
            p1Score = newScore;
            p1ScoreText.text = p1Score.ToString();
        }
        else
        {
            p2Score = newScore;
            p2ScoreText.text = p2Score.ToString();
        }
    }

    public void AppendDigits(string digits)
    {
        editField.text = editField.text + digits;
    }
    public void Backspace()
    {
        if (editField.text.Length > 0)
            editField.text = editField.text.Substring(0, editField.text.Length - 1);
    }
    public void Clear()
    {
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
