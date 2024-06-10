using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using TMPro;
using UnityEngine.SceneManagement;

public class UISystem : MonoBehaviour
{
    [SerializeField] private GameObject ESCPanel;
    private GameObject player;

    [SerializeField] private string[] whySoMad;

    public static UISystem instance;

    public bool esc = false;
    public bool showingText = false;
    public TMP_Text tmp;

    private float tScale = 1f;
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance.gameObject;
        instance = this;
        esc = false;
        ESCPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UseEscape();
        }
    }

    public void UseEscape()
    {
        esc = !esc;
        ESCPanel.SetActive(esc);
        if (esc)
        {
            tScale = Time.timeScale;
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = tScale;
        }
    }

    public void ShowText()
    {
        if (!showingText)
        {
            tmp.text = whySoMad[Random.Range(0, whySoMad.Length)];
            showingText = true;
            Invoke("HideText", 6f);
        }
    }

    public void HideText()
    {
        showingText = false;
        tmp.text = "";
    }

    public void LoadTitle()
    {
        UseEscape();
        SceneManager.LoadScene("Main");
    }

    public void GameExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
