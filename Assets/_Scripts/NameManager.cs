using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Doozy.Engine;

public class NameManager : MonoBehaviour
{
    [HideInInspector]
    public string Name;

    public static NameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void CheckName()
    {
        StartCoroutine(WaitAFrame());
        IEnumerator WaitAFrame()
        {
            yield return null;
            if (string.IsNullOrEmpty(Name))
            {
                GameEventMessage.SendEvent("NoName");
            }
            else
            {
                GameEventMessage.SendEvent("NameSet");
            }
        }
    }
}