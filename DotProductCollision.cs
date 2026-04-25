using Godot;

namespace MathAndShaders
{
    public partial class DotProductCollision : Node3D
    {
        [Export]
        private MeshInstance3D[] _walls;

        [Export]
        private float _speed = 1f;

        [Export]
        private float _radius = 0.5f;

        [Export]
        private Vector3 _direction = Vector3.Right;

        public override void _Ready()
        {
            _direction = _direction.Normalized();
        }

        public override void _Process(double delta)
        {
            Vector3 currentPosition = GlobalPosition;
            Vector3 newPosition = currentPosition + _direction * _speed * (float)delta;

            foreach (MeshInstance3D wall in _walls)
            {
                // Let's assume the following:
                //
                // B is the center point of the ball (vec3).
                // d is the movement direction of the ball (vec3, normalized)
                // r is the radius of the ball (float).
                //
                // The point on the ball that will hit the wall is
                //     X = B + d * r
                //
                // If P is any point on the wall (vec3) and n (vec3) is the
                // normal of the wall, then a vector from P to X is
                //     PX = X - P = (B + d * r) - P
                //
                // If the ball is behind the wall, n and PX are pointing in the
                // opposite directions:
                //     dot(PX, n) < 0
                //
                // If the ball is moving towards the wall, d and n are pointing
                // in opposite-ish directions:
                //     dot(d, n) < 0
                //
                // If both of these conditions are true, the ball has collided
                // with the wall and the direction should be swapped.

                // Since the orientation of the wall is set to Y in the editor,
                // the normal points to the up direction in the local space and
                // thus Vector3.Up is used.
                Vector3 normal = (wall.GlobalTransform.Basis * Vector3.Up).Normalized(); // n

                // Using the global origin seems to work here, but it might not
                // work if the mesh is rotated or scaled. At least that's what
                // Copilot told me when I asked about it.
                Vector3 pointOnWall = wall.GlobalTransform.Origin; // P
                Vector3 pointOnBall = GlobalPosition + _direction * _radius; // X
                Vector3 wallToBall = pointOnBall - pointOnWall; // PX

                bool isBehindWall = wallToBall.Dot(normal) < 0;
                bool movingTowardsWall = _direction.Dot(normal) < 0;

                if (movingTowardsWall && isBehindWall)
                {
                    GD.Print($"Ball collided with {wall.Name} wall.");
                    _direction *= -1;
                    break;
                }
            }

            GlobalPosition = newPosition;
        }
    }
}
