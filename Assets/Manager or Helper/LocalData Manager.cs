using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;



public static class HighScoreManager
{
    private static string filePath = Application.persistentDataPath + "/highScore.dat";

    public static void Save(int highScore)
    {
        byte[] encryptedData = EncryptionHelper.Encrypt(highScore.ToString());
        File.WriteAllBytes(filePath, encryptedData);
    }

    public static int Load()
    {
        if (!File.Exists(filePath)) return 0;

        byte[] encryptedData = File.ReadAllBytes(filePath);
        string decryptedData = EncryptionHelper.Decrypt(encryptedData);

        return int.TryParse(decryptedData, out int score) ? score : 0;
    }
}



public static class RankingScoreManager
{
    private static string filePath = Application.persistentDataPath + "/rankingScores.json";

    public static void Save(Queue<int> rankingScores)
    {
        string json = JsonUtility.ToJson(new ScoreData { scores = new List<int>(rankingScores) });
        byte[] encryptedData = EncryptionHelper.Encrypt(json);
        File.WriteAllBytes(filePath, encryptedData);
    }

    public static Queue<int> Load()
    {
        if (!File.Exists(filePath)) return new Queue<int>();

        byte[] encryptedData = File.ReadAllBytes(filePath);
        string decryptedJson = EncryptionHelper.Decrypt(encryptedData);
        ScoreData data = JsonUtility.FromJson<ScoreData>(decryptedJson);

        return new Queue<int>(data.scores ?? new List<int>());
    }

    [System.Serializable]
    private class ScoreData
    {
        public List<int> scores;
    }
}



public static class UnsavedHighScoreFlagManager
{
    private const string key = "isUnsavedHighScore";

    public static void Save(bool isUnsavedHighScore)
    {
        PlayerPrefs.SetInt(key, isUnsavedHighScore ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool Load()
    {
        return PlayerPrefs.GetInt(key, 0) == 1;
    }
}



public static class VolumeBGMManager
{
    private const string key = "volumeBGM";

    public static void Save(float volumeBGM)
    {
        PlayerPrefs.SetFloat(key, volumeBGM);
        PlayerPrefs.Save();
    }

    public static float Load()
    {
        return PlayerPrefs.GetFloat(key, 1);
    }
}



public static class VolumeSEManager
{
    private const string key = "volumeBGM";

    public static void Save(float volumeBGM)
    {
        PlayerPrefs.SetFloat(key, volumeBGM);
        PlayerPrefs.Save();
    }

    public static float Load()
    {
        return PlayerPrefs.GetFloat(key, 1);
    }
}



public static class AdEnableTimeManager
{
    private const string key = "enableTime";

    public static void Save(DateTime time)
    {
        PlayerPrefs.SetString(key, time.ToString("o")); // ISO 8601形式で保存
        PlayerPrefs.Save();
    }

    public static DateTime Load()
    {
        string checkTimeString = PlayerPrefs.GetString(key, DateTime.MinValue.ToString("o"));
        return DateTime.Parse(checkTimeString, null, System.Globalization.DateTimeStyles.RoundtripKind);
    }
}