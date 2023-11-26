using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public Transform ViewPoint;
    public float MouseSensitivity = 1f;
    private float VerticalRotation;
    private Vector2 MouseInput;

    public float MoveSpeed = 5f;

    public float RunSpeed = 10f;

    public float activeMovespeed;

    public KeyCode Runningcode = KeyCode.LeftShift;
    public KeyCode JumpingButton = KeyCode.Space;
    

    private Vector3 MoveDirection,Movement;

    public CharacterController Charcon;

    public float JumpForce = 12f, gravitymod = 2.5f;

    public Transform GroundCheckPoint;
    private bool isgrounded;
    public LayerMask GroundLayers;

    public GameObject Bulletimpact;

    //public float TimeBetweeenShots = .1f;
    private float ShotCounter;

    private Camera Cam;

    public float MaxHeat = 10f, /*heatpershot = 1f,*/ coolrate = 4f, overheatcoolrate = 5f;
    private float HeatCounter;
    private bool OverHeated;

    public Gun[] Guns;
    private int SelectedGun;

    public float MuzzleDisplayTime;
    private float MuzzleCounter;

    public GameObject PlayerHitImpact;

    public int Healthofplayer;

    public Animator Anim;
    public GameObject PlayerModel;

    public Transform ModelGunPoint, GunHolder;


    // Start is called before the first frame update
    void Start()
    {
        Healthofplayer = 100;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Cam = Camera.main;

        //SwitchGun();

        photonView.RPC("Setgun", RpcTarget.All, SelectedGun);

        //Transform newtrans = SpawnManager.instance.GetSpawnPoint();
        //transform.position = newtrans.position;
        //transform.rotation = newtrans.rotation;

        if(photonView.IsMine)
        {
            PlayerModel.SetActive(false);
            UICanvasScript.instance.HealthImage.fillAmount = (float)Healthofplayer / 100f;
        }
        else
        {
            GunHolder.parent = ModelGunPoint;
            GunHolder.localPosition = Vector3.zero;
            GunHolder.localRotation = Quaternion.identity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + MouseInput.x, transform.rotation.eulerAngles.z);

            VerticalRotation += MouseInput.y;
            VerticalRotation = Mathf.Clamp(VerticalRotation, -60f, 60f);

            ViewPoint.rotation = Quaternion.Euler(-VerticalRotation, ViewPoint.rotation.eulerAngles.y, ViewPoint.rotation.eulerAngles.z);

            MoveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

            if (Input.GetKey(Runningcode))
            {
                activeMovespeed = RunSpeed;
            }
            else
            {
                activeMovespeed = MoveSpeed;
            }

            float Yvol = Movement.y;

            Movement = ((transform.forward * MoveDirection.z) + (transform.right * MoveDirection.x)).normalized * activeMovespeed;
            Movement.y = Yvol;

            if (Charcon.isGrounded)
            {
                Movement.y = 0f;
            }

            isgrounded = Physics.Raycast(GroundCheckPoint.position, Vector3.down, .25f, GroundLayers);

            if (Input.GetButtonDown("Jump") && isgrounded)
            {
                Movement.y = JumpForce;
            }

            Movement.y += Physics.gravity.y * Time.deltaTime * gravitymod;

            Charcon.Move(Movement * Time.deltaTime);

            LockTheCursortothemiddle();

            Changetheweapons();

            ChangeGunWithnumbers();

            /*

            if (Guns[SelectedGun].GunEffect.activeInHierarchy)
            {
                MuzzleCounter -= Time.deltaTime;
                if (MuzzleCounter <= 0)
                {
                    photonView.RPC("DeactivatetheMuzzles", RpcTarget.All);
                    //Guns[SelectedGun].GunEffect.SetActive(false);
                }
            }
            */

            photonView.RPC("DeactivateMuzzelTimer", RpcTarget.All);


            if (!OverHeated)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    ShootingMechanic();
                }


                if (Input.GetMouseButton(0) && Guns[SelectedGun].isAutomatic)
                {
                    ShotCounter -= Time.deltaTime;

                    if (ShotCounter <= 0)
                    {
                        ShootingMechanic();
                    }

                }
                HeatCounter -= coolrate * Time.deltaTime;
                UICanvasScript.instance.Overheatimage.fillAmount = (float)HeatCounter / (float)MaxHeat;
            }
            else
            {
                HeatCounter -= overheatcoolrate * Time.deltaTime;
                UICanvasScript.instance.Overheatimage.fillAmount = (float)HeatCounter / (float)MaxHeat;
                if (HeatCounter <= 0)
                {
                    OverHeated = false;
                    UICanvasScript.instance.Overheat.text = " ";
                }
            }

            if (HeatCounter < 0)
            {
                HeatCounter = 0;
            }

            Anim.SetBool("gronded", isgrounded);
            Anim.SetFloat("speed", MoveDirection.magnitude);
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            Cam.transform.position = ViewPoint.position;
            Cam.transform.rotation = ViewPoint.rotation;
        }
    }

    public void ShootingMechanic()
    {
        if (PhotonNetwork.IsConnected)
        {
            Ray ray = Cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
            ray.origin = Cam.transform.position;

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if(hit.collider.gameObject.tag == "Player")
                {
                    Debug.Log("Hit " + hit.collider.gameObject.GetPhotonView().Owner.NickName);
                    PhotonNetwork.Instantiate(PlayerHitImpact.name, hit.point, Quaternion.identity);

                    hit.collider.gameObject.GetPhotonView().RPC("DealDamage", RpcTarget.All, photonView.Owner.NickName, Guns[SelectedGun].playerDamage);

                }
                else
                {
                    GameObject BulletImpactObject = Instantiate(Bulletimpact, hit.point + (hit.normal) * 0.002f, Quaternion.LookRotation(hit.normal, Vector3.up));

                    Destroy(BulletImpactObject, 5f); ;
                }
            }

            ShotCounter = Guns[SelectedGun].timeBetweenShots;

            HeatCounter += Guns[SelectedGun].heatpershot;

            UICanvasScript.instance.Overheatimage.fillAmount = (float)HeatCounter / (float)MaxHeat;

            if (HeatCounter >= MaxHeat)
            {
                HeatCounter = MaxHeat;

                OverHeated = true;
                UICanvasScript.instance.Overheat.text = "Weapon Overheated";

            }

            photonView.RPC("ActivatetheMuzzles", RpcTarget.All);

            //Guns[SelectedGun].GunEffect.SetActive(true);
            //MuzzleCounter = MuzzleDisplayTime;
        }
    }

    public void LockTheCursortothemiddle()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }else if(Input.GetMouseButtonDown(0))
            {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void Changetheweapons()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            SelectedGun++;

            if (SelectedGun >= Guns.Length)
            {
                SelectedGun = 0;
            }

            photonView.RPC("Setgun", RpcTarget.All, SelectedGun);

        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            SelectedGun--;

            if(SelectedGun < 0)
            {
                SelectedGun = Guns.Length - 1;
            }

            photonView.RPC("Setgun", RpcTarget.All, SelectedGun);
        }

    }

    public void ChangeGunWithnumbers()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectedGun = 0;
            photonView.RPC("Setgun", RpcTarget.All, SelectedGun);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectedGun = 1;
            photonView.RPC("Setgun", RpcTarget.All, SelectedGun);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectedGun = 2;
            photonView.RPC("Setgun", RpcTarget.All, SelectedGun);
        }
    }

    public void SwitchGun()
    {
        foreach( Gun gun in Guns)
        {
            gun.gameObject.SetActive(false);
        }

        photonView.RPC("DeactivatetheMuzzles", RpcTarget.All);
        //Guns[SelectedGun].GunEffect.SetActive(false);
        Guns[SelectedGun].gameObject.SetActive(true);
    }

    //is a way to call this function and run it on the network;
    [PunRPC]
    public void DealDamage(string Damager, int DamageAmount)
    {
        TakeDamage(Damager,DamageAmount);
    }

    public void TakeDamage(string Damager, int DamageAmount)
    {
        Healthofplayer -= DamageAmount;
        if(photonView.IsMine)
        {
            UICanvasScript.instance.HealthImage.fillAmount = (float)Healthofplayer / 100f;
        }
        Debug.Log(photonView.Owner.NickName + " has been hit by " + Damager);

        if(photonView.IsMine && Healthofplayer <= 0)
        {
            PlayerSpawner.instance.Die(Damager);
        }
    }

    //this swithces the gun
    [PunRPC]
    public void Setgun(int GunToSwitcht)
    {
        if(GunToSwitcht < Guns.Length)
        {
            SelectedGun = GunToSwitcht;
            SwitchGun();
        }

    }

    [PunRPC]
    public void ActivatetheMuzzles()
    {
        Guns[SelectedGun].GunEffect.SetActive(true);
        MuzzleCounter = MuzzleDisplayTime;
    }

    [PunRPC]
    public void DeactivatetheMuzzles()
    {
        Guns[SelectedGun].GunEffect.SetActive(false);
    }

    [PunRPC]
    public void DeactivateMuzzelTimer()
    {
        if (Guns[SelectedGun].GunEffect.activeInHierarchy)
        {
            MuzzleCounter -= Time.deltaTime;
            if (MuzzleCounter <= 0)
            {
                photonView.RPC("DeactivatetheMuzzles", RpcTarget.All);
                //Guns[SelectedGun].GunEffect.SetActive(false);
            }
        }
    }


}
