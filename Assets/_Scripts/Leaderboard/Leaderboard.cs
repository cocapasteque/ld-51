using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Proyecto26;
using RSG;
using UnityEngine;

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

    public static IPromise<BoardEntry> SendScore(BoardEntry entry, string key)
    {
        var stringEntry = JsonConvert.SerializeObject(entry);
        var encryptedEntry = Encrypt(stringEntry, key);
        Debug.Log("Sending " + encryptedEntry);
        
        return RestClient.Post<BoardEntry>(api, encryptedEntry).Then(res =>
        {
            Debug.Log(res);
            return res;
        });
    }

    public static string Encrypt(string clearText, string key)
    {
        var clearBytes = Encoding.Unicode.GetBytes(clearText);

        using var encryptor = Aes.Create();
        var pdb = new Rfc2898DeriveBytes(key,
            new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });

        encryptor.Key = pdb.GetBytes(32);
        encryptor.IV = pdb.GetBytes(16);

        using var ms = new MemoryStream();
        using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.Close();
        }

        clearText = Convert.ToBase64String(ms.ToArray());
        return clearText;
    }
}

[Serializable]
public class BoardEntry
{
    [JsonProperty("id")] public int id;
    [JsonProperty("name")] public string name;
    [JsonProperty("score")] public float score;
    [JsonProperty("time")] public float time;
}

[Serializable]
public class EncryptedEntry
{
    [JsonProperty("entry")] public string Entry { get; set; }
}