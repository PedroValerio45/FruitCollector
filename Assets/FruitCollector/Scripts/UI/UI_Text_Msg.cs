// Added UI script for date time msg

using System.Collections;
using NUnit.Compatibility;
using TMPro;
using UnityEngine;

public class UI_Text_Msg : MonoBehaviour
{
    private TextMeshProUGUI textMesh;

    void Start()
    {
        // textMesh = GetComponentInChildren<TextMeshProUGUI>(); // I do this in ChangeMessageText() instead

        StartCoroutine(FadeOutAfterDelay(5f));
    }

    private IEnumerator FadeOutAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameObject.SetActive(false);
    }

    public void ChangeMessageText(string text)
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        textMesh.text = text;
        Debug.Log("changing message text");
    }
}