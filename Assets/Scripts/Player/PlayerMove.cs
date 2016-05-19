using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMove : NetworkBehaviour
{
    [SerializeField] float speed = 10;
    [SerializeField] int shotDamage = 100;
    [SerializeField] float shotDelay = 0.5f;

    private Rigidbody myBody;
    private float elapsedTime = 0f;

    public override void OnStartLocalPlayer()
    {
        if (isLocalPlayer)
        {
            GetComponent<NetworkIdentity>().localPlayerAuthority = true;
            GetComponent<MeshRenderer>().material.color = Color.red;
        }
    }

    [Command]
    void CmdFire()
    {
        print("Atirando");

        // This [Command] code is run on the server!

        //Ray ray = new Ray(transform.position, transform.forward);
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100) && elapsedTime >= shotDelay)
        {
            if (hit.transform.tag == "Enemy")
                //print("Acertou");
                hit.transform.GetComponent<Combat>().CmdTakeDamage(shotDamage);
        }

        //// create the bullet object locally
        //var bullet = (GameObject)Instantiate(
        //     bulletPrefab,
        //     transform.position + transform.forward,
        //     Quaternion.identity);

        //bullet.GetComponent<Rigidbody>().velocity = transform.forward * 6;

        //// spawn the bullet on the clients
        //NetworkServer.Spawn(bullet);

        //// when the bullet is destroyed on the server it will automaticaly be destroyed on clients
        //Destroy(bullet, 2.0f);
    }

    void Start()
    {
        #if UNITY_ANDROID
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
                GameObject.Find("MobileSingleStickControl").SetActive(true);
        #endif

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR
                GameObject.Find("MobileSingleStickControl").SetActive(false);
        #endif

        myBody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        elapsedTime += Time.deltaTime;

        if (!isLocalPlayer)
            return;

        #if UNITY_ANDROID
        Vector2 moveVector = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal"), CrossPlatformInputManager.GetAxis("Vertical")) * speed;

        myBody.velocity = new Vector3(moveVector.x, 0, moveVector.y);

        if (CrossPlatformInputManager.GetButton("Shoot") && elapsedTime >= shotDelay)
        {
            CmdFire();
            elapsedTime = 0f;
        }
        #endif

        #if UNITY_STANDALONE_WIN || UNITY_EDITOR

        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");

        GetComponent<Rigidbody>().velocity = new Vector3(x, 0, z) * speed;

        if (Input.GetMouseButton(0) && elapsedTime >= shotDelay)
        {
            // Command function is called from the client, but invoked on the server
            CmdFire();
            elapsedTime = 0f;
        }
        #endif
    }
}