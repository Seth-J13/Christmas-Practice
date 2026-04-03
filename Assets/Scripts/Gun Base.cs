using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    //Variables
    //Player
    private CrossHairFireSpread crossHair;
    private PlayerUIManager playerUI;
    //Gun
    private Transform firePoint;
    [SerializeField] private string gunName = null;
    //Ammo
    private GameObject bullet;
    private Queue<GameObject> bullets = new Queue<GameObject>();
    private Queue<BulletManager> bulletManagers = new Queue<BulletManager>();
    [SerializeField] private int maxMagAmmo = 25;
    [SerializeField] private int maxBeltAmmo = 255;
    private int currMagAmmo = 0;
    private int currBeltAmmo = 0;
    //Reloading 
    [SerializeField] private float reloadTime = 1.5f;
    private bool reloading = false;
    //CrossHair Related
    [SerializeField] Vector2 spread = Vector2.zero;
    [SerializeField][Range(1, 15)] private float spreadDecreaseRate = 1.0f;
    [SerializeField][Range(1, 10)] private float spreadExpansionRate = 3;
    //Fire type
    [SerializeField][Range(0.001f, 1)] private float fireRate = 1.5f;
    [SerializeField] private bool allowTriggerFinger = false;
    [SerializeField] private bool hitscan = false;
    //Functional
    private bool firing = false;
    private bool canFireAgain = true;
    private bool triggerWaitOver = true;
    private bool aimingDownSights = false;
    //Unity Basics Functions
    private void Awake()
    {
        if (gunName == null)
        {
            print("Error: No gun name");
            SetMaxBeltAmmo(0);
            SetMaxMagAmmo(0);
            SetFireRate(0);
            SetReloadTime(0);
        }
    }
    private void Start()
    {
        GameObject player = GameObject.Find("Player");

        //Set currMagAmmo & currBeltAmmo
        currMagAmmo = maxMagAmmo;
        currBeltAmmo = maxBeltAmmo;

        //Get player component for UI
        playerUI = player.GetComponent<PlayerUIManager>();

        //Get bullet prefab
        bullet = Resources.Load<GameObject>("Prefabs/Guns/Bullets/oldBullet");

        //firePoint GameObject transform
        firePoint = transform.Find("FirePoint").transform;

        //Player cross hair
        crossHair = player.transform.Find("Cross Hair").GetComponent<CrossHairFireSpread>();
        crossHair.SetDecreaseRate(spreadDecreaseRate);
        crossHair.SetMaxSpread(spread);
        crossHair.SetSpreadExpansionRate(spreadExpansionRate);

        //Spawn Bullets
        GameObject tempBullet;
        BulletManager bm;
        for (int i = 0; i < maxMagAmmo * 2; i++)
        {
            //Spawn bullet GameObject
            tempBullet = Instantiate(bullet);
            tempBullet.name = "bullet " + i; //give bullet a identifiable name
            bm = tempBullet.GetComponent<BulletManager>(); //Get bullet manager from the bullet
            bm.SetBulletNumber(i); //Set bullet id
            bm.SetBulletSpread(spread, crossHair);//Set bullet spread and give reference to the crossHair
            tempBullet.transform.position = new Vector3(i * 3, -20, 0);//place bullet seperate from each other for debugging
            //Add GameObject and bullet manager to respective queues for object pooling
            bullets.Enqueue(tempBullet);
            bulletManagers.Enqueue(bm);
        }
    }
    private void Update()
    {
        if (firing && canFireAgain && !reloading && currMagAmmo > 0)
        {
            if (!allowTriggerFinger)
                Fire();
            else if (allowTriggerFinger && triggerWaitOver)
            {
                StartCoroutine(TriggerWait());
                Fire();
            }

        }
    }
    //Methods
    //Getters and Setters
    //Max Mag Ammo
    public void SetMaxMagAmmo(int ammoAmount)
    {
        this.maxMagAmmo = ammoAmount;
    }
    public int GetMaxMagAmmo() => maxMagAmmo;
    //Max Belt Ammo
    public void SetMaxBeltAmmo(int maxBeltAmmo)
    {
        this.maxBeltAmmo = maxBeltAmmo;
    }
    public int GetMaxBeltAmmo() => maxBeltAmmo;
    //Fire Rate
    public void SetFireRate(float fireRate)
    {
        this.fireRate = fireRate;
    }
    public float GetFireRate() => fireRate;
    
    //Hitscan
    public bool GetHitscan() => hitscan;
    
    //Reload Time
    public void SetReloadTime(float reloadTime)
    {
        this.reloadTime = reloadTime;
    }
    //Reloading bool
    public bool GetReloading() => reloading;
    //Firing bool
    public void SetFiring(bool b, float lastShot = 0f)
    {
        if (lastShot != 0f)
        {
            canFireAgain = true;
        }
        firing = b;
    }
    public bool GetFiring() => firing;
    
    //Trigger Finger bool
    public bool GetAllowTriggerFinger() => allowTriggerFinger;
    
    //AimingDownSights bool
    public void SetAimDownSights(bool b)
    {
        aimingDownSights = b;
    }
    public bool GetAimDownSights() => aimingDownSights;
    
    //Gun Name
    public string GetGunName() => gunName;
    
    //Actions
        //Fire
    private void Fire()
    {
        canFireAgain = false;
        StartCoroutine(FireBullet());
    }
        //AimDownSights
    private void AimDownSights()
    {

    }
    public void StartReload()
    {
        reloading = true;
        StartCoroutine(ReloadGun());
    }
    //IEnumerators
    IEnumerator FireBullet()
    {
        if(hitscan)
        {
            //Figure out hitscan
        }
        else
        {
        //Get the next bullet GameObject and subsequent Bullet Manager
            GameObject newBullet = bullets.Dequeue();
            BulletManager bm = bulletManagers.Dequeue();
        //Move bullet to gun firepoint position
            newBullet.transform.position = firePoint.position;
            bm.FireBullet(); //methods to add force to rigid body
        //puts bullet and manager back into queue
            bullets.Enqueue(newBullet);
            bulletManagers.Enqueue(bm);
        //Ammo tracking
            currMagAmmo--;
            playerUI.UpdateGunAmmo(currMagAmmo, currBeltAmmo);
        }
        crossHair.UpdateCrossHairPositions();
        yield return new WaitForSeconds(fireRate);
        canFireAgain = true;
    }
    IEnumerator TriggerWait()
    {
        triggerWaitOver = false;
        yield return new WaitForSeconds(fireRate * 0.75f);
        triggerWaitOver = true;
    }
    IEnumerator ReloadGun()
    {
        yield return new WaitForSeconds(reloadTime);
        if(currBeltAmmo > maxMagAmmo)
        {
            currBeltAmmo -= maxMagAmmo - currMagAmmo;
            currMagAmmo = maxMagAmmo;
        }
        else if(currBeltAmmo > 0)
        {
            currBeltAmmo = currMagAmmo + currBeltAmmo > maxMagAmmo ? currBeltAmmo - (maxMagAmmo - currMagAmmo) : currBeltAmmo;
            currMagAmmo = currMagAmmo + currBeltAmmo > maxMagAmmo ? currMagAmmo + (maxMagAmmo - currMagAmmo) : currMagAmmo + currBeltAmmo;
            if (currMagAmmo <= maxMagAmmo)
                currBeltAmmo = 0;
        }
        if(currBeltAmmo < 0)
            playerUI.UpdateGunAmmo(currMagAmmo);
        else
            playerUI.UpdateGunAmmo(currMagAmmo, currBeltAmmo);
        reloading = false;
    }
}
