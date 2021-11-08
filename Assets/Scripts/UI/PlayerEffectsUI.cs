using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEffectsUI : MonoBehaviour
{
    public GameObject IconPrefab;
    private PlayerBuffs _playerBuffs;
    private int _effectsCount;

    void Start()
    {
        _playerBuffs = GameObject.Find("Player").GetComponent<PlayerBuffs>();
        _effectsCount = _playerBuffs.Effects.Count;
    }

    void Update()
    {
        if (_effectsCount != _playerBuffs.Effects.Count)
        {
            // Destroy all the current effect icons
            foreach (Transform child in transform) 
            {
                GameObject.Destroy(child.gameObject);
            }

            // Instatiate all the icons
            for(int i = 0; i < _playerBuffs.Effects.Count; i++)
            {
                Sprite icon = _playerBuffs.Effects[i].GetComponent<IStatEffect>().GetIcon();

                // Don't display if it doesnt have an icon
                if (icon == null)
                {
                    continue;
                }                

                // Instantiate the effects icon and position it accordingly
                GameObject effectIcon = Instantiate(IconPrefab, transform.position, Quaternion.identity);
                effectIcon.transform.position += new Vector3(i * (IconPrefab.GetComponent<RectTransform>().rect.width), 0.0f, 0.0f);
                effectIcon.transform.parent = transform;
                effectIcon.GetComponent<Image>().sprite = icon;
            }

            _effectsCount = _playerBuffs.Effects.Count;
        }
    }
}
