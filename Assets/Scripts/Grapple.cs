using UnityEngine;

public class Grapple : MonoBehaviour
{
    //Grapple Functional Variables
    Transform firepoint;
    Ray ray;
    LineRenderer line;
    Vector3 hitPoint;
    [SerializeField][Range(0.001f, 1000f)] float distFromGrapplePoint = 10f;
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

            line.SetPosition(0, firepoint.position);
            if((player.transform.position.magnitude - hitPoint.magnitude) > distFromGrapplePoint)
            {
                print(player.transform.position.magnitude - hitPoint.magnitude);
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
