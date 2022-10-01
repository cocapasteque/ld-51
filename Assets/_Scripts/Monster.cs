using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Monster : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        var obj = eventData.pointerDrag;
        bool good = GameManager.Instance.CheckFood(obj.GetComponent<FoodItem>());
        Destroy(obj);
    }
}