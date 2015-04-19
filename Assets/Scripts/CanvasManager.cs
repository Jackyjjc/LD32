using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasManager : MonoBehaviour {
	
	public GameObject countryActionPanel;
	public GameObject traitPanel;
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ToggleCountryActionPanel(Image toggleButtonImage) {
		countryActionPanel.SetActive(!countryActionPanel.activeSelf);
		traitPanel.SetActive(!traitPanel.activeSelf);
		toggleButtonImage.rectTransform.Rotate(new Vector3(0f, 0f, 180f));
	}
}
