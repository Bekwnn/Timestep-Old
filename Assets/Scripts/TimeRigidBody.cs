using UnityEngine;
using System.Collections;

public class TimeRigidBody {

	public TimeObject timeObject;
	public Rigidbody rigidBody;
	private bool timeWasForward;

	//real 1:1 object velocities used for restoring the rigidbody velocities when time resumes going forward
	public Vector3 realVelocity;
	public Vector3 realAngularVelocity;

	public TimeRigidBody(TimeObject timeObject, Rigidbody rigidBody)
	{
		this.timeObject = timeObject;
		this.rigidBody = rigidBody;
	}

	public void FreezeRigidBody ()
	{
		rigidBody.velocity = Vector3.zero;
		rigidBody.angularVelocity = Vector3.zero;

		timeWasForward = true;
	}

	public void FixedRigidBodyForwardUpdate()
	{
		//TODO: fix physics during time acceleration/decceleration

		if (timeWasForward)
		{
			rigidBody.velocity = realVelocity * timeObject.timeScale;
			rigidBody.angularVelocity = realAngularVelocity * timeObject.timeScale;
		}
		else
		{
			float ratio = timeObject.timeScale / timeObject.pastTimeScale;
			rigidBody.velocity *= ratio;
			rigidBody.angularVelocity *= ratio;

			realVelocity = rigidBody.velocity / timeObject.timeScale;
			realAngularVelocity = rigidBody.angularVelocity / timeObject.timeScale;
		}

		rigidBody.velocity += Physics.gravity * timeObject.timeScale * Time.fixedDeltaTime;

		timeWasForward = false;
	}
}
