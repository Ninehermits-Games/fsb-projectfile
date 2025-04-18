using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class ScoreManager : MonoBehaviour {

    public Text scoreText, highScoreText ,finalscoreText;
    public float scoreCounts, highScoreCounts, pointPerSecond;
    public bool scoreIncreasing;
    public bool coinDoublePoints;



    [DllImport("__Internal")]
    private static extern void SendScore(int score, string game);
	
    // Use this for initialization
    void Start() {
        if (PlayerPrefs.HasKey("HighScores")) {
            highScoreCounts = PlayerPrefs.GetFloat("HighScores");
        }
    }
	
    // Update is called once per frame
    void Update () {
        if (scoreIncreasing) {
            scoreCounts += pointPerSecond * Time.deltaTime;
        }

        if (scoreCounts > highScoreCounts) {
            highScoreCounts = scoreCounts;
            PlayerPrefs.SetFloat("HighScores", highScoreCounts);
        }
		
        scoreText.text = "Score: " + Mathf.Round(scoreCounts);
        finalscoreText.text = Mathf.Round(scoreCounts).ToString();
        highScoreText.text = "High Score: " + Mathf.Round(highScoreCounts);
    }

    public void AddScore(int point) {
        scoreCounts += coinDoublePoints ? point * 2 : point;
    }
}