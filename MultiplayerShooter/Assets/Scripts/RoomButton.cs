using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    public TMP_Text ButtonText;

    private RoomInfo Info;

    public void setbuttonDetails(RoomInfo inputinfo)
    {
        Info = inputinfo;

        ButtonText.text = Info.Name;
    }

    public void OpenRoom()
    {
        Launcher.instance.JoinTheRoom(Info);
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
