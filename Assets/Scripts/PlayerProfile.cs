using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerProfile : MonoBehaviour {

	// Singleton
	public static PlayerProfile instance;
	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);    
		}
		DontDestroyOnLoad(instance);
	}

	// Use this for initialization
	void Start () {
		tooltip = GameObject.FindGameObjectWithTag("tooltip");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public InputField ideaNameText;
	public InputField ideaDescriptionText;
	public InputField ideaFounderText;
	public ToggleGroup group;

	public GameObject tooltip;
	public string founderName;
	public string ideaName;
	public string ideaDescription;
	public string trait;

    public void StartGame() {
		if(ideaNameText.text.Length == 0 || ideaDescriptionText.text.Length == 0 || ideaFounderText.text.Length == 0) {
			if(ideaFounderText.text.Length == 0) {
				ideaFounderText.animator.SetTrigger("invalid");
			}

			if(ideaNameText.text.Length == 0) {
				ideaNameText.animator.SetTrigger("invalid");
			}

			if(ideaDescriptionText.text.Length == 0) {
				ideaDescriptionText.animator.SetTrigger("invalid");
			}

			return;
		}

		this.founderName = ideaFounderText.text;
		this.ideaName = ideaNameText.text;
		this.ideaDescription = ideaDescriptionText.text;
		this.ideaFounderText = null;
		this.ideaNameText = null;
		this.ideaDescriptionText = null;
		foreach(var t in group.ActiveToggles()) {
			this.trait = t.GetComponentInChildren<Text>().text;
		}
		Debug.Log (trait);

		Application.LoadLevel("scene");
	}
}
