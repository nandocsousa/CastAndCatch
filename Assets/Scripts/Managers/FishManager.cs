using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
	public static FishManager Instance = null;

	#region EVENTS
	public static event Action E_FishDenied;
	public static event Action<string, string> E_FishGranted;
	#endregion
	#region VARIABLES
	[Header("FISH SPECIFICATIONS")]
	[SerializeField] private string[] _fishNames = { "John", "Jack", "Joe"}; //placeholder names
	[SerializeField] private string[] _fishQuality = { "Bronze", "Silver", "Gold"}; //placeholder qualities
	#endregion

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

	private void OnEnable()
	{
		FishingRodController.E_FishBiteTimerOver += HandleFishBiteTimerOver;
	}

	private void OnDisable()
	{
		FishingRodController.E_FishBiteTimerOver -= HandleFishBiteTimerOver;
	}

	#region GENERAL METHODS
	//this is responsible for generating the fish given to the player if he does get one
	//eventually the quality/rarity of the fish will be affected by stuff like fishing rod quality and bait
	private void GenerateFish()
	{
		string fishName = _fishNames[UnityEngine.Random.Range(0, _fishNames.Length)];
		string fishQuality = _fishQuality[UnityEngine.Random.Range(0, _fishQuality.Length)];

		E_FishGranted?.Invoke(fishName, fishQuality);
	}
	#endregion

	#region EXTERNAL CALLABLES

	#endregion
	#region EVENT HANDLERS
	//this function will be called once the fishbitetimer is over
	//it gets a random chance to give the player a fish
	//later on i will make it so the chance to give a fish is affected by stuff like fishing rod quality and bait
	private void HandleFishBiteTimerOver()
	{
		float randomValue = UnityEngine.Random.Range(-1f, 1f);

		if (randomValue >= 0.01f)
		{
			GenerateFish();
		}
		else if (randomValue <= 0f)
		{
			E_FishDenied?.Invoke();
		}
	}
	#endregion
}
