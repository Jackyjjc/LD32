using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountryActionPanel : MonoBehaviour, CountrySelectionEventListener {

	private Country currentDisplayCountry;
	
	public void SelectCountry(Country info) {
		this.currentDisplayCountry = info;
	}
	
	public void DeselectCountry() {
		this.currentDisplayCountry = null;
	}

	public Text countryActionPanelTitleText;

	// Use this for initialization
	void Start () {
		GameManger.instance.RegisterCountrySelectionListener(this);
	}
	
	// Update is called once per frame
	void Update () {
		if(currentDisplayCountry == null) {
			countryActionPanelTitleText.text = "No actions can be performed.";
			return;
		}
		countryActionPanelTitleText.text = "Spread your idea in " + currentDisplayCountry.name;
	}
}
