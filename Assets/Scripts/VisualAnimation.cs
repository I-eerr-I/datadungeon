using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualAnimation : MonoBehaviour
{
    public Sprite monster;
    public Sprite monsterAttack;
    public Sprite monsterHurt;
    public Sprite monsterDeadBlack;
    public Sprite monsterDeadWhite;
    public Sprite noData;

    public float animationSpeed;

    [Header("Sound")]
    public AudioClip monsterAttackSound;

    Image image;
    AudioSource audioSource;

    void Start()
    {
        image = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDeadAnimation()
    {
        StartCoroutine("dead_animation");
    }

    public void PlayMonsterAttackAnimation()
    {
        audioSource.clip = monsterAttackSound;
        audioSource.Play();
        StartCoroutine("monster_attack_animation");
    }

    IEnumerator monster_attack_animation()
    {
        image.sprite = monsterAttack;
        yield return new WaitForSeconds(0.5f);
        image.sprite = monsterHurt;
        yield return new WaitForSeconds(0.5f);
        image.sprite = monster;
    }

    IEnumerator dead_animation()
    {
        image.sprite = monsterDeadBlack;
        for(int i = 0; i < 8; i++)
        {
            image.sprite = (image.sprite == monsterDeadWhite) ? monsterDeadBlack : monsterDeadWhite;
            yield return new WaitForSeconds(animationSpeed);
        }
        image.sprite = noData;
    }
}
