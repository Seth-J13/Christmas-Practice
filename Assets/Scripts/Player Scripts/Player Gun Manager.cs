using System;
using System.Collections;
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


    public event Action<GunBase> SwitchedGuns;
    public delegate void LeftMClick();
    public static LeftMClick leftMClick;
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
        StartCoroutine(WaitAFrame());
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
            //Add inventory management when picking up a new gun when player already has one
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
    private void SwitchGuns(bool switchToNextGun = true)
    {
        ///Set up switching guns mechanic. 
        ///Have non used guns disabled and only hold gun enabled
        if (currentGun.GetReloading())
            return;
        currentGun.gameObject.SetActive(false);
        if(switchToNextGun) //switch to next gun
            currGunNode = currGunNode.Next != null ? currGunNode.Next : currGunNode;
        else //switch to the prior gun
            currGunNode = currGunNode.Previous != null ? currGunNode.Previous : currGunNode;
        //Set the current gun to the new active gun
        currentGun = currGunNode.Value.GetComponent<GunBase>();
        currentGun.gameObject.SetActive(true);
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
        print("Picked Gun Up");
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
        leftMClick?.Invoke();
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
        if (currGunNode.Next == null)
            return;
        SwitchGuns();
        StartCoroutine(WaitAFrame());
    }
    void OnPrevious(InputValue v)
    {
        if (currGunNode.Previous == null)
            return;
        SwitchGuns(false);
        StartCoroutine(WaitAFrame());
    }

    IEnumerator WaitAFrame()
    {
        yield return new WaitForEndOfFrame();
        SwitchedGuns.Invoke(currentGun);

    }
}
