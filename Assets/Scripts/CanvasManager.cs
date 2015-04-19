using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasManager : MonoBehaviour {
	
	public GameObject countryActionPanel;
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToggleCountryActionPanel(Image toggleButtonImage) {
		countryActionPanel.SetActive(!countryActionPanel.activeSelf);
		toggleButtonImage.rectTransform.Rotate(new Vector3(0f, 0f, 180f));
	}
}
