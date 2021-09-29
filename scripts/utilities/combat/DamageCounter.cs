using Godot;
using System;

public class DamageCounter : Node2D
{
    private AnimationPlayer animationPlayer;
    private Label damageLabel;
    private string damage;

    public override void _Ready()
    {
        animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
        damageLabel = (Label)GetNode("Label");

        damageLabel.Text = damage.ToString();
        animationPlayer.Play("damageCounter");
    }
    public void init(string _damage) {
        damage = _damage;
    }
    public void OnAnimationFinished(string animationName) {
        QueueFree();
    }
}
