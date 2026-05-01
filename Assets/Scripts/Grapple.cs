using UnityEngine;

public class Grapple : MonoBehaviour
{
    Transform firepoint;
    GameObject hand; 
    Ray ray;

    private void OnEnable()
    {
        PlayerGunManager.leftMClick += FireGrapple;

    }
    private void OnDisable()
    {
        PlayerGunManager.leftMClick -= FireGrapple;
    }
    private void Start()
    {
        firepoint = gameObject.transform.Find("FirePoint").transform;
        hand = GameObject.Find("Player").transform.Find("Shoulder").Find("Hand").gameObject;
        print(firepoint.position);
    }
    void FireGrapple()
    {
        ray = new Ray(firepoint.position, hand.transform.forward );
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            print(hit.point);
        }
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
            print("pp poo poo");
        }
    }
}
