using UnityEngine;
using System;
using System.Collections;

public sealed class GameAction {

	public readonly string actionName;
	public readonly int baseCost;
	public readonly TimeSpan baseDuration;
	public readonly float baseRate;

	public GameAction(string actionName, int baseCost, TimeSpan baseDuration, float baseRate) {
		this.actionName = actionName;
		this.baseCost = baseCost;
		this.baseDuration = baseDuration;
		this.baseRate = baseRate;
	}
}
