using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour {

	public CharacterCamera mouseLook;
	public TimeObject timeObject;
	public Camera cameraObject;
	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;

	private CharacterController controller;
	private Vector3 moveDirection = Vector3.zero;

	void Start()
	{
		mouseLook.Init (transform, cameraObject.transform);
		controller = GetComponent<CharacterController>();
	}

	void Update()
	{
		if (controller.isGrounded)
		{
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;

			if (Input.GetButton("Jump"))
				moveDirection.y = jumpSpeed;
		}
		moveDirection.y -= gravity * Time.deltaTime * timeObject.timeScale;
		controller.Move(moveDirection * Time.deltaTime * timeObject.timeScale);

		mouseLook.LookRotation (transform, cameraObject.transform);
	}
}
