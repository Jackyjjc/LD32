using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class CountryInfo {
	public readonly string name;
	public long population;
	public long numBelievers;
	
	public CountryInfo(string name, long population) {
		this.name = name;
		this.population = population;
		this.numBelievers = 0;
	}
}

public class WorldMap : MonoBehaviour {

	public Dictionary<string, CountryInfo> countryInfoTable;

	// Use this for initialization
	void Start () {
		CountryInfo[] coutryInfos = new CountryInfo[] {
			new CountryInfo("Russia", 143000000L),
			new CountryInfo("MiddleEast", 215000000L),
			new CountryInfo("Mongolia", 2800000L),
			new CountryInfo("China", 1357000000),
		};
		
		this.countryInfoTable = new Dictionary<string, CountryInfo>();
		foreach(var info in coutryInfos) {
			countryInfoTable[info.name] = info;
		}
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

	private void SelectCountry(string countryName) {
		Debug.Log("selecting coutry " + countryName);
		
		if(!countryInfoTable.ContainsKey(countryName)) {
			Debug.LogError("User selected invalid country " + countryName);
		}
		
		CountryInfo info = countryInfoTable[countryName];
		Debug.Log("Tell left panel to display country info" + info);

		GameManger.instance.NotifySelectCountry(info);
	}

	private void DeselectCountry() {
		GameManger.instance.NotifySelectCountry(null);
	}
}
