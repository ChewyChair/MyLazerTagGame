using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public MqttPublisher mqttPublisher;
    public MqttReceiver mqttReceiver;
    public CustomAREffects care;
    public GrenadeController grenadeControl;
    public PlayerState playerState;
    public OpponentHealth opponentHealth;

    private class MqttMessage
    {
        public string type;
        public string player_id;
        public string action;
        public bool isHit;
        public string game_state;
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
        Result res = new Result();
        res.player_id = x.player_id;
        res.action = x.action;
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
        // for now we assume that the player is player 1.
        if (x.player_id == "1") {
            switch (x.action) {
                case "grenade":
                    grenadeControl.ThrowGrenade();
                    opponentHealth.GrenadeReceived();
                    care.OnPlayerGrenadeButtonPressed();
                    break;
                
                case "spear":
                    opponentHealth.SpearReceived();
                    care.OnPlayerSpearButtonPressed();
                    break;

                case "shield":
                    playerState.ActivateShield();
                    care.OnPlayerShieldButtonPressed();
                    break;

                case "hammer":
                    opponentHealth.HammerReceived();
                    care.OnPlayerHammerButtonPressed();
                    break;

                case "punch":
                    opponentHealth.PunchReceived();
                    care.OnPlayerPunchButtonClicked();
                    break;

                case "portal":
                    opponentHealth.PortalReceived();
                    care.OnPlayerPortalButtonClicked();
                    break;

                default:
                    break;
            }
        } else {
            switch (x.action) {
                case "grenade":
                    playerState.GrenadeReceived();
                    care.OnOpponentGrenadeButtonPressed();
                    break;
                
                case "spear":
                    playerState.SpearReceived();
                    care.OnOpponentSpearButtonPressed();
                    break;

                case "shield":
                    opponentHealth.ActivateShield();
                    care.OnOpponentShieldButtonPressed();
                    break;

                case "hammer":
                    playerState.HammerReceived();
                    care.OnOpponentHammerButtonPressed();
                    break;

                case "punch":
                    playerState.PunchReceived();
                    care.OnOpponentPunchButtonClicked();
                    break;

                case "portal":
                    playerState.PortalReceived();
                    care.OnOpponentPortalButtonClicked();
                    break;

                default:
                    break;
            }
        }
        // UPDATE UI
    }

    private IEnumerator PublishMessage(string publishMsg, float seconds) {
        yield return new WaitForSeconds(seconds);
        mqttPublisher.Publish(publishMsg);
        // Debug.Log("Sent result:" + publishMsg);     
    }

}