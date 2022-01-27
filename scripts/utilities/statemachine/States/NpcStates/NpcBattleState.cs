using Godot;
using System.Linq;

public class NpcBattleState : NpcMoveState
{
    private float weaponRange = Constants.DEF_ATTACKRANGE; // Placeholder.
    private const float tickSpeed = Constants.TICK;
    private float combatEscapeTime = Constants.COMBATESCAPETIME;
    private float combatCooldown = 0;
    private float tickDelta = 0;
    private int ticks = 0;
    private bool staggered = false;
    private Character target;

    public override void Enter() {
        target = owner.GetTarget();
        if (target == null) {
            EmitSignal(nameof(Finished), "idle");
            return;
        }
        else if (target == owner) {
            GD.Print(string.Format("{0}: Why would I hit myself?", owner.entityName));
            EmitSignal(nameof(Finished), "idle");
            return;
        }
        owner.GetMovePath(owner.GlobalPosition, target.Position, owner);
    }

    public override void Exit()
    {
        owner.ClearTargets();
        combatCooldown = 0;
        base.Exit();
    }

    public override void Update(float delta)
    {
        target = owner.GetTarget();
        if (target != null) {
            if (staggered) {
                TickLoop(delta);
            } else if (owner.Position.DistanceTo(target.Position) < weaponRange) {
                combatCooldown = 0;
                AttackTarget();
                return;
            } else {
                CombatEscape(delta);
            }
            base.Update(delta);
        } else {
            EmitSignal(nameof(Finished), "idle");
        }
    }

    private void TickLoop(float delta) {
        tickDelta += delta;
        if (tickDelta >= tickSpeed) {
            ticks++;
            tickDelta = 0;
        }
        if (ticks >= owner.stats.attackSpeed) {
            tickDelta = 0;
            ticks = 0;
            staggered = false;
        }

    }

    private void CombatEscape(float delta) {
        combatCooldown += delta;
        if (combatCooldown >= combatEscapeTime) {
            EmitSignal(nameof(Finished), "idle");
        }
    }

    private void AttackTarget() {
        if (target.IsDead()) {
            owner.ClearCurrentTarget();
            if (owner.GetTarget() == null) {
                EmitSignal(nameof(Finished), "idle");
                return;
            }
        }
        // Here the attack is supposed to be fetched from a dictionary with weighing.
        Attack attack = new Attack(owner);
        // Play attack animation at speed x
        target.TakeAttack(attack);
        staggered = true;
    }

    public override void HandleAttack()
    {
        combatCooldown = 0;
        if (owner.IsDead()) {
            base.HandleAttack();
        }
    }

    protected override void MovementLoop(float delta)
    {
        float distanceToLast = owner.movePath.Any() ? owner.Position.DistanceTo(owner.movePath.Last()) : 0;
        if (distanceToLast > 0) {
            if (distanceToLast > weaponRange) {
                base.MovementLoop(delta);
            } else if (owner.movePath.Last() != target.Position) {
                owner.GetMovePath(owner.GlobalPosition, target.Position, owner);
            }   
        }
    }
}