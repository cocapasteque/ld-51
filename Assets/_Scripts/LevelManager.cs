using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Dictionary<int, int> SetupRecipe(int level)
    {
        int sum = 0;
        int variety = 1;
        for (int i = 1; i <= int.MaxValue; i++)
        {
            sum += i;
            if (sum >= (level / 2) + 1)
            {
                variety = i;
                break;
            }
        }

        variety = Mathf.Min(variety, GameManager.Instance.FoodPrefabs.Count);

        //key: index of object | value: amount
        Dictionary<int, int> recipe = new Dictionary<int, int>();
        List<int> keys = new List<int>();
        for (int i = 0; i < variety; i++)
        {
            int newKey;
            do
            {
                newKey = Random.Range(0, GameManager.Instance.FoodPrefabs.Count);
            } while (recipe.ContainsKey(newKey));

            recipe.Add(newKey, 1);
            keys.Add(newKey);
        }
        for (int i = variety; i < (level / 2) + 1; i++)
        {
            recipe[keys[Random.Range(0, keys.Count)]]++;
        }
        return recipe;
    }
}