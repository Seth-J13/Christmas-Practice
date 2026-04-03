using TMPro;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    //Variables
        //Unity Basics Variables
    TextMeshProUGUI currAmmo;
    TextMeshProUGUI gunNameText;
        //Text
    private const string startPickUpPhrase = "Press \"F\" to pick up ";

    //Unity Basics Functions
    private void Awake()
    {
        currAmmo = transform.Find("HUD").transform.Find("Gun Ammo").GetComponent<TextMeshProUGUI>();
        gunNameText = transform.Find("HUD").transform.Find("PickUpText").GetComponent<TextMeshProUGUI>();
    }
    //Methods
        //UI
    public void UpdateGunAmmo(int currMagAmmo = 0, int currBeltAmmo = 0)
    {
        currAmmo.text = currMagAmmo + "/" + currBeltAmmo;
    }
    public void UpdatePickUpText()
    {
        gunNameText.text = "";
    }
    public void UpdatePickUpText(string gunName)
    {
        gunNameText.text = startPickUpPhrase + gunName;
    }
}
