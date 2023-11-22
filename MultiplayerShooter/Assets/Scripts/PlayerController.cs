using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    public Transform LocationToShoot;

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

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Cam = Camera.main;

        SwitchGun();

        Transform newtrans = SpawnManager.instance.GetSpawnPoint();
        transform.position = newtrans.position;
        transform.rotation = newtrans.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        MouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + MouseInput.x,transform.rotation.eulerAngles.z);

        VerticalRotation += MouseInput.y;
        VerticalRotation = Mathf.Clamp(VerticalRotation, -60f, 60f);

        ViewPoint.rotation = Quaternion.Euler(-VerticalRotation, ViewPoint.rotation.eulerAngles.y, ViewPoint.rotation.eulerAngles.z);

        MoveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        if(Input.GetKey(Runningcode))
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

        if(Charcon.isGrounded)
        {
            Movement.y = 0f;
        }

        isgrounded = Physics.Raycast(GroundCheckPoint.position, Vector3.down, .25f, GroundLayers);

        if(Input.GetButtonDown("Jump") && isgrounded)
        {
            Movement.y = JumpForce;
        }

        Movement.y += Physics.gravity.y * Time.deltaTime * gravitymod;

        Charcon.Move(Movement * Time.deltaTime);

        LockTheCursortothemiddle();

        Changetheweapons();

        ChangeGunWithnumbers();

        if (Guns[SelectedGun].GunEffect.activeInHierarchy)
        {
            MuzzleCounter -= Time.deltaTime;
            if (MuzzleCounter <= 0)
            {

                Guns[SelectedGun].GunEffect.SetActive(false);
            }
        }


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

        if(HeatCounter < 0)
        {
            HeatCounter = 0;
        }



    }

    private void LateUpdate()
    {
        Cam.transform.position = ViewPoint.position;
        Cam.transform.rotation = ViewPoint.rotation;
    }

    public void ShootingMechanic()
    {
        Ray ray = Cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = Cam.transform.position;

        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            GameObject BulletImpactObject = Instantiate(Bulletimpact, hit.point + (hit.normal) * 0.002f, Quaternion.LookRotation(hit.normal, Vector3.up));

            Destroy(BulletImpactObject, 5f); ;
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

        Guns[SelectedGun].GunEffect.SetActive(true);
        MuzzleCounter = MuzzleDisplayTime;
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

            SwitchGun();

        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            SelectedGun--;

            if(SelectedGun < 0)
            {
                SelectedGun = Guns.Length - 1;
            }

            SwitchGun();
        }

    }

    public void ChangeGunWithnumbers()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SelectedGun = 0;
            SwitchGun();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectedGun = 1;
            SwitchGun();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SelectedGun = 2;
            SwitchGun();
        }
    }

    public void SwitchGun()
    {
        foreach( Gun gun in Guns)
        {
            gun.gameObject.SetActive(false);
        }

        Guns[SelectedGun].GunEffect.SetActive(false);
        Guns[SelectedGun].gameObject.SetActive(true);
    }

}
