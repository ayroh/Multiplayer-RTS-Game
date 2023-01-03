using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.EventSystems;
using TMPro;

public class DragDropScript : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler{

    [SerializeField] private RectTransform parentTransform;
    [SerializeField] private bool isPointedSkill = false;
    [SerializeField] private bool isSecondTapable = false;
    [SerializeField] private UnityEngine.UI.Text manaText;
    [System.NonSerialized] public bool isInGame = false;
    private RectTransform rectTransform;
    private Vector3 vel = Vector3.zero;
    private Vector3 startingPosition;
    private int groundLayer;
    private int secondHitLayers;
    public string cardName;
    public int cardMana;

    private void Awake() {
        groundLayer = 1 << LayerMask.NameToLayer("Ground");
        secondHitLayers = 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("MissHit");
        rectTransform = transform as RectTransform;
        startingPosition = rectTransform.anchoredPosition;
        if (parentTransform == null)
            parentTransform = transform.parent as RectTransform;
        if (string.IsNullOrEmpty(cardName))
            cardName = name.Substring(0, name.IndexOf("Card"));
    }
    
    public void SetIsInGame(bool choice) {
        isInGame = choice;
        if (choice)
            GetComponent<UnityEngine.UI.Button>().enabled = false;
        else
            GetComponent<UnityEngine.UI.Button>().enabled = true;
    }

    public void SetStartingPoint(Vector3 newPosition) => startingPosition = newPosition;
    public void OnBeginDrag(PointerEventData eventData) {
        if (isInGame) 
            EventManager.HoldingCard();
    }

    public void OnDrag(PointerEventData eventData) {

        if (isInGame) {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePosition)) {
                rectTransform.position = Vector3.SmoothDamp(rectTransform.position, globalMousePosition, ref vel, 0.05f);
                //rectTransform.position = Vector3.Lerp(rectTransform.position, globalMousePosition, 0.4f);
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData) {
        if (isInGame) {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePosition)) {
                if (SkillPanelScript.cardOffsetY < globalMousePosition.y && ManaScript.mana >= cardMana) {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (isPointedSkill) {
                        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer)) {
                            if (isSecondTapable) {
                                StartCoroutine(WaitForSecondTap(hitInfo));
                                return;
                            }
                            else
                                EventManager.CastCard(cardName, cardMana, hitInfo.point);
                        }
                        else {
                            rectTransform.anchoredPosition = startingPosition;
                            EventManager.DroppedCard();
                            return;
                        }
                    }
                    else
                        EventManager.CastCard(cardName, cardMana);

                    gameObject.SetActive(false);
                    EventManager.RemoveCard(cardName);
                }
                else
                    rectTransform.anchoredPosition = startingPosition;
                EventManager.DroppedCard();
            }
        }
    }

    private void OnValidate() {
        if (!string.IsNullOrEmpty(cardName)) {
            name = cardName + "Card";
            GetComponentInChildren<UnityEngine.UI.Text>().text = cardName;
            GetComponentInChildren<UnityEngine.UI.Text>().name = cardName + "Text";
            manaText.text = cardMana.ToString();
        }
    }

    public void ClickCard() => EventManager.CardClicked(this);

    private IEnumerator WaitForSecondTap(RaycastHit hitInfo) {
        yield return WaitForKeyPress(KeyCode.Mouse0);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit secondHitInfo, 100f, secondHitLayers)) {
            EventManager.CastCard(cardName, cardMana, hitInfo.point, secondHitInfo.point);
            gameObject.SetActive(false);
            EventManager.RemoveCard(cardName);
        }
        else
            rectTransform.anchoredPosition = startingPosition;
        EventManager.DroppedCard();
    }

    private IEnumerator WaitForKeyPress(KeyCode key) {
        bool done = false;
        while (!done){
            if (Input.GetKeyDown(key))
                done = true; 
            yield return null;
        }
    }

}
