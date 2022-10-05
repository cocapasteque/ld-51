using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FoodItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string Name;

    private bool _onConveyor = true;
    private bool _falling;

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
        else if (_falling)
        {
            transform.Translate(Vector3.down * GameManager.Instance.FallingSpeed * Time.deltaTime, Space.World);
            if (transform.position.y <= -100)
                Destroy(gameObject);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        _onConveyor = false;
        GameManager.Instance.SetDragging(true);
        transform.SetAsLastSibling();
        _falling = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GameManager.Instance.MainCam.ScreenToWorldPoint(eventData.position);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GameManager.Instance.SetDragging(false);
        if (!_onConveyor)
            _falling = true;
    }

    public void SetOnConveyor()
    {
        _onConveyor = true;
    }
}