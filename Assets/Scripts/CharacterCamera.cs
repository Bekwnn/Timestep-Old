using UnityEngine;
using System;

[Serializable]
public class CharacterCamera
{

	public float sensitivityX;
	public float sensitivityY;
	public bool clampVerticalRotation;
	public float minimumX;
	public float maximumX;
	public bool smooth;
	public float smoothTime;
	
	private Quaternion m_CharacterTargetRot;
	private Quaternion m_CameraTargetRot;

	public void Init(Transform character, Transform camera)
	{
		m_CharacterTargetRot = character.localRotation;
		m_CameraTargetRot = character.localRotation;
	}

	public void LookRotation(Transform character, Transform camera)
	{
		float yRot = Input.GetAxis ("Mouse X") * sensitivityX;
		float xRot = Input.GetAxis ("Mouse Y") * sensitivityY;

		m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
		m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);

		if (clampVerticalRotation)
			m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

		if (smooth)
		{
			character.localRotation = Quaternion.Slerp(
				character.localRotation, m_CharacterTargetRot, smoothTime * Time.deltaTime
			);
			camera.localRotation = Quaternion.Slerp(
				camera.localRotation, m_CameraTargetRot, smoothTime * Time.deltaTime
			);
		}
		else
		{
			character.localRotation = m_CharacterTargetRot;
			camera.localRotation = m_CameraTargetRot;
		}
	}

	Quaternion ClampRotationAroundXAxis(Quaternion q)
	{
		q.x /= q.w;
		q.y /= q.w;
		q.z /= q.w;
		q.w = 1.0f;

		float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

		angleX = Mathf.Clamp (angleX, minimumX, maximumX);

		q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

		return q;
	}
}
