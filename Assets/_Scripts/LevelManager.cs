using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public void SetupRecipe(int level)
    {
        int sum = 0;
        int variety = 1;
        for (int i = 1; i <= int.MaxValue; i++)
        {
            sum += i;
            if (sum >= level)
            {
                variety = i;
                break;
            }
        }

        variety = Mathf.Min(variety, GameManager.Instance.FoodPrefabs.Count);

        //key: index of object | value: amount
        Dictionary<int, int> Recipe = new Dictionary<int, int>();
        List<int> Keys = new List<int>();
        for (int i = 0; i < variety; i++)
        {
            int newKey;
            do
            {
                newKey = Random.Range(0, GameManager.Instance.FoodPrefabs.Count);
            } while (Recipe.ContainsKey(newKey));

            Recipe.Add(newKey, 1);
            Keys.Add(newKey);
        }
        for (int i = variety; i < level; i++)
        {
            Recipe[Keys[Random.Range(0, Keys.Count)]]++;
        }
    }
}