using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsumablesUI : MonoBehaviour
{
    public Inventory Inventory;
    public Image Icon;
    public Text ItemCount;
    public PlayerBuffs PlayerBuffs;
    private ConsumableItem _currentItem; 
    private int _currentItemIdx;

    void Start()
    {
        _currentItemIdx = 0;
        // TODO: add error checking if no consumables are in the inventory
        UpdateCurrentItem();
    }

    void Update()
    {
        // TODO: add menu controller to disable this
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Cycle to previous consumable item
            _currentItemIdx--;
            if (_currentItemIdx < 0)
            {
                _currentItemIdx = Inventory.Consumables.Count - 1;
            }
            UpdateCurrentItem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Use current consumable item
            PlayerBuffs.Effects.Add(_currentItem.Effect);
            Inventory.Consumables[_currentItemIdx].Count--;
            ItemCount.text = Inventory.Consumables[_currentItemIdx].Count.ToString();
            if (Inventory.Consumables[_currentItemIdx].Count <= 0)
            {
                // TODO: remove the icon
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Cycle to next consumable item
            _currentItemIdx++;
            if (_currentItemIdx >= Inventory.Consumables.Count)
            {
                _currentItemIdx = 0;
            }
            UpdateCurrentItem();
        }
    }

    private void UpdateCurrentItem()
    {
        _currentItem = Inventory.Consumables[_currentItemIdx].Consumable;
        Icon.sprite = _currentItem.Icon;
        ItemCount.text = Inventory.Consumables[_currentItemIdx].Count.ToString();
    }
}
