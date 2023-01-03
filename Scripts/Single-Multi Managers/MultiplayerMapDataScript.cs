using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MultiplayerMapDataScript : MonoBehaviour
{
    public static MultiplayerMapDataScript instance;

    private void Awake() {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public UserSO userSO;
    public ModifySO modifySO;
    public LevelSO levelSO;
    public CanonListSO canonListSO;
    public Material Blue;
    public Material Red;
    public GameObject EndGamePanel;
    public GameObject canvas;
    public GameObject healthBarObject;
    public BoxCollider roadMainCollider;
    public ItemTypeSO redItemType;
    public ItemTypeSO blueItemType;
    public Transform blueCastleSpawnPoint;
    public Transform redCastleSpawnPoint;
    public GameObject settingsPanel;
    public UnityEngine.UI.Toggle SettingsButton;
    public GameObject vCam;
    public TextMeshProUGUI EndTimeText;
    public const int END_GAME_EVENT = 0;

    // Minions use this to reach transform
    [System.NonSerialized] public CastleScript redCastle;
    [System.NonSerialized] public CastleScript blueCastle;

}
