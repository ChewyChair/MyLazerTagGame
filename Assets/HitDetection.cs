using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public MqttPublisher mqttPublisher;
    public MqttReceiver mqttReceiver;
    public CustomAREffects care;
    public GrenadeController grenadeControl;

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
        Debug.Log("Checking for hit:" + newMsg + "HIT");
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

        switch (res.action) {
            case "grenade":
                grenadeControl.ThrowGrenade();
                care.OnGrenadeButtonPressed();
                StartCoroutine(PublishMessage(publishMsg, 2f));
                break;
            
            case "spear":
                grenadeControl.ThrowGrenade();
                care.OnGrenadeButtonPressed();
                StartCoroutine(PublishMessage(publishMsg, 1f));
                break;

            default:
                StartCoroutine(PublishMessage(publishMsg, 1f));
                break;
        }

    }

    private IEnumerator PublishMessage(string publishMsg, float seconds) {
        yield return new WaitForSeconds(seconds);
        mqttPublisher.Publish(publishMsg);
    }

}