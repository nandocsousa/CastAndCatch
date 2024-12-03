using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//all possible game states
public enum GameStates { Menu, Fishing};

//main game manager class
public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;
	#region EVENTS
	#endregion
	#region VARIABLES
	[Header("CHEATS")]
	[SerializeField] private bool _isCheating = true; //this is for debugging and testing purposes

	[Header("VALUES")]
	[SerializeField] private float _totalMoney = 0f; //the total money the player has
	#endregion

	private void OnEnable()
	{
		FishingRodController.E_FishBiteTimerOver += HandleFishBiteTimerOver;
		FishManager.E_FishDenied += HandleFishDenied;
		FishManager.E_FishGranted += HandleFishGranted;
	}

	private void OnDisable()
	{
		FishingRodController.E_FishBiteTimerOver -= HandleFishBiteTimerOver;
		FishManager.E_FishDenied -= HandleFishDenied;
		FishManager.E_FishGranted -= HandleFishGranted;
	}

	private void Awake()
	{
		#region SINGLETON
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		DontDestroyOnLoad(gameObject);
		#endregion
	}

	#region EVENT HANDLERS
	//just for debugging
	private void HandleFishBiteTimerOver()
	{
		Debug.Log("THE TIMER ENDED");
	}

	//not finalized
	private void HandleFishGranted(string fishName, string fishQuality)
	{
		Debug.Log("Nice catch! It's a " + fishName + " of " + fishQuality + " quality!");
	}

	//not finalized
	private void HandleFishDenied()
	{
		Debug.Log("Bad luck, couldn't catch anything :(");
	}
	#endregion

	#region EXTERNAL CALLABLES
	//check if the player is cheating (devs only)
	public bool IsCheating()
	{
		return _isCheating;
	}

	//get how much money the player has
	public float GetTotalMoney()
	{
		return _totalMoney;
	}

	public void SendLureCoordinates(double longitude, double latitude)
	{

	}
	#endregion
}
