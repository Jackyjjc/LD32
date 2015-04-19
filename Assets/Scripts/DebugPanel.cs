using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DebugPanel : MonoBehaviour, CountrySelectionEventListener {

	private Country currentDisplayCountry;
	public Text sample;
	private List<Text> children;

	void Start() {
		GameManger.instance.RegisterCountrySelectionListener(this);
		children = new List<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		if(currentDisplayCountry == null) {
			return;
		}
		List<Effect> effects = currentDisplayCountry.GetEffects();
		for(int i = 0; i < effects.Count; i++) {
			if(i >= children.Count) {
				Text newStatus = Instantiate(sample, Vector3.zero, Quaternion.identity) as Text;
				newStatus.transform.SetParent(gameObject.transform);
				children.Add(newStatus);
			}
			children[i].text = effects[i].ToString();
		}

		int diff = children.Count - effects.Count;
		if(diff > 0) {
			List<Text> childrenToRemove = children.GetRange(children.Count - diff, diff);
			children.RemoveAll(x => childrenToRemove.Contains(x));
			childrenToRemove.ForEach(x => {
				x.transform.SetParent(null);
				Destroy(x);
			});
		}
	}

	public void SelectCountry(Country info) {
		this.currentDisplayCountry = info;
	}
	
	public void DeselectCountry() {
		this.currentDisplayCountry = null;
	}
}
