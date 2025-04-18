using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System;
using System.Text.RegularExpressions;
using System.Threading;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject idleplayer;
    public Text readyText;
    public GameObject readyTextObj;
    public float readyTextBlinkSpeed;


    public Transform platformGenerator;
    private Vector3 platformGeneratorStartPoint;

    public PlayerController playerController;
    private Vector3 playerStartPoint;

    private ObjectDestroyer[] objectDestroyers;
    private ScoreManager scoreManager;

    [Header("UI")]
    public GameObject GameOver_panel;
    public GameObject Start_panel;
    public GameObject leaderboard_panel;
    private PowerUpManager powerUpManager;

    [DllImport("__Internal")]
    private static extern void Initialization(string url, string key);

    [DllImport("__Internal")]
    private static extern void SendScore(int score, int game);

    [DllImport("__Internal")]
    private static extern void GetLeaderboard(string leaderboard);

    public Text leaderDebug;
    private protected string _leaderboard = "fsbleaderboard";
    private protected string _url = "https://crvftqummfbglerhzokt.supabase.co";
    private protected string _key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImNydmZ0cXVtbWZiZ2xlcmh6b2t0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MzA4NzM3NzIsImV4cCI6MjA0NjQ0OTc3Mn0.qN-j4_J5VGWorv6mpxi1JttdwZIz44MweFY-cUkNQZ4";

    [Serializable]
    public class LeaderboardEntry
    {
        public string username { get; set; }
        public int score { get; set; }
    }

    private LeaderboardEntry[] _leaderboardEntries;
    public Text[] leaderboardTextUI; // Array of Text UI elements to display leaderboard entries

    // Use this for initialization
    void Start()
    {
        // Time.timeScale = 0f;
        platformGeneratorStartPoint = platformGenerator.position;
        playerStartPoint = playerController.transform.position;
        powerUpManager = FindObjectOfType<PowerUpManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        Initialization(_url, _key);
    }
    public void GameStart()
    {
        StartCoroutine(GameStartWaiting());
        Time.timeScale = 1f;
        Start_panel.SetActive(false);
    }
    public void RestartGame()
    {

        scoreManager.scoreIncreasing = false;
        playerController.gameObject.SetActive(false);
        GameOver_panel.SetActive(true);
        SendScore((int)(scoreManager.scoreCounts), 1);
    }

    public void ResetGame()
    {
        try
        {

            Time.timeScale = 1f;
            StartCoroutine(GameStartWaiting());
            powerUpManager.InActivePowerUpMode();
            GameOver_panel.SetActive(false);
            objectDestroyers = FindObjectsOfType<ObjectDestroyer>();
            for (int i = 0; i < objectDestroyers.Length; i++)
            {
                objectDestroyers[i].gameObject.SetActive(false);
            }

            playerController.transform.position = playerStartPoint;
            platformGenerator.position = platformGeneratorStartPoint;
            // playerController.gameObject.SetActive(true);
            scoreManager.scoreCounts = 0;
            // scoreManager.scoreIncreasing = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error resetting game: {e.Message} \n {e.StackTrace}");
        }

    }
    public void HomeResetGame()
    {
        try
        {

            // StartCoroutine(GameStartWaiting());
            powerUpManager.InActivePowerUpMode();
            GameOver_panel.SetActive(false);
            objectDestroyers = FindObjectsOfType<ObjectDestroyer>();
            for (int i = 0; i < objectDestroyers.Length; i++)
            {
                objectDestroyers[i].gameObject.SetActive(false);
            }

            playerController.transform.position = playerStartPoint;
            platformGenerator.position = platformGeneratorStartPoint;
            playerController.gameObject.SetActive(false);
            idleplayer.SetActive(true);
            scoreManager.scoreCounts = 0;
            Start_panel.SetActive(true);
            // scoreManager.scoreIncreasing = true;
            scoreManager.scoreIncreasing = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error resetting game: {e.Message} \n {e.StackTrace}");
        }
    }
    public void GetLeaderboardData()
    {
        try
        {
            leaderboard_panel.SetActive(true);
            GetLeaderboard(_leaderboard);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error getting leaderboard: {e.Message} \n {e.StackTrace}");
        }
    }
    public void OnLeaderboardReceived(string value)
    {
        try
        {
            var leaderboardList = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(value);
            _leaderboardEntries = leaderboardList.ToArray();
            // Debug the first entry (optional)
            if (_leaderboardEntries.Length > 0)
            {
                Debug.Log($"Top player: {_leaderboardEntries[0].username}, Score: {_leaderboardEntries[0].score}");
                UpdateLeaderboardDisplay();
            }

            // Update the UI with leaderboard data
        }
        catch (Exception e)
        {
            Debug.LogError($"Error parsing leaderboard: {e.Message} \n {e.StackTrace}");
        }

    }
    private void UpdateLeaderboardDisplay()
    {
        for (int i = 0; i < leaderboardTextUI.Length; i++)
        {
            if (i < _leaderboardEntries.Length)
            {
                // Populate the Text UI with username and score
                leaderboardTextUI[i].text = $"{i + 1}. {_leaderboardEntries[i].username} - {_leaderboardEntries[i].score}";
            }
            else
            {
                // Clear any remaining UI elements if the leaderboard data is smaller
                leaderboardTextUI[i].text = string.Empty;
            }
        }
    }
    public IEnumerator GameStartWaiting()
    {
        readyTextObj.SetActive(true);
        idleplayer.SetActive(true);
        player.SetActive(false);

        // Countdown logic
        int countdown = 3; // Start countdown from 3 seconds
        UnityEngine.UI.Text readyText = readyTextObj.GetComponent<UnityEngine.UI.Text>();

        while (countdown > 0)
        {
            readyText.text = countdown.ToString(); // Update the text with the countdown number
            yield return new WaitForSeconds(1f); // Wait for 1 second
            countdown--; // Decrease countdown
        }

        // Display "Go" at the end of the countdown
        readyText.text = "Go!";
        yield return new WaitForSeconds(1f); // Wait for a moment before starting the game

        // Hide the countdown text and start the game
        readyTextObj.SetActive(false);
        idleplayer.SetActive(false);
        player.SetActive(true);
        scoreManager.scoreIncreasing = true;
    }



    public void backLeaderBoard(bool value)
    {
        leaderboard_panel.SetActive(value);
    }
    public GameObject audioManager;
    public Toggle audioToggle;
   
    
    public void ToggleMute()
    {
        
        
        if (audioToggle.isOn)
        {
           audioManager.SetActive(true);
           
           
        }
        else
        {
            audioManager.SetActive(false);
            
           
        }
    }
}