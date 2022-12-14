using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Doozy.Engine.UI;
using Doozy.Engine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public Camera MainCam;
    public Vector2 FoodItemScaleRange;


    public Image TimerBar;
    public Image DangerIndicator;
    public float ResetDuration = 0.7f;

    public float BaseConveyorSpeed = 50f;
    public float BaseFallingSpeed = 50f;
    public float ConveyorSpeedLevelIncrease = 1.15f;
    [HideInInspector] public float ConveyorSpeed;
    [HideInInspector] public float FallingSpeed;

    public List<GameObject> FoodPrefabs;
    public List<GameObject> BadFoodPrefabs;

    public Transform FoodParent;
    public LevelManager LevelMgr;
    public RankingManager RankingMgr;

    public float BaseGoodRandomRate = 0.9f;
    public float FixedSpawnDuration = 0.75f;
    public Transform SpawnPosMin, SpawnPosMax, BurnPos;

    public TextMeshProUGUI RecipeText;
    public UIView RecipeView;
    public Transform MainCanvas;
    public TextMeshProUGUI LevelIndicator;

    public Monster Monster;

    [HideInInspector] public bool LevelDone = false;
    
    private bool _timerRunning = false;
    private bool _draggingFood = false;
    private int _currentLevel = 0;
    private List<GameObject> _levelItemPool;
    private List<GameObject> _levelRandomItemPool;
    Dictionary<int, int> _currentRecipe;
    private float _currentGoodRandomRate;
    private bool _spawning = false;
    private float _time;
    private float _conveyorSpeedLevelModifier = 1f;

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

        MainCam = Camera.main;
    }

    private void Update()
    {
        ConveyorSpeed = BaseConveyorSpeed * _conveyorSpeedLevelModifier * MainCanvas.localScale.x;
        FallingSpeed = BaseFallingSpeed * MainCanvas.localScale.x;

        if (_timerRunning)
        {
            _time += Time.deltaTime;
            TimerBar.fillAmount += Time.deltaTime / 10f;
            DangerIndicator.color = new Color(1f, 1f, 1f, TimerBar.fillAmount);
            if (TimerBar.fillAmount >= 1f)
            {
                GameOver();
            }
        }
    }

    public void ResetTimer()
    {
        float t = 0;
        float startValue = TimerBar.fillAmount;
        float value;
        StartCoroutine(LerpBack());
        IEnumerator LerpBack()
        {
            while (TimerBar.fillAmount > 0)
            {
                t += Time.deltaTime / ResetDuration;
                value = Mathf.Lerp(startValue, 0f, t);
                TimerBar.fillAmount = value;
                DangerIndicator.color = new Color(1f, 1f, 1f, value);
                yield return null;
            }
        }
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
        EnableDragging(!value);
        _draggingFood = value;
    }

    public void EnableDragging(bool value)
    {
        foreach (Transform child in FoodParent)
        {
            child.GetComponent<CanvasGroup>().blocksRaycasts = value;
        }
    }

    public void LevelCompleted()
    {
        LevelDone = true;
        _currentLevel++;
        LevelIndicator.text = "Level " + _currentLevel;
        _spawning = false;
        RecipeView.Hide();
        StopTimer();
        ResetTimer();
        EnableDragging(false);

        StartCoroutine(WaitForShow());
        IEnumerator WaitForShow()
        {
            yield return new WaitForSeconds(3f);
            RecipeView.Show();
        }
    }

    public void SetupNextLevel()
    {
        _conveyorSpeedLevelModifier = Mathf.Pow(ConveyorSpeedLevelIncrease, _currentLevel - 1);
        _currentGoodRandomRate = Mathf.Pow(BaseGoodRandomRate, _currentLevel - 1);
        LevelDone = false;
        SetupItemPools();
        UpdateRecipe();   
        SpawnItems();
    }

    public void StartNextLevel()
    {
        StartTimer();
        _spawning = true;
    }

    public void GameOver()
    {
        _timerRunning = false;
        _spawning = false;
        RecipeView.Hide();
        RankingMgr.SendScore(_currentLevel, _time);
        Monster.Lose();
        GameEventMessage.SendEvent("GameOver");
    }

    public void StartGame()
    {
        ResetTimer();
        _currentLevel = 1;
        LevelIndicator.text = "Level " + _currentLevel;
        _time = 0f;
        Debug.Log("Starting");
        Monster.StartWalking();
        StartCoroutine(StartDelayed());

        IEnumerator StartDelayed()
        {
            yield return new WaitForSeconds(ResetDuration + 0.05f);
            RecipeView.Show();
        }
    }

    public void SetupItemPools()
    {
        _levelItemPool = new List<GameObject>();
        _levelRandomItemPool = new List<GameObject>();
        _currentRecipe = LevelMgr.SetupRecipe(_currentLevel);
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
        newItem.transform.localScale = Vector3.one * Random.Range(FoodItemScaleRange.x, FoodItemScaleRange.y);
        newItem.transform.SetAsLastSibling();
        newItem.transform.position = new Vector3(Random.Range(SpawnPosMin.position.x, SpawnPosMax.position.x), Random.Range(SpawnPosMin.position.y, SpawnPosMax.position.y), 0f);
    }

    public bool CheckFood(FoodItem item)
    {
        int idToRemove = -1;
        foreach (KeyValuePair<int, int> kvp in _currentRecipe)
        {
            if (FoodPrefabs[kvp.Key].GetComponent<FoodItem>().Name == item.Name)
            {
                idToRemove = kvp.Key;
                break;
            }
        }
        if (idToRemove >= 0)
        {
            if (_currentRecipe[idToRemove] <= 1)
            {
                _currentRecipe.Remove(idToRemove);
                if (_currentRecipe.Count == 0)
                {
                    LevelCompleted();
                }
            }
            else
            {
                _currentRecipe[idToRemove]--;
            }
            UpdateRecipe();
            return true;            
        }
        else
            return false;
    }

    public void UpdateRecipe()
    {
        string txt = "";
        foreach (KeyValuePair<int, int> kvp in _currentRecipe)
        {
            txt += $"{kvp.Value}x <sprite index={kvp.Key}> ";
        }
        RecipeText.text = txt;
    }
}