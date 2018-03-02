using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BLINDED_AM_ME;

public class carscript : MonoBehaviour
{
	Vector3 pos;
	Camera mainCamera;
	public Camera driverCamera;
	float positionPercent = 0;
	bool isFirstPersonMode = false;
	bool isOrientationOverride = false;

	Touch touch;

	private Vector3[] nodeData;

	public Vector3[] NodeData
	{
		get { return nodeData; }
		private set { nodeData = value; }
	}

	public float PositionPercent
	{
		get { return positionPercent; }
		set { positionPercent = value; }
	}

	void Start ()
	{
		SetGUIStyle();

		// obtain tracking path
		NodeData = GameObject.Find("Track").GetComponent<Spline3D>().GetTrackPoints();

		// draw path line for debugging
		if (GameObject.Find("Track").GetComponent<LineRenderer>().enabled) DrawPath();

		mainCamera = Camera.main;
		SwitchCamera(isFirstPersonMode);
		
		#if UNITY_ANDROID
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.autorotateToPortrait = true;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.AutoRotation;
		#endif
	}

	void Update ()
	{
		UpdateUI();
		DetectOrientationChange();

		// transform.position = iTween.PointOnPath(NodeData, PositionPercent); // Get target position 
		pos = iTween.PointOnPath(nodeData, PositionPercent);
		iTween.MoveUpdate(gameObject, iTween.Hash("position",pos, "orienttopath",true, "time", 0.2f));
	}

	public Vector3 GetPointOnPath(float pos)
	{
		return iTween.PointOnPath(nodeData, pos);
	}

	void DrawPath()
	{
		LineRenderer line = GameObject.Find("Track").GetComponent<LineRenderer>();
		line.positionCount = nodeData.Length;
		line.SetPositions(nodeData);
		Debug.Log("line segments: "+ line.positionCount);
	}

	void DetectOrientationChange()
	{
		#if UNITY_EDITOR
		return;
		#endif

		if (isOrientationOverride) return;

		switch (Screen.orientation)
		{
			case ScreenOrientation.Portrait:
				isFirstPersonMode = false;
				SwitchCamera(isFirstPersonMode);
				break;

			case ScreenOrientation.LandscapeLeft:
			case ScreenOrientation.LandscapeRight:
				isFirstPersonMode = true;
				SwitchCamera(isFirstPersonMode);
				break;
		}
	}

	const float addSpeed = 0.002f;
	void Drive_Forward()
	{
		PositionPercent += addSpeed;
		if(PositionPercent>1.0f) PositionPercent = 0.0f;
	}
	void Drive_Back()
	{
		PositionPercent -= addSpeed * 0.5f;
		if(PositionPercent<0.0f) PositionPercent = 1.0f;
	}
	
	void UpdateUI()
	{
		bool drive_Fwd = false;
		bool drive_Bwd = false;
		
		bool input_Key_Space = Input.GetKey(KeyCode.Space);
		bool input_Key_R = Input.GetKey(KeyCode.R);
		if( !(input_Key_Space & input_Key_R) ){
			drive_Fwd |= input_Key_Space;
			drive_Bwd |= input_Key_R;
		}

		#if UNITY_ANDROID
		float accel = Input.acceleration.z;
		if(accel < -0.2f){
			drive_Fwd |= true;
		}
		else if(accel > 0.2f){
			drive_Bwd = true;
		}
		#endif
		
		#if UNITY_EDITOR
		drive_Fwd = true;
		#endif

		if (drive_Fwd) Drive_Forward();
		if (drive_Bwd) Drive_Back();

		if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.C))
		{
			isFirstPersonMode = isFirstPersonMode ? false : true;
			isOrientationOverride = true;
			SwitchCamera(isFirstPersonMode);
		}
	}

	void SwitchCamera(bool firstpersonmode)
	{
			mainCamera.enabled = firstpersonmode ? false : true;
			driverCamera.enabled = firstpersonmode ? true : false;
	}

	GUIStyle labelStyle;
	void SetGUIStyle()
	{
		labelStyle = new GUIStyle();
		labelStyle.fontSize = 20;
		labelStyle.normal.textColor = Color.white;
		labelStyle.wordWrap = true;
	}

	void OnGUI()
	{
		string text;
		text = "\nPos "+PositionPercent.ToString("0.00%");

		#if UNITY_EDITOR
		text += "\nSpace:gas, R:reverse, C:camera";
		#endif

		#if UNITY_ANDROID
		text += "\n AccelX " + Input.acceleration.x.ToString("0.00");
		text += "\n AccelY " + Input.acceleration.y.ToString("0.00");
		text += "\n AccelZ " + Input.acceleration.z.ToString("0.00");
		#endif

		GUI.Label(new Rect(0, 0, Screen.width-100, Screen.height-100), text, labelStyle);
	}
}
