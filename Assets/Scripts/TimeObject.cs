using UnityEngine;
using System;
using System.Collections.Generic;

public class TimeObject : MonoBehaviour {

	public Rigidbody rigidBody;
	public int capturePerSecond = 30;

	public bool bOnLocalTime;	//determines whether object is on its own timescale
	public float localTimeScale;

	public float localTimeMultiplier = 1f;	//update and rewind are multiplied by this factor
	
	public float pastTimeScale { get; private set; } //time scale during last fixed update
	public float time { get; set; }
	public float timeScale { get; private set; } //either the local time scale or global for a time object

	private TimeRigidBody timeRigidBody;

	private TimeHistory<Vector3> positionHistory;
	private TimeHistory<Quaternion> rotationHistory;
	private TimeHistory<Vector3> scaleHistory;

	private TimeHistory<Vector3> realVelocityHistory;
	private TimeHistory<Vector3> realAngularVelocityHistory;

	// Use this for initialization
	void Start ()
	{
		//check for a rigid body and if there is one create a timeRigidBody
		if (!rigidBody) rigidBody = GetComponent<Rigidbody>();
		if (rigidBody) timeRigidBody = new TimeRigidBody(this, rigidBody);

		//initialize timeScale
		timeScale = (bOnLocalTime)? localTimeScale : TimeManager.global.timeScale;

		positionHistory = new TimeHistory<Vector3>(transform.position, capturePerSecond);
		rotationHistory = new TimeHistory<Quaternion>(transform.rotation, capturePerSecond);
		scaleHistory = new TimeHistory<Vector3>(transform.localScale, capturePerSecond);

		if (timeRigidBody != null)
		{
			realVelocityHistory = new TimeHistory<Vector3>(Vector3.zero, capturePerSecond);
			realAngularVelocityHistory = new TimeHistory<Vector3>(Vector3.zero, capturePerSecond);
		}
	}

	//TODO: create coroutine 
	void FixedUpdate()
	{
		timeScale = (bOnLocalTime)? localTimeScale : TimeManager.global.timeScale;
		timeScale *= localTimeMultiplier;

		time += Time.fixedDeltaTime * timeScale;
		time = (time <= 0f) ? 0 : time;

		if (timeScale > 0f)
		{
			if (timeRigidBody != null) timeRigidBody.FixedRigidBodyForwardUpdate();
			ForwardUpdate();
		}

		else if (timeScale <= 0f)
		{
			if (timeRigidBody != null) timeRigidBody.FreezeRigidBody();

			if (timeScale < 0f)
				RewindUpdate();
		}

		pastTimeScale = timeScale;
	}

	void ForwardUpdate () {

		TimeHistory<Vector3>.ForwardUpdate    (positionHistory, time, transform.position);
		TimeHistory<Quaternion>.ForwardUpdate (rotationHistory, time, transform.rotation);
		TimeHistory<Vector3>.ForwardUpdate    (scaleHistory,    time, transform.localScale);

		//update rigidbody info
		if (timeRigidBody != null)
		{
			TimeHistory<Vector3>.ForwardUpdate(realVelocityHistory,        time, timeRigidBody.realVelocity);
			TimeHistory<Vector3>.ForwardUpdate(realAngularVelocityHistory, time, timeRigidBody.realAngularVelocity);
		}

		//TODO: update animation info
	}

	void RewindUpdate()
	{
		TimeInfo<Vector3> posTimeInfo = TimeHistory<Vector3>.RewindTo (positionHistory, time);	//used to set object's time

		//rewind transform
		transform.position   = posTimeInfo.data;
		transform.rotation   = TimeHistory<Quaternion>.RewindTo (rotationHistory, time).data;
		transform.localScale = TimeHistory<Vector3>.RewindTo    (scaleHistory,    time).data;

		//rewind rigidbody
		if (timeRigidBody != null)
		{
			timeRigidBody.realVelocity        = TimeHistory<Vector3>.RewindTo(realVelocityHistory,        time).data;
			timeRigidBody.realAngularVelocity = TimeHistory<Vector3>.RewindTo(realAngularVelocityHistory, time).data;
		}

		//TODO: rewind animation

		time = posTimeInfo.time;
	}

	public void RewindBy(float seconds)
	{
		float clampedSeconds = (seconds >= 0f)? seconds : 0f;	//function isn't meant for fastforwarding'

		TimeInfo<Vector3> posTimeInfo = TimeHistory<Vector3>.RewindTo (positionHistory, time - clampedSeconds);	//used to set object's time

		//rewind transform
		transform.position   = posTimeInfo.data;
		transform.rotation   = TimeHistory<Quaternion>.RewindTo (rotationHistory, time - clampedSeconds).data;
		transform.localScale = TimeHistory<Vector3>.RewindTo    (scaleHistory,    time - clampedSeconds).data;
		
		//rewind rigidbody
		if (timeRigidBody != null)
		{
			timeRigidBody.realVelocity        = TimeHistory<Vector3>.RewindTo(realVelocityHistory,        time - clampedSeconds).data;
			timeRigidBody.realAngularVelocity = TimeHistory<Vector3>.RewindTo(realAngularVelocityHistory, time - clampedSeconds).data;
		}
		
		//TODO: rewind animation
		time = posTimeInfo.time;
	}
}
