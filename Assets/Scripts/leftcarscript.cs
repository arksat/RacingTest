using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BLINDED_AM_ME;


public class leftcarscript : MonoBehaviour {

	Vector3[] nodeData;
	Vector3 pos;

	float mainCarPosition = 0;

	GameObject mainBase;

	// Use this for initialization
	void Start () {
		mainBase = GameObject.Find("base_right");
	}
	
	// Update is called once per frame
	void Update () {

		mainCarPosition = mainBase.GetComponent<carscript>().PositionPercent;

		if(mainCarPosition>1.0f) mainCarPosition = 0.0f;
		if(mainCarPosition<0.0f) mainCarPosition = 1.0f;

		pos = mainBase.GetComponent<carscript>().GetPointOnPath(mainCarPosition + 0.02f);
		iTween.MoveUpdate(gameObject, iTween.Hash("position", pos, "orienttopath", true, "time", 0.2f));
	}
}
