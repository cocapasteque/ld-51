using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image TimerBar;
    public float ConveyorSpeed = 50f;

    public List<GameObject> FoodPrefabs;
    public List<GameObject> BadFoodPrefabs;

    public Transform FoodParent;
    public LevelManager LevelMgr;

    public float FixedSpawnDuration = 0.75f;
    public Transform SpawnPosMin, SpawnPosMax, BurnPos;
    

    private bool _timerRunning = true;
    private bool _draggingFood;
    public int _currentLevel = 0;
    private List<GameObject> _levelItemPool;
    private List<GameObject> _levelRandomItemPool;
    Dictionary<int, int> _currentRecipe;
    public float _currentGoodRandomRate = 0.9f;
    private bool _spawning;

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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetupItemPools();
            ResetTimer();
            SpawnItems();
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

    public void SetDragging(bool value)
    {
        foreach (Transform child in FoodParent)
        {
            child.GetComponent<CanvasGroup>().blocksRaycasts = !value;          
        }
        _draggingFood = value;
    }

    public void NextLevel()
    {
        _currentLevel++;
    }

    public void SetupItemPools()
    {
        _levelItemPool = new List<GameObject>();
        _levelRandomItemPool = new List<GameObject>();
        Dictionary<int, int> _currentRecipe = LevelMgr.SetupRecipe(_currentLevel);
        foreach (KeyValuePair<int, int> kvp in _currentRecipe)
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                _levelItemPool.Add(FoodPrefabs[kvp.Key]);
                _levelItemPool.Add(FoodPrefabs[kvp.Key]);
            }
        }
        for (int i = 0; i < _levelItemPool.Count; i++)
        {
            float rnd = Random.Range(0f, 1f);
            if (rnd <= _currentGoodRandomRate)
            {
                _levelRandomItemPool.Add(_levelItemPool[Random.Range(0, _levelItemPool.Count)]);
            }
            else
            {
                _levelRandomItemPool.Add(BadFoodPrefabs[Random.Range(0, BadFoodPrefabs.Count)]);
            }
        }
        _levelItemPool = _levelItemPool.Concat(_levelRandomItemPool).ToList();
        _levelItemPool = _levelItemPool.Randomize().ToList();
    }

    public void SpawnItems()
    {
        StartCoroutine(Spawning());

        IEnumerator Spawning()
        {
            _spawning = true;
            for (int i = 0; i < _levelItemPool.Count; i++)
            {
                if (!_spawning)
                    break;

                SpawnItem(_levelItemPool[i]);
                yield return new WaitForSeconds(FixedSpawnDuration / _levelItemPool.Count);
            }
            while(_spawning)
            {
                SpawnItem(_levelRandomItemPool[Random.Range(0, _levelRandomItemPool.Count)]);
                yield return new WaitForSeconds(FixedSpawnDuration / _levelItemPool.Count);
            }
        }
    }

    public void SpawnItem(GameObject item)
    {
        var newItem = Instantiate(item);
        newItem.transform.parent = FoodParent;
        newItem.transform.localScale = Vector3.one;
        newItem.transform.SetAsLastSibling();
        newItem.transform.position = new Vector3(Random.Range(SpawnPosMin.position.x, SpawnPosMax.position.x), Random.Range(SpawnPosMin.position.y, SpawnPosMax.position.y), 0f);
    }
}