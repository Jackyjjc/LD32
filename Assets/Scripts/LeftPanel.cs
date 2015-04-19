using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LeftPanel : MonoBehaviour, CountrySelectionEventListener {

	private Country currentDisplayCountry;

	public Text countryNameLabel;
	public Text believerLabel;
	public Text resistenceLabel;

	public void SelectCountry(Country info) {
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
			believerLabel.text = "";
			return;
		}

		countryNameLabel.text = currentDisplayCountry.name;
		believerLabel.text = "Followers: " + (currentDisplayCountry.believerPercentage / 1000.0f).ToString() + " %";
		resistenceLabel.text = "Resistence: " + currentDisplayCountry.resistence + " / 100";
	}
}
