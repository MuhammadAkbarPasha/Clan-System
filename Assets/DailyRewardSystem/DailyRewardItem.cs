using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardItem : MonoBehaviour
{
    public Image bgImage;
    public TextMeshProUGUI coinText;
    public GameObject completedObj;
    public GameObject inCompleteObj;

    public void SetActive(bool isActive,bool isClaimed)
    {
        //Debug.LogError("DRI :" + isActive + " : " + isClaimed);
        if (isActive)
        {
            bgImage.sprite = DailyRewardSystem.Instance.collectBgSprite;
            inCompleteObj.SetActive(true);
            completedObj.SetActive(false);
            coinText.color = Color.white;
        }
        else
        {
            coinText.color = DailyRewardSystem.Instance.textBlueColor;
            bgImage.sprite = DailyRewardSystem.Instance.normalBgSprite;
            if (isClaimed)
            {
                completedObj.SetActive(true);
                inCompleteObj.SetActive(false);
            }
            else
            {
                completedObj.SetActive(false);
                inCompleteObj.SetActive(true);
            }
        }
    }
}
