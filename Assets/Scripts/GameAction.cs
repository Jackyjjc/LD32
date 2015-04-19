using UnityEngine;
using System;
using System.Collections;

public sealed class GameAction {

	public readonly string actionName;
	public readonly string description;
	public readonly int baseCost;
	public readonly int resistenceModifier;
	public readonly TimeSpan baseDuration;
	public readonly float baseRate;

	public GameAction(string actionName, int cost, TimeSpan baseDuration, float baseRate, int resistenceModifier, string extraRequirement = "") {
		this.actionName = actionName;
		this.description = string.Format("Influence Cost: {0}\nImpact: {1}\nDuration: {2}", 
		                                 cost.ToString(), 
		                                 ImpactDescription(baseRate), 
		                                 DurationDescription(baseDuration)) + extraRequirement;
		this.baseCost = cost;
		this.resistenceModifier = resistenceModifier;
		this.baseDuration = baseDuration;
		this.baseRate = baseRate;
	}

	private string ImpactDescription(float baseRate) {
		if(baseRate >= 0 && baseRate < 0.1f) {
			return "Low";
		} else if (baseRate - 0.1f >= float.Epsilon && baseRate < 1f) {
			return "Medium";
		} else if (baseRate - 1f >= float.Epsilon && baseRate < 10f){
			return "High";
		} else {
			return "Very High";
		}
	}

	private string DurationDescription(TimeSpan duration) {
		if (duration.TotalDays >= 0 && duration.TotalDays < 5) {
			return "Very Short";
		} else if (duration.TotalDays >= 5 && duration.TotalDays < 25) {
			return "Short";
		} else if (duration.TotalDays >= 25 && duration.TotalDays < 90) {
			return "Medium";
		} else if (duration.TotalDays >= 90 && duration.TotalDays < 180) {
			return "Long";
		} else {
			return "Very Long";
		}
	}
}
