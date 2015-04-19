using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipShowable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
	public string message;
	public int xModifier = 0;
	private GameObject tooltip;
	private bool active = false;

	void Start() {
		tooltip = (GameManger.instance == null) ? PlayerProfile.instance.tooltip : GameManger.instance.tooltip;
		message = message.Replace("\\n", "\n");
	}

	void Update() {
		if(!active) {
			return;
		}
		(tooltip.GetComponentInChildren<Text>() as Text).text = message;
	}

	public void ShowToolTip(Vector3 position) {
		tooltip.SetActive(true);
		active = true;
		tooltip.transform.position = new Vector3(position.x + 12 + xModifier, position.y - 10, position.z);
	}
	
	public void HideToolTip() {
		active = false;
		tooltip.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		ShowToolTip(gameObject.transform.position);
	}

	public void OnPointerExit(PointerEventData eventData) {
		HideToolTip();
	}

}
