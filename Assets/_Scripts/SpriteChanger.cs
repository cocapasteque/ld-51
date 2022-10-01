using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    public List<Sprite> Sprites;
    public float Cooldown = 0.2f;

    private Image _img;
    private int _currentIndex = 0;

    private void Awake()
    {
        _img = GetComponent<Image>();
        StartCoroutine(ChangeSprites());
    }

    private IEnumerator ChangeSprites()
    {
        while (true)
        {
            yield return new WaitForSeconds(Cooldown);
            _currentIndex = (_currentIndex + 1) % Sprites.Count;
            _img.sprite = Sprites[_currentIndex];
        }
    }
}