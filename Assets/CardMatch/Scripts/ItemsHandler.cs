﻿using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("ItemRelated")]
    int[] spriteIndexes;
    List<Item> allItemList = new List<Item>();

    internal Item currFlippedItem;
    [HideInInspector]
    public bool canClick;
    int itemCount;

    StageData currStageData;
    private void Awake()
    {
        inst = this;
        gridLayout = itemPerent.GetComponent<GridLayoutGroup>();
        gridLayoutRectTrans = itemPerent.GetComponent<RectTransform>();
        currStageData = new StageData();
        currStageData.cardItems = new List<CardItemData>();
    }



    #region Generation Mathods
    public void GenerateItemAndIndex()
    {
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
        currStageData.isRedSprite = s == redSprite;
        currStageData.cardItems.Clear();
        for (int i = 0; i < totalItemCount; i++)
        {
            Item item = Instantiate(itemPrefab, itemPerent);
            item.graphic.resetSprite = s;
            allItemList.Add(item);
            currStageData.cardItems.Add(new CardItemData());
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
            allItemList[indexList[i, 0]].graphic.spriteIndex = allItemList[indexList[i, 0]].index = currStageData.cardItems[indexList[i, 0]].index = spriteIndexes[i];
            allItemList[indexList[i, 0]].Init();
            allItemList[indexList[i, 1]].graphic.spriteIndex = allItemList[indexList[i, 1]].index = currStageData.cardItems[indexList[i, 1]].index = spriteIndexes[i];
            allItemList[indexList[i, 1]].Init();
        }
    }

    void GenerateItemsFromStageData()
    {
        itemPerent.ClearChildren();
        int destroiedCount = 0;
        for (int i = 0; i < totalItemCount; i++)
        {
            Item item = Instantiate(itemPrefab, itemPerent);
            item.graphic.resetSprite = currStageData.isRedSprite ? redSprite : yellowSprite;
            item.graphic.spriteIndex = item.index = currStageData.cardItems[i].index;
            if (currStageData.cardItems[i].isDestroyed)
            {
                item.DestroyItem();
                destroiedCount++;
            }
            item.Init();
            allItemList.Add(item);
        }
        itemCount = totalItemCount - destroiedCount;
        canClick = true;
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
                        new Delayed.Action(UIManager.inst.winPanel.Activate, .5f);
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
        if (currItem) iTween.PunchPosition(currItem.gameObject, iTween.Hash("x", .2f, "time", 0.5f));
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

    public void OnGridSwitchOn(int row, int column, bool isLoadingSavedData = false)
    {
        float size = (gridLayoutRectTrans.rect.width - column - 1) / column;
        gridLayout.cellSize = new Vector2(size, size);
        gridLayout.constraintCount = column;
        totalItemCount = row * column;
        UIManager.inst.totalTime = totalItemCount * 5;
        currStageData.row = row;
        currStageData.column = column;
        if (!isLoadingSavedData)
        {
            UIManager.inst.currTime = UIManager.inst.totalTime;
            GenerateItemAndIndex();
        }
    }

    #region LoadnSave the Stage data 
    internal void LoadGameSavedData()
    {
        // Load stored data
        Debug.Log("Loading saved data");
        string json = System.IO.File.ReadAllText(Utility.stageDataPath);
        StageData stageData = JsonUtility.FromJson<StageData>(json);
        currStageData = stageData;
        UIManager.inst.currScore.Value = stageData.score;
        UIManager.inst.currTime = stageData.time;
        UIManager.inst.StartGame();
        OnGridSwitchOn(stageData.row, stageData.column, true);
        GenerateItemsFromStageData();
        PlayerPrefs.SetInt(Utility.StageDataSavePrefKey, 0);
    }

    internal void SaveStageData()
    {
        Debug.Log("Saving data");
        PlayerPrefs.SetInt(Utility.StageDataSavePrefKey, 1);
        currStageData.time = (int)UIManager.inst.currTime;
        currStageData.score = (int)UIManager.inst.currScore.Value;

        string json = JsonUtility.ToJson(currStageData);

        System.IO.File.WriteAllText(Utility.stageDataPath, json);
    }

    public void UpdateStageData(int index)
    {
        currStageData.cardItems[index].isDestroyed = true;
    }
    #endregion
}

[Serializable]
public class CardItemData
{
    public int index;
    public bool isDestroyed = false;
}
[Serializable]
public class StageData
{
    public int row;
    public int column;
    public bool isRedSprite;
    public int time;
    public int score;
    public List<CardItemData> cardItems;
}
