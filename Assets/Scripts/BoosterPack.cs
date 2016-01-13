using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class BoosterPack : MonoBehaviour
{
    public RectTransform costTextField;
    public int amountOfCards = 5;
    public Element boosterElement;
    public float cost;
    
    [Range(0,1)]
    public float[] percentagePerRarity = new float[4];
    public Rarity[] rarities = new Rarity[4];

    Dictionary<float, Rarity> percentageDictionary = new Dictionary<float, Rarity>();


    void Start()
    {
        int i = 0;

        foreach (float f in percentagePerRarity)
        {
            percentageDictionary.Add(f+Random.Range(0.001f,0.004f),rarities[i]);
            i++;
        }
    }

    public void GenerateRandomCards()
    {
        Card[] generatedCards = new Card[amountOfCards];
        List<Rarity> spawningRarities = new List<Rarity>();

        while (spawningRarities.Count < amountOfCards)
        {
            foreach (KeyValuePair<float, Rarity> pair in percentageDictionary)
            {
                if (Random.value <= pair.Key)
                {
                    spawningRarities.Add(pair.Value);
                }
            }
        }

        if (spawningRarities.Count > amountOfCards)
        {
            spawningRarities.RemoveRange(amountOfCards, Mathf.Abs(spawningRarities.Count-amountOfCards));
        }

        int i = 0;

        //get cards from CardHandler
        foreach (Rarity r in spawningRarities)
        {
            //does work only array shows no elements in inspector
            if (boosterElement == Element.All)
            {
                generatedCards[i] = CardHandler.instance.GetBoosterCard(r);
            }
            else
            {
                generatedCards[i] = CardHandler.instance.GetBoosterCard(r,boosterElement);
            }
            i++;
        }

        BoosterHandler.instance.SetGotCards(generatedCards);
    }

    public void SetCost(float costToShow)
    {
        string formatString = "" + costToShow;
        formatString = formatString.Replace('.', ',');
        if (costTextField)
        {
            costTextField.GetComponent<Text>().text = formatString;
        }
    }
}
