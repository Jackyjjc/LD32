using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeftPanel : MonoBehaviour, CountrySelectionEventListener {

	private CountryInfo currentDisplayCountry;

	public Text countryNameLabel;
	public Text populationLabel;
	public Text believerLabel;

	public void SelectCountry(CountryInfo info) {
		Debug.Log("Left Panel displaying info");
		this.currentDisplayCountry = info;
	}

	public void DeselectCountry() {
		Debug.Log("Left Panel stop displaying info");
		this.currentDisplayCountry = null;
	}

	void Start() {
		GameManger.instance.RegisterCountrySelectionListener(this);
	}

	void Update() {
		if(currentDisplayCountry == null) {
			countryNameLabel.text = "No Country is selected";
			populationLabel.text = "";
			believerLabel.text = "";
			return;
		}

		countryNameLabel.text = "Country Name: " + currentDisplayCountry.name;
		populationLabel.text = "Total Population: " + currentDisplayCountry.population.ToString();
		believerLabel.text = "Total Believers: " + currentDisplayCountry.numBelievers.ToString();
	}
}
