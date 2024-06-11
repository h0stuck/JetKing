using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    [SerializeField] private GameObject startButton;

    public bool gameStart = false;

    //private float uiPos = 0;

    // Start is called before the first frame update
    void Start()
    {
        startButton.transform.position = Camera.main.WorldToScreenPoint(new Vector3(0, -2f, 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
