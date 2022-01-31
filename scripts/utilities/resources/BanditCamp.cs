using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class BanditCamp : Camp
{
	[Signal]
	public delegate void SpawnEntity(MovingEntity entity);
	private Timer spawnTimer = new Timer();
	private PackedScene packedBandit;
	private Node charactersNode;
	private int spawnedBandits = 0;

	public override void _Ready()
	{
		workerProfession = Constants.BANDIT_PROFESSION;
		packedBandit = (PackedScene)GD.Load(Constants.BANDIT);
		charactersNode = GetNode("../../Characters");
		spawnTimer.OneShot = true;   // Instancing the spawnTimer
		spawnTimer.WaitTime = 60;
		spawnTimer.Connect("timeout", this, nameof(SpawnBandit));
		AddChild(spawnTimer);
		spawnTimer.Start();
		base._Ready();
	}
	private void OnBanditDeath(Character bandit) {
		spawnedBandits--;
	}
	private void SpawnBandit() {
		if (spawnedBandits < maxWorkers) {
			spawnedBandits++;
			Npc banditInstance = (Npc)packedBandit.Instance().Duplicate();
			banditInstance.Position = Position;
			banditInstance.entityName = "Bad Boy Bandit";
			charactersNode.AddChild(banditInstance);
			banditInstance.Connect("Dead", this, nameof(OnBanditDeath));
			EmitSignal(nameof(SpawnEntity), banditInstance);
			GD.Print("Spawned a bandit");
		}
		spawnTimer.Start();
	}
}
