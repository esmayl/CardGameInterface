using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    List<RaycastResult> hits = new List<RaycastResult>();

    Card _cardHit;

    Card cardHit
    {
        get { return _cardHit; }
        set
        {
            if (value.gameObject.name.Contains("Medium") || value.gameObject.name.Contains("Small"))
            {
                ZoomOnCard(value);
                _cardHit = value;
            }
        }
    }

    void Start()
    {
        if (CardHandler.instance)
        {
            CardHandler.instance.DisplayCards(CardHandler.instance.GetCards(Element.All));
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CheckHits();
        }
    }

    void CheckHits()
    {
        Vector3 screenPos = Input.mousePosition;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPos;

        EventSystem.current.RaycastAll(eventData, hits);

        if (hits.Count > 0)
        {

            foreach (var hit in hits)
            {
                if (hit.gameObject.CompareTag("Card"))
                {
                    cardHit = hit.gameObject.GetComponent<Card>();
                    return;
                }

                //used to show right cards per filter

                if (hit.gameObject.CompareTag("Filter") && SceneManager.GetActiveScene().name == "Collection")
                {
                    GetComponent<AudioSource>().Play();

                    CardHandler handler = CardHandler.instance;

                    switch (hit.gameObject.name)
                    {
                        case "Fire":
                            handler.DisplayCards(handler.GetCards(Element.Fire));
                            break;
                        case "Water":
                            handler.DisplayCards(handler.GetCards(Element.Water));
                            break;
                        case "Earth":
                            handler.DisplayCards(handler.GetCards(Element.Earth));
                            break;
                        case "Lightning":
                            handler.DisplayCards(handler.GetCards(Element.Lightning));
                            break;
                        case "All":
                            handler.DisplayCards(handler.GetCards(Element.All));
                            break;
                    }
                    return;
                }

                if (hit.gameObject.CompareTag("Filter") && SceneManager.GetActiveScene().name == "Store")
                {

                    GetComponent<AudioSource>().Play();

                    BoosterPack[] tempArray;

                    switch (hit.gameObject.name)
                    {
                        case "Fire":
                            tempArray = BoosterHandler.instance.GetBoosters(Element.Fire);
                            BoosterHandler.instance.DisplayBoosters(tempArray);
                            break;
                        case "Water":
                            tempArray = BoosterHandler.instance.GetBoosters(Element.Water);
                            BoosterHandler.instance.DisplayBoosters(tempArray);
                            break;
                        case "Earth":
                            tempArray = BoosterHandler.instance.GetBoosters(Element.Earth);
                            BoosterHandler.instance.DisplayBoosters(tempArray);
                            break;
                        case "Lightning":
                            tempArray = BoosterHandler.instance.GetBoosters(Element.Lightning);
                            BoosterHandler.instance.DisplayBoosters(tempArray);                            
                            break;
                        case "All":
                            tempArray = BoosterHandler.instance.allBoosters.ToArray();
                            BoosterHandler.instance.DisplayBoosters(tempArray);
                            break;
                    }
                    return;
                }

                if (hit.gameObject.CompareTag("StoreButton"))
                {
                    //make camera fade to black
                    GetComponent<AudioSource>().Play();

                    SceneManager.LoadScene("Store");
                    return;
                }

                if (hit.gameObject.CompareTag("CollectionButton"))
                {
                    GetComponent<AudioSource>().Play();

                    SceneManager.LoadScene("Collection");
                    return;
                }

                if (hit.gameObject.CompareTag("Background") && SceneManager.GetActiveScene().name == "Collection")
                {
                    CardHandler.instance.UnsetLargeCard();
                    return;
                }
                if (hit.gameObject.CompareTag("Background"))
                {
                    return;
                }
                if (hit.gameObject.CompareTag("Booster"))
                {
                    if (!hit.gameObject.GetComponent<BoosterPack>())
                    {
                        if (CardHandler.instance.money > hit.gameObject.transform.parent.GetComponent<BoosterPack>().cost)
                        {
                            CardHandler.instance.money -= hit.gameObject.transform.parent.GetComponent<BoosterPack>().cost;
                            hit.gameObject.SendMessageUpwards("GenerateRandomCards");
                        }
                    }
                    else
                    {
                        if (CardHandler.instance.money > hit.gameObject.transform.parent.GetComponent<BoosterPack>().cost)
                        {
                            CardHandler.instance.money -= hit.gameObject.GetComponent<BoosterPack>().cost;
                            hit.gameObject.GetComponent<BoosterPack>().GenerateRandomCards();
                        }
                    }
                    return;
                }
            }
        }
        else
        {
            CardHandler.instance.UnsetLargeCard();
        }
    }

    void ZoomOnCard(Card selectedCard)
    {
        Debug.Log("Zooming on "+ selectedCard.cardName);

        CardHandler.instance.SetLargeCardInfo(selectedCard);
    }
}
