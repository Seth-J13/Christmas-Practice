using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
            return instance;
        }
    }
    //Variables
        //Player
    GameObject player;
        
    //Unity Basics
    private void OnEnable()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        Instance.player = Resources.Load<GameObject>("Prefabs/Critical Assets/Player");
        if (GameObject.Find("Player") == null)
            Instance.player = Instantiate(Instance.player);
        Instance.player.transform.position = Vector3.up;
        Instance.player.name = "Player";
  
    }
    //Methods
}
