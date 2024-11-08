using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraController : MonoBehaviour
{
	private void Start()
	{
		if (SystemInfo.supportsGyroscope)
		{
			Input.gyro.enabled = true;
		}
	}

	private void Update()
	{
		if (SystemInfo.supportsGyroscope)
		{
			//get the rotation from the gyroscope
			Quaternion deviceRotation = Input.gyro.attitude;

			//adjust to Unity’s coordinate system for portrait mode
			Quaternion rotationFix = Quaternion.Euler(90, 0, 0); //this corrects the orientation for portrait
			transform.localRotation = rotationFix * deviceRotation * Quaternion.Euler(0, 0, 180);
		}
	}
}
