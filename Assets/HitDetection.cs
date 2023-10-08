using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public MqttPublisher mqttPublisher;
    public MqttReceiver mqttReceiver;
    public CustomAREffects care;
    public ReloadEffectController reloadEffectController;
    public GrenadeController grenadeController;
    public ShieldController shieldController;
    public ScoreboardController scoreboardController;
    public GunController gunController;
    public PlayerState playerState;
    public OpponentHealth opponentHealth;

    private string player_id = null;

    [Serializable] 
    private class Player
    {
        public int hp;
        public int bullets;
        public int grenades;
        public int shield_hp;
        public int deaths;
        public int shields;
    }

    [Serializable] 
    private class GameState
    {
        public Player p1;
        public Player p2;
    }

    private class MqttMessage
    {
        public string type;
        public string player_id;
        public string action;
        public bool isHit;
        public GameState game_state;
    }

    private class Result
    {
        public string player_id;
        public string action;
        public bool isHit;
    }

    void Start()
    {
        mqttReceiver.OnMessageArrived += OnMessageArrivedHandler;
    }

    private void OnMessageArrivedHandler(string newMsg)
    {
        MqttMessage x = JsonUtility.FromJson<MqttMessage>(newMsg);

        if (x.type == "QUERY") {
            CheckForHit(x);
        } else { // x.type == "UPDATE"
            UpdateVisualiser(x);
        }
    }

    private void CheckForHit(MqttMessage x) {
        if (x.player_id != player_id) {
            return;
        }
        Result res = new Result
        {
            player_id = x.player_id,
            action = x.action
        };
        // detect hit
        if (care.isTargetVisible) {
            res.isHit = true;
        } else {
            res.isHit = false;
        }

        string publishMsg = JsonUtility.ToJson(res);

        // reply with hitdetection
        mqttPublisher.Publish(publishMsg);
    }

    private void UpdateVisualiser(MqttMessage x) {
        if (player_id == null) {
            return;
        }
        if (x.player_id == player_id) {
            switch (x.action) {
                case "gun":
                    care.ShootGun(x.isHit);
                    break;

                case "grenade":
                    care.OnPlayerGrenadeButtonPressed();
                    break;
                
                case "spear":
                    care.OnPlayerSpearButtonPressed();
                    break;

                case "shield":
                    care.OnPlayerShieldButtonPressed();
                    break;

                case "hammer":
                    care.OnPlayerHammerButtonPressed();
                    break;

                case "punch":
                    care.OnPlayerPunchButtonClicked();
                    break;

                case "portal":
                    care.OnPlayerPortalButtonClicked();
                    break;

                case "web":
                    care.OnPlayerWebButtonPressed();
                    break;

                case "reload":
                    care.OnPlayerReload();
                    reloadEffectController.PlayReloadEffect();
                    break;

                default: // none
                    break;
            }
        } else {
            switch (x.action) {
                case "gun":
                    care.ShootOpponentGun(x.isHit);
                    break;

                case "grenade":
                    care.OnOpponentGrenadeButtonPressed();
                    break;
                
                case "spear":
                    care.OnOpponentSpearButtonPressed();
                    break;

                case "shield":
                    care.OnOpponentShieldButtonPressed();
                    break;

                case "hammer":
                    care.OnOpponentHammerButtonPressed();
                    break;

                case "punch":
                    care.OnOpponentPunchButtonClicked();
                    break;

                case "portal":
                    care.OnOpponentPortalButtonClicked();
                    break;

                case "web":
                    care.OnOpponentWebButtonPressed();
                    break;

                default: // none
                    break;
            }
        }
        
        // UPDATE UI
        GameState game_state = JsonUtility.FromJson<GameState>(JsonUtility.ToJson(x.game_state));
        Player p1, p2;
        if (player_id == "1") {
            p1 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p1));
            p2 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p2)); 
        } else {
            p2 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p1));
            p1 = JsonUtility.FromJson<Player>(JsonUtility.ToJson(game_state.p2)); 
        }

        scoreboardController.SetScore(p2.deaths, p1.deaths);
        grenadeController.SetGrenades(p1.grenades);
        shieldController.SetShields(p1.shields);
        gunController.SetAmmo(p1.bullets);

        bool isInit = x.action == "none";

        playerState.SetShieldHp(p1.shield_hp, isInit);
        playerState.SetHealth(p1.hp, isInit);
        opponentHealth.SetShieldHp(p2.shield_hp, isInit);
        opponentHealth.SetHealth(p2.hp, isInit);

        if (p1.shield_hp > 0) {
            care.OnPlayerShieldButtonPressed();
        }
        if (p2.shield_hp > 0) {
            care.OnOpponentShieldButtonPressed();
        }
    }

    private IEnumerator PublishMessage(string publishMsg, float seconds) {
        yield return new WaitForSeconds(seconds);
        mqttPublisher.Publish(publishMsg);    
    }

    public void SetPlayer(string x) {
        player_id = x;

        Result res = new Result
        {
            player_id = player_id,
            action = "none"
        };

        string publishMsg = JsonUtility.ToJson(res);

        // reply with hitdetection
        mqttPublisher.Publish(publishMsg);
    }

}