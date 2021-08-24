using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GiftPanel : MonoBehaviour
{
    
    
    public Text GiftMessage;
    public void SetMessage(string msg)
    {
        GiftMessage.text = msg; 
    }

}
