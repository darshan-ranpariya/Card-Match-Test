using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGraphics : MonoBehaviour
{
    public uImage graphic;
    public Image bgImg;
    public SpriteCollection sprites;
    [HideInInspector]
    public int spriteIndex;
    
    public Sprite resetSprite;

    //public Image bgImg;

    private void Awake()
    {
        bgImg = GetComponent<Image>();
    }

    private void OnEnable()
    {
        graphic.Value = sprites.GetSprite(spriteIndex);
        bgImg.sprite = resetSprite;
    }

    public void SetSprite()
    {
        SetImageComps(true);
    }

    public void ResetSprite()
    {
        SetImageComps(false);
    }

    void SetImageComps(bool b)
    {
        for (int i = 0; i < graphic.imgComps.Length; i++)
        {
            graphic.imgComps[i].gameObject.SetActive(b);
        }
    }
}
