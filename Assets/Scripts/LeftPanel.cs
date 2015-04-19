using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeftPanel : MonoBehaviour, CountrySelectionEventListener {

	private Country currentDisplayCountry;

	public Text countryNameLabel;
	public Text believerLabel;
	public Text resistenceLabel;

	public void SelectCountry(Country info) {
		this.currentDisplayCountry = info;
	}

	public void DeselectCountry() {
		this.currentDisplayCountry = null;
	}

	void Start() {
		GameManger.instance.RegisterCountrySelectionListener(this);
	}

	void Update() {
		if(currentDisplayCountry == null) {
			countryNameLabel.text = "No Country is selected";
			believerLabel.text = "";
			return;
		}

		countryNameLabel.text = currentDisplayCountry.name;
		believerLabel.text = "Followers: " + (currentDisplayCountry.believerPercentage / (float)Country.accuracy).ToString() + " %";
		believerLabel.GetComponent<TooltipShowable>().message = currentDisplayCountry.CalculateConversionRate() + "%"; 
		resistenceLabel.text = "Resistence: " + currentDisplayCountry.resistence + " / 100";
	}
}
