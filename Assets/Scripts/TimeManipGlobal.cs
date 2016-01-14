using UnityEngine;
using System.Collections;

public class TimeManipGlobal : MonoBehaviour, TimeManip {
	public Color color = Color.magenta;

	public void Fire()
	{
		TimeManager.global.timeScale = Input.GetAxis("TimeAxis");
	}

	public void Activate(Camera playerCam)
	{

	}

	public void Deactivate(Camera playerCam)
	{

	}

	public void OnEquip(UIColorChange uiElem)
	{
		uiElem.ChangeImages (color);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
