using Godot;

namespace MathAndShaders
{
    public partial class Camera : Camera3D
    {
        [Export]
        private float _movementSpeed = 4f;

        [Export]
        private float _rotationSpeed = 1f;

        public override void _Process(double delta)
        {
            float fDelta = (float)delta;

            // Position.
            Vector3 position = GlobalPosition;
            float positionChange = _movementSpeed * fDelta;

            if (Input.IsActionPressed("move_up"))
            {
                position.Y += positionChange;
            }
            else if (Input.IsActionPressed("move_down"))
            {
                position.Y -= positionChange;
            }

            if (Input.IsActionPressed("move_right"))
            {
                position.X += positionChange;
            }
            else if (Input.IsActionPressed("move_left"))
            {
                position.X -= positionChange;
            }

            if (Input.IsActionPressed("move_back"))
            {
                position.Z += positionChange;
            }
            else if (Input.IsActionPressed("move_forward"))
            {
                position.Z -= positionChange;
            }

            GlobalPosition = position;

            // Rotation.
            Vector3 rotation = GlobalRotation;
            float rotationChange = _rotationSpeed * fDelta;

            if (Input.IsActionPressed("rotate_up"))
            {
                rotation.X += rotationChange;
            }
            else if (Input.IsActionPressed("rotate_down"))
            {
                rotation.X -= rotationChange;
            }

            if (Input.IsActionPressed("rotate_left"))
            {
                rotation.Y += rotationChange;
            }
            else if (Input.IsActionPressed("rotate_right"))
            {
                rotation.Y -= rotationChange;
            }

            GlobalRotation = rotation;
        }
    }
}
