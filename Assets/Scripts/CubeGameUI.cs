using UnityEngine;
using TMPro;

public class CubeGameUI : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float timer;

    void Update()
    {
        timer += Time.deltaTime;
        timerText.text = "생존 시간 : " + timer.ToString("0.00");
    }
}
