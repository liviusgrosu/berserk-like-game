using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour
{
    public string InteractText;
    private GameObject _interactTextObj;
    public GameObject InteractTextPrefab;
    public bool DestroyTextAfter;

    private float _time;
    public float FadeDuration = 1.0f;

    private bool CanInteract;

    void Start()
    {
        // Instantiate the text and assign it to the prompt game object
        Transform promptsParent = GameObject.Find("Prompts").transform;
        _interactTextObj = Instantiate(InteractTextPrefab, promptsParent.position, Quaternion.identity);
        _interactTextObj.transform.parent = promptsParent;
        // Update the text
        _interactTextObj.GetComponent<Text>().text = InteractText;
        // Hide the text
        _interactTextObj.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    void Update()
    {
        // Move the text in relation from camera to world coordinates
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
        // TODO: adjust for various screen resolutions
        pos = new Vector3(pos.x, pos.y - 50.0f, pos.z);
        _interactTextObj.GetComponent<RectTransform>().transform.position = pos;

        if (Input.GetKey(KeyCode.E) && CanInteract)
        {
            // Call the interact function found in this gameobject
            GetComponent<IInteract>().Interact();
            
            if (DestroyTextAfter)
            {
                // Destroy text if need be
                Destroy(_interactTextObj);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Player")
        {
            // Start fading in the pick up prompt text
            StopAllCoroutines();
            _time = 0.0f;
            StartCoroutine(FadeIn());
            CanInteract = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.name == "Player")
        {
            // Start fading out the pick up prompt text
            StopAllCoroutines();
            _time = FadeDuration;
            StartCoroutine(FadeOut());
            CanInteract = false;
        }
    }

    IEnumerator FadeIn()
    {
        // Fade in the pick up prompt text
        while (_time < FadeDuration)
        {
            _time += Time.deltaTime;
            _interactTextObj.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, _time / FadeDuration);
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        // Fade out the pick up prompt text
        while (_time > 0.0f)
        {
            _time -= Time.deltaTime;
            _interactTextObj.GetComponent<Text>().color = new Color(1.0f, 1.0f, 1.0f, _time / FadeDuration);
            yield return null;
        }
    }
}