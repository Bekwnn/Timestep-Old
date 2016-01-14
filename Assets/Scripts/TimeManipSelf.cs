using UnityEngine;
using System.Collections;

public class TimeManipSelf : MonoBehaviour, TimeManip {
	public float timeLeapDistance = 1f;
	public Color color = Color.yellow;
	public TimeObject timeObject;
	private float selfFreezeTimer;

	public void Fire()
	{
		//move controls to here?
	}
	
	public void Activate(Camera playerCam)
	{
		timeObject.localTimeScale = 0f;
		selfFreezeTimer = timeLeapDistance;
		TimeManager.global.timeScale = 1f;
	}
	
	public void Deactivate(Camera playerCam)
	{
		//TODO: make gradual?
		float before = timeObject.time;
		timeObject.RewindBy (timeLeapDistance);
		Debug.Log ("Before: " + before + ", After: " + timeObject.time);
	}
	
	public void OnEquip(UIColorChange uiElem)
	{
		uiElem.ChangeImages (color);
	}

	void Unfreeze()
	{
		timeObject.localTimeScale = 1f;
		TimeManager.global.timeScale = 0f;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		selfFreezeTimer -= Time.deltaTime;
		if (selfFreezeTimer <= 0f)
		{
			Unfreeze();
		}
	}
}
