using UnityEngine;

public class FeetManager : MonoBehaviour
{
    //Variables
    private GameObject parent;
    [SerializeField]
    private string[] names;
    //Unity Basics
    private void Awake()
    {
        parent = transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        foreach(string name in names)
        {
            if(other.name.StartsWith(name))
            {
                parent.GetComponent<PlayerMovement>().Grouded();
                break;
            }
        }
    }
}
