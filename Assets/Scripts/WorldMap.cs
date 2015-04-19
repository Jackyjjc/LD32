using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldMap : MonoBehaviour {

	public Text globalBelieverPercentageText;
	private GameObject globalProgressBar;
	
	public Country currentSelectedCountry;
	public Country[] countries;
	public Dictionary<string, Country> countryTable;

	// Use this for initialization
	void Start () {
		this.countries = new Country[] {
			new Country("Russia"),
			new Country("MiddleEast"),
			new Country("Mongolia"),
			new Country("China"),
			new Country("India"),
			new Country("SoutheastAsia")
		};
		
		this.countryTable = new Dictionary<string, Country>();
		foreach(var info in countries) {
			countryTable[info.name] = info;
		}

		globalProgressBar = GameObject.FindGameObjectWithTag("GlobalProgressBar");
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Fire1") && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hit.collider == null) {
				DeselectCountry();
			} else {
				SelectCountry(hit.collider.gameObject.name);
			}
		}
	}

	public void GameUpdate(DateTime currentDate) {
		foreach(var country in countries) {
			country.Update(currentDate);
		}

		int globalPercentage = 0;
		foreach(var c in countries) {
			globalPercentage += c.believerPercentage;
		}
		float globalPer = (((float)globalPercentage / countries.Length) / 1000f);
		globalBelieverPercentageText.text = globalPer.ToString("0.00") + "%";

		//update the progress bar as well
		RectTransform rectTransform = globalProgressBar.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(125 * (globalPer / 100), rectTransform.sizeDelta.y);
	}

	private void SelectCountry(string countryName) {
		Debug.Log("selecting coutry " + countryName);
		
		if(!countryTable.ContainsKey(countryName)) {
			Debug.LogError("User selected invalid country " + countryName);
		}
		
		Country country = countryTable[countryName];
		Debug.Log("Tell left panel to display country info" + country);
		currentSelectedCountry = country;

		GameManger.instance.NotifySelectCountry(country);
	}

	private void DeselectCountry() {
		GameManger.instance.NotifySelectCountry(null);
	}
}
