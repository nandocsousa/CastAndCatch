using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TouchManager : MonoBehaviour
{
	#region EVENTS
	public static event Action E_LurePulled; //invoked when the player pulls back the lure
	#endregion
	#region VARIABLES
	[Header("INPUT COMPONENT")]
	[SerializeField] private PlayerInput _playerInput;
	//input actions
	private InputAction _pullLureAction; //touching the screen when fishing state waiting
	#endregion

	private void Awake()
	{
		_pullLureAction = _playerInput.actions["PullLure"];
	}

	private void OnEnable()
	{
		_pullLureAction.performed += HandleLurePulled;
	}

	private void OnDisable()
	{
		_pullLureAction.performed -= HandleLurePulled;
	}

	#region EVENT HANDLERS
	//handles what happens when the lure is pulled back
	private void HandleLurePulled(InputAction.CallbackContext context)
	{
		E_LurePulled?.Invoke();
	}
	#endregion
}
