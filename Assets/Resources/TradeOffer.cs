using UnityEngine;
using UnityEngine.UI;

public class TradeOffer : MonoBehaviour
{
    ItemManager _itemManager;


    void Start()
    {
        _itemManager = FindFirstObjectByType<ItemManager>();
    }

    void Update()
    {
        if (_itemManager == null)
        {
            _itemManager = FindFirstObjectByType<ItemManager>();
        }
    }


    string tradeItem1;
    string tradeItem2;
    string tradeItem3;


    public void SetItems(int item1, int item2, int item3)
    {
        if (_itemManager == null)
        {
            _itemManager = FindFirstObjectByType<ItemManager>();
        }
        SetButtonEnable(item1, item2);
        var grid = transform;

        var firstItem = grid.GetChild(0);
        var secondItem = grid.GetChild(1);
        var thirdItem = grid.GetChild(3);

        SetImage(firstItem.gameObject, item1);
        SetImage(secondItem.gameObject, item2);
        SetImage(thirdItem.gameObject, item3);


        SetItem(item1, ref tradeItem1);
        SetItem(item2, ref tradeItem2);
        SetItem(item3, ref tradeItem3);
    }

    void SetItem(int item, ref string tradeItem)
    {
        if (item == 1)
        {
            tradeItem = "Burger";
        }
        else if (item == 2)
        {
            tradeItem = "Soda";
        }
        else
        {
            tradeItem = "Book";
        }
    }

    void SetImage(GameObject obj, int item)
    {
        Sprite image;

        if(item == 1)
        {
            image = Resources.Load<Sprite>("sprites/burger");
        }
        else if(item == 2)
        {
            image = Resources.Load<Sprite>("sprites/soda");
        }
        else
        {
            image = Resources.Load<Sprite>("sprites/book");
        }
        obj.GetComponent<Image>().sprite = image;
    }


    void SetButtonEnable(int tradeItem1, int tradeItem2)
    {
        var grid = transform;

        var button = grid.GetChild(4);

        button.GetComponent<Button>().interactable = _itemManager.HasItems(tradeItem1, tradeItem2); 

    }


    public void MakeTrade()
    {
        _itemManager.MakeTrade(tradeItem1, tradeItem2, tradeItem3);
        var grid = transform;
        var button = grid.GetChild(4);

        button.GetComponent<Button>().interactable = false;
    }


}
