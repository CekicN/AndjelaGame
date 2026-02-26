using UnityEngine;
using TMPro;
using System.Collections;

public class TypewriterText : MonoBehaviour
{
    public TMP_Text textMeshPro; 
    public float letterDelay = 0.05f;
    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("TypewriterText: No Canvas found on this GameObject or its parents! Add a Canvas component.", gameObject);
            return;
        }
        canvas.enabled = false; 
    }

    public void ShowText(string newText)
    {
        StopAllCoroutines();
        if (canvas != null)
            canvas.enabled = true; 
        StartCoroutine(TypeText(newText));
    }

    private void Update()
    {
        if (canvas != null && canvas.enabled && Input.GetKeyDown(KeyCode.Escape))
        {
            StopAllCoroutines();
            canvas.enabled = false;
        }
    }

    private IEnumerator TypeText(string textToShow)
    {
        textMeshPro.text = "";
        foreach (char c in textToShow)
        {
            textMeshPro.text += c;
            yield return new WaitForSeconds(letterDelay);
        }
    }
}