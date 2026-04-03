using UnityEngine;
using UnityEditor;
using System.Collections;

public class TestScript : MonoBehaviour
{
    GameObject playerPoint;
    GameObject firePoint;
    Camera cam;
    Transform t;
    Transform p;
    [Range(0.01f, 10.0f)]
    public float dist = 1.0f;
    [Range(-100, 100)]
    public float x = 0;
    [Range(-100, 100)]
    public float y = 0;
    [Range(-100, 100)]
    public float z = 0;
    private void Start()
    {
        cam = GameObject.Find("Player Cam").GetComponent<Camera>();
        playerPoint = GameObject.Find("Player");
        firePoint = GameObject.Find("FirePoint");
        t = firePoint.transform;
        p = playerPoint.transform;
    }
    private void FixedUpdate()
    {
        transform.position = (p.forward * dist) + t.position;
        StartCoroutine(RotateTransform());
    }
    IEnumerator RotateTransform()
    {
        yield return new WaitForEndOfFrame();
        transform.rotation = Quaternion.AngleAxis(90, Vector3.right);
        Quaternion rot = Quaternion.FromToRotation(transform.forward, cam.transform.forward);
        transform.rotation = rot;
        transform.rotation = Quaternion.Euler(transform.rotation.x + x, transform.rotation.y + y, transform.rotation.z + z);
    }
    private void OnDrawGizmos()
    {
        //Vector3 spread = new Vector3(x, y, z);
        //float xCirc = Mathf.Pow(spread.x - t.position.x, 2);
        //float yCirc = Mathf.Pow(spread.y - t.position.y, 2);
        Gizmos.DrawLine(transform.position, transform.position + -transform.up);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.forward + transform.position);
        
    }
}
