using UnityEngine;
using System.Collections;

public class TimeManipLocal : MonoBehaviour, TimeManip {
	public Color color = Color.cyan;
	private TimeObject currentTarget;

	public void Fire()
	{
		if (currentTarget)
		{
			currentTarget.localTimeScale = Input.GetAxis("TimeAxis");
		}
	}
	
	public void Activate(Camera playerCam)
	{
		RaycastHit hit;
		if (Physics.Raycast (playerCam.transform.position, playerCam.transform.forward, out hit))
		{
			TimeObject tempTO = hit.collider.gameObject.GetComponent<TimeObject>();
			if (tempTO)
			{
				currentTarget = tempTO;
				currentTarget.bOnLocalTime = true;
				currentTarget.localTimeScale = 0f;
			}
		}
	}
	
	public void Deactivate(Camera playerCam)
	{
		RaycastHit hit;
		if (Physics.Raycast (playerCam.transform.position, playerCam.transform.forward, out hit))
		{
			TimeObject tempTO = hit.collider.gameObject.GetComponent<TimeObject>();
			if (tempTO)
			{
				tempTO.bOnLocalTime = false;
				tempTO.localTimeScale = 0f;
			}
		}
	}

	public void OnEquip(UIColorChange uiElem)
	{
		uiElem.ChangeImages (Color.cyan);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
