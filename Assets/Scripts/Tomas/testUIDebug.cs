using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class testUIDebug : MonoBehaviour
{
	public TextMeshProUGUI lattext;
	public TextMeshProUGUI lontext;
	public TextMeshProUGUI lurelandedtext;
	private bool lureLanded = false;

	private void OnEnable()
	{
		FishingRodController.E_LureLanded += HandleLureLanded;
	}

	private void OnDisable()
	{
		FishingRodController.E_LureLanded -= HandleLureLanded;
	}

	private void Start()
	{
		lurelandedtext.text = "landed at: " + GameManager.Instance.GetLureLatitude().ToString() + " // " + GameManager.Instance.GetLureLongitude().ToString();
	}

	private void Update()
	{
		lattext.text = GameManager.Instance.GetPlayerLatitude().ToString();
		lontext.text = GameManager.Instance.GetPlayerLongitude().ToString();

		if (lureLanded)
		{
			lurelandedtext.text = "landed at: " + GameManager.Instance.GetLureLatitude().ToString() + " // " + GameManager.Instance.GetLureLongitude().ToString();
		}
	}

	private void HandleLureLanded(double unused1, double unused2)
	{
		lureLanded = true;
	}
}
