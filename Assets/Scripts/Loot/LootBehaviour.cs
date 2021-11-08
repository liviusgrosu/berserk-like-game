using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootBehaviour : MonoBehaviour
{
    [HideInInspector]
    public GameObject LootData;
    private GameObject PickUpText;
    public GameObject PickUpTextPrefab;

    private float _time;
    public float FadeDuration = 1.0f;

    void Start()
    {
        // Instantiate the text and assign it to the prompt game object
        Transform promptsParent = GameObject.Find("Prompts").transform;
        PickUpText = Instantiate(PickUpTextPrefab, promptsParent.position, Quaternion.identity);
        PickUpText.transform.parent = promptsParent;
        // Update the text
        PickUpText.GetComponent<Text>().text = "[E] Pickup";
        // Hide the text
        PickUpText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    void Update()
    {
        // Move the text in relation from camera to world coordinates
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        // TODO: adjust for various screen resolutions
        pos = new Vector3(pos.x, pos.y - 50.0f, pos.z);
        PickUpText.GetComponent<RectTransform>().transform.position = pos;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Player")
        {
            // Start fading in the pick up prompt text
            _time = 0.0f;
            StartCoroutine(FadeIn());
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.name == "Player")
        {
            // Start fading out the pick up prompt text
            _time = FadeDuration;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn()
    {
        // Fade in the pick up prompt text
        while (_time < FadeDuration)
        {
            _time += Time.deltaTime;
            PickUpText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, _time / FadeDuration);
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        // Fade out the pick up prompt text
        while (_time > 0.0f)
        {
            _time -= Time.deltaTime;
            PickUpText.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, _time / FadeDuration);
            yield return null;
        }
    }
}
