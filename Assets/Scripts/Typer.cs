using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Typer : MonoBehaviour
{
    public float letterPause = 0.2f;
    [HideInInspector]
    public string message;
    Text textComp;
    bool called;
    void Start()
    {
        if (!called)
        {
            textComp = GetComponent<Text>();
            textComp.text = "";
            StartCoroutine(TypeText());
            called = true;
        }
    }

    private void OnEnable()
    {
        if (!called)
        {
            textComp = GetComponent<Text>();
            textComp.text = "";
            StartCoroutine(TypeText());
            called = true;
        }
    }
    IEnumerator TypeText()
    {
        called = true;
        foreach (char letter in message.ToCharArray())
        {
            textComp.text += letter;
            yield return 0;
            yield return new WaitForSeconds(letterPause);
        }
    }
    private void OnDisable()
    {
        called = false;
        StopCoroutine(TypeText());
    }
}