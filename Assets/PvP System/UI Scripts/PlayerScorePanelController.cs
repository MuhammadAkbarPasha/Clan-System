using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScorePanelController : MonoBehaviour
{


    public RealTimePVP realTimePVP;
    public GameObject PointsPanelParent;
    public GameObject PointPanel;
    public List<GameObject> Points = new List<GameObject>();

    
    public void SetUpPoints(int totalPoints=0) 
    {
        Points.Clear();
        for (int i=0;i<totalPoints;i++) 
        {
            Button button = Instantiate(PointPanel, PointsPanelParent.transform).GetComponent<Button>();
            button.onClick.AddListener(() => 
            { 
                realTimePVP.IncreasePointsRPCCaller(1);
              
            });
            Points.Add(button.gameObject); 
            
        } 
    }
    public void PlayerFoundDifference(int n=0)
     {
        Debug.Log("coming here in UI too "+n);
        Points[n].transform.GetChild(0).gameObject.GetComponent<Image>().enabled=true;

    }

    public void PointsPanelParentAfterGameEnd() 
    {
        foreach ( Transform t  in PointsPanelParent.transform) 
        {
            Destroy(t.gameObject);
        }
    }




}
