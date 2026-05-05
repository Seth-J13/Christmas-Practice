using UnityEngine;

public class Grapple : MonoBehaviour
{
    //Grapple Functional Variables
    Transform firepoint;
    Ray ray;
    LineRenderer line;
    Vector3 hitPoint;
    [SerializeField] const float maxDistGrapplePoint = 10f;
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
    private void Update()
    {
        //if line is active then update the lines position
        if (line.enabled) 
        {

            //Don't use magnitude. It doesn't work
            print(player.transform.position.magnitude + " player | " + (hitPoint + player.transform.position).magnitude + " hit");
            line.SetPosition(0, firepoint.position);
            //Get dist between hitPoint and player position
            Vector3 playerDistFromGrapple = hitPoint - player.transform.position;
            //Get the abs of playerDistFromGrapple
            playerDistFromGrapple = new Vector3(Mathf.Abs(playerDistFromGrapple.x), Mathf.Abs(playerDistFromGrapple.y), Mathf.Abs(playerDistFromGrapple.z));
            if(playerDistFromGrapple.x > maxDistGrapplePoint || playerDistFromGrapple.y > maxDistGrapplePoint || playerDistFromGrapple.z > maxDistGrapplePoint)
            {
                playerBody.AddForce(hitPoint, ForceMode.Force);
            }
        }
    }
    void FireGrapple()
    {
        ray = new Ray(firepoint.position, hand.transform.forward );
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            line.enabled = true;
            hitPoint = hit.point;
            line.SetPosition(1, hitPoint);
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
            Transform t = gameObject.transform.Find("FirePoint").transform;
            Gizmos.DrawRay(t.position, GameObject.Find("Player").transform.Find("Shoulder").Find("Hand").forward);
        }
        catch
        {
        }
    }
}
