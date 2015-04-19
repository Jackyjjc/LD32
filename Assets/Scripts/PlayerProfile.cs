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
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public InputField ideaNameText;
	public InputField ideaDescriptionText;

	public string ideaName;
	public string ideaDescription;

    public void StartGame() {
		if(ideaNameText.text.Length == 0 || ideaDescriptionText.text.Length == 0) {
			if(ideaNameText.text.Length == 0) {
				ideaNameText.animator.SetTrigger("invalid");
			}

			if(ideaDescriptionText.text.Length == 0) {
				ideaDescriptionText.animator.SetTrigger("invalid");
			}

			return;
		}

		this.ideaName = ideaNameText.text;
		this.ideaDescription = ideaDescriptionText.text;
		this.ideaNameText = null;
		this.ideaDescriptionText = null;

		Application.LoadLevel("scene");
	}
}
