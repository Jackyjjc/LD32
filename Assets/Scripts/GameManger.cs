using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public interface CountrySelectionEventListener {
	void SelectCountry(CountryInfo selectedCountryInfo);
	void DeselectCountry();
}

public class GameManger : MonoBehaviour {
	
	private static readonly float tickFrequency = 1/5.0f;
	private static float timeElapsedSinceLastTick = 0;

	private CountrySelectionEventListener cseListener;
	public void NotifySelectCountry(CountryInfo info) {
		if(info == null) {
			cseListener.DeselectCountry();
		} else {
			cseListener.SelectCountry(info);
		}
	}
	public void RegisterCountrySelectionListener(CountrySelectionEventListener l) {
		cseListener = l;
	}

	private WorldMap worldMap;

	// Use this for initialization
	void Start () {
		this.worldMap = GameObject.FindGameObjectWithTag("worldMap").GetComponent<WorldMap>();
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsedSinceLastTick += Time.deltaTime;
		while((timeElapsedSinceLastTick - tickFrequency) >= float.Epsilon) {
			GameUpdate();
			timeElapsedSinceLastTick -= tickFrequency;
		}
	}

	void GameUpdate() {
		if(worldMap.countryInfoTable["Russia"].numBelievers < worldMap.countryInfoTable["Russia"].population) {
			worldMap.countryInfoTable["Russia"].numBelievers += 1;
		}
	}

	public static GameManger instance;
	void Awake()
	{
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy(gameObject);    
		}
	}
}
