using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public interface CountrySelectionEventListener {
	void SelectCountry(Country selectedCountryInfo);
	void DeselectCountry();
}

public class GameManger : MonoBehaviour {

	// Singleton
	public static GameManger instance;
	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);    
		}
	}

	// Game speed related
	private static float normal = 1/5.0f;
	private static float medium = 1/10.0f;
	private static float high = 1/20.0f;

	private bool started = false;
	private bool paused = false;
	private float tickFrequency = normal;
	private float timeElapsedSinceLastTick = 0;

	public void ChangeSpeed(int speedLevel) {
		switch(speedLevel) {
		case 0:
			paused = true;
			break;
		case 1:
			paused = false;
			tickFrequency = normal;
			break;
		case 2:
			paused = false;
			tickFrequency = medium;
			break;
		case 3:
			paused = false;
			tickFrequency = high;
			break;
		default:
			break;
		}
	}

	// Country Selection Related
	private LinkedList<CountrySelectionEventListener> cseListeners = new LinkedList<CountrySelectionEventListener>();
	public void NotifySelectCountry(Country info) {
		foreach(var listener in cseListeners) {
			if(info == null) {
				listener.DeselectCountry();
			} else {
				listener.SelectCountry(info);
			}
		}
	}
	public void RegisterCountrySelectionListener(CountrySelectionEventListener l) {
		cseListeners.AddFirst(l);
	}

	//Update related
	public Text influencePointText;
	private int influencePoint;

	public Text dateText;
	private DateTime currentDate;
	private WorldMap worldMap;

	public GameObject actionButtonPrefab;
	public GameObject actionPanel;
	private delegate void Action(DateTime currentDate);
	private LinkedList<Action> actions;

	void SetUpGameActions ()
	{
		GameAction[] gameActions = new GameAction[] {
			new GameAction ("Word Of Mouth", 1, new TimeSpan (30, 0, 0, 0), 0.01f),
			new GameAction ("Publish Books", 3, new TimeSpan (90, 0, 0, 0), 0.015f),
			new GameAction ("Social Media", 2, new TimeSpan (8, 0, 0, 0), 0.8f),
			new GameAction ("TV Shows", 5, new TimeSpan (20, 0, 0, 0), 0.4f),
			new GameAction ("Public Speech", 4, new TimeSpan (16, 0, 0, 0), 0.3f),
			new GameAction ("National Tour", 10, new TimeSpan (60, 0, 0, 0), 0.5f, "\nRequire: 1 Celebrity"),
			new GameAction ("Political Party", 15, new TimeSpan(365, 0, 0, 0), 0.01f, "\nRequire: 1 Political Leader"),
			new GameAction ("Revolution", 20, new TimeSpan(10, 0, 0, 0), 5f, "\nRequire: 2 Leader of any kind"),
			new GameAction ("Coup d'état", 30, new TimeSpan(2, 0, 0, 0), 50f, "\nRequire: 1 Political Leader + 1 Military Leader")
		};
		foreach (var action in gameActions) {
			var copyAction = action;
			GameObject button = Instantiate (actionButtonPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			button.GetComponent<Button>().onClick.AddListener(() => this.InitiateCountryAction (copyAction));
			button.GetComponent<TooltipShowable>().message = copyAction.description;
			button.GetComponentInChildren<Text>().text = copyAction.actionName;
			button.transform.SetParent(actionPanel.transform);
		}
	}

	void Start () {
		currentDate = new DateTime(2015, 4, 18);
		influencePoint = 100;
		this.worldMap = GameObject.FindGameObjectWithTag("worldMap").GetComponent<WorldMap>();
		this.actions = new LinkedList<Action>();

		GameObject introTextObj = GameObject.FindGameObjectWithTag("IntroText");
		Text intro = introTextObj.GetComponent<Text>();

		if(PlayerProfile.instance != null) {
			intro.text = string.Format(intro.text, PlayerProfile.instance.founderName, PlayerProfile.instance.ideaName);
		}

		SetUpGameActions();
	}

	public void StartGame() {
		GameObject.FindGameObjectWithTag("IntroPanel").SetActive(false);
		started = true;
	}

	public void InitiateCountryAction(GameAction gameAction) {
		Debug.Log("User initiated action: " + gameAction.actionName);
		Country selectedCountry = worldMap.currentSelectedCountry;
		if(selectedCountry == null) {
			Debug.Log("Cannot perform action because no country is selected");
			return;
		}

		int actionCost = selectedCountry.GetActionCost(gameAction);
		if(actionCost > influencePoint) {
			//TODO: NOTIFY user
			Debug.Log("Insufficient influence point to perform action: " + gameAction.actionName + " in country: " + selectedCountry.name);
			return;
		}

		actions.AddLast(delegate(DateTime date) {
			Debug.Log("Action " + gameAction.actionName + " is picked up in the queue");
			influencePoint -= actionCost;
			selectedCountry.AddEffect(new Effect(gameAction, date.Add(gameAction.baseDuration)));
		});
	}

	void Update () {
		if(!started || paused) {
			return;
		}

		timeElapsedSinceLastTick += Time.deltaTime;
		while((timeElapsedSinceLastTick - tickFrequency) >= float.Epsilon) {
			GameUpdate();
			timeElapsedSinceLastTick -= tickFrequency;
		}
		influencePointText.text = "Influence Points: " + influencePoint;
		dateText.text = currentDate.ToString("dd, MMM, yyyy");
	}

	void GameUpdate() {
		currentDate = currentDate.AddDays(1);
		foreach(var action in actions) {
			action(currentDate);
		}
		actions.Clear();

		worldMap.GameUpdate(currentDate);
	}
}
