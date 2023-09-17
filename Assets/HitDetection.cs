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

    private class GameState
    {
        public string player_id;
        public string action;
        // string for now
        public string gamestate;
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
        GameState x = JsonUtility.FromJson<GameState>(newMsg);

        // do stuff with gamestate

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

        // for now we assume that the player is player 1.
        if (res.player_id == "1") {
            switch (res.action) {
                case "grenade":
                    grenadeControl.ThrowGrenade();
                    opponentHealth.GrenadeReceived();
                    care.OnPlayerGrenadeButtonPressed();
                    StartCoroutine(PublishMessage(publishMsg, 2f));
                    break;
                
                case "spear":
                    opponentHealth.SpearReceived();
                    care.OnPlayerSpearButtonPressed();
                    StartCoroutine(PublishMessage(publishMsg, 1f));
                    break;

                case "shield":
                    playerState.ActivateShield();
                    care.OnPlayerShieldButtonPressed();
                    mqttPublisher.Publish(publishMsg);
                    break;

                case "hammer":
                    opponentHealth.HammerReceived();
                    care.OnPlayerHammerButtonPressed();
                    StartCoroutine(PublishMessage(publishMsg, 1f));
                    break;

                default:
                    StartCoroutine(PublishMessage(publishMsg, 1f));
                    break;
            }
        } else {
            switch (res.action) {
                case "grenade":
                    playerState.GrenadeReceived();
                    care.OnOpponentGrenadeButtonPressed();
                    StartCoroutine(PublishMessage(publishMsg, 2f));
                    break;
                
                case "spear":
                    playerState.SpearReceived();
                    care.OnOpponentSpearButtonPressed();
                    StartCoroutine(PublishMessage(publishMsg, 1f));
                    break;

                case "shield":
                    opponentHealth.ActivateShield();
                    care.OnOpponentShieldButtonPressed();
                    mqttPublisher.Publish(publishMsg);;
                    break;

                case "hammer":
                    playerState.HammerReceived();
                    care.OnOpponentHammerButtonPressed();
                    StartCoroutine(PublishMessage(publishMsg, 1f));
                    break;

                default:
                    StartCoroutine(PublishMessage(publishMsg, 1f));
                    break;
            }
        }
 

    }

    private IEnumerator PublishMessage(string publishMsg, float seconds) {
        yield return new WaitForSeconds(seconds);
        mqttPublisher.Publish(publishMsg);
        // Debug.Log("Sent result:" + publishMsg);     
    }

}