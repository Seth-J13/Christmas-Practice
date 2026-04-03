using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGunManager : MonoBehaviour
{
    //Variables
        //Gun and Player
    private GunBase currentGun;
    private LinkedListNode<GameObject> currGunNode;
    private PlayerUIManager playerUI;
        //Functionality
    private LinkedList<GameObject> gunList;
    private GunBase potentialGun;
    private bool hasGun = false;
    private bool gunShopping = false;
    private bool leftClick = false;
    private float changeInTime = 0;
    //Unity Basics
    private void Start()
    {
        gunList = new LinkedList<GameObject>();
        playerUI = transform.GetComponent<PlayerUIManager>();
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Gun"))
        {
            gunShopping = true;
            potentialGun = other.GetComponent<GunBase>();
            playerUI.UpdatePickUpText(potentialGun.GetGunName());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        gunShopping = false;
        potentialGun = null;
        playerUI.UpdatePickUpText();
    }
    //Methods
    public void SetMainGun()
    {
        if(!hasGun)
        {
            currentGun = potentialGun;
            potentialGun = null;
            currGunNode = new LinkedListNode<GameObject>(currentGun.gameObject);
            gunList.AddFirst(currGunNode);
            hasGun = true;
            PickUpGun();
        }
        else
        {
            ///Add inventory management when picking up a new gun when player already has one
            //if player has less than 3 weapons then just pick gun up
            //otherwise drop current gun
            if(gunList.Count < 3 && potentialGun != null)
            {
                gunList.AddLast(new LinkedListNode<GameObject>(potentialGun.gameObject));
                potentialGun = null;
                SwitchGuns();
                PickUpGun();
            }
            else
            {
                //DROP GUN CODE
            }
        }
        playerUI.UpdateGunAmmo(currentGun.GetMaxMagAmmo(), currentGun.GetMaxBeltAmmo());
    }
    private void SwitchGuns()
    {
        ///Set up switching guns mechanic. 
        ///Have gun non used guns disabled and only held gun enabled
        currentGun.gameObject.SetActive(false);
        currGunNode = currGunNode.Next;
        currentGun = currGunNode.Value.GetComponent<GunBase>();
        currentGun.gameObject.SetActive(true);

        //TEMP
        //currentGun.gameObject.SetActive(false);
        //foreach (LinkedListNode<GameObject> gun in gunList)
        //{
        //    if (gun.Value == potentialGun.gameObject)
        //    {
        //        currGunNode = gun;
        //        currentGun = gun.Value.GetComponent<GunBase>();
        //        currentGun.gameObject.SetActive(true);
        //        //[1911 (inactive), Bullfrog (active)]
        //    }
        //}
    }
    public void PickUpGun()
    {
        Transform gun = currentGun.transform;
        Transform shoulder = transform.Find("Shoulder");
        Transform hand = shoulder.GetChild(0);
        //Disable trigger and rigidbody physics
        gun.GetComponent<Collider>().enabled = false;
        gun.GetComponent<Rigidbody>().isKinematic = true;
        gun.Find("Collider").GetComponent<Collider>().enabled = false;
        //Make gun child of the hand in 
        gun.parent = hand;
        //Set position and rotation of gun
        gun.position = hand.position;
        gun.rotation = hand.rotation;
    }
    public void UpdateVerticalLookPosition(float lookSpeedX, float lookSpeedY, float lookAngleX, float lookAngleY)
    {
        if(hasGun)
        {
            float shoulderMovement = lookAngleY * -lookSpeedY * Time.deltaTime;
            if (shoulderMovement > 85)
                shoulderMovement = 85;
            else if (shoulderMovement < -90)
                shoulderMovement = -90;
            //Change gun rotation
            transform.Find("Shoulder").transform.rotation = Quaternion.Euler(shoulderMovement, lookAngleX * lookSpeedX * Time.deltaTime, 0);
        }
    }
    public bool GetLeftClick() => leftClick;
    //Actions
    void OnReload(InputValue v)
    {
        if (hasGun && !currentGun.GetReloading())
        {
            currentGun.SetFiring(false);
            currentGun.StartReload();
        }
    }
    void OnFire(InputValue v)
    {
        //Is player pressing or releasing the button
        leftClick = v.Get<float>() == 1;
        if (hasGun && currentGun.GetAllowTriggerFinger())
        {
            float lastShot = Time.time - changeInTime;
            if (lastShot > currentGun.GetFireRate() * 0.75f)
                currentGun.SetFiring(leftClick, lastShot);
            else
                currentGun.SetFiring(leftClick);
        }
        else if(hasGun)
            currentGun.SetFiring(leftClick);
        changeInTime = !leftClick ? Time.time : changeInTime;
    }
    void OnAimDownSights(InputValue v)
    {
        if(hasGun)
            currentGun.SetAimDownSights(v.Get<float>() == 1 ? true : false);
    }
    void OnInteract(InputValue v)
    {
        if(v.Get<float>() == 1 && gunShopping)
        {
            SetMainGun();
            playerUI.UpdatePickUpText();
        }
    }
    void OnNext(InputValue v)
    {
        print("OnNext: " + v.Get<float>());
    }
    void OnPrevious(InputValue v)
    {
        print("OnPrevious: " + v.Get<float>());
    }
}
