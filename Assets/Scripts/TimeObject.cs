using UnityEngine;
using System;
using System.Collections.Generic;

public struct TimeInfo<T> {
	public T data;
	public float time;
	
	public TimeInfo(T value, float time)
	{
		data = value;
		this.time = time;
	}

	//TODO: increase error margin for the comparison?
	public static bool operator == (TimeInfo<T> lhs, TimeInfo<T> rhs)
	{
		return (lhs.data.Equals(rhs.data));
	}

	public static bool operator != (TimeInfo<T> lhs, TimeInfo<T> rhs)
	{
		return !(lhs.data.Equals(rhs.data));
	}
}

public class TimeObject : MonoBehaviour {

	public Rigidbody rigidBody;

	public bool bOnLocalTime;	//determines whether object is on its own timescale
	public float localTimeScale;

	public float localTimeMultiplier = 1f;	//update and rewind are multiplied by this factor
	
	public float pastTimeScale { get; private set; } //time scale during last fixed update
	public float time { get; private set; }
	public float timeScale { get; private set; } //the complete, local time scale for a time object

	private TimeRigidBody timeRigidBody;

	private List<TimeInfo<Vector3>> positionHistory = new List<TimeInfo<Vector3>>();
	private List<TimeInfo<Quaternion>> rotationHistory = new List<TimeInfo<Quaternion>>();
	private List<TimeInfo<Vector3>> scaleHistory = new List<TimeInfo<Vector3>>();

	private List<TimeInfo<Vector3>> realVelocityHistory = new List<TimeInfo<Vector3>>();
	private List<TimeInfo<Vector3>> realAngularVelocityHistory = new List<TimeInfo<Vector3>>();

	// Use this for initialization
	void Start ()
	{
		//check for a rigid body and if there is one create a timeRigidBody
		if (!rigidBody) rigidBody = GetComponent<Rigidbody>();
		if (rigidBody) timeRigidBody = new TimeRigidBody(this, rigidBody);

		//initialize timeScale
		timeScale = (bOnLocalTime)? localTimeScale : TimeManager.global.timeScale;

		//starting info for histories
		positionHistory.Add(new TimeInfo<Vector3>(transform.position, this.time));
		rotationHistory.Add(new TimeInfo<Quaternion>(transform.rotation, this.time));
		scaleHistory.Add(new TimeInfo<Vector3>(transform.localScale, this.time));

		if (timeRigidBody != null)
		{
			realVelocityHistory.Add(new TimeInfo<Vector3>(timeRigidBody.rigidBody.velocity, this.time));
			realAngularVelocityHistory.Add(new TimeInfo<Vector3>(timeRigidBody.rigidBody.angularVelocity, this.time));
		}
	}

	//time manip done here to control memory use
	void FixedUpdate()
	{
		timeScale = (bOnLocalTime)? localTimeScale : TimeManager.global.timeScale;
		timeScale *= localTimeMultiplier;

		time += Time.fixedDeltaTime * timeScale;
		time = (time <= 0f) ? 0 : time;

		if (timeScale > 0f)
		{
			timeRigidBody.FixedRigidBodyForwardUpdate();
			ForwardUpdate();
		}

		else if (timeScale <= 0f)
		{
			timeRigidBody.FreezeRigidBody();

			if (timeScale < 0f)
				RewindUpdate();
		}

		pastTimeScale = timeScale;
	}

	void ForwardUpdate () {
		//update transform info
		TimeInfo<Vector3> currentPositionInfo = new TimeInfo<Vector3>(transform.position, this.time);
		TimeInfo<Quaternion> currentRotationInfo = new TimeInfo<Quaternion>(transform.rotation, this.time);
		TimeInfo<Vector3> currentScaleInfo = new TimeInfo<Vector3>(transform.localScale, this.time);

		UpdateList<Vector3>(currentPositionInfo, positionHistory);
		UpdateList<Quaternion>(currentRotationInfo, rotationHistory);
		UpdateList<Vector3>(currentScaleInfo, scaleHistory);

		//update rigidbody info
		if (timeRigidBody != null)
		{
			TimeInfo<Vector3> currentRealVelocityInfo = new TimeInfo<Vector3> (timeRigidBody.realVelocity, this.time);
			TimeInfo<Vector3> currentRealAngularVelocityInfo = new TimeInfo<Vector3> (timeRigidBody.realAngularVelocity, this.time);
		
			UpdateList<Vector3>(currentRealVelocityInfo, realVelocityHistory);
			UpdateList<Vector3>(currentRealAngularVelocityInfo, realAngularVelocityHistory);
		}

		//TODO: update animation info
	}

	void RewindUpdate()
	{
		//rewind transform
		transform.position   = RewindHistory<Vector3>(positionHistory).data;
		transform.rotation   = RewindHistory<Quaternion>(rotationHistory).data;
		transform.localScale = RewindHistory<Vector3>(scaleHistory).data;

		//rewind rigidbody
		if (timeRigidBody != null)
		{
			timeRigidBody.realVelocity        = RewindHistory<Vector3>(realVelocityHistory).data;
			timeRigidBody.realAngularVelocity = RewindHistory<Vector3>(realAngularVelocityHistory).data;
		}

		//TODO: rewind animation
	}

	void UpdateList<T>(TimeInfo<T> newInfo, List<TimeInfo<T>> historyList)
	{
		if (!newInfo.data.Equals(historyList[historyList.Count - 1].data))
		{
			historyList.Add(newInfo);
		}
		else
		{
			TimeInfo<T> tempinfo = historyList[historyList.Count - 1];
			tempinfo.time = time;
			historyList[historyList.Count - 1] = tempinfo;
		}
	}

	// returns the relevant time info from the history list
	TimeInfo<T> RewindHistory<T>(List<TimeInfo<T>> historyList)
	{
		TimeInfo<T> newTimeInfo = historyList[historyList.Count - 1];
		while (historyList.Count > 1 &&
		       time < historyList[historyList.Count - 2].time)
		{
			newTimeInfo = historyList[historyList.Count - 2];
			historyList.RemoveAt(historyList.Count - 1);
		}
		newTimeInfo.time = time;
		return newTimeInfo;
	}
}
