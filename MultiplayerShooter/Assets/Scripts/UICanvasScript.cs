using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICanvasScript : MonoBehaviour
{
    public static UICanvasScript instance;
    public TMP_Text Overheat;
    public Image Overheatimage;

    public Image HealthImage;

    public GameObject DeathScreen;
    public TMP_Text DeathText;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
