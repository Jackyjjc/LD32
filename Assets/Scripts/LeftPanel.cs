using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LeftPanel : MonoBehaviour, CountrySelectionEventListener {

	private Country currentDisplayCountry;
	private List<GameObject> objects;

	public Text countryNameLabel;
	public Text believerLabel;
	public Text resistenceLabel;
	public GameObject traitPanel;
	public GameObject traitPrefab;

	public void SelectCountry(Country info) {
		this.currentDisplayCountry = info;
		foreach(var t in info.traits) {
			GameObject go = Instantiate(traitPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			go.GetComponent<Text>().text = t.name;
			go.GetComponent<TooltipShowable>().message = t.description;
			go.transform.SetParent(traitPanel.transform);
		}
	}

	public void DeselectCountry() {
		this.currentDisplayCountry = null;
		foreach(var o in objects) {
			o.transform.SetParent(null);
			Destroy(o);
		}
		objects.Clear();
	}

	void Start() {
		GameManger.instance.RegisterCountrySelectionListener(this);
		objects = new List<GameObject>();
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
