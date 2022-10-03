using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RankingManager : MonoBehaviour
{
    private const string apiSecretKey = "ThisIsLudumDare512022##";

    public Rank[] topRanks;
    public Rank personalRank;

    public void Refresh()
    {
        Leaderboard.GetLeaderboard().Then(res =>
        {
            Debug.Log("Leaderboard got: " + JsonConvert.SerializeObject(res));
            for (var i = 0; i < res.Length; i++)
            {
                var entry = res[i];
                var minutes = (int)entry.score / 60;

                topRanks[i].name.text = entry.name;
                topRanks[i].time.text = entry.score.ToString(CultureInfo.InvariantCulture);
            }
        });

        Leaderboard.GetOwn(NameManager.Instance.Name).Then(res =>
        {
            Debug.Log("Personal got: " + JsonConvert.SerializeObject(res));
            var minutes = (int)res.score / 60;
            personalRank.name.text = res.name;
            personalRank.time.text = res.score.ToString(CultureInfo.InvariantCulture);
            personalRank.rankNr.text = $"{res.id + 1}.";

            if (res.id <= 0)
            {
                personalRank.rankNr.text = "-";
            }
        });
    }

    public void SendScore(int level, float seconds)
    {
        Debug.Log("Sending score");
        Leaderboard.SendScore(new BoardEntry()
        {
            name = NameManager.Instance.Name,
            score = level,
            time = seconds
        }, apiSecretKey).Then(res => { Debug.Log("Score sent: " + JsonConvert.SerializeObject(res)); });
    }
}

[Serializable]
public class Rank
{
    public TextMeshProUGUI rankNr;
    public TextMeshProUGUI name;
    public TextMeshProUGUI time;
}