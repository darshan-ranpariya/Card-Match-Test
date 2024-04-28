using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSizeSwitch : MonoBehaviour
{
    public UISwitch sw;
    TMP_Text text;
    public int row, column;

    private void OnValidate()
    {
        sw = GetComponent<UISwitch>();
        text = GetComponentInChildren<TMP_Text>();
        text.text = row + " x " + column;
    }

    private void OnEnable()
    {
        sw.SwitchedOn += OnSwitchedOn;
    }

    private void OnDisable()
    {
        sw.SwitchedOn -= OnSwitchedOn;
    }

    private void OnSwitchedOn()
    {
        ItemsHandler.inst.OnGridSwitchOn(row, column);
        UIManager.inst.StartGame();
        sw.Set(false);
    }
}
