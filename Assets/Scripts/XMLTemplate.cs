using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class XMLTemplate
{
    [XmlElement(ElementName = "Cards")]public List<CardTemplate> cards;
}

public class CardTemplate: ScriptableObject
{
    [XmlElement(ElementName = "iconSmallName")]public string iconSmallName;
    [XmlElement(ElementName = "iconMediumName")]public string iconMediumName;
    [XmlElement(ElementName = "iconLargeName")]public string iconLargeName;
    [XmlElement(ElementName = "ElementType")]public Element cardElement;
    [XmlElement(ElementName = "CardCost")]public string cost;
    [XmlElement(ElementName = "CardText")]public string cardDescription;
    [XmlElement(ElementName = "Rarity")] public Rarity rarity;
    [XmlElement(ElementName = "CardName")]public string cardName;
}