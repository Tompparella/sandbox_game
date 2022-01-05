using Godot;
using System;

public class LogisticsItem : Item
{

	public LogisticsItem(float _supplyValue = 0)
	{
		supplyValue = _supplyValue;
	}
	public LogisticsItem()
	{
	}
	[Export]
	public float supplyValue { get; private set; } = 0;
}
