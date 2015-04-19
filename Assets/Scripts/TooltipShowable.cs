using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipShowable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public string message;
	public int xModifier = 0;
	private GameObject tooltip;

	void Start() {
		tooltip = GameObject.FindGameObjectWithTag("tooltip");
		message = message.Replace("\\n", "\n");
	}

	public void ShowToolTip(Vector3 position, string content) {
		tooltip.SetActive(true);
		tooltip.transform.position = new Vector3(position.x + 12 + xModifier, position.y - 10, position.z);
		(tooltip.GetComponentInChildren<Text>() as Text).text = content;
	}
	
	public void HideToolTip() {
		tooltip.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData) {
		ShowToolTip(gameObject.transform.position, message);
	}

	public void OnPointerExit(PointerEventData eventData) {
		HideToolTip();
	}

}
