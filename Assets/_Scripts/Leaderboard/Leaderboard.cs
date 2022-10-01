using System;
using Newtonsoft.Json;
using Proyecto26;
using RSG;

public class Leaderboard
{
    private static string api = "https://lboard51.cocapasteque.tech/leaderboard";

    public static IPromise<BoardEntry[]> GetLeaderboard()
    {
        return RestClient.GetArray<BoardEntry>(api);
    }

    public static IPromise<BoardEntry> GetOwn(string name)
    {
        return RestClient.Get<BoardEntry>($"{api}/{name}");
    }

    public static IPromise<BoardEntry> SendScore(BoardEntry entry)
    {
        return RestClient.Post<BoardEntry>(api, entry);
    }
}

[Serializable]
public class BoardEntry
{
    [JsonProperty("id")] public int id;
    [JsonProperty("name")] public string name;
    [JsonProperty("score")] public float score;
}