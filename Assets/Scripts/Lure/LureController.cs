using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureController : MonoBehaviour
{
	#region EVENTS
	public static event Action E_LandedOnWater;
	public static event Action E_LandedOnGround;
	#endregion
	#region VARIABLES
	[Header("COMPONENTS")]
	[SerializeField] private Rigidbody _rigidbody;
	#endregion

	private void OnEnable()
	{
		TouchManager.E_LurePulled += HandleLurePulled;
		FishManager.E_FishDenied += HandleFishDenied; //temporary
		FishManager.E_FishGranted += HandleFishGranted; //temporary

		FishingRodController.E_LureLanded += HandleLureLanded;

		TagChecker.E_LureLandedOnWater += HandleLandedOnWater;
	}

	private void OnDisable()
	{
		TouchManager.E_LurePulled -= HandleLurePulled;
		FishManager.E_FishDenied -= HandleFishDenied; //temporary
		FishManager.E_FishGranted -= HandleFishGranted; //temporary

        FishingRodController.E_LureLanded -= HandleLureLanded;

        TagChecker.E_LureLandedOnWater -= HandleLandedOnWater;
    }

	//test method not sure about it yet
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Water") || collision.gameObject.CompareTag("Ground"))
		{
			_rigidbody.isKinematic = true;
			//Destroy(gameObject);
		}
		//if (GameManager.Instance.IsCheating())
		//{
		//	if (collision.gameObject.CompareTag("Water"))
		//	{
		//		_rigidbody.isKinematic = true;
		//		E_LandedOnWater?.Invoke();
		//	}
		//	else if (collision.gameObject.CompareTag("Ground"))
		//	{
		//		E_LandedOnGround?.Invoke();
		//		Destroy(gameObject);
		//	}
		//}
	}

	#region EVENT HANDLERS
	private void HandleLurePulled()
	{
		Destroy(gameObject);
	}

	//temporary
	private void HandleFishGranted(string fishName, string fishQuality)
	{
		Destroy(gameObject);
	}

	//temporary
	private void HandleFishDenied()
	{
		Destroy(gameObject);
	}

	private void HandleLureLanded(double unused1, double unused2)
	{
		_rigidbody.isKinematic = true;
	}

	private void HandleLandedOnWater(bool isWater)
	{
		if (!isWater)
		{
			Destroy(gameObject);
		}
	}
	#endregion
}
