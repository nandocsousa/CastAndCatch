using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Android;

//all possible game states
public enum GameStates { Menu, Fishing};

//main game manager class
public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;
	#region EVENTS
	public static event Action<double, double> E_SendLureLandCoords;
	#endregion
	#region VARIABLES
	[Header("CHEATS")]
	[SerializeField] private bool _isCheating = true; //this is for debugging and testing purposes

	[Header("VALUES")]
	[SerializeField] private float _totalMoney = 0f; //the total money the player has

	[Header("GPS")]
	[SerializeField] private bool _locationIsReady = false;
	[SerializeField] private bool _locationGrantedAndroid = false;
	private double _playerLatitude;
	private double _playerLongitude;
	private double _lureLandedLatitude = 0;
	private double _lureLandedLongitude = 0;

	private GameObject _dialog = null;

	//public TextMeshProUGUI _text;
	#endregion

	private void OnEnable()
	{
		FishingRodController.E_FishBiteTimerOver += HandleFishBiteTimerOver;
		FishManager.E_FishDenied += HandleFishDenied;
		FishManager.E_FishGranted += HandleFishGranted;

		FishingRodController.E_LureLanded += HandleLureLanded;

		TagChecker.E_LureLandedOnWater += testLandWater;
	}

	private void OnDisable()
	{
		FishingRodController.E_FishBiteTimerOver -= HandleFishBiteTimerOver;
		FishManager.E_FishDenied -= HandleFishDenied;
		FishManager.E_FishGranted -= HandleFishGranted;

		FishingRodController.E_LureLanded -= HandleLureLanded;

		TagChecker.E_LureLandedOnWater -= testLandWater;
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

	private void Start()
	{
		#region GPS
		if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
		{
			Permission.RequestUserPermission(Permission.FineLocation);
			_dialog = new GameObject();
		}
		else
		{
			_locationGrantedAndroid = true;
			_locationIsReady = NativeGPSPlugin.StartLocation();
		}
		#endregion
	}

	private void Update()
	{
		#region GPS
		if (_locationIsReady)
		{
			_playerLatitude = NativeGPSPlugin.GetLatitude();
			_playerLongitude = NativeGPSPlugin.GetLongitude();
		}
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

	private void HandleLureLanded(double latitude, double longitude)
	{
		_lureLandedLatitude = latitude;
		_lureLandedLongitude = longitude;
		E_SendLureLandCoords?.Invoke(latitude, longitude);
	}

	private void testLandWater(bool onWater)
	{
		if (onWater)
		{
			//_text.text = "LANDED ON WATER TAG";
		}
		else
		{
			//_text.text = "LANDED ON GROUND TAG";
		}
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

	public double GetPlayerLatitude()
	{
		return _playerLatitude;
	}

	public double GetPlayerLongitude()
	{
		return _playerLongitude;
	}

	public double GetLureLatitude()
	{
		return _lureLandedLatitude;
	}

	public double GetLureLongitude()
	{
		return _lureLandedLongitude;
	}
	#endregion
}
