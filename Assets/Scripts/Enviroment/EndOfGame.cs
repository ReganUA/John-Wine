using UnityEngine;
using UnityEngine.UI;

public class EndOfGame : MonoBehaviour
{
    public Image titles;
    private void Awake()
    {
        titles.enabled = false;
    }
    public void RunTitles()
    {
        titles.enabled = true;
    }
}
