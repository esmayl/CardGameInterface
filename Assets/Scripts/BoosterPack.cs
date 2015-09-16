using UnityEngine;
using System.Collections.Generic;

public class BoosterPack : MonoBehaviour
{
    public int amountOfCards = 5;
    public Element boosterType;
    public float cost;
    public Card[] testlist;
    
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

        testlist = GenerateRandomCards();

    }

    Card[] GenerateRandomCards()
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
            Debug.Log(r);
            
            //does work only array shows no elements in inspector
            generatedCards[i] = CardHandler.instance.GetBoosterCard(r);
            Debug.Log(generatedCards[i].cardDescription);
            i++;
        }

        return generatedCards;
    }
}
