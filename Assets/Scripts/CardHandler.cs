using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEngine.UI;

public class CardHandler : MonoBehaviour
{
    public static CardHandler instance;

    internal Text moneyTextField;
    public Rarity[] rarities = new Rarity[4];
    public Sprite[] raritySprites = new Sprite[4];
    public Sprite[] backgroundSprites = new Sprite[4];
    public GameObject smallCardTemplate;
    public GameObject mediumCardTemplate;

    public GameObject largeCardInScene;
    
    public RectTransform mainPanel;
    public RectTransform sidePanel;

    float _money = 100f;

    internal float money
    {
        get { return _money; }
        set
        {
            _money = value;
            if (moneyTextField)
            {
                moneyTextField.text = "" + _money;
            }
        }
    }

    internal Dictionary<string, Card> cardDictionary = new Dictionary<string, Card>();

    internal Dictionary<Rarity, Sprite> rarityImages = new Dictionary<Rarity, Sprite>();
    List<GameObject> instanceListSidePanel = new List<GameObject>();
    List<GameObject> instanceListMainPanel = new List<GameObject>();

    XMLTemplate cardDoc = new XMLTemplate();

    void Awake()
    {
        if (mainPanel == null)
        {
            ResetLocalVars();
        }
    }

    void Start()
    {

       if (!instance)
       {
           instance = this;
       }

       if (cardDictionary.Count < 1)
       {
           LoadAllCards();
       }

       LoadGotCards();

       int i = 0;
       foreach (Sprite s in raritySprites)
       {
           rarityImages.Add(rarities[i],s);
           i++;
       }

       DisplayCards(GetCards(Element.All));
    }

    void OnLevelWasLoaded(int level)
    {

        if (level != 0)
        {
            moneyTextField = GameObject.Find("Money").transform.FindChild("MoneyText").GetComponent<Text>();
            moneyTextField.text = "" + this.money;
            return;
        }
        

        if (mainPanel == null)
        {
            ResetLocalVars();
            SaveGotCards();
        }
        else
        {
            return;
        }

        instanceListSidePanel.Clear();
        instanceListMainPanel.Clear();
        
        DisplayCards(GetCards(Element.All));
    }

    public void ResetLocalVars()
    {
        GameObject[] test = GameObject.FindGameObjectsWithTag("CardHandler");
        CardHandler handler = new CardHandler();


        //setting local variables back when going back to Collection
        foreach (GameObject ga in test)
        {
            if (ga.GetComponent<CardHandler>())
            {
                handler = ga.GetComponent<CardHandler>();
            }

            if (handler.mainPanel != null && mainPanel == null)
            {
                smallCardTemplate = handler.smallCardTemplate;
                mediumCardTemplate = handler.mediumCardTemplate;
                mainPanel = handler.mainPanel;
                sidePanel = handler.sidePanel;
                largeCardInScene = handler.largeCardInScene;
                Destroy(handler.gameObject);
            }
        }

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

    public void LoadGotCards()
    {
        string s = PlayerPrefs.GetString("Cards");

        string[] allCards = s.Split(',');

        for (int i = 0; i < allCards.Length; i++)
        {
            allCards[i] = allCards[i].Trim();
        }

        foreach (string c in allCards)
        {
            Debug.Log(c);

            foreach (KeyValuePair<string,Card> p in cardDictionary)
            {
                if (p.Value.cardName == c)
                {
                    p.Value.found = true;
                }
            }
        }

        //load save file
        //compare with dictionary of all cards
        //set all cards in file to found
    }

    public void SaveGotCards()
    {

        string allCards = "";

        List<string> tempList = new List<string>();
        
        PlayerPrefs.DeleteKey("Cards");
        PlayerPrefs.Save();

        foreach (KeyValuePair<string,Card> c in cardDictionary)
        {
            if (c.Value.found)
            {
                string tempString = c.Value.cardName;
                tempList.Add(tempString);
            }

        }
        allCards = string.Join(",", tempList.ToArray());

        if (allCards != "")
        {
            PlayerPrefs.SetString("Cards", allCards);
            PlayerPrefs.Save();
        }
        else
        {
            Debug.LogError("!!");
        }

        //get got cards from dictionary
        //save ID to file
    }

    public void SetCard(bool found, string cardName)
    {
        if (!cardDictionary[cardName].found)
        {
            cardDictionary[cardName].found = found;
        }
    }

    public Card[] GetCards(Element elementToSortBy)
    {
        List<Card> foundCards = new List<Card>();
        if (elementToSortBy != Element.All)
        {
            foreach (KeyValuePair<string, Card> item in cardDictionary)
            {
                if (item.Value.cardElement == elementToSortBy && item.Value.found)
                {
                    foundCards.Add(item.Value);
                }
            }
        }
        else
        {
            foreach (KeyValuePair<string, Card> item in cardDictionary)
            {
                if (item.Value.found)
                {
                    foundCards.Add(item.Value);
                }
            }
            
        }
        return foundCards.ToArray();
    }

    public Card GetBoosterCard(Rarity rarityToSortBy,Element elementToSortBy = Element.All)
    {
        List<Card> tempCardList = new List<Card>();

        if (elementToSortBy == Element.All)
        {
            foreach (KeyValuePair<string, Card> pair in cardDictionary)
            {
                if (pair.Value.rarity == rarityToSortBy)
                {
                    tempCardList.Add(pair.Value);
                }
            }
            return tempCardList[UnityEngine.Random.Range(0, tempCardList.Count)];
        }
        
        foreach (KeyValuePair<string, Card> pair in cardDictionary)
        {
            if (pair.Value.rarity == rarityToSortBy && pair.Value.cardElement == elementToSortBy)
            {
                tempCardList.Add(pair.Value);
            }
        }
        if (tempCardList.Count < 1)
        {
            foreach (KeyValuePair<string, Card> pair in cardDictionary)
            {
                if (pair.Value.cardElement == elementToSortBy)
                {
                    tempCardList.Add(pair.Value);
                }
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

        SetUpSidebarCards();

        if (!mainPanel) { return; }

        if (mainPanel.childCount > 0 )
        {
            foreach (RectTransform child in mainPanel)
            {
                Destroy(child.gameObject);
            }
            instanceListMainPanel = new List<GameObject>();

            foreach (Card c in cardsToDisplay)
            {
                GameObject tempCard = Instantiate(mediumCardTemplate);
                tempCard.GetComponent<Card>().SetAll(c);
                instanceListMainPanel.Add(tempCard);

                tempCard.transform.SetParent(mainPanel.transform, false);
            }
        }
        else
        {
            foreach (Card c in cardsToDisplay)
            {
                Debug.Log("Placing medium cards!");
                GameObject tempCard = Instantiate(mediumCardTemplate);
                tempCard.GetComponent<Card>().SetAll(c);
                instanceListMainPanel.Add(tempCard);

                tempCard.transform.SetParent(mainPanel.transform, false);
            }
        }
        
        SetUpMainbarCards();

    }

    void SetUpSidebarCards()
    {
        if (!sidePanel) { return;}

        if (sidePanel.childCount > 1)
        {
            foreach (RectTransform r in sidePanel)
            {
                Destroy(r.gameObject);
            }

            instanceListSidePanel.Clear();
        }

        foreach (Card c in GetCards(Element.All))
        {
            GameObject tempCard = Instantiate(smallCardTemplate);
            tempCard.GetComponent<Card>().SetAll(c);
            instanceListSidePanel.Add(tempCard);

            tempCard.transform.SetParent(sidePanel.transform, false);
        }


        //setup of cards
        foreach (GameObject ga in instanceListSidePanel)
        {
            if (!ga)
            {
                instanceListSidePanel.Remove(ga);
            }

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
        }
    }

    void SetUpMainbarCards()
    {
        foreach (GameObject ga in instanceListMainPanel)
        {
            if (!ga)
            {
                instanceListMainPanel.Remove(ga);
            }
            if (ga.GetComponent<Card>())
            {
                Rarity tempRarity = ga.GetComponent<Card>().rarity;
                Element tempElement = ga.GetComponent<Card>().cardElement;
                ga.GetComponent<Card>().SetRarity(rarityImages[tempRarity]);
                ga.GetComponent<Card>().SetMediumImage();
                ga.GetComponent<Card>().SetText();
                ga.GetComponent<Card>().SetBackground(SelectCardBackground(tempElement));
                ga.name = ga.GetComponent<Card>().cardName + " Medium";
            }
        }
    }

    public Sprite SelectCardBackground(Element compareElement)
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
}
