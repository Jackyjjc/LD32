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
	public GameObject influencePointPanel;
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
			new GameAction ("Word Of Mouth", 1, new TimeSpan (30, 0, 0, 0), 0.01f, 1),
			new GameAction ("Publish Books", 3, new TimeSpan (90, 0, 0, 0), 0.015f, 1),
			new GameAction ("Social Media", 2, new TimeSpan (8, 0, 0, 0), 0.8f, 2),
			new GameAction ("TV Shows", 5, new TimeSpan (20, 0, 0, 0), 0.4f, 2),
			new GameAction ("Public Speech", 4, new TimeSpan (16, 0, 0, 0), 0.3f, 5),
			new GameAction ("National Tour", 10, new TimeSpan (60, 0, 0, 0), 0.5f, 10, "\nRequire: 1 Celebrity"),
			new GameAction ("Political Party", 15, new TimeSpan(365, 0, 0, 0), 0.01f, 15, "\nRequire: 1 Political Leader"),
			new GameAction ("Revolution", 20, new TimeSpan(10, 0, 0, 0), 5f, 50, "\nRequire: 2 Leader of any kind"),
			new GameAction ("Coup d'état", 30, new TimeSpan(2, 0, 0, 0), 50f, 90, "\nRequire: 1 Political Leader + 1 Military Leader")
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
		influencePoint = 10;
		this.worldMap = GameObject.FindGameObjectWithTag("worldMap").GetComponent<WorldMap>();
		this.actions = new LinkedList<Action>();

		if(PlayerProfile.instance != null) {
			GameObject introTextObj = GameObject.FindGameObjectWithTag("IntroText");
			Text intro = introTextObj.GetComponent<Text>();
			intro.text = string.Format(intro.text, PlayerProfile.instance.founderName, PlayerProfile.instance.ideaName);
		}

		SetUpGameActions();
		this.messages = new List<GameMessage>();
	}

	public void StartGame() {
		GameObject.FindGameObjectWithTag("IntroPanel").SetActive(false);
		started = true;
	}

	public void RestartGame() {
		Application.LoadLevel("start");
	}

	public void InitiateCountryAction(GameAction gameAction) {
		Country selectedCountry = worldMap.currentSelectedCountry;
		if(selectedCountry == null) {
			PostMessageToBoard("Cannot perform action because no country is selected", Color.red);
			return;
		}

		int actionCost = selectedCountry.GetActionCost(gameAction);
		if(actionCost > influencePoint) {
			PostMessageToBoard("Insufficient influence point to perform action: " + gameAction.actionName + " in country: " + selectedCountry.name, Color.red);
			return;
		}

		PostMessageToBoardWithTime("You performed action: " + gameAction.actionName);

		actions.AddLast(delegate(DateTime date) {
			Debug.Log("Action " + gameAction.actionName + " is picked up in the queue");
			influencePoint -= actionCost;
			selectedCountry.AddEffect(new Effect(gameAction.actionName, gameAction.baseRate, date.Add(gameAction.baseDuration), gameAction.resistenceModifier));
		});
	}

	public GameObject finalScreen;

	void Update () {
		if(!started) {
			return;
		}

		if(worldMap.isFullControl()) {
			finalScreen.SetActive(true);
			finalScreen.GetComponentInChildren<Text>().text = "Congratulations! You have win the game!";
		} else if (worldMap.isZeroControl() && influencePoint == 0) {
			finalScreen.SetActive(true);
			finalScreen.GetComponentInChildren<Text>().text = "Sorry! You have lost the game!";
		}

		if(paused) {
			return;
		}

		timeElapsedSinceLastTick += Time.deltaTime;
		while((timeElapsedSinceLastTick - tickFrequency) >= float.Epsilon) {
			GameUpdate();
			timeElapsedSinceLastTick -= tickFrequency;
		}
		influencePointText.text = "Influence Points: " + influencePoint;
		influencePointPanel.GetComponent<TooltipShowable>().message = 
			"All your actions will consume influence\npoints. Influence Points regenerates over\ntime as the number of your follower\ngrows. (Regenerating: " + influencePointAcc + "%)";
		dateText.text = currentDate.ToString("dd, MMM, yyyy");
	}

	private float influencePointAcc = 0;

	void GameUpdate() {
		currentDate = currentDate.AddDays(1);

		influencePointAcc += worldMap.globalPer;
		while(influencePointAcc > 100) {
			influencePoint++;
			influencePointAcc -= 100;
		}

		//Clean up message queue
		List<GameMessage> messagesToRemove = messages.FindAll(x => (currentDate - x.creationDate).TotalDays > messagePreserveDuration);
		messages.RemoveAll(x => messagesToRemove.Contains(x));
		foreach(var m in messagesToRemove) {
			m.messageObj.transform.SetParent(null);
			Destroy(m.messageObj);
		}

		//Perform Actions
		foreach(var action in actions) {
			action(currentDate);
		}
		actions.Clear();

		worldMap.GameUpdate(currentDate);
	}

	private static readonly int messagePreserveDuration = 90;
	private struct GameMessage {
		public DateTime creationDate;
		public GameObject messageObj;
	}
	private List<GameMessage> messages;
	public GameObject messageBoard;
	public GameObject messageBoardMessagePrefab;

	public GameObject PostMessageToBoard(string message) {
		GameObject messageObj = Instantiate(messageBoardMessagePrefab, Vector3.zero, Quaternion.identity) as GameObject;
		Text textComponent = messageObj.GetComponent<Text>();
		textComponent.text = message;
		messageObj.transform.SetParent(messageBoard.transform);
	
		GameMessage gameMessage = new GameMessage();
		gameMessage.creationDate = currentDate;
		gameMessage.messageObj = messageObj;
		messages.Add(gameMessage);

		return messageObj;
	}

	public void PostMessageToBoardWithTime(string message) {
		PostMessageToBoard(currentDate.ToString("dd, MMM, yyyy") + " : " + message);
	}

	public void PostMessageToBoardWithTime(string message, Color color) {
		PostMessageToBoard(currentDate.ToString("dd, MMM, yyyy") + " : " + message, color);
	}

	public void PostMessageToBoard(string message, Color color) {
		PostMessageToBoard(message).GetComponent<Text>().color = color;
	}
}
