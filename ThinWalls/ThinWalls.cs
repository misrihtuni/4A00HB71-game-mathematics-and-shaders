using Godot;

namespace MathAndShaders
{
    public partial class ThinWalls : Node3D
    {
        private float _time = 0;

        [ExportGroup("Walls")]
        [Export]
        private MeshInstance3D[] _walls;

        [ExportGroup("Ball")]
        [Export]
        private MeshInstance3D _ball;

        [Export]
        private float _ballSpeed = 5f;

        [Export]
        private Vector3 _ballDirection = Vector3.Right;

        [Export]
        private float _ballRadius = 1f;

        [ExportGroup("Refresh Rate")]
        [Export]
        private bool _limitRefreshRate = false;

        [Export(PropertyHint.None, "0,or_greater,suffix:s/frame")]
        private float _updateInterval = 0.5f;

        [ExportGroup("Misc")]
        [Export]
        private Camera _camera;

        [Export]
        private Marker3D _initialBallPosition;

        [Export]
        private Marker3D _initialCameraPosition;

        [Export]
        private bool _printLogs = true;

        public override void _Ready()
        {
            _ballDirection = _ballDirection.Normalized();
            _ball.GlobalPosition = _initialBallPosition.GlobalPosition;
            _camera.GlobalPosition = _initialCameraPosition.GlobalPosition;
            _camera.GlobalRotation = _initialCameraPosition.GlobalRotation;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("reset_ball"))
            {
                _ball.GlobalPosition = _initialBallPosition.GlobalPosition;
            }

            if (@event.IsActionPressed("reset_camera"))
            {
                _camera.GlobalPosition = _initialCameraPosition.GlobalPosition;
                _camera.GlobalRotation = _initialCameraPosition.GlobalRotation;
            }
        }

        public override void _Process(double delta)
        {
            if (_limitRefreshRate)
            {
                _time += (float)delta;

                if (_time < _updateInterval)
                {
                    return;
                }

                _time = 0;
            }

            Vector3 ballVelocity = _ballSpeed * _ballDirection;
            Vector3 ballMovement = ballVelocity * (float)delta;
            Vector3 newBallPosition = _ball.GlobalPosition + ballMovement;

            if (_limitRefreshRate && _printLogs)
            {
                GD.Print(
                    $"[Ball] "
                        + $"Velocity: {ballVelocity}, "
                        + $"Movement: {ballMovement}, "
                        + $"New Position: {newBallPosition}"
                );
            }

            foreach (MeshInstance3D wall in _walls)
            {
                Vector3 wallNormal = (wall.GlobalTransform.Basis * Vector3.Forward).Normalized();
                float dotProduct = ballVelocity.Dot(wallNormal);

                Vector3 wallCenter = wall.GlobalTransform.Origin;
                Vector3 testPoint = _ball.GlobalPosition + (_ballDirection * _ballRadius);
                float signedDistance = wallNormal.Dot(testPoint - wallCenter);

                if (signedDistance < 0 && dotProduct > 0)
                {
                    newBallPosition -= ballMovement;
                    _ballDirection *= -1;
                }

                if (_limitRefreshRate && _printLogs)
                {
                    GD.Print(
                        $"[{wall.Name}] "
                            + $"Center: {wallCenter}, "
                            + $"Normal: {wallNormal}, "
                            + $"Test point: {testPoint}, "
                            + $"Dot Product: {dotProduct}, "
                            + $"Signed Distance: {signedDistance}"
                    );
                }
            }

            if (_limitRefreshRate && _printLogs)
            {
                GD.Print();
            }

            _ball.GlobalPosition = newBallPosition;
        }
    }
}
