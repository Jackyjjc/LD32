using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Effect {
	public string name;
	public float modifier;
	public DateTime endDate;
	public Effect(string name, float modifier, DateTime endDate) {
		this.name = name;
		this.modifier = modifier;
		this.endDate = endDate;
	}

	public override string ToString ()
	{
		return name + " ends " + endDate.ToString("dd, MMM, yyyy");
	}
}

public class Country {
	public readonly string name;
	public int believerPercentage;
	public int resistence = 0;
	private List<Effect> effects;

	public Country(string name) {
		this.name = name;
		this.believerPercentage = 0;
		this.effects = new List<Effect>();
	}

	public void Update(DateTime currentDate) {
		int numRemoved = effects.RemoveAll((e) => e.endDate <= currentDate);

		if(believerPercentage < 1000000 && currentDate.Day == 1 && !effects.Exists(x => x.modifier < 0)) {
			float supressionValue = UnityEngine.Random.Range(0f, 100f);
			if(supressionValue < resistence) {
				//thing happened.
				GenerateResistentEvent(currentDate);
			}
		}

		float conversionRate = CalculateConversionRate();
		if((conversionRate - 0 < float.Epsilon) || believerPercentage >= 100000) {
			return;
		}

		float randomModifier = UnityEngine.Random.Range(1.05f, 0.95f);
		ChangeFollowerBy(conversionRate * randomModifier);
	}

	public void ChangeFollowerBy(float conversionRate) {
		int numConverted = (int)(conversionRate * 1000);
		believerPercentage += numConverted;
		believerPercentage = Math.Min(Math.Max(believerPercentage, 0), 100000);
	}

	public List<Effect> GetEffects() {
		return effects;
	}

	public void AddEffect(Effect effect) {
		Debug.Log("effect: " + effect.name + " is added to the country: " + name);

		Debug.Log("Finding Effect " + effect.name);
		Effect e = FindEffect(effect.name);
		if(e != null) {
			effects.Remove(e);
		}

		effects.Add(effect);

		//TODO revise this
		if(effect.modifier > 0) {
			float n = effect.modifier * 1000;
			float newBase = 10000 / n;
			newBase = (float)Math.Max(newBase, 1.001);
			resistence += (int)Mathf.Log(n, newBase);
			resistence = Math.Min(resistence, 100);
		}
	}

	public float CalculateConversionRate() {
		float conversionRate = -0.001f;
		foreach(var e in effects) {
			conversionRate += e.modifier;
		}
		return conversionRate;
	}

	public Effect FindEffect(string effect) {
		foreach(var e in this.effects) {
			if((e as Effect).name.Equals(effect)) {
				return (Effect)e;
			}
		}
		return null;
	}

	public int GetActionCost(GameAction action) {
		return action.baseCost;
	}
	
	public void GenerateResistentEvent(DateTime currentDate) {
		int level = (int)GenerateRandomVar(resistence, Math.Min(resistence - 0, 100 - resistence));
		float suddenDrop = 0f;

		level = Math.Min (Math.Max(0, level), 100);
		if (level >= 0 && level < 10) {
			suddenDrop = 0.5f;
			AddEffect(new Effect(string.Format("Internet War"), -0.03f, currentDate + new TimeSpan(10, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("Some people on the Internet start attacking followers of {0}", PlayerProfile.instance.ideaName), Color.yellow
			);
		} else if (level >= 10 && level < 20) {
			suddenDrop = 0.7f;
			AddEffect(new Effect(string.Format("Open Criticism"), -0.05f, currentDate + new TimeSpan(20, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("Some people in {0} openly criticized {1}", name, PlayerProfile.instance.ideaName), Color.yellow
			);
		} else if (level >= 20 && level < 30) {
			suddenDrop = 0.9f;
			AddEffect(new Effect(string.Format("Media Criticism"), -0.07f, currentDate + new TimeSpan(35, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("Public media in {0} openly criticized {1}", name, PlayerProfile.instance.ideaName), Color.yellow
			);
		} else if (level >= 30 && level < 40) {
			suddenDrop = 1f;
			AddEffect(new Effect(string.Format("Government Warning"), -0.1f, currentDate + new TimeSpan(70, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("Some government officials in {0} warn the public about {1}", name, PlayerProfile.instance.ideaName), Color.yellow
			);
		} else if (level >= 40 && level < 50) {
			suddenDrop = 2f;
			AddEffect(new Effect(string.Format("Protests"), -0.3f, currentDate + new TimeSpan(20, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("There are public protests against {0} in {1}", PlayerProfile.instance.ideaName, name), Color.yellow
			);
		} else if (level >= 50 && level < 60) {
			suddenDrop = 5f;
			AddEffect(new Effect(string.Format("Setarian violence"), -0.4f, currentDate + new TimeSpan(10, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("Violence outbreak between supporter of {0} and its oppositions in {1}", PlayerProfile.instance.ideaName, name), Color.yellow
			);
		} else if (level >= 60 && level < 70) {
			suddenDrop = 1f;
			AddEffect(new Effect(string.Format("Fear"), -0.1f, currentDate + new TimeSpan(90, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("People in {0} are reluctant to follow {1} because of recent developments", name, PlayerProfile.instance.ideaName), Color.yellow
				);
		} else if (level >= 70 && level < 80) {
			suddenDrop = 1f;
			AddEffect(new Effect(string.Format("Restrict Access"), -0.1f, currentDate + new TimeSpan(90, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("Access to materials about {0} are becoming increasingly hard in {1}", PlayerProfile.instance.ideaName, name), Color.yellow
			);
		} else if (level >= 80 && level < 90) {
			suddenDrop = 5f;
			AddEffect(new Effect(string.Format("Restrict Speech"), -1f, currentDate + new TimeSpan(365, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("The government of {0} bans all discussions about {1}", name, PlayerProfile.instance.ideaName), Color.yellow
			);
		} else {
			suddenDrop = 10f;
			AddEffect(new Effect(string.Format("Arrestment"), -0.5f, currentDate + new TimeSpan(180, 0, 0, 0)));
			GameManger.instance.PostMessageToBoardWithTime(
				string.Format("Police start arresting followers of {0} in {1}", PlayerProfile.instance.ideaName, name), Color.yellow
			);
		}

		ChangeFollowerBy(-suddenDrop);
		GameManger.instance.PostMessageToBoardWithTime(string.Format("Followers in {0} decreased by {1}", name, suddenDrop), Color.yellow);
	}

	private float GenerateRandomVar(float mean, float stdDev) {
		float u1 = UnityEngine.Random.Range(0f, 1f); //these are uniform(0,1) random doubles
		float u2 = UnityEngine.Random.Range(0f, 1f);
		float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2); //random normal(0,1)
		float randNormal = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
		return randNormal;
	}

	/*public class Distribution<T> {
		public T value;
		public float prob;
	}

	private T pickOne<T>(Distribution<T>[] distribution) {
		
		float sum = 0;
		foreach(var d in distribution) {
			sum += d.prob;
		}
		
		float result = UnityEngine.Random.Range(0, sum);
		
		int i = 0;
		Distribution<T> cur = distribution[i];
		while(result > cur.prob) {
			result -= cur.prob;
			i++;
			cur = distribution[i];
		}
		
		return cur.value;
	}*/
}
