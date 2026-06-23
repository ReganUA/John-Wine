using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndOfGame : MonoBehaviour
{
    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Update()
    {
        GoUp();
    }
    private void GoUp()
    {
        rectTransform.anchoredPosition -= new Vector2(0, 50f * Time.deltaTime);
    }
}
