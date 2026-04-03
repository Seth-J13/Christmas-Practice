using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrossHairFireSpread : MonoBehaviour
{
    //Variables
        //HUD Hairs
    private Image upLine;
    private Image downLine;
    private Image leftLine;
    private Image rightLine;
        //RectTransforms
    private RectTransform up;
    private RectTransform down;
    private RectTransform left;
    private RectTransform right;
        //Original Positions
    private Vector2 upOrgPos;
    private Vector2 downOrgPos;
    private Vector2 leftOrgPos;
    private Vector2 rightOrgPos;
        //New Positions
    private Vector2 upNewPos;
    private Vector2 downNewPos;
    private Vector2 leftNewPos;
    private Vector2 rightNewPos;
        //Functional
    private bool canReset = false;
    private Vector2 maxSpread = Vector2.zero;
    private float decreaseRate = 0;
    private float spreadExpansionRate = 3;
    private const float magnifier = 1000000;
    private PlayerGunManager playerGunManager;
    //Unity Basics Functions
    private void Start()
    {
        //Get parent player object
        playerGunManager = transform.GetComponentInParent<PlayerGunManager>();
        //Getting HUD Elements
        upLine = transform.Find("Up Line").GetComponent<Image>();
        downLine = transform.Find("Down Line").GetComponent<Image>();
        leftLine = transform.Find("L Line").GetComponent<Image>();
        rightLine = transform.Find("R Line").GetComponent<Image>();
        //Set RectTransforms
        up = upLine.rectTransform;
        down = downLine.rectTransform;
        left = leftLine.rectTransform;
        right = rightLine.rectTransform;
        //Setting Original Positions
        upOrgPos = up.localPosition;
        downOrgPos = down.localPosition;
        leftOrgPos = left.localPosition;
        rightOrgPos = right.localPosition;
        //Setting New Positions
        upNewPos = up.localPosition;
        downNewPos = down.localPosition;
        leftNewPos = left.localPosition;
        rightNewPos = right.localPosition;
    }
    private void Update()
    {
        if (canReset)
            MoveCrossHairBack();
    }
    //Methods
    public void SetMaxSpread(Vector2  maxSpread)
    {
        this.maxSpread = maxSpread;
    }
    public Vector2 GetMaxSpread()
    {
        return maxSpread;
    }
    public Vector2 GetCurrentSpreadRange()
    {
        return new Vector2(rightNewPos.x, upNewPos.y);
    }
    public void SetDecreaseRate(float decreaseRate)
    {
        this.decreaseRate = decreaseRate * magnifier;
    }
    public void SetSpreadExpansionRate(float spreadControl)
    {
        spreadExpansionRate = spreadControl;
    }
    public void UpdateCrossHairPositions()
    {
        //Check to see if current positions are beyond maxSpread range
        bool upBool = up.localPosition.y < maxSpread.y;
        bool downBool = down.localPosition.y > -maxSpread.y;
        bool leftBool = left.localPosition.x > -maxSpread.x;
        bool rightBool = right.localPosition.x < maxSpread.x;
        //If bools are true they will move the cross hairs along their axises
        if (upBool)
            upNewPos += upNewPos/spreadExpansionRate + Vector2.up;
        if (downBool)
            downNewPos += downNewPos/spreadExpansionRate + Vector2.down;
        if(leftBool)
            leftNewPos += leftNewPos/spreadExpansionRate + Vector2.left;
        if(rightBool)
            rightNewPos += rightNewPos/spreadExpansionRate + Vector2.right;
        //If one bool fails then assume max range has been met
        if(upBool && downBool && leftBool && rightBool)
        {
            //Stops any Corutines from continuing and update cross hairs to be on new position
            StopAllCoroutines();
            up.localPosition = upNewPos;
            down.localPosition = downNewPos;
            left.localPosition = leftNewPos;
            right.localPosition = rightNewPos;
            //Start another Corutine in case this is last shot and will be able to reset
            StartCoroutine(CanResetCrossHair());
        }
        else if(playerGunManager.GetLeftClick())
        {
            StopAllCoroutines();
            StartCoroutine(CanResetCrossHair());
        }
    }
    public void MoveCrossHairBack()
    {
        //Check if current positions are extended past the original positions
        bool upBool = up.localPosition.y > upOrgPos.y;
        bool downBool = down.localPosition.y < downOrgPos.y;
        bool leftBool = left.localPosition.x < leftOrgPos.x;
        bool rightBool = right.localPosition.x > rightOrgPos.x;
        //tempTime is to slow and smooth the rate of cross hairs moving back
        float tempTime = decreaseRate * Time.deltaTime;
        //If true then update the new position to move towards the original positions
        if (upBool)
            upNewPos -= (new Vector2(0, upNewPos.y / tempTime) + Vector2.up);
        if (downBool)
            downNewPos -= (new Vector2(0, downNewPos.y / tempTime) + Vector2.down);
        if (leftBool)
            leftNewPos -= (new Vector2(leftNewPos.x / tempTime, leftNewPos.y) + Vector2.left);
        if (rightBool)
            rightNewPos -= (new Vector2(rightNewPos.x / tempTime, rightNewPos.y) + Vector2.right);
        //If any bools fail cross hairs are at their original position with no need to continue to move back
        if (upBool && downBool && leftBool && rightBool)
        {
            //Update cross hair positions with new positions
            up.localPosition = upNewPos;
            down.localPosition = downNewPos;
            left.localPosition = leftNewPos;
            right.localPosition = rightNewPos;
        }
        else
            canReset = false;
    }
    //IEnumerators
    IEnumerator CanResetCrossHair()
    {
        canReset = false;
        yield return new WaitForSeconds(1);
        canReset = true;
    }
}
