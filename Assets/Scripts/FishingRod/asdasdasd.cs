using System;
using UnityEngine;

public enum TestStates { Idle, Casting, Waiting, Pulling, Paused }

public class asdasdasd : MonoBehaviour
{
    #region EVENTS
    public static event Action E_FishBiteTimerOver;
    #endregion
    #region VARIABLES
    [Header("Water Area")]
    [SerializeField] private float waterAreaRadius = 3.09f;  // Radius of the water area (~3 meters)
    [SerializeField] private Vector3 waterAreaCenter = new Vector3(20, 0, 0);  // Center of the water area, 20 meters away from the player
    [Header("COMPONENTS")]
    [SerializeField] private Transform _lureSpawnPoint;
    [SerializeField] private GameObject _lurePrefab;
    [SerializeField] private Rigidbody _lureRigidbody;
    [SerializeField] private LineRenderer _lureLineRenderer;

    private GameObject _currentLure;

    [Header("VALUES")]
    [SerializeField] private float _minLaunchSpeed = 1.5f;
    [SerializeField] private float _stopLaunchThresholdSpeed = 1f;
    [SerializeField] private float _peakAcceleration = 0f;
    [SerializeField] private float _launchForceMultiplier = 10f;
    [SerializeField] private float _minWaitTime = 2f;
    [SerializeField] private float _maxWaitTime = 5f;
    [SerializeField] private float _randomWaitTime = 0f;

    private bool _isCasting = false;
    private TestStates _currentState = TestStates.Idle;

    // Store the initial geographic coordinates
    public double initialLatitude;
    public double initialLongitude;

    #endregion

    #region METHODS
    // Set the initial coordinates (e.g., from GPS or your current position)
    public void SetInitialCoordinates(double latitude, double longitude)
    {
        initialLatitude = latitude;
        initialLongitude = longitude;
    }

    private void Awake()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
    }

    private void Start()
    {
        SetInitialCoordinates(NativeGPSPlugin.GetLatitude(), NativeGPSPlugin.GetLongitude());
        DrawWaterArea();
    }

    private void Update()
    {
        switch (_currentState)
        {
            case TestStates.Idle:
                HandleIdleState();
                break;
            case TestStates.Casting:
                UpdateLureLineRenderer();
                break;
            case TestStates.Waiting:
                HandleWaitingState();
                break;
            case TestStates.Pulling:
                break;
            case TestStates.Paused:
                break;
        }
    }

    // Handle when in the idle state
    private void HandleIdleState()
    {
        Vector3 acceleration = Input.acceleration;
        if (acceleration.magnitude > _minLaunchSpeed)
        {
            _isCasting = true;
            _peakAcceleration = Mathf.Max(acceleration.magnitude, _peakAcceleration);
        }

        if (_isCasting && acceleration.magnitude < _stopLaunchThresholdSpeed)
        {
            _isCasting = false;
            LaunchLure();
        }
    }

    // Update the line renderer to follow the lure during casting
    private void UpdateLureLineRenderer()
    {
        _lureLineRenderer.SetPosition(0, _lureSpawnPoint.position);
        _lureLineRenderer.SetPosition(1, _currentLure.transform.position);
    }

    // Handle the waiting state after the lure lands
    private void HandleWaitingState()
    {
        _randomWaitTime -= Time.deltaTime;
        if (_randomWaitTime < 0f)
        {
            FishBiteTimerOver();
        }
    }

    // Launch the lure and calculate the landing position
    private void LaunchLure()
    {
        float launchForce = _peakAcceleration * _launchForceMultiplier;
        GameObject lure = Instantiate(_lurePrefab, _lureSpawnPoint.position, Quaternion.identity);
        Rigidbody lureRigidbody = lure.GetComponent<Rigidbody>();
        _currentLure = lure;

        _lureLineRenderer.enabled = true;

        // Get the phone's rotation and apply to launch direction
        Quaternion phoneRotation = Input.gyro.attitude;
        Quaternion rotationFix = Quaternion.Euler(90, 0, 0);
        Quaternion adjustedRotation = rotationFix * phoneRotation * Quaternion.Euler(0, 0, 180);
        Vector3 launchDirection = adjustedRotation * Vector3.forward;

        lureRigidbody.AddForce(launchDirection * launchForce, ForceMode.Impulse);

        // Predict where the lure will land
        Vector3 predictedLandingPosition = PredictLureLandingPosition(lureRigidbody.velocity);
        Debug.Log("Predicted Landing Position: " + predictedLandingPosition);

        // Convert predicted position to geographic coordinates (lat, long)
        double predictedLatitude, predictedLongitude;
        ConvertToGeographicCoordinates(predictedLandingPosition, out predictedLatitude, out predictedLongitude);

        Debug.Log("Predicted Landing Latitude: " + predictedLatitude + ", Longitude: " + predictedLongitude);

        _peakAcceleration = 0f;
        ChangeFishingState(TestStates.Casting);
    }

    // Calculate the predicted landing position based on velocity and gravity
    private Vector3 PredictLureLandingPosition(Vector3 initialVelocity)
    {
        float timeToGround = Mathf.Sqrt((2 * Mathf.Abs(transform.position.y)) / Physics.gravity.y);
        Vector3 horizontalVelocity = new Vector3(initialVelocity.x, 0, initialVelocity.z);
        Vector3 horizontalDirection = horizontalVelocity.normalized;

        float horizontalSpeed = horizontalVelocity.magnitude;
        float horizontalDistance = horizontalSpeed * timeToGround;

        Vector3 predictedPosition = transform.position + horizontalDirection * horizontalDistance;
        return predictedPosition;
    }

    // Check if the bait landed inside the water area (circle)
    bool IsBaitInWaterArea(Vector3 landingPosition)
    {
        // Calculate the distance between the bait's landing position and the center of the water area
        float distanceToWaterAreaCenter = Vector3.Distance(landingPosition, waterAreaCenter);

        // If the distance is less than or equal to the radius, the bait landed in the water
        return distanceToWaterAreaCenter <= waterAreaRadius;
    }

    // Convert world position to geographic coordinates (this is simplified and assumes a small area)
    private void ConvertToGeographicCoordinates(Vector3 worldPosition, out double latitude, out double longitude)
    {
        // Simple conversion for small-scale (use more advanced methods for large distances)
        float earthRadius = 6378f; // Earth radius in kilometers
        float latitudeDelta = worldPosition.z / earthRadius;  // Assuming z is north-south
        float longitudeDelta = worldPosition.x / (earthRadius * Mathf.Cos(Mathf.Deg2Rad * (float)initialLatitude));

        latitude = initialLatitude + latitudeDelta * Mathf.Rad2Deg;
        longitude = initialLongitude + longitudeDelta * Mathf.Rad2Deg;
    }

    // Start the random wait timer for fishing
    private void FishBiteTimerOver()
    {
        FishManager.E_FishGranted += HandleFishGranted;
        FishManager.E_FishDenied += HandleFishDenied;
        E_FishBiteTimerOver?.Invoke();
    }

    private void ResetToIdle()
    {
        _currentLure = null;
        _lureLineRenderer.enabled = false;
        _randomWaitTime = 0f;
        _peakAcceleration = 0f;
        ChangeFishingState(TestStates.Idle);
    }
    #endregion

    //generate a fixed random amount of time for the fish bite timer to wait
    private void GenerateRandomWaitTime(float minTime, float maxTime)
    {
        _randomWaitTime = UnityEngine.Random.Range(minTime, maxTime);
    }

    #region EVENT HANDLERS
    private void HandleLurePulled()
    {
        ResetToIdle();
    }

    private void HandleLandedOnWater()
    {
        GenerateRandomWaitTime(_minWaitTime, _maxWaitTime);
        ChangeFishingState(TestStates.Waiting);
    }

    private void HandleLandedOnGround()
    {
        ResetToIdle();
    }

    private void HandleFishGranted(string fishName, string fishQuality)
    {
        ResetToIdle();
    }

    private void HandleFishDenied()
    {
        ResetToIdle();
    }
    #endregion

    #region EXTERNAL CALLABLES
    public void ChangeFishingState(TestStates newState)
    {
        if (newState == _currentState) return;
        _currentState = newState;
    }
    #endregion

    void DrawWaterArea()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 100;  // Points to draw the circle
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        for (int i = 0; i < 100; i++)
        {
            float angle = i * Mathf.PI * 2 / 100;
            float x = waterAreaCenter.x + Mathf.Cos(angle) * waterAreaRadius;
            float z = waterAreaCenter.z + Mathf.Sin(angle) * waterAreaRadius;
            lineRenderer.SetPosition(i, new Vector3(x, waterAreaCenter.y, z));
        }
    }

}
