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

	public Dictionary<string, GameAction> possibleActions;
	private delegate void Action(DateTime currentDate);
	private LinkedList<Action> actions;

	void Start () {
		currentDate = new DateTime(2015, 4, 18);
		influencePoint = 5;
		this.worldMap = GameObject.FindGameObjectWithTag("worldMap").GetComponent<WorldMap>();
		this.actions = new LinkedList<Action>();

		GameAction[] gameActions = new GameAction[] {
			new GameAction("WordOfMouth", 1, new TimeSpan(1, 0, 0, 0), 1f),
			new GameAction("BookPrinting", 2, new TimeSpan(365, 0, 0, 0), 0.02f)
		};

		possibleActions = new Dictionary<string, GameAction>();
		foreach(var action in gameActions) {
			possibleActions[action.actionName] = action;
		}

		GameObject introTextObj = GameObject.FindGameObjectWithTag("IntroText");
		Text intro = introTextObj.GetComponent<Text>();
		intro.text = string.Format(intro.text, PlayerProfile.instance.founderName, PlayerProfile.instance.ideaName);
	}

	public void StartGame() {
		GameObject.FindGameObjectWithTag("IntroPanel").SetActive(false);
		started = true;
	}

	public void InitiateCountryAction(string action) {
		Debug.Log("User initiated action: " + action);
		Country selectedCountry = worldMap.currentSelectedCountry;
		if(selectedCountry == null) {
			Debug.Log("Cannot perform action because no country is selected");
			return;
		}

		GameAction gameAction = possibleActions[action];

		int actionCost = selectedCountry.GetActionCost(gameAction);
		if(actionCost > influencePoint) {
			//TODO: NOTIFY user
			Debug.Log("Insufficient influence point to perform action: " + action + " in country: " + selectedCountry.name);
			return;
		}

		actions.AddLast(delegate(DateTime date) {
			Debug.Log("Action " + action + " is picked up in the queue");
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
