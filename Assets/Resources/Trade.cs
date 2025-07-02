using UnityEditor;
using UnityEngine;

public class Trade : MonoBehaviour
{
    
    void Start()
    {
        
    }


    void Update()
    {
        
    }


    public void Create(int item1,int item2, int item3, int item4, int item5, int item6)
    {
        Time.timeScale = 0;
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        var backGround = transform.GetChild(0).GetChild(0);

        var obj1 = CreateTradeOffer(item1, item2, item3);
        var obj2 = CreateTradeOffer(item4, item5, item6);

        obj1.transform.parent = backGround.transform;
        obj2.transform.parent = backGround.transform;
    }


    public GameObject CreateTradeOffer(int item1, int item2, int item3)
    {
        var obj = Instantiate(Resources.Load<GameObject>("TradeOffer"));

        var trade = obj.GetComponent<TradeOffer>();
        trade.SetItems(item1, item2, item3);

        return obj;
    }


    public void Close()
    {
        Time.timeScale = 1; 
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        Destroy(gameObject);
    }

}
