using System.IO;
using TMPro;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public PlayerData playerData = new PlayerData();
    public TextMeshProUGUI textMeshLastScore, textMeshBestScore;
    [HideInInspector]
    public int bestScore;
    [HideInInspector]
    public int lastScore;
    [HideInInspector]
    public string path;

    private const string TEXT_LAST_SCORE = "Last Score :\n\r", TEXT_BEST_SCORE = "Best Score :\n\r";


    private void Awake()
    {
        path = Application.persistentDataPath + "/playerData.json";
    }

    private void Start()
    {
        LoadPlayerData();
        if (File.Exists(path))
        {
            if (playerData.lastScore < 0)
            {
                playerData.lastScore = 0;
            }

            if (playerData.bestScore < 0)
            {
                playerData.bestScore = 0;
            }
            textMeshLastScore.text = TEXT_LAST_SCORE + playerData.lastScore.ToString();
            textMeshBestScore.text = TEXT_BEST_SCORE + playerData.bestScore.ToString();
        }
        else
        {
            textMeshLastScore.text = TEXT_LAST_SCORE + "0";
            textMeshBestScore.text = TEXT_BEST_SCORE + "0";
        }
    }

    public void SavePlayerData()
    {
        string data;

        SaveScore();
        data = JsonUtility.ToJson(playerData);
        File.WriteAllText(path, data);
        Debug.LogWarning("Données sauvegardées à : " + path);
    }

    public void LoadPlayerData()
    {
        if (File.Exists(path))
        {
            string data = File.ReadAllText(path);

            playerData = JsonUtility.FromJson<PlayerData>(data);
            Debug.LogWarning("Données chargées à partir de : " + path);
        }
        else
        {
            Debug.LogWarning("Aucune sauvegarde à : " + path);
        }
    }

    private void SaveScore()
    {
        if (playerData.lastScore < 0)
        {
            playerData.lastScore = 0;
        }
        else
        {
            playerData.lastScore = lastScore;
        }

        if (playerData.bestScore < 0)
        {
            playerData.bestScore = 0;
        }
        else
        {
            playerData.bestScore = BestScore();
        }
    }

    private int BestScore()
    {
        int score;

        if (playerData.lastScore > playerData.bestScore)
        {
            score = playerData.lastScore;
        }
        else
        {
            score = playerData.bestScore;
        }
        return score;
    }
}
