using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private float elapsedTime;
    private bool isRunning = true;

    private void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}