using UnityEngine;

public class Grapple : MonoBehaviour
{
    //Grapple Functional Variables
    Transform firepoint;
    Ray ray;
    LineRenderer line;
    Vector3 hitPoint = Vector3.zero;
    static float maxHangingPoint = 3.5f;
    static float maxGrappleDist = 50f;
    static float grappleSpeed = 150;
    Vector3 distFromPlayer;
    Vector3 playerDistFromGrapple;
    //Player variables
    GameObject player;
    GameObject hand;
    Rigidbody playerBody;
    

    private void OnEnable()
    {
        PlayerGunManager.leftMClickDown += FireGrapple;
        PlayerGunManager.leftMClickUp += DisableGrapple;
    }
    private void OnDisable()
    {
        PlayerGunManager.leftMClickDown -= FireGrapple;
        PlayerGunManager.leftMClickUp -= DisableGrapple;
    }
    private void Start()
    {
        //getting the grapples firepoint from which the line comes from
        firepoint = gameObject.transform.Find("FirePoint").transform;
        //getting the player holding the grapple
        player = GameObject.Find("Player");
        playerBody = player.GetComponent<Rigidbody>();
        //getting the forward direction from the players hand obj
        hand = player.transform.Find("Shoulder").Find("Hand").gameObject;

        line = firepoint.transform.GetComponent<LineRenderer>();
        line.enabled = false;
    }

    private void FixedUpdate()
    {
        //if line is active then update the lines position
        if (line.enabled)
        {
            //Don't use magnitude. It doesn't work
            print(player.transform.position.magnitude + " player | " + (hitPoint + player.transform.position).magnitude + " hit");
            line.SetPosition(0, firepoint.position);
            //Get the abs of playerDistFromGrapple
            if (playerDistFromGrapple.x > maxHangingPoint || playerDistFromGrapple.y > maxHangingPoint || playerDistFromGrapple.z > maxHangingPoint)
                playerBody.AddForce((hitPoint - player.transform.position) * Time.deltaTime * grappleSpeed, ForceMode.Acceleration);

        }
    }
    
    void FireGrapple()
    {
        ray = new Ray(firepoint.position, hand.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hitPoint = hit.point;
            //Get dist between hitPoint and player position
            distFromPlayer = hitPoint - player.transform.position;
            playerDistFromGrapple = new Vector3(Mathf.Abs(distFromPlayer.x), Mathf.Abs(distFromPlayer.y), Mathf.Abs(distFromPlayer.z));
            if (Mathf.Pow(distFromPlayer.x, 2) + Mathf.Pow(distFromPlayer.y, 2) + Mathf.Pow(distFromPlayer.z, 2) < Mathf.Pow(maxGrappleDist, 2))
            {
                line.enabled = true;
                line.SetPosition(1, hitPoint);
            }
        }
    }
    private void DisableGrapple()
    {
        line.enabled = false;        
    }

    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.DrawLine(firepoint.position, hitPoint);
            Gizmos.DrawWireSphere(player.transform.position, maxGrappleDist);
            Gizmos.DrawWireSphere(hitPoint, 3);
            //Transform t = gameObject.transform.Find("FirePoint").transform;
            //Gizmos.DrawRay(t.position, GameObject.Find("Player").transform.Find("Shoulder").Find("Hand").forward);
        }
        catch
        {
        }
    }
}
