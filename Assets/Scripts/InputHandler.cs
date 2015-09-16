using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{
    public LayerMask mask;

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

                #region Collection code

                if (hit.gameObject.CompareTag("Filter") && Application.loadedLevelName == "Collection")


                {
                    switch (hit.gameObject.name)
                    {
                        case "Fire":
                            CardHandler.instance.DisplayCards(CardHandler.instance.GetCards(Element.Fire));
                            break;
                        case "Water":
                            CardHandler.instance.DisplayCards(CardHandler.instance.GetCards(Element.Water));
                            break;
                        case "Earth":
                            CardHandler.instance.DisplayCards(CardHandler.instance.GetCards(Element.Earth));
                            break;
                        case "Lightning":
                            CardHandler.instance.DisplayCards(CardHandler.instance.GetCards(Element.Lightning));
                            break;
                        case "All":
                            List<Card> tempList = new List<Card>();
                            foreach (KeyValuePair<string, Card> c in CardHandler.instance.cardDictionary)
                            {
                                tempList.Add(c.Value);
                            }
                            CardHandler.instance.DisplayCards(tempList.ToArray());
                            break;
                    }
                    return;
                }
                #endregion

                if (hit.gameObject.CompareTag("Filter") && Application.loadedLevelName == "Store")
                {
                    switch (hit.gameObject.name)
                    {
                        case "Fire":
                            //show all fire boosters

                            break;
                        case "Water":

                            break;
                        case "Earth":

                            break;
                        case "Lightning":
                            
                            break;
                        case "All":

                            break;
                    }
                    return;
                }

                if (hit.gameObject.CompareTag("StoreButton"))
                {
                    //make camera fade to black

                    Application.LoadLevel("Store");
                    return;
                }

                if (hit.gameObject.CompareTag("CollectionButton"))
                {
                    Application.LoadLevel("Collection");
                    return;
                }

                if (hit.gameObject.CompareTag("Background") && Application.loadedLevelName == "Collection")
                {
                    CardHandler.instance.UnsetLargeCard();
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
