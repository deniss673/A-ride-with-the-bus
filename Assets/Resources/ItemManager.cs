using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{

    private Coroutine _damage = null;
    private Coroutine _bookCoroutine = null;


    private int burgerCount=5;
    private int bookCount=5;
    private int sodaCount=5;


    private TextMeshProUGUI _burger;
    private TextMeshProUGUI _soda;
    private TextMeshProUGUI _book;

    void Start()
    {
        _burger = GameObject.FindGameObjectWithTag("burger").GetComponent<TextMeshProUGUI>();
        _soda = GameObject.FindGameObjectWithTag("soda").GetComponent<TextMeshProUGUI>();
        _book = GameObject.FindGameObjectWithTag("book").GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        SetNumbers();
        SetHealth();
    }

    public void GetHeal()
    {
        if (burgerCount == 0)
        {
            return;
        }
        burgerCount--;
        var obj = GameObject.FindFirstObjectByType<Player>();
        obj.GetHeal();
    }

    public void GetDamage()
    {
        if(_damage == null && sodaCount>0)
        {
            sodaCount--;
            _damage = StartCoroutine(SetDamage());
        }
    }


    IEnumerator SetDamage()
    {
        var obj = GameObject.FindFirstObjectByType<Player>();

        obj.SetDamage(true);

        yield return new WaitForSecondsRealtime(15f);

        obj.SetDamage(false);
        _damage = null;
    }


    public void GetItem(string itemName)
    {
        if(itemName == "Burger")
        {
            burgerCount++;
        }
        if(itemName == "Soda")
        {
            sodaCount++;
        }
        if(itemName == "Book")
        {
            bookCount++;
        }
    }


    private void SetNumbers()
    {
        _book.text = bookCount.ToString();
        _burger.text = burgerCount.ToString();
        _soda.text = sodaCount.ToString();
    }

    private void SetHealth()
    {
        var player = FindFirstObjectByType<Player>();
        var health = player.GetHealth();

        var healthBar = GameObject.FindGameObjectWithTag("health").GetComponent<Image>();

        healthBar.fillAmount = health / 100f;
    }


    public void DropItem()
    {

        var random = Random.Range(0, 100);


        if(random < 30)
        {
            GetItem("Burger");
        }
        else if (random < 70)
        {
            GetItem("Soda");
        }
        else
        {
            GetItem("Book");
        }
    }



    private List<NpcScript> _npcs = new List<NpcScript>();



    public void UseBook()
    {
        if(_bookCoroutine == null && bookCount>0)
        {
            bookCount--;
            _bookCoroutine = StartCoroutine(BookCoroutine());
        }
    }

    IEnumerator BookCoroutine()
    {
        SetNPCs();
        yield return new WaitForSecondsRealtime(15f);
        ResetNpcs();
        _bookCoroutine = null;

    }



    private void SetNPCs()
    {
        var npcs = GameObject.FindObjectsByType<NpcScript>(FindObjectsSortMode.None);

        foreach (var npc in npcs)
        {
            if (npc.GotBook())
            {
                _npcs.Add(npc);
            }
        }
    }


    private void ResetNpcs()
    {
        foreach(var npc in _npcs)
        {
            npc.SetPassiveAfterBook();
        }
    }

    int GetBurgers()
    {
        return burgerCount;
    }

    int GetBooks()
    {
        return bookCount;
    }

    int GetSoda()
    {
        return sodaCount;
    }

    public bool HasItems(int item1, int item2)
    {
        return GetItem(item1) > 0
            && GetItem(item2) > 0;
    }

    int GetItem(int item)
    {
        if(item == 1)
        {
            return GetBurgers();
        }
        else if(item == 2)
        {
            return GetSoda();
        }
        else
        {
            return GetBooks();
        }
    }

    void ModifyItems(string item, bool add)
    {
        if (item == "Burger")
        {
            burgerCount += (add ? 1 : -1);
        }
        else if (item == "Soda")
        {
            sodaCount += (add ? 1 : -1);
        }
        else
        {
            bookCount += (add ? 1 : -1);
        }
    }

    public void MakeTrade(string item1, string item2, string item3) {
        ModifyItems(item1, false);
        ModifyItems(item2, false);
        ModifyItems(item3, true);
    }

}
