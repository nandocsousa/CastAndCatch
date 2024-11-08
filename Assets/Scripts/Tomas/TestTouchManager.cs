using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestTouchManager : MonoBehaviour
{
	public static event Action E_LurePulled;

	private PlayerInput playerInput;
	private InputAction pullLureAction;

	private void Awake()
	{
		playerInput = GetComponent<PlayerInput>();
		pullLureAction = playerInput.actions["PullLure"];
	}

	private void OnEnable()
	{
		pullLureAction.performed += HandleLurePulled;
	}

	private void OnDisable()
	{
		pullLureAction.performed -= HandleLurePulled;
	}

	private void HandleLurePulled(InputAction.CallbackContext context)
	{
		Debug.Log("Lure Pulled!");
		E_LurePulled?.Invoke();
	}
}
