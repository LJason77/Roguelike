using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Wall : MonoBehaviour
{
    [FormerlySerializedAs("Hp")] public int hp = 2;

    // 受到攻击的图片
    [FormerlySerializedAs("DamageSprite")] public Sprite damageSprite;

    // 自身受到攻击的时候
    public void TakeDamage()
    {
        hp -= 1;
        GetComponent<SpriteRenderer>().sprite = damageSprite;

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}