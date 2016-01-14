using UnityEngine;
using System.Collections;

interface TimeManip {

	void Fire();
	void Activate(Camera playerCam);
	void Deactivate(Camera playerCam);
	void OnEquip(UIColorChange uiElem);
}
