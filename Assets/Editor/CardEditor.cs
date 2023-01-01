using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CardEditor : EditorWindow
{
    private static XMLTemplate xmlTemplate;
    
    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Card editor")]
    static void Init()
    {
        
        FileStream cardDocStream = new FileStream("Assets/Resources/Data/CardData/cards.xml", FileMode.Open);

        //get all cards from XML
        if (cardDocStream.CanRead)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLTemplate));
            xmlTemplate = (XMLTemplate) serializer.Deserialize(cardDocStream);
            cardDocStream.Close();

        }
        
        CardEditor wnd = GetWindow<CardEditor>();
        wnd.titleContent = new GUIContent("Card editor");
    }
    
    void CreateGUI()
    {
        VisualElement container = new VisualElement();
        
        Button saveButton = new Button();
        saveButton.text = "Save";
        saveButton.clicked += SaveButtonOnclicked;
        container.Add(saveButton);
        
        ScrollView scrollContainer = new ScrollView();
        
        Debug.Log(rootVisualElement);
        foreach (CardTemplate xmlTemplateCard in xmlTemplate.cards)
        {
            SerializedObject serializedObject = new SerializedObject(xmlTemplateCard);

            Foldout foldout = new Foldout();
            foldout.text = xmlTemplateCard.cardName;
            
            TextField newTextField = new TextField("Cost");
            newTextField.value = xmlTemplateCard.cost;
            newTextField.bindingPath = "cost";
            
            newTextField.Bind(serializedObject);

            foldout.Add(newTextField);

            TextField newTextField2 = new TextField("Card Description");
            newTextField2.bindingPath = "cardDescription";
            newTextField2.Bind(serializedObject);
            
            foldout.Add(newTextField2);

            TextField newTextField3 = new TextField("Card Element");
            newTextField3.bindingPath = "cardElement";
            newTextField3.Bind(serializedObject);

            foldout.Add(newTextField3);
            
            TextField newTextField4 = new TextField("Card Name");
            newTextField4.bindingPath = "cardName";
            newTextField4.Bind(serializedObject);
            
            foldout.Add(newTextField4);

            // DropdownField iconLargePicker = new DropdownField();
            // iconLargePicker.AppendAction("test",Action);
            // foldout.Add(iconLargePicker);
            
            scrollContainer.Add(foldout);
        }
        
        container.Add(scrollContainer);
        rootVisualElement.Add(container);
    }

    private void Action(DropdownMenuAction obj)
    {
        throw new System.NotImplementedException();
    }

    private void SaveButtonOnclicked()
    {
        FileStream cardDocStream = new FileStream("Assets/Resources/Data/CardData/cards.xml", FileMode.Truncate);

        //get all cards from XML
        if (cardDocStream.CanWrite)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XMLTemplate));
            serializer.Serialize(cardDocStream,xmlTemplate);
            cardDocStream.Close();
        }
    }
}