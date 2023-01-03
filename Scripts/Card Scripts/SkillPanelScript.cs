using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillPanelScript : MonoBehaviour {

    [SerializeField] private DeckSO deckSO;
    [SerializeField] private AllCardsSO allCardsSO;
    [SerializeField] private int maxCardAmount = 4;
    public static float cardOffsetY;
    private List<string> hand;
    private List<RectTransform> handPositions;

    private void OnEnable() => EventManager.OnRemoveCard += RemoveCard;

    private void OnDisable() => EventManager.OnRemoveCard -= RemoveCard;

    private void Awake() {
        cardOffsetY = (transform as RectTransform).position.y + (transform as RectTransform).rect.height;
        if (maxCardAmount > deckSO.Deck.Count) {
            Debug.LogError("GAME WILL CRASH. ADD MORE CARD PREFABS");
            return;
        }
        handPositions = transform.Cast<RectTransform>().ToList();
        hand = new List<string>(new string[maxCardAmount]);
        Random.InitState(System.DateTime.Now.Millisecond);
        int random = -1;
        for (int i = 0; i < maxCardAmount; ++i) {
            while (hand.Contains(deckSO.Deck[random = Random.Range(0, deckSO.Deck.Count)]));
            ReplaceCard(i, random);
        }
    }

    private void ReplaceCard(int replace, int prefabIndex) {
        DragDropScript cardScript = Instantiate(allCardsSO.Cards.Find(obj => obj.GetComponent<DragDropScript>().cardName == deckSO.Deck[prefabIndex]), handPositions[replace]).GetComponent<DragDropScript>(); // deckSO.Deck[prefabIndex], handPositions[replace]).GetComponent<DragDropScript>();
        cardScript.GetComponent<UnityEngine.UI.Button>().enabled = false;
        cardScript.SetIsInGame(true);
        hand[replace] = cardScript.cardName;
    }

    public void RemoveCard(string CardName) {
        int index = hand.IndexOf(CardName);
        int deckIndex = deckSO.Deck.IndexOf(CardName);
        hand[index] = null;
        Destroy(handPositions[index].GetChild(0).gameObject);
        int random = -1;
        while (hand.Contains(deckSO.Deck[random = Random.Range(0, deckSO.Deck.Count)]) && random != deckIndex) ;
        //random = deckSO.Deck.FindIndex(obj => obj.Equals(CardName)); // Hand stays same this way
        ReplaceCard(index, random);
    }
}
