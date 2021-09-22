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
        _currentItem = Inventory.Consumables[_currentItemIdx];
        Icon.sprite = _currentItem.Icon;
        ItemCount.text = _currentItem.Count.ToString();
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
            _currentItem = Inventory.Consumables[_currentItemIdx];
            Icon.sprite = _currentItem.Icon;
            ItemCount.text = _currentItem.Count.ToString();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Use current consumable item
            PlayerBuffs.Effects.Add(_currentItem.Effect);
            _currentItem.Count --;
            ItemCount.text = _currentItem.Count.ToString();
            if (_currentItem.Count <= 0)
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
            _currentItem = Inventory.Consumables[_currentItemIdx];
            Icon.sprite = _currentItem.Icon;
            ItemCount.text = _currentItem.Count.ToString();
        }
    }
}
