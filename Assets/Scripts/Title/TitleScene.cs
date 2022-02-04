using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    // Start is called before the first frame update
    public Button titleButton;
    public GameObject logoPanel;
    private Image logoImg;

    private float rogo_r, rogo_g, rogo_b;
    private float alpha = 0;
    [SerializeField, Range(2.0f, 5.0f)] float fadeinTime;
    [SerializeField, Range(2.0f, 5.0f)] float stableTime;
    [SerializeField, Range(2.0f, 5.0f)] float fadeoutTime;

    private int clock = 0;
    void Start()
    {
        logoImg = logoPanel.GetComponent<Image>();
        rogo_r = logoImg.color.r;
        rogo_g = logoImg.color.g;
        rogo_b = logoImg.color.b;
    }

    // Update is called once per frame
    void Update()
    {
        //1Fにつき1回（1秒に60回）の呼び出しを想定
        if (clock <= fadeinTime * 60) alpha = 1.0f / (fadeinTime * 60) * clock;
        else if (clock > (fadeinTime + stableTime) * 60) alpha = Mathf.Max(0, 1.0f - 1.0f / (fadeoutTime * 60) * (clock - (fadeinTime + stableTime) * 60));
        logoImg.color = new Color(rogo_r, rogo_g, rogo_b, alpha);
        clock++;

        if (clock >= (fadeinTime + stableTime + fadeoutTime) * 60) SceneManager.LoadScene("Choose");
    }
}
