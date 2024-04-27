using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemsHandler : MonoBehaviour
{
    public static ItemsHandler inst;

    public Item itemPrefab;
    public Transform itemPerent;
    public SpriteCollection sprites;
    public Sprite redSprite;
    public Sprite yellowSprite;

    GridLayoutGroup gridLayout;
    RectTransform gridLayoutRectTrans;
 
    int totalItemCount = 0;
    int[] totalItemCountArr = new int[] { 20, 24, 28 };

    [Header("ItemRelated")]
    int[] spriteIndexes;
    List<Item> allItemList = new List<Item>();

    internal Item currFlippedItem;
    [HideInInspector]
    public bool canClick;
    int itemCount;
    private void Awake()
    {
        inst = this;
        gridLayout = itemPerent.GetComponent<GridLayoutGroup>();
        gridLayoutRectTrans = itemPerent.GetComponent<RectTransform>();
    }

    #region Generation Mathods
    public void GenerateItemAndIndex()
    {
        //totalItemCount = (Rand.GetFloat(0f, 1f) < 0.3f) ? totalItemCountArr[0] : (Rand.GetFloat(0f, 1f) < 0.6f ? totalItemCountArr[1] : totalItemCountArr[2]);
        spriteIndexes = Rand.GetIntUniqueArray(totalItemCount / 2, 0, sprites.sprites.Count);
        allItemList = new List<Item>();
        itemPerent.ClearChildren();
        GenerateItems();
        GenerateIndexes();
        itemCount = totalItemCount;
        canClick = true;
    }

    private void GenerateItems()
    {
        Sprite s = (Rand.GetFloat(0f, 1f) < 0.5f ? yellowSprite : redSprite);
        for (int i = 0; i < totalItemCount; i++)
        {
            Item item = Instantiate(itemPrefab, itemPerent);
            item.graphic.resetSprite = s;
            allItemList.Add(item);
        }
    }

    void GenerateIndexes()
    {
        int[] totalIndexes = Rand.GetIntUniqueArray(totalItemCount, 0, totalItemCount + 1);
        int[,] indexList = new int[totalItemCount / 2, 2];
        int j = 0;
        for (int i = 0; i < totalIndexes.Length - 1; i = i + 2)
        {
            indexList[j, 0] = totalIndexes[i] == totalItemCount ? 0 : totalIndexes[i];
            indexList[j, 1] = totalIndexes[i + 1] == totalItemCount ? 0 : totalIndexes[i + 1];
            j++;
        }
        for (int i = 0; i < totalItemCount / 2; i++)
        {
            //print(string.Format("Index: {0} spriteIndex: {3} \nitem1 : {1}  item2 : {2}", i, indexList[i, 0], indexList[i, 1], spriteIndexes[i]));
            allItemList[indexList[i, 0]].graphic.spriteIndex = allItemList[indexList[i, 0]].index = spriteIndexes[i];
            allItemList[indexList[i, 1]].graphic.spriteIndex = allItemList[indexList[i, 1]].index = spriteIndexes[i];
        }
    }
    #endregion

    Item currItem;
    Coroutine ResetItemsCoroutine;
    public void OnItemBtnClick(Item item)
    {
        if (currFlippedItem != null && currItem != null)
        {
            StopCoroutine(ResetItemsCoroutine);
            currItem.ResetItem();
            currFlippedItem.ResetItem();
            currFlippedItem = null;
            currItem = null;
            currFlippedItem = item;
            return;
        }
        else
        {
            currItem = item;
            if (currFlippedItem != null)
            {
                canClick = false;
                if (currFlippedItem.index == currItem.index)
                {
                    UIManager.inst.UpdateScore(1);
                    AudioPlayer.PlaySFX("Match");
                    new Delayed.Action(currItem.DestroyItem, 1f);
                    new Delayed.Action(currFlippedItem.DestroyItem, 1f);
                    itemCount -= 2;
                    currFlippedItem = null;
                    currItem = null;
                    canClick = true;
                    if (itemCount == 0)
                    {
                        UIManager.inst.winPanel.Activate();
                    }
                }
                else
                {
                    ResetItemsCoroutine = StartCoroutine(ResetFaultItems());
                }
            }
            else
            {
                currFlippedItem = currItem;
                currItem = null;
            }
        }
    }

    IEnumerator ResetFaultItems()
    {
        yield return new WaitForSeconds(.5f);
        if(currItem) iTween.PunchPosition(currItem.gameObject, iTween.Hash("x", .2f, "time", 0.5f));
        if (currFlippedItem) iTween.PunchPosition(currFlippedItem.gameObject, iTween.Hash("x", .2f, "time", 0.5f));
        AudioPlayer.PlaySFX("Fault");
        yield return new WaitForSeconds(1f);
        ResetItems();
    }

    void ResetItems()
    {
        if (currItem) currItem.ResetItem();
        if (currFlippedItem) currFlippedItem.ResetItem();
        currFlippedItem = null;
        currItem = null;
        canClick = true;
    }

    public void OnGridSwitchOn(int row, int column)
    {
        float size = (gridLayoutRectTrans.rect.width - column - 1) / column;
        gridLayout.cellSize = new Vector2(size, size);
        gridLayout.constraintCount = column;
        totalItemCount = row * column;
        GenerateItemAndIndex();
    }
}

