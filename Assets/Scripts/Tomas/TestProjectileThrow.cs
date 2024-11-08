using UnityEngine;

public class ProjectileController : MonoBehaviour
{
	public Transform lureSpawnPoint;
	public GameObject lurePrefab;
	private GameObject currentLure;

	public LineRenderer lureLineRenderer;

	public Rigidbody lureRigidbody;
	public float peakThreshold = 1.5f;
	public float launchThreshold = 1f;
	private bool isCasting = false;
	private bool hasLure = false;
	private float peakAcceleration = 0;

	void Start()
	{
		if (SystemInfo.supportsGyroscope)
		{
			Input.gyro.enabled = true;
		}
	}

	void Update()
	{
		Vector3 acceleration = Input.acceleration;

		if (!hasLure)
		{
			//check if acceleration exceeds the peak threshold
			if (acceleration.magnitude > peakThreshold)
			{
				isCasting = true;
				peakAcceleration = Mathf.Max(peakAcceleration, acceleration.magnitude);
			}

			//check if acceleration falls below the launch threshold after a peak
			if (isCasting && acceleration.magnitude < launchThreshold)
			{
				LaunchLure();
				isCasting = false;
				peakAcceleration = 0; //teset for next cast
				hasLure = true;
				TestTouchManager.E_LurePulled += HandleLurePulled;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			Debug.Log("AAAAAAA");
			hasLure = false;
			lureLineRenderer.enabled = false;
		}
	}

	private void LateUpdate()
	{
		if (hasLure)
		{
			lureLineRenderer.SetPosition(0, lureSpawnPoint.position);
			lureLineRenderer.SetPosition(1, currentLure.transform.position);
		}
	}

	void LaunchLure()
	{
		//spawn the lure at the specified point
		GameObject lure = Instantiate(lurePrefab, lureSpawnPoint.position, Quaternion.identity);
		Rigidbody lureRb = lure.GetComponent<Rigidbody>();
		currentLure = lure;
		lureLineRenderer.enabled = true;
		lureLineRenderer.SetPosition(1, currentLure.transform.position);

		//use the phone’s gyroscope orientation for launch direction
		Quaternion deviceRotation = Input.gyro.attitude;

		//adjust to Unity’s coordinate system
		Quaternion rotationFix = Quaternion.Euler(90, 0, 0);
		Quaternion adjustedRotation = rotationFix * deviceRotation * Quaternion.Euler(0, 0, 180);

		Vector3 launchDirection = adjustedRotation * Vector3.forward; //forward direction based on phone orientation
		float launchForce = peakAcceleration * 10; //adjust multiplier as needed

		lureRb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
	}

	private void HandleLurePulled()
	{
		hasLure = false;
		lureLineRenderer.enabled = false;
		Destroy(currentLure);
		currentLure = null;
		TestTouchManager.E_LurePulled -= HandleLurePulled;
	}

	void OnDrawGizmos()
	{
		//draw a line indicating the forward direction of the object
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2);
	}
}
