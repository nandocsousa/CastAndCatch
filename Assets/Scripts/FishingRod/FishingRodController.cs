using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//all the different possible fishing states
public enum FishingStates { Idle, Casting, Waiting, Pulling, Paused}

//the main fishing rod class
public class FishingRodController : MonoBehaviour
{
	#region EVENTS
	public static event Action E_FishBiteTimerOver;
	public static event Action<double, double> E_LureLanded;
	#endregion
	#region VARIABLES
	[Header("COMPONENTS")]
	[SerializeField] private Transform _lureSpawnPoint; //where the lure is spawned
	[SerializeField] private GameObject _lurePrefab; //the lure GameObject
	[SerializeField] private Rigidbody _lureRigidbody; //the lure rigidbody
	[SerializeField] private LineRenderer _lureLineRenderer; //line renderer between the rod and the lure
	private GameObject _currentLure; //the current active lure
	public TextMeshProUGUI _statetext; //temporary remove after

	[Header("VALUES")]
	[SerializeField] private float _minLaunchSpeed = 1.5f; //minimum speed to start launching lure
	[SerializeField] private float _stopLaunchThresholdSpeed = 1f; //if the player reaches this speed the launch is over
	[SerializeField] private float _peakAcceleration = 0f; //the peak acceleration reached while casting the lure
	private float _launchForceMultiplier = 10f; //multiplier for the launch force
	[SerializeField] private float _minWaitTime = 2f; //minimum wait time for a fish to bite
	[SerializeField] private float _maxWaitTime = 5f; //maximum wait time for a fish to bite
	[SerializeField] private float _randomWaitTime = 0f;
	private float _metersPerUnit = 0.2f; //scale factor for converting Unity units into meters
	private const double _EarthRadius = 6378137.0; //earth radius in meters approximated

	[Header("GPS")]
	[SerializeField] private double _playerLat;
	[SerializeField] private double _playerLon;

	private float _yThresholdOffset = -20f; //offset for checking when the lure is below a certain position
	private float _initialYPosition; //the initial y pos of the lure

	private bool _isCasting = false; //checks when the player is trying to cast a lure
	private FishingStates _currentState = FishingStates.Idle; //initial state is idle
	#endregion

	private void Awake()
	{
		//enable gyroscope input if available
		if (SystemInfo.supportsGyroscope)
		{
			Input.gyro.enabled = true;
		}
	}

	private void OnEnable()
	{
		TagChecker.E_LureLandedOnWater += HandleLandedOnWater;
	}

	private void OnDisable()
	{
		TagChecker.E_LureLandedOnWater -= HandleLandedOnWater;
	}

	private void Start()
    {
		_initialYPosition = _lureSpawnPoint.position.y;
    }

    private void Update()
	{
		_statetext.text = _currentState.ToString();

		_playerLat = GameManager.Instance.GetPlayerLatitude();
		_playerLon = GameManager.Instance.GetPlayerLongitude();
        #region FISHING STATE LOGIC
        switch (_currentState)
		{
			//in this state the player can launch the lure or do anything else
			case FishingStates.Idle:
				Vector3 acceleration = Input.acceleration; //the current acceleration of the phone

				//if the current acceleration surpasses the minimum launch speed, we launch the lure
				if (acceleration.magnitude > _minLaunchSpeed)
				{
					_isCasting = true;
					_peakAcceleration = Mathf.Max(acceleration.magnitude, _peakAcceleration);
				}

				//if the acceleration falls bellow the min speed threshold we assume the player stopped the movement
				if (_isCasting && acceleration.magnitude < _stopLaunchThresholdSpeed)
				{
					//_peakAcceleration = 0f; //reset peak acceleration
					_isCasting = false; //turn of casting

					//subscribe to relevant events
					//LureController.E_LandedOnWater += HandleLandedOnWater;
					//LureController.E_LandedOnGround += HandleLandedOnGround;

					LaunchLure();
				}
				break;
			//in this state the lure is being cast and waiting for other events
			case FishingStates.Casting:
				_lureLineRenderer.SetPosition(0, _lureSpawnPoint.position); //start pos of the fishing line
				_lureLineRenderer.SetPosition(1, _currentLure.transform.position); //end pos of the fishing line

				if (_currentLure.transform.position.y < _initialYPosition + _yThresholdOffset)
				{
					OnLureBelowThreshold(_currentLure.transform.position);
				}
				break;
			//in this state the lure successfuly landed in water and is now waiting for a fish
			case FishingStates.Waiting:
				Debug.Log("IM WAITING");

				_lureLineRenderer.SetPosition(0, _lureSpawnPoint.position); //start pos of the fishing line
				_lureLineRenderer.SetPosition(1, _currentLure.transform.position); //end pos of the fishing line

				_randomWaitTime -= Time.deltaTime;

				if (_randomWaitTime < 0f)
				{
					FishBiteTimerOver();
				}
				//FishBiteTimer(_randomWaitTime);
				break;
			case FishingStates.Pulling:
				break;
			case FishingStates.Paused:
				break;
		}
		#endregion
	}

    #region GENERAL METHODS
    //launches the lure :|
    //private void LaunchLure()
    //{
    //	float launchForce = _peakAcceleration * _launchForceMultiplier;
    //       //GameObject lure = Instantiate(_lurePrefab, _lureSpawnPoint.position, Quaternion.identity);
    //       GameObject lure = Instantiate(_lurePrefab, _lureSpawnPoint.position, transform.rotation);
    //       Rigidbody lureRigidbody = lure.GetComponent<Rigidbody>();
    //	_currentLure = lure; //pass the created lure into our variable
    //	_lureLineRenderer.enabled = true; //turn on the fishing line
    //	_lureLineRenderer.SetPosition(1, _currentLure.transform.position); //set the end position to the lure

    //	//get the phones rotation
    //	Quaternion phoneRotation = Input.gyro.attitude;

    //	//simplify the rotation fix
    //	//Quaternion adjustedRotation = Quaternion.Euler(90, 0, 0) * phoneRotation;
    //	Quaternion rotationFix = Quaternion.Euler(90,0,0);
    //	Quaternion adjustedRotation = rotationFix * phoneRotation;

    //	//the forward direction taking into account the phons orientation
    //	//Vector3 launchDirection = adjustedRotation * Vector3.forward;
    //	Vector3 launchDirection = transform.rotation * (phoneRotation * Vector3.forward);

    //	//visualize the launch direction
    //	Debug.DrawRay(_lureSpawnPoint.position, launchDirection * 5, Color.red, 5f);

    //	//apply the force
    //	lureRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);
    //	Debug.Log($"Launch Force: {launchForce}, Launch Direction: {launchDirection}");

    //	_peakAcceleration = 0f;

    //	ChangeFishingState(FishingStates.Casting);
    //}

    private void LaunchLure()
    {
        //determine the launch force based on the peak acceleration
        float launchForce = _peakAcceleration * _launchForceMultiplier;

        //instantiate the lure at the spawn point with the correct rotation
        GameObject lure = Instantiate(_lurePrefab, _lureSpawnPoint.position, transform.rotation);
        Rigidbody lureRigidbody = lure.GetComponent<Rigidbody>();
		lureRigidbody.isKinematic = false;
        _currentLure = lure;

        //enable the fishing line renderer
        _lureLineRenderer.enabled = true;
        _lureLineRenderer.SetPosition(1, _currentLure.transform.position);

        //use the fishing rod's rotation to determine the launch direction
        Vector3 launchDirection = transform.rotation * Vector3.forward;

        //visualize the launch direction (for debugging)
        Debug.DrawRay(_lureSpawnPoint.position, launchDirection * 5, Color.red, 5f);

        //apply the launch force to the lure's rigidbody
        lureRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

        //reset peak acceleration after the launch
        _peakAcceleration = 0f;

        //change the fishing state to Casting
        ChangeFishingState(FishingStates.Casting);
    }



    //generate a fixed random amount of time for the fish bite timer to wait
    private void GenerateRandomWaitTime(float minTime, float maxTime)
	{
		_randomWaitTime = UnityEngine.Random.Range(minTime, maxTime);
	}

	private void OnLureBelowThreshold(Vector3 lurePosition)
	{
		ConvertUnityPositionToGPS(lurePosition, out double lureLat, out double lureLon);
		E_LureLanded?.Invoke(lureLat, lureLon);

		//ChangeFishingState(FishingStates.Waiting);
	}

	//this function waits for a certain amount of time before announcing that the time has ended
	private void FishBiteTimerOver()
	{
		FishManager.E_FishGranted += HandleFishGranted;
		FishManager.E_FishDenied += HandleFishDenied;
		E_FishBiteTimerOver?.Invoke();
	}

	//QoL function, easier to call than to reset stuff manually every time
	private void ResetToIdle()
	{
		_currentLure = null;
		_lureLineRenderer.enabled = false;
		_randomWaitTime = 0f;
		_peakAcceleration = 0f;
		ChangeFishingState(FishingStates.Idle);
	}

	//converts the Unity world position of the lure into real world coordinates
	private void ConvertUnityPositionToGPS(Vector3 lurePosition, out double lureLat, out double lureLon)
	{
		//calculate the offset in Unity space
		Vector3 offset = lurePosition - transform.position;

		//convert the offset to meters
		double offsetX = offset.x * _metersPerUnit;
		double offsetZ = offset.z * _metersPerUnit;

		//calculate the latitude and longitude offsets
		double deltaLat = (offsetZ / _EarthRadius) * (180 / Mathf.PI);
		double deltaLon = (offsetX / (_EarthRadius * Mathf.Cos((float)(_playerLat * Mathf.PI / 180)))) * (180 / Mathf.PI);

		//calculate the lures real world latitude and longitude
		lureLat = _playerLat + deltaLat;
		lureLon = _playerLon + deltaLon;
	}
	#endregion

	#region EVENT HANDLERS
	//handles what happens when the lure is pulled
	private void HandleLurePulled()
	{
		//unsubscribe
		TouchManager.E_LurePulled -= HandleLurePulled;

		ResetToIdle();
	}

	//handles what happens if the lure lands in water
	private void HandleLandedOnWater(bool onWater)
	{
		if (onWater)
		{
			//subscribe to relevant events
			//TouchManager.E_LurePulled += HandleLurePulled;

			Debug.Log("LANDED ON WATER");
			GenerateRandomWaitTime(_minWaitTime, _maxWaitTime);
			ChangeFishingState(FishingStates.Waiting);
		}
		else
		{
			Debug.Log("LANDED ON GROUND");

			ResetToIdle();
		}
	}

	//handles what happens if the lure lands on the ground
	private void HandleLandedOnGround()
	{
		//unsubscribe from events
		//LureController.E_LandedOnWater -= HandleLandedOnWater;
		LureController.E_LandedOnGround -= HandleLandedOnGround;

		Debug.Log("LANDED ON GROUND");

		ResetToIdle();
	}

	//this function will do something when a fish is caught, to be decided
	private void HandleFishGranted(string fishName, string fishQuality)
	{
		//unsubscribe
		FishManager.E_FishDenied -= HandleFishDenied;
		FishManager.E_FishGranted -= HandleFishGranted;
		
		ResetToIdle();
	}

	//this one will do something when a fish isnt caught, also to be decided
	private void HandleFishDenied()
	{
		//unsubscribe
		FishManager.E_FishDenied -= HandleFishDenied;
		FishManager.E_FishGranted -= HandleFishGranted;

		ResetToIdle();
	}
	#endregion

	#region EXTERNAL CALLABLES
	//this method allows me to change the current state anywhere
	public void ChangeFishingState(FishingStates newState)
	{
		//check if the new state isnt already the current one
		if (newState == _currentState)
		{
			return;
		}
		_currentState = newState;
	}
	#endregion
}
