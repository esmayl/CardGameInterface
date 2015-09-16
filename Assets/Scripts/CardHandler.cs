using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

public class CardHandler : MonoBehaviour
{
    public static CardHandler instance;


    public Rarity[] rarities = new Rarity[4];
    public Sprite[] raritySprites = new Sprite[4];
    public Sprite[] backgroundSprites = new Sprite[4];
    public GameObject smallCardTemplate;
    public GameObject mediumCardTemplate;

    public GameObject largeCardInScene;
    
    public RectTransform mainPanel;
    public RectTransform sidePanel;

    FileStream saveFile;
    XMLTemplate cardDoc = new XMLTemplate();

    public Dictionary<string, Card> cardDictionary = new Dictionary<string, Card>();

    Dictionary<Rarity, Sprite> rarityImages = new Dictionary<Rarity, Sprite>();
    List<GameObject> instanceListSidePanel = new List<GameObject>(); 
    List<GameObject> instanceListMainPanel = new List<GameObject>(); 

    void Awake()
    {
        instance = this;
        LoadAllCards();

        if (Application.loadedLevelName == "Store") { return; }

        int i = 0;
        foreach (Sprite s in raritySprites)
        {
            rarityImages.Add(rarities[i],s);
            i++;
        }

       List<Card> tempCards =new List<Card>();
       foreach (KeyValuePair<string,Card> pair in cardDictionary)
       {
            tempCards.Add(pair.Value);   
       }
       DisplayCards(tempCards.ToArray());

       #region TestCode
       //XMLTemplate cardDoc = new XMLTemplate();
       //cardDoc.cards = new List<CardTemplate>();
       //
       //for (int i = 0; i < 10; i++) 
       //{ 
       //    CardTemplate card = new CardTemplate();
       //
       //    if (i%2 == 1)
       //    {
       //        card.cardName = "Card" + i;
       //        card.cardElement = Element.Earth;
       //        card.cardDescription = "fdfgjdfdfhbdg";
       //        card.cost = "1,25";
       //        card.rarity = Rarity.Common;
       //        card.iconLargeName = "LargeIcon";
       //        card.iconMediumName = "MediumIcon";
       //        card.iconSmallName = "SmallIcon";
       //    }
       //    else
       //    {
       //        card.cardName = "Card" + i;
       //        card.cardElement = Element.Fire;
       //        card.cardDescription = "Lots of text ";
       //        card.cost = "5,15";
       //        card.rarity = Rarity.Uncommon;
       //        card.iconLargeName = "LargeIcon";
       //        card.iconMediumName = "MediumIcon";
       //        card.iconSmallName = "SmallIcon";
       //    }
       //
       //    cardDoc.cards.Add(card);
       //}
       //
       //FileStream cardDocStream = new FileStream("Assets/Data/CardData/cards.xml", FileMode.Create);
       //
       //if (cardDocStream.CanWrite)
       //{
       //    XmlSerializer serializer = new XmlSerializer(typeof(XMLTemplate));
       //    serializer.Serialize(cardDocStream, cardDoc);
       //}
#endregion
    }

    public void SetLargeCardInfo(Card c)
    {
        largeCardInScene.GetComponent<Card>().SetAll(c);

        if (!largeCardInScene.GetComponent<Animator>().GetBool("SlideIn"))
        {
            largeCardInScene.GetComponent<Animator>().SetBool("SlideIn", true);
        }
        else
        {
            largeCardInScene.GetComponent<Animator>().SetTrigger("Reload");
        }

        largeCardInScene.GetComponent<Card>().SetRarity(rarityImages[c.rarity]);
        largeCardInScene.GetComponent<Card>().SetSmallImage();
        largeCardInScene.GetComponent<Card>().SetText();
        largeCardInScene.GetComponent<Card>().SetBackground(SelectCardBackground(c.cardElement));
    }

    public void UnsetLargeCard()
    {
        if(largeCardInScene.GetComponent<Animator>().GetBool("SlideIn"))
        {
         largeCardInScene.GetComponent<Animator>().SetBool("SlideIn",false);   
        }
    }

    void SetGotCards()
    {
        //load save file
        //compare with dictionary of all cards
        //set all cards in file to found
    }

    void SaveGotCards()
    {

        //get got cards from dictionary
        //save ID to file
    }

    void SetCard(bool found, string cardName)
    {
        cardDictionary[cardName].found = found;
    }

    public Card[] GetCards(Element elementToSortBy)
    {
        List<Card> foundCards = new List<Card>();

        foreach (KeyValuePair<string,Card> item in cardDictionary)
        {
            if (item.Value.cardElement == elementToSortBy)
            {
                foundCards.Add(item.Value);
            }
        }
        return foundCards.ToArray();
    }

    public Card GetBoosterCard(Rarity rarityToSortBy)
    {
        List<Card> tempCardList = new List<Card>();


        foreach (KeyValuePair<string,Card> pair in cardDictionary)
        {
            if (pair.Value.rarity == rarityToSortBy)
            {
                tempCardList.Add(pair.Value);
            }
        }

        return tempCardList[UnityEngine.Random.Range(0, tempCardList.Count)];
    }

    void LoadAllCards()
    {

        if (Application.isWebPlayer)
        {

            TextAsset cardFile = Resources.Load<TextAsset>("Data/CardData/cards");
            XmlSerializer serializer = new XmlSerializer(typeof(XMLTemplate));

            cardDoc = (XMLTemplate)serializer.Deserialize(new StringReader(cardFile.text));

            foreach (CardTemplate card in cardDoc.cards)
            {
                Card newData = new Card();
                newData.cardElement = card.cardElement;
                newData.cardDescription = card.cardDescription;
                newData.cost = float.Parse(card.cost);
                newData.found = false;

                newData.iconLarge = GetIcon(card.iconLargeName);
                newData.iconMedium = GetIcon(card.iconMediumName);
                newData.iconSmall = GetIcon(card.iconSmallName);
                newData.rarity = card.rarity;
                newData.cardName = card.cardName;
                cardDictionary.Add(newData.cardName, newData);
            }
        }
        else
        {
            FileStream cardDocStream = new FileStream("Assets/Resources/Data/CardData/cards.xml", FileMode.Open);

            //get all cards from XML
            if (cardDocStream.CanRead)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XMLTemplate));
                cardDoc = (XMLTemplate) serializer.Deserialize(cardDocStream);
                cardDocStream.Close();

            }
            //create cards in dictionary with XML data
            foreach (CardTemplate card in cardDoc.cards)
            {
                Card newData = new Card();
                newData.cardElement = card.cardElement;
                newData.cardDescription = card.cardDescription;
                newData.cost = float.Parse(card.cost);
                newData.found = false;

                newData.iconLarge = GetIcon(card.iconLargeName);
                newData.iconMedium = GetIcon(card.iconMediumName);
                newData.iconSmall = GetIcon(card.iconSmallName);
                newData.rarity = card.rarity;
                newData.cardName = card.cardName;
                cardDictionary.Add(newData.cardName, newData);
            }
        }

    }

    public void DisplayCards(Card[] cardsToDisplay)
    {
        //small cards setup
        if (sidePanel.childCount < 1)
        {
            foreach (KeyValuePair<string,Card> c in cardDictionary)
            {
                GameObject tempCard = Instantiate(smallCardTemplate);
                tempCard.GetComponent<Card>().SetAll(c.Value);
                instanceListSidePanel.Add(tempCard);

                tempCard.transform.SetParent(sidePanel.transform, false);
            }
            SetUpSidebarCards();
        }

        //Medium cards setup
        if (mainPanel.childCount > 0)
        {
            foreach (RectTransform child in mainPanel)
            {
                Destroy(child.gameObject);
            }
            instanceListMainPanel.Clear();
        }
        foreach (Card c in cardsToDisplay)
        {
            GameObject tempCard = Instantiate(mediumCardTemplate);
            tempCard.GetComponent<Card>().SetAll(c);
            instanceListMainPanel.Add(tempCard);

            tempCard.transform.SetParent(mainPanel.transform, false);
        }
        SetUpMainbarCards();

    }

    Sprite SelectCardBackground(Element compareElement)
    {
        switch (compareElement)
        {
                case Element.Fire:
                    return backgroundSprites[0];
                case Element.Water:
                    return backgroundSprites[1];
                case Element.Lightning:
                    return backgroundSprites[2];
                case Element.Earth:
                    return backgroundSprites[3];
        }
        return null;
    }

    Sprite GetIcon(string iconName)
    {
        Sprite[] foundIcon = new Sprite[1];

        if (Resources.LoadAll<Sprite>("CardImages/" + iconName).Length>0)
        {
            foundIcon = Resources.LoadAll<Sprite>("CardImages/" + iconName);
        }
        if (foundIcon[0])
        {
            
            return foundIcon[0];
        }
        else
        {
            return null;
        }
    }

    void SetUpSidebarCards()
    {
        foreach (GameObject ga in instanceListSidePanel)
        {
            if (!ga) { break;}
            if (ga.GetComponent<Card>())
            {
                Rarity tempRarity = ga.GetComponent<Card>().rarity;
                Element tempElement = ga.GetComponent<Card>().cardElement;
                ga.GetComponent<Card>().SetRarity(rarityImages[tempRarity]);
                ga.GetComponent<Card>().SetSmallImage();
                ga.GetComponent<Card>().SetText();
                ga.GetComponent<Card>().SetBackground(SelectCardBackground(tempElement));
                ga.name = ga.GetComponent<Card>().cardName + " Small";
            }
            else
            {
                ga.AddComponent<Card>();
                Rarity tempRarity = ga.GetComponent<Card>().rarity;
                Element tempElement = ga.GetComponent<Card>().cardElement;
                ga.GetComponent<Card>().SetRarity(rarityImages[tempRarity]);
                ga.GetComponent<Card>().SetSmallImage();
                ga.GetComponent<Card>().SetText();
                ga.GetComponent<Card>().SetBackground(SelectCardBackground(tempElement));
                ga.name = ga.GetComponent<Card>().cardName + " Small";
            }
        }
    }

    void SetUpMainbarCards()
    {
        foreach (GameObject ga in instanceListMainPanel)
        {
            Rarity tempRarity = ga.GetComponent<Card>().rarity;
            Element tempElement = ga.GetComponent<Card>().cardElement;
            ga.GetComponent<Card>().SetRarity(rarityImages[tempRarity]);
            ga.GetComponent<Card>().SetMediumImage();
            ga.GetComponent<Card>().SetText();
            ga.GetComponent<Card>().SetBackground(SelectCardBackground(tempElement));
            ga.name = ga.GetComponent<Card>().cardName+" Medium";
        }
    }
}
