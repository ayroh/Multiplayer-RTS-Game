using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class CardMenuScript : MonoBehaviour
{
    [SerializeField] private DeckSO deckSO;
    [SerializeField] private AllCardsSO allCardsSO;
    [SerializeField] private List<RectTransform> deckPlaces;
    [SerializeField] private RectTransform allCardsTransform;
    private List<RectTransform> allCardsList;

    // Change Deck Scripts
    private bool isCardsChanged;
    private GameObject ClickedNonDeckCard;
    private GameObject ClickedDeckCard;

    private void OnEnable() => EventManager.OnCardClicked += CardClicked;

    private async void OnDisable() {
        // Saving only deckso. Dont need to load before save.
        EventManager.OnCardClicked -= CardClicked;
        if (isCardsChanged)
        {
            await FirebaseManager.instance.SaveSO(deckSO)
                .ContinueWith(task => isCardsChanged = false);
        }
            
    }


    private void CardClicked(DragDropScript CardScript) {
        if(!deckSO.Deck.Any(obj => obj.Equals(CardScript.cardName))) {
            if (ClickedDeckCard != null) {
                int allCardsIndex = allCardsSO.Cards.FindIndex(obj => obj.GetComponent<DragDropScript>().cardName.Equals(CardScript.cardName));
                int deckIndex = deckSO.Deck.FindIndex(obj => obj.Equals(ClickedDeckCard.GetComponent<DragDropScript>().cardName));
                Destroy(deckPlaces[deckIndex].GetChild(0).gameObject);
                //int childIndex = CardScript.transform.GetSiblingIndex();
                Destroy(CardScript.gameObject);
                Instantiate(allCardsSO.Cards[allCardsIndex], deckPlaces[deckIndex]);
                Instantiate(allCardsSO.Cards.Find(obj => obj.GetComponent<DragDropScript>().cardName == deckSO.Deck[deckIndex]), allCardsTransform); //.transform.SetSiblingIndex(childIndex);
                deckSO.Deck[deckIndex] = allCardsSO.Cards[allCardsIndex].GetComponent<DragDropScript>().cardName;
                ClickedNonDeckCard = null;
                ClickedDeckCard = null;
                isCardsChanged = true;
                SortNonDeck();
            }
            else
                ClickedNonDeckCard = CardScript.gameObject;
        }
        else {
            if (ClickedNonDeckCard != null) {
                int allCardsIndex = allCardsSO.Cards.FindIndex(obj => obj.GetComponent<DragDropScript>().cardName.Equals(ClickedNonDeckCard.GetComponent<DragDropScript>().cardName));
                int deckIndex = deckSO.Deck.FindIndex(obj => obj.Equals(CardScript.GetComponent<DragDropScript>().cardName));
                Destroy(deckPlaces[deckIndex].GetChild(0).gameObject);
                //int childIndex = ClickedNonDeckCard.transform.GetSiblingIndex();
                Destroy(ClickedNonDeckCard.gameObject);
                Instantiate(allCardsSO.Cards[allCardsIndex], deckPlaces[deckIndex]);
                Instantiate(allCardsSO.Cards.Find(obj => obj.GetComponent<DragDropScript>().cardName == deckSO.Deck[deckIndex]), allCardsTransform); //.transform.SetSiblingIndex(childIndex);
                deckSO.Deck[deckIndex] = allCardsSO.Cards[allCardsIndex].GetComponent<DragDropScript>().cardName;
                ClickedNonDeckCard = null;
                ClickedDeckCard = null;
                isCardsChanged = true;
                SortNonDeck();
            }
            else
                ClickedDeckCard = CardScript.gameObject;
        }
    }

    public void Initialize() {

        for (int i = 0; i < deckSO.Deck.Count; ++i) {
            try {
                Instantiate(allCardsSO.Cards.Find(obj => obj.GetComponent<DragDropScript>().cardName == deckSO.Deck[i]), deckPlaces[i]);
            }
            catch (Exception cast) when (cast is InvalidCastException || cast is NullReferenceException || cast is ArgumentException || cast is MissingReferenceException || cast is UnassignedReferenceException) {
                for (int j = 0; j < i; ++j)
                    Destroy(deckPlaces[j].GetChild(0).gameObject);
                FillDeckRandom();
                i = -1;
                continue;
            }
        }
        for (int i = 0; i < allCardsSO.Cards.Count; ++i) {
            if (deckSO.Deck.Contains(allCardsSO.Cards[i].GetComponent<DragDropScript>().cardName))
                continue;
            Instantiate(allCardsSO.Cards[i], allCardsTransform);
        }
        SortNonDeck();
    }

    private void SortNonDeck() {
        allCardsList = allCardsTransform.Cast<RectTransform>().ToList();
        allCardsList.Sort((RectTransform t1, RectTransform t2) => { return t1.name.CompareTo(t2.name); });
        allCardsTransform.DetachChildren();
        for (int i = 0; i < allCardsList.Count; ++i)
            allCardsList[i].SetParent(allCardsTransform);
    }

    private void FillDeckRandom() {
        int random = -1;
        for(int i = 0;i < deckSO.Deck.Count; ++i) {
            while (deckSO.Deck.Contains(allCardsSO.Cards[random = Random.Range(0, allCardsSO.Cards.Count)].GetComponent<DragDropScript>().cardName)) ;
            deckSO.Deck[i] = allCardsSO.Cards[random].GetComponent<DragDropScript>().cardName;
        }
    }

}
