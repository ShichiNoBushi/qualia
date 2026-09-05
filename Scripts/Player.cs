using Godot;
using System;

public partial class Player : CharacterBody2D
{
	[Export] public float speed {get; set;} = 300.0f;
	[Export] public Projector projector {get; set;}
	
	public AnimatedSprite2D sprite;
	public string lastAnim;
	
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		
		sprite.Play("walk_front");
		lastAnim = "walk_front";
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
