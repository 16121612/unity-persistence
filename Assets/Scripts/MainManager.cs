using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text HighscoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private static string m_PlayerName;
    private int m_Highscore;
    private int m_Points;
    
    private bool m_GameOver = false;

    
    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        LoadHighscore();
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";

        if(m_Points >= m_Highscore)
        {
            HighscoreText.text = $"Best Score : {m_PlayerName} : {m_Points}";
        }
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        if(m_Points >= m_Highscore)
        {
            SaveHighscore();
        }
    }

    public static void SetPlayerName(string name)
    {
        Debug.Log("asdf");
        m_PlayerName = name;
    }

    [System.Serializable]
    class HighscoreData
    {
        public string PlayerName;
        public int Score;
    }

    public void SaveHighscore()
    {
        HighscoreData highscore = new HighscoreData();
        highscore.PlayerName = m_PlayerName;
        highscore.Score = m_Points;

        string json = JsonUtility.ToJson(highscore);
        string path = Application.persistentDataPath + "/highscores.json";
        File.WriteAllText(path, json);
    }

    public void LoadHighscore()
    {
        string path = Application.persistentDataPath + "/highscores.json";
        if(File.Exists(path))
        {
            HighscoreData highscore = JsonUtility.FromJson<HighscoreData>(File.ReadAllText(path));
            m_Highscore = highscore.Score;

            HighscoreText.text = $"Best Score : {highscore.PlayerName} : {m_Highscore}";
        }
    }
}
