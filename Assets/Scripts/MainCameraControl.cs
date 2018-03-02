using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraControl : MonoBehaviour {
	private GameObject player = null;
	private Vector3 offset = Vector3.zero;

	Vector3 newPosition;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		newPosition = player.transform.position + offset;
        transform.position = newPosition;
		// transform.position = Vector3.Lerp(transform.position, newPosition, 5.0f * Time.deltaTime);
		transform.LookAt(player.transform.position);

	}
}
