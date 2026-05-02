using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{

    [SerializeField] private GameObject killScreen;
    [SerializeField] private float time = 120f;
    [SerializeField] private TextMeshProUGUI timerText;

    private float timer;
    private bool update = true;

    private void Start()
    {
        timer = time;
    }
    
    private void Update()
    {
        if (!update) return;
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            //lose
            Debug.Log("Lost the game");
            StopTimer();
            killScreen.gameObject.SetActive(true);
            return;
        }

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.RoundToInt(timer % 60);
        
        timerText.text = $"{minutes}:{seconds}";
    }

    public void StopTimer()
    {
        update = false;
    }
}
