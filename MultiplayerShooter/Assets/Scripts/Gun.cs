using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public bool isAutomatic;
    public float timeBetweenShots = .1f, heatpershot = 1f;
    public GameObject GunEffect;
    public int playerDamage;

}
