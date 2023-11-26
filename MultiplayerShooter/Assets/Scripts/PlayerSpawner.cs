using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public static PlayerSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    public GameObject PlayerPrefab;

    private GameObject Player;

    public GameObject DeathParticles;


    private void Start()
    {
        if(PhotonNetwork.IsConnected)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer() {

        Transform spawnpoint = SpawnManager.instance.GetSpawnPoint();

        Player = PhotonNetwork.Instantiate(PlayerPrefab.name, spawnpoint.position, spawnpoint.rotation);

    }

    public void Die(string Damager)
    {
        UICanvasScript.instance.DeathText.text = "You were killed by " + Damager;

        if(Player !=null)
        {
            StartCoroutine(ShowCharacter());
        }

    }

    public IEnumerator ShowCharacter()
    {
        PhotonNetwork.Instantiate(DeathParticles.name, Player.transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(Player);
        UICanvasScript.instance.DeathScreen.SetActive(true);
        yield return new WaitForSeconds(5f);
        UICanvasScript.instance.DeathScreen.SetActive(false);
        SpawnPlayer();
    }

}
