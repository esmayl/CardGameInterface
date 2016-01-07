using UnityEngine;
using UnityEngine.UI;


public enum Element
{
    All,
    Fire,
    Water,
    Lightning,
    Earth
}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Special
}

//A container class only used to store data
public class Card : MonoBehaviour
{
    public Sprite iconSmall;
    public Sprite iconMedium;
    public Sprite iconLarge;
    public Element cardElement;
    public Rarity rarity;
    public float cost;

    internal string cardName;
    internal string cardDescription;
    internal bool found;
    internal Text cardTextField;
    internal Image cardImage;
    internal Image rarityImage;

    void Awake()
    {
        foreach (Transform t in transform)
        {
            if (t.childCount > 0)
            {
                foreach (Transform p in t)
                {
                    switch (p.name)
                    {
                        case "CardImage":
                            cardImage = p.GetComponent<Image>();
                            break;
                        case "CardText":
                            cardTextField = p.GetComponent<Text>();
                            break;
                        case "RaritySymbol":
                            rarityImage = p.GetComponent<Image>();
                            break;
                    }
                }
            }
            switch (t.name)
            {
                case "CardImage":
                    cardImage = t.GetComponent<Image>();
                    break;
                case "CardText":
                    cardTextField = t.GetComponent<Text>();
                    break;
                case "RaritySymbol":
                    rarityImage = t.GetComponent<Image>();
                    break;
            }
        }

    }

    public void SetText()
    {
        cardTextField.text = cardName + "\n\n" + cardDescription;
    }

    public void SetRarity(Sprite raritySprite)
    {
        if (rarityImage)
        {
            rarityImage.sprite = raritySprite;
        }
    }

    public void SetSmallImage()
    {
        if (cardImage && iconSmall)
        {
            cardImage.sprite = iconSmall;
        }
    }

    public void SetMediumImage()
    {
        if (cardImage && iconMedium)
        {
            cardImage.sprite = iconMedium;
        }
    }

    public void SetLargeImage()
    {
        if (cardImage && iconLarge)
        {
            cardImage.sprite = iconLarge;
        }
    }

    public void SetBackground(Sprite bg)
    {
        GetComponent<Image>().sprite = bg;
    }

    public void SetAll(Card newCardInfo)
    {
        iconSmall = newCardInfo.iconSmall;
        iconMedium = newCardInfo.iconMedium;
        iconLarge = newCardInfo.iconLarge;
        cardElement = newCardInfo.cardElement;
        rarity = newCardInfo.rarity;
        cost = newCardInfo.cost;
        cardName = newCardInfo.cardName;
        cardDescription = newCardInfo.cardDescription;
        found = newCardInfo.found;
    }

    public static Color FindColor(Element element)
    {
        switch (element)
        {
            case Element.Earth:
                return Color.white;
                break;
            case Element.Lightning:
                return Color.yellow;
                break;
            case Element.Water:
                return Color.cyan;
                break;
            case Element.Fire:
                return Color.red;
                break;
            default:
                return Color.white;
                break;
        }
    }
}
