using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitedTimeOfferProduct : MonoBehaviour
{
    [SerializeField]
    string productId;
    public Text LevelRequired;
    public Text Rewards;
    public Text OriginalPrice;
    public Text DiscountedPrice;
    public Text timeLeftText;
    public Button button;
    public int timeLeft;
    public List<StoreSystem.Item> items = new List<StoreSystem.Item>();
    public void SetUi(string levelRequired, List<StoreSystem.Item> items, string originalPrice, string discountedPrice)
    {

        this.items = items;
        this.LevelRequired.text = levelRequired;
        this.OriginalPrice.text = originalPrice;
        this.DiscountedPrice.text = discountedPrice;
    }
    public void SetUi(List<StoreSystem.Item> items, string originalPrice, string discountedPrice)
    {

        this.items = items;
        this.OriginalPrice.text = originalPrice;
        this.DiscountedPrice.text = discountedPrice;
    }
    public void StartTimer(int time)
    {
        timeLeft = time;
        InvokeRepeating(nameof(DeductTimer), 1, 1);
    }
    public void SetUpButtonOnClick(string id)
    {
        productId = id;
        button.onClick.AddListener(() =>
        {
            StoreSystemController.Instance.OnClickPurchaseBtn(productId);
        });
    }
    public void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }
    public void DeductTimer()
    {
        timeLeft--;
        if (timeLeft <= 0)
        {
            CancelInvoke(nameof(DeductTimer));
            Destroy(this.gameObject, 1f);
        }
    }
}
