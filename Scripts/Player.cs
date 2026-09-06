using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float speed {get; set;} = 300.0f;
	public Projector projector {get; set;}
	
	public AnimatedSprite2D sprite;
	public string lastAnim;
	public Vector2 facing = Vector2.Down;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		GameSession session = GetNode<GameSession>("/root/GameSession");
		
		projector = session.playerProjector;
		
		if (session.returnPosition != Vector2.Zero)
		{
			GlobalPosition = session.returnPosition;
			facing = session.returnFacing;
		}
		
		if (facing != Vector2.Zero)
		{
			if (Mathf.Abs(facing.X) > Mathf.Abs(facing.Y))
			{
				lastAnim = facing.X > 0 ? "walk_right" : "walk_left";
			}
			else
			{
				lastAnim = facing.Y > 0 ? "walk_front" : "walk_back";
			}
			
			sprite.Play(lastAnim);
		}
		else
		{
			sprite.Play("walk_front");
			lastAnim = "walk_front";
		}
		sprite.Pause();
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			facing = direction;
			Velocity = direction * speed;
			PlayWalkAnim(direction);
		}
		else
		{
			Velocity = Vector2.Zero;
			sprite.Pause();
		}

		MoveAndSlide();
	}
	
	public override void _UnhandledInput(InputEvent e)
	{
		if (!e.IsActionPressed("ui_accept"))
		{
			return;
		}
		
		GD.Print("Player: Accept key pressed.");
		GD.Print($"Player: pos={GlobalPosition} facing={facing} to={GlobalPosition + facing * 32}");
		
		var space = GetWorld2D().DirectSpaceState;
		var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + facing * 32f);
		var hit = space.IntersectRay(query);
		
		GD.Print($"Player: hit count={hit.Count}");
		
		if (hit.Count > 0)
		{
			GD.Print($"Player: collider={hit["collider"]} type={hit["collider"].AsGodotObject().GetType()}");
		}
		
		if (hit.Count > 0 && hit["collider"].AsGodotObject() is NPC npc)
		{
			GD.Print("Player: Interact with NPC.");
			npc.Interact(this);
		}
		else
		{
			GD.Print("Player: No NPC present.");
		}
	}
	
	public void PlayWalkAnim(Vector2 direction)
	{
		string anim;
		
		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			anim = direction.X > 0 ? "walk_right" : "walk_left";
		}
		else
		{
			anim = direction.Y > 0 ? "walk_front" : "walk_back";
		}
		
		if (anim != lastAnim || !sprite.IsPlaying())
		{
			sprite.Play(anim);
			lastAnim = anim;
		}
	}
}
