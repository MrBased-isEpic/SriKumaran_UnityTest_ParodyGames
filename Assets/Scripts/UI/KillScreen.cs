using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KillScreen : MonoBehaviour
{
    [SerializeField] private Button retryBtn;
    [SerializeField] private Button quitBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        retryBtn.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        
        quitBtn.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
