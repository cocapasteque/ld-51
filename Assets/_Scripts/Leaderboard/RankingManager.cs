using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class RankingManager : MonoBehaviour
{
    public string apiSecretKey;

    public Rank[] topRanks;
    public Rank personalRank;

    void Start()
    {
        Refresh();
    }

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

        // TODO: CHANGE NAME HERE FOR PLAYER NAME
        Leaderboard.GetOwn("cocapasteque").Then(res =>
        {
            Debug.Log("Personal got: " + JsonConvert.SerializeObject(res));
            var minutes = (int)res.score / 60;
            personalRank.name.text = res.name;
            personalRank.time.text = res.score.ToString(CultureInfo.InvariantCulture);
            personalRank.rankNr.text = $"{res.id + 1}";

            if (res.id == -1)
            {
                personalRank.rankNr.text = "-";
            }
        });
    }

    public void SendScore()
    {
        Debug.Log("Sending score");
        Leaderboard.SendScore(new BoardEntry()
        {
            name = "cocapasteque",
            score = 500
        }, apiSecretKey).Then(res => { Debug.Log("Score sent: " + JsonConvert.SerializeObject(res)); });
    }
}

[Serializable]
public class Rank
{
    public Text rankNr;
    public Text name;
    public Text time;
}