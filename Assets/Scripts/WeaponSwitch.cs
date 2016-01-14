using UnityEngine;
using System.Collections;

public enum ETimeManip {
	GLOBAL,
	LOCAL,
	SELF,
	NONE
}

public class WeaponSwitch : MonoBehaviour {

	public UIColorChange wepColorChange;
	public Camera playerCam;

	private ETimeManip _currentManip;
	public ETimeManip currentManip
	{
		get { return _currentManip; }
		set
		{
			_currentManip = value;
			switch (value)
			{
				case ETimeManip.GLOBAL:
					equipManip(GetComponent<TimeManipGlobal>());
					break;
				case ETimeManip.LOCAL:
					equipManip(GetComponent<TimeManipLocal>());
					break;
				case ETimeManip.SELF:
					equipManip(GetComponent<TimeManipSelf>());
					break;
				default:
					currentEquipped = null;
					break;
			}
		}
	}
	private TimeManip currentEquipped;

	// Use this for initialization
	void Start () {
		currentManip = ETimeManip.GLOBAL;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Equip 1"))
		{
			currentManip = ETimeManip.GLOBAL;
		}
		else if (Input.GetButtonDown("Equip 2"))
		{
			currentManip = ETimeManip.LOCAL;
		}
		else if (Input.GetButtonDown ("Equip 3"))
		{
			currentManip = ETimeManip.SELF;
		}

		if (currentEquipped != null)
		{
			currentEquipped.Fire();
		}

		if (Input.GetButtonDown("Activate"))
		{
			currentEquipped.Activate(playerCam);
		}

		if (Input.GetButtonDown("Deactivate"))
		{
			currentEquipped.Deactivate(playerCam);
		}
	}

	private void equipManip(TimeManip manip)
	{
		currentEquipped = manip;
		currentEquipped.OnEquip(wepColorChange);
	}
}
