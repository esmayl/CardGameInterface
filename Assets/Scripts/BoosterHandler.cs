using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoosterHandler : MonoBehaviour
{
    public static BoosterHandler instance;

    public RectTransform largeBooster;
    public RectTransform boosterPanel;
    public RectTransform cardHolderGroup;

    internal List<BoosterPack> allBoosters = new List<BoosterPack>();


    List<Card> cardsInScene = new List<Card>();
    Card[] boosterCards;

	void Awake ()
	{
	    instance = this;

	    foreach (GameObject ga in Resources.LoadAll<GameObject>("Boosters"))
	    {
	        if (!ga.name.Contains("Large"))
	        {
	            allBoosters.Add(ga.GetComponent<BoosterPack>());
	        }
	    }

	    foreach (RectTransform r in cardHolderGroup)
	    {
	        if (r.GetComponent<Card>())
	        {
	            cardsInScene.Add(r.GetComponent<Card>());
	        }
	    }
        DisplayBoosters(allBoosters.ToArray());
    }
	
    public void DisplayBoosters(BoosterPack[] boostersToDisplay)
    {
        if (boosterPanel.childCount > 0)
        {
            foreach (RectTransform r in boosterPanel)
            {
                Destroy(r.gameObject);
            }
        }

        foreach (BoosterPack b in boostersToDisplay)
        {
            if (b == null) break;

            GameObject tempBooster = Instantiate(b.gameObject);
            tempBooster.GetComponent<BoosterPack>().SetCost(b.cost);
            tempBooster.transform.SetParent(boosterPanel.transform,false);
        }
    }

    public BoosterPack[] GetBoosters(Element elementToFilterBy)
    {
        List<BoosterPack> foundBoosterPacks = new List<BoosterPack>();

        foreach (BoosterPack b in allBoosters)
        {
            if (b.boosterElement == elementToFilterBy)
            {
                foundBoosterPacks.Add(b);
            }
        }

        return foundBoosterPacks.ToArray();
    }

    public void SetGotCards(Card[] cardsToDisplay)
    {
        boosterCards = cardsToDisplay;

        largeBooster.FindChild("BoosterIcon").GetComponent<Image>().color =
            Card.FindColor(cardsToDisplay[0].cardElement);

        largeBooster.GetComponent<Animator>().SetTrigger("Opening");
        
        int i = 0;

        foreach (Card c in cardsInScene)
        {
            Rarity tempRarity = boosterCards[i].rarity;
            Sprite bg = CardHandler.instance.SelectCardBackground(boosterCards[i].cardElement);
            Sprite rarity = CardHandler.instance.rarityImages[tempRarity];

            c.SetAll(boosterCards[i]);
            c.SetBackground(bg);
            c.SetText();
            c.SetMediumImage();
            c.SetRarity(rarity);
            CardHandler.instance.SetCard(true, boosterCards[i].cardName);
            i++;
        }
    }

}
