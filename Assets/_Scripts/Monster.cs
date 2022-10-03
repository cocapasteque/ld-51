using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Monster : MonoBehaviour, IDropHandler
{
    public Image MonsterImg;
    

    public Sprite BaseSprite;

    public List<Sprite> EatingSprites, AfterEndSprites;
    public Sprite GoodEatingEnd, BadEatingEnd, LevelEndSprite;
    public float EatingSpriteCooldown = 0.13f;
    public float AfterDoneEatingDelay = 1.5f;
    

    public List<AudioClip> GoodAudioClips, BadAudioClips;
    public AudioClip BurpClip;
    public float BurpChance = 0.2f;

    private AudioSource _audio;

    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    public void OnDrop(PointerEventData eventData)
    {
        var obj = eventData.pointerDrag;
        bool good = GameManager.Instance.CheckFood(obj.GetComponent<FoodItem>());

        PlayAudio(good);
        PlayEatingAnim(good);

        GameManager.Instance.SetDragging(false);
        Destroy(obj);
    }

    private void PlayAudio(bool good)
    {
        if (good)
            _audio.PlayOneShot(GoodAudioClips[Random.Range(0, GoodAudioClips.Count)]);
        else
            _audio.PlayOneShot(BadAudioClips[Random.Range(0, BadAudioClips.Count)]);
    }

    private void PlayEatingAnim(bool good)
    {
        StopAllCoroutines();
        StartCoroutine(Eating());
        IEnumerator Eating()
        {
            for (int i = 0; i < EatingSprites.Count; i++)
            {
                MonsterImg.sprite = EatingSprites[i];
                yield return new WaitForSeconds(EatingSpriteCooldown);
            }
            if (GameManager.Instance.LevelDone)
            {
                MonsterImg.sprite = LevelEndSprite;
                _audio.PlayOneShot(BurpClip);
                yield return new WaitForSeconds(AfterDoneEatingDelay);
                for (int i = 0; i < AfterEndSprites.Count; i++)
                {
                    MonsterImg.sprite = AfterEndSprites[i];
                    yield return new WaitForSeconds(EatingSpriteCooldown);
                }
            }
            else
            {
                MonsterImg.sprite = good ? GoodEatingEnd : BadEatingEnd;
                if (good)
                {
                    var rnd = Random.Range(0f, 1f);
                    if (rnd <= BurpChance)
                    {
                        _audio.PlayOneShot(BurpClip);
                    }
                }
            }
        }
    }
}