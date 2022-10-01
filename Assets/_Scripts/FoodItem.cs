using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string Name;

    private bool _onConveyor = true;

    private void Update()
    {
        if (_onConveyor)
        {
            transform.Translate(Vector3.right * GameManager.Instance.ConveyorSpeed * Time.deltaTime, Space.World);
            if (transform.position.x >= GameManager.Instance.BurnPos.position.x)
            {
                Destroy(gameObject);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        _onConveyor = false;
        GameManager.Instance.SetDragging(true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameManager.Instance.SetDragging(false);
    }

    public void SetOnConveyor()
    {
        _onConveyor = true;
    }
}