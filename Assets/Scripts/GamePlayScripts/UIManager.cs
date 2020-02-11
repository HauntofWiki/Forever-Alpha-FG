using System.Collections;
using System.Collections.Generic;
using GamePlayScripts;
using GamePlayScripts.CharacterMoves;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UIManager 
{
    //Background UI
    public Image player1HealthBarEmpty;
    public Image player1HealthBarDifferential;
    public Image player1HealthBarLow;
    public Image player1HealthBarMiddle;
    public Image player1HealthBarFull;
    public Image player2HealthBarEmpty;
    public Image player2HealthBarDifferential;
    public Image player2HealthBarLow;
    public Image player2HealthBarMiddle;
    public Image player2HealthBarFull;
    public Image player1MeterBar;
    public Image player2MeterBar;
    public Text gameTimer;
    public Image leftPanel;
    public Image rightPanel;

    public int RoundCount = 0;
    
    //Foreground UI
    public Text KO;
    public Text PreRound;
    public List<string> Round1Quotes;
    public List<string> RoundNQuotes;
    public bool QuoteSet = false;
    public string Text = "";
    

    public UIManager()
    {
        //Set UI OBjects
        player1HealthBarEmpty = GameObject.Find("Player 1 Empty").GetComponent<Image>();
        player1HealthBarDifferential = GameObject.Find("Player 1 Differential").GetComponent<Image>();
        player1HealthBarLow = GameObject.Find("Player 1 Low").GetComponent<Image>();
        player1HealthBarMiddle = GameObject.Find("Player 1 Middle").GetComponent<Image>();
        player1HealthBarFull = GameObject.Find("Player 1 Full").GetComponent<Image>();
        player2HealthBarEmpty = GameObject.Find("Player 2 Empty").GetComponent<Image>();
        player2HealthBarDifferential = GameObject.Find("Player 2 Differential").GetComponent<Image>();
        player2HealthBarLow = GameObject.Find("Player 2 Low").GetComponent<Image>();
        player2HealthBarMiddle = GameObject.Find("Player 2 Middle").GetComponent<Image>();
        player2HealthBarFull = GameObject.Find("Player 2 Full").GetComponent<Image>();
        //player1MeterBar = GameObject.Find("Player 1 Meter").GetComponent<Image>();
        //player2MeterBar = GameObject.Find("Player 2 Meter").GetComponent<Image>();
        gameTimer = GameObject.Find("Text Timer").GetComponent<Text>();
        leftPanel = GameObject.Find("Panel Portrait Left").GetComponent<Image>();
        leftPanel.material = (Material) Resources.Load("Materials/Portrait Camera Player 1");
        rightPanel = GameObject.Find("Panel Portrait Right").GetComponent<Image>();
        rightPanel.material = (Material) Resources.Load("Materials/Portrait Camera Player 1");
        KO = GameObject.Find("TextKO").GetComponent<Text>();
        KO.enabled = false;
        PreRound = GameObject.Find("TextRoundStart").GetComponent<Text>();

        //Set initial Health Bar values
        player1HealthBarEmpty.fillAmount = 1f;
        player1HealthBarDifferential.fillAmount = 1f;
        player1HealthBarLow.fillAmount = 0.25f;
        player1HealthBarMiddle.fillAmount = 0.99f;
        player1HealthBarFull.fillAmount = 1f;
        player2HealthBarEmpty.fillAmount = 1f;
        player2HealthBarDifferential.fillAmount = 1f;
        player2HealthBarLow.fillAmount = 0.25f;
        player2HealthBarMiddle.fillAmount = 0.99f;
        player2HealthBarFull.fillAmount = 1f;

        Round1Quotes = new List<string>
        {
            "File RageQuit.exe not found.",
            "TakeTheThrow.exe",
            "WatchYourToes is not a recognized command.",
            "sudo ./HoldBackToBLock",
            "Out of disk space on /excuses volume"
        };

    }

    public void Update(GamePlayManager.GameStates gameState, int frameCount)
    {
        
        if (gameState == GamePlayManager.GameStates.PreRound)
        {
            PreRound.enabled = true;
            string quote;
            if (!QuoteSet)
            {
                var pretext = "Round " + RoundCount + " Loading...\r\n";
                quote = (pretext + Round1Quotes[Random.Range(0, Round1Quotes.Count)]);
                QuoteSet = true;
            }
            else quote = Round1Quotes[0];

            PreRoundRoutine(frameCount, quote.ToCharArray());
        }
        if (gameState == GamePlayManager.GameStates.PostRound)
        {
            KoRoutine(frameCount);
        }

        if (gameState == GamePlayManager.GameStates.RoundActive)
        {
            PreRound.enabled = false;
            KO.enabled = false;
        }
    }
    
    public void Update(int gameTime, CharacterManager p1, CharacterManager p2)
    {
        SetClock(gameTime);
        SetHealthBars(p1, p2);
        SetMeterBars();
        DisplayComboCounter();
    }

    private void SetMeterBars()
    {
        
    }

    private void SetClock(int time)
    {
        gameTimer.text = time.ToString();
    }

    private void SetHealthBars(CharacterManager p1, CharacterManager p2)
    {
        //Player 1 health bars
        if (p1.CurrentHealth / p1.MaxHealth <= 0.25)
        {
            player1HealthBarFull.fillAmount = 0;
            player1HealthBarMiddle.fillAmount = 0;
            player1HealthBarLow.fillAmount = p1.CurrentHealth / p1.MaxHealth;
        }
        else if (p1.CurrentHealth / p1.MaxHealth < 1)
        {
            player1HealthBarFull.fillAmount = 0;
            player1HealthBarMiddle.fillAmount = p1.CurrentHealth / p1.MaxHealth;
            player1HealthBarLow.fillAmount = 0;
        }
        else if (p1.CurrentHealth / p1.MaxHealth >= 1)
        {
            player1HealthBarFull.fillAmount = p1.CurrentHealth / p1.MaxHealth;
            player1HealthBarMiddle.fillAmount = 0;
            player1HealthBarLow.fillAmount = 0;
        }

        if (!p2.ComboActive)
            player1HealthBarDifferential.fillAmount = p1.CurrentHealth / p1.MaxHealth;
            
        //Player 2 health bars
        if (p2.CurrentHealth / p1.MaxHealth <= 0.25)
        {
            player2HealthBarFull.fillAmount = 0;
            player2HealthBarMiddle.fillAmount = 0;
            player2HealthBarLow.fillAmount = p2.CurrentHealth / p1.MaxHealth;
        }
        else if (p2.CurrentHealth / p1.MaxHealth < 1)
        {
            player2HealthBarFull.fillAmount = 0;
            player2HealthBarMiddle.fillAmount = p2.CurrentHealth / p1.MaxHealth;
            player2HealthBarLow.fillAmount = 0;
        }
        else if (p2.CurrentHealth / p1.MaxHealth >= 1)
        {
            player2HealthBarFull.fillAmount = p2.CurrentHealth / p1.MaxHealth;
            player2HealthBarMiddle.fillAmount = 0;
            player2HealthBarLow.fillAmount = 0;
        }
        
        if (!p1.ComboActive)
            player2HealthBarDifferential.fillAmount = p2.CurrentHealth / p2.MaxHealth;
    }

    public void StartMenu()
    {
        
    }
    
    private void DisplayComboCounter()
    {
        
    }

    public void Reset()
    {
        PreRound.enabled = false;
        KO.enabled = false;
    }

    public void KoRoutine(int frameCount)
    {
        KO.enabled = true;
        if (frameCount % 32 == 0)
        {
            KO.text = "K.O. ";
        }
        if (frameCount % 32 == 16)
        {
            KO.text = "K.O._";
        }
    }

    public void PreRoundRoutine(int frameCount, char[] quote)
    {
        if (frameCount < quote.Length)
            Text += quote[frameCount];

        PreRound.text = Text;

    }
}
