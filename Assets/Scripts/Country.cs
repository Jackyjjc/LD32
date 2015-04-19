using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Effect {
	public GameAction action;
	public DateTime endDate;
	public Effect(GameAction action, DateTime endDate) {
		this.action = action;
		this.endDate = endDate;
	}

	public override string ToString ()
	{
		return action.actionName + " ends " + endDate.ToString("dd, MMM, yyyy");
	}
}

public enum Demography {
	Uneducated,
	Educated,
	LowerClass,
	MiddleClass,
	UpperClass,
	Worker,
	Studetns,
	Extremists,
	Religionist,
	Politicians,
	Businessmen,
	Celebrity
}

public class Country {
	public readonly string name;
	public int believerPercentage;
	private float conversionRate = 0;
	private List<Effect> effects;

	public Country(string name) {
		this.name = name;
		this.believerPercentage = 0;
		this.effects = new List<Effect>();
	}

	public void Update(DateTime currentDate) {
		int numRemoved = effects.RemoveAll((e) => e.endDate <= currentDate);
		if (numRemoved > 0) {
			conversionRate = CalculateConversionRate();
		}

		//apply the effects to the growth rate
		//apply the growth rate to the number of people
		if(conversionRate == 0 || believerPercentage >= 100000) {
			return;
		}

		float randomModifier = UnityEngine.Random.Range(1.05f, 0.95f);

		//Debug.Log("conversion rate: " + conversionRate);
		//Debug.Log("randomMonfied: " + (conversionRate * randomModifier));
		//Debug.Log("wow: " + (conversionRate * randomModifier * 1000));

		int numConverted = (int)(conversionRate * randomModifier * 1000);
		believerPercentage += numConverted;
		believerPercentage = Math.Min(Math.Max(believerPercentage, 0), 100000);
	}

	public List<Effect> GetEffects() {
		return effects;
	}

	public void AddEffect(Effect effect) {
		Debug.Log("effect: " + effect.action.actionName + " is added to the country: " + name);

		Debug.Log("Finding Effect " + effect.action.actionName);
		Effect e = FindEffect(effect.action.actionName);
		if(e != null) {
			effects.Remove(e);
		}

		effects.Add(effect);
		this.conversionRate = CalculateConversionRate();
	}

	public float CalculateConversionRate() {
		float conversionRate = -0.005f;
		foreach(var e in effects) {
			conversionRate += e.action.baseRate;
		}
		return conversionRate;
	}

	public Effect FindEffect(string effect) {
		foreach(var e in this.effects) {
			Debug.Log((e as Effect).action.actionName + " and " + effect);
			if((e as Effect).action.actionName.Equals(effect)) {
				return (Effect)e;
			}
		}
		return null;
	}

	public int GetActionCost(GameAction action) {
		return action.baseCost;
	}
}
