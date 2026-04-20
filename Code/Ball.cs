// Copyright (c) 2026 Miska Rihu
// License: MIT License (see LICENSE in project root)

using Godot;

namespace MathAndShaders
{
    public partial class Ball : MeshInstance3D
    {
        #region Fields

        [Export]
        private Vector3 _initialDirection = Vector3.Zero;

        #endregion Fields


        #region Properties

        [Export]
        public float Speed { get; private set; } = 10;

        public Vector3 CurrentDirection { get; private set; }

        #endregion Properties


        #region Public API

        public override void _Ready()
        {
            CurrentDirection = _initialDirection.Normalized();
        }

        public void Move()
        {
            var fDelta = (float)GetProcessDeltaTime();
            var movement = Speed * CurrentDirection * fDelta;
            GlobalPosition = GlobalPosition + movement;
        }

        #endregion Public API
    }
}
