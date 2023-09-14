using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    public MqttPublisher mqttPublisher;
    public MqttReceiver mqttReceiver;

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
        res.isHit = true;

        string publishMsg = JsonUtility.ToJson(res);
        mqttPublisher.Publish(publishMsg);
    }

}