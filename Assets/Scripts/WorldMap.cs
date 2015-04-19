using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class CountryTrait {
	public string name;
	public string description;

	public CountryTrait(string name, string des) {
		this.name = name;
		this.description = des;
	}
}

public class WorldMap : MonoBehaviour {

	public Text globalBelieverPercentageText;
	public float globalPer;
	private GameObject globalProgressBar;
	
	public Country currentSelectedCountry;
	public Country[] countries;
	public Dictionary<string, Country> countryTable;

	// Use this for initialization
	void Start () {
		List<CountryTrait> traits = new List<CountryTrait>(new CountryTrait[] {
			new CountryTrait("Freedom", "This country cannot restrict speech or accesses"),
			new CountryTrait("Militaristic", "Cannot perform 'Coup d'état' on this country"),
			new CountryTrait("Educated", "Impact of 'Publish Books' last a month longer"),
			new CountryTrait("Dictatorial", "Impact of 'Restrict Speech' and 'Restrict Access' last a month longer"),
			/*new CountryTrait("Peace Lover", "Peaceful Ideology actions costs 1 less.\nViolent Ideology actions cost 1 more"),
			new CountryTrait("Warmonger", "Violent Ideology actions costs 1 less.\nPeaceful Ideology actions cost 1 more"),
			new CountryTrait("Capitalist", "'National Tour' costs 5 more influence points"),
			new CountryTrait("Couch Potato", "Effect of 'TV show' last 1 month longer"),*/
		});

		this.countries = new Country[] {
			new Country("Russia", choose<CountryTrait>(traits,UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("MiddleEast", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("Mongolia", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("China", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("India", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("SoutheastAsia", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("Europe", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("ANZ", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("EastAsia", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("Africa", choose<CountryTrait>(traits,UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("Greenland", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("NorthAmerica", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2))),
			new Country("SouthAmerica", choose<CountryTrait>(traits, UnityEngine.Random.Range(1, traits.Count / 2)))
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
		globalPercentage = Math.Min(globalPercentage, countries.Length * Country.hundredPer);
		this.globalPer = (((float)globalPercentage / countries.Length) / Country.accuracy);
		globalBelieverPercentageText.text = globalPer.ToString("0.00") + "%";

		//update the progress bar as well
		RectTransform rectTransform = globalProgressBar.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(125 * (globalPer / 100), rectTransform.sizeDelta.y);
	}

	private void SelectCountry(string countryName) {
		
		if(!countryTable.ContainsKey(countryName)) {
			Debug.LogError("User selected invalid country " + countryName);
		}
		
		Country country = countryTable[countryName];
		currentSelectedCountry = country;

		GameManger.instance.NotifySelectCountry(country);
	}

	private void DeselectCountry() {
		currentSelectedCountry = null;
		GameManger.instance.NotifySelectCountry(null);
	}

	public bool isZeroControl() {
		return globalPer == 0;
	}

	public bool isFullControl() {
		return globalPer >= countries.Length * Country.hundredPer;
	}

	public List<T> choose<T>(List<T> list, int n) {
		if(n <= 0 || list.Count <= 0) return new List<T>();
		T t = list[UnityEngine.Random.Range(0, list.Count)];
		List<T> results = choose(list.FindAll(x => !x.Equals(t)), n - 1);
		results.Add(t);
		return results;
	}
}
