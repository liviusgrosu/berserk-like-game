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
    private bool _UIActive;

    void Start()
    {
        // Error checking if no consumables are in the inventory
        if (Inventory.Consumables.Count == 0)
        {
            Icon.gameObject.SetActive(false);
            ItemCount.gameObject.SetActive(false);
        }
        else
        {
            // Initialize the script
            Init();
        }
    }

    void Update()
    {
        // If an item has recently been added to the inventory then initialize the UI
        if (!_UIActive && Inventory.Consumables.Count != 0)
        {
            Init();
        }

        // Update the consumables count text
        if (ItemCount.gameObject.activeSelf)
        {
            ItemCount.text = Inventory.Consumables[_currentItemIdx].Count.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) && _UIActive)
        {
            // Cycle to previous consumable item
            _currentItemIdx--;
            if (_currentItemIdx < 0)
            {
                _currentItemIdx = Inventory.Consumables.Count - 1;
            }
            UpdateCurrentItem();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2) && _UIActive)
        {
            // Use current consumable item
            PlayerBuffs.AddBuff(_currentItem.Effect);
            Inventory.Consumables[_currentItemIdx].Count--;
            ItemCount.text = Inventory.Consumables[_currentItemIdx].Count.ToString();
            if (Inventory.Consumables[_currentItemIdx].Count <= 0)
            {
                // Remove from inventory
                Inventory.Consumables.RemoveAt(_currentItemIdx);
                _currentItemIdx++;

                // Deactive the UI when no items are available
                if (Inventory.Consumables.Count == 0)
                {
                    _UIActive = false;
                    Icon.gameObject.SetActive(false);
                    ItemCount.gameObject.SetActive(false);
                    return;
                }

                // Switch to the next item
                if(_currentItemIdx >= Inventory.Consumables.Count)
                {
                    _currentItemIdx = 0;
                }
                UpdateCurrentItem();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && _UIActive)
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
        // Update the current item
        _currentItem = Inventory.Consumables[_currentItemIdx].Consumable;
        // Update the UI elements
        Icon.sprite = _currentItem.Icon;
        ItemCount.text = Inventory.Consumables[_currentItemIdx].Count.ToString();
    }

    private void Init()
    {
        // Set the current item to the first consumable in the inventory
        _currentItemIdx = 0;

        // Activate the UI
        _UIActive = true;
        Icon.gameObject.SetActive(true);
        ItemCount.gameObject.SetActive(true);

        UpdateCurrentItem();
    }
}
