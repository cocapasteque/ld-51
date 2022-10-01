using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image TimerBar;
    public float ConveyorSpeed = 50f;

    public List<GameObject> FoodPrefabs;

    private bool _timerRunning = true;

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_timerRunning)
        {
            TimerBar.fillAmount += Time.deltaTime / 10f;
            if (TimerBar.fillAmount >= 1f)
            {
                _timerRunning = false;
                Debug.Log("Game Over");
            }
        }
    }

    public void ResetTimer()
    {
        TimerBar.fillAmount = 0;
    }

    public void StartTimer()
    {
        _timerRunning = true;
    }

    public void StopTimer()
    {
        _timerRunning = false;
    }
}