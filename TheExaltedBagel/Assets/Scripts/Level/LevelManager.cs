using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField] private float deathScorePenalty = 1000f;
    [SerializeField] private float respawnTime = 1.5f;
    [SerializeField] private RawImage[] uiBagels = new RawImage[3];

    private float score = 0f;
    private Player player;
    private Checkpoint currentCheckpoint;

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public Checkpoint CurrentCheckpoint
    {
        set { this.currentCheckpoint = value; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public float Score
    {
        get { return this.score; }
        set { this.score = value; }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Awake()
    {
        instance = this;
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && this.player.gameObject.activeSelf)
        {
            KillPlayer(false);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void KillPlayer(bool isFirstSpawn)
    {
        if (!isFirstSpawn)
        {
            this.score -= this.deathScorePenalty;
            this.player.KillPlayer();
            StartCoroutine(WaitingForRespawn());
        }
        else
        {
            this.currentCheckpoint.ResetSection(this.player, isFirstSpawn);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    public void UpdateGravityChargeUI()
    {
        uint chargeCount = this.player.GravityChargeCount;
        for (uint i = 0; i < this.player.GravityChargeMax; ++i)
        {
            if (this.uiBagels[i] != null)
            {
                this.uiBagels[i].enabled = ((i + 1) > chargeCount);
            }
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////
    IEnumerator WaitingForRespawn()
    {
        yield return new WaitForSeconds(this.respawnTime);
        this.currentCheckpoint.ResetSection(this.player, false);
    }
}
