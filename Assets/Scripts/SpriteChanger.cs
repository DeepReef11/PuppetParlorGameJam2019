using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteChanger : MonoBehaviour
{
    public SpriteRenderer spriteChanger;
    private Sprite[] sprites;
    private int currentSprite = 0;

    void Awake()
    {
        sprites = Resources.LoadAll<Sprite>("Puppet Customers");
    }

    public void ChangeSpriteTo(int spriteIndexNumber)
    {
        currentSprite = spriteIndexNumber;
    }
}
