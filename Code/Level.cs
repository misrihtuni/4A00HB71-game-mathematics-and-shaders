// Copyright (c) 2026 Miska Rihu
// License: MIT License (see LICENSE in project root)

using Godot;

namespace MathAndShaders
{
    public partial class Level : Node3D
    {
        [Export]
        public Wall[] Walls { get; private set; }

        [Export]
        public Ball Ball { get; private set; }

        public override void _Ready()
        {
            foreach (var wall in Walls)
            {
                GD.Print(
                    $"{wall.Name}:\n"
                        + $" position={wall.GlobalPosition}\n"
                        + $" basis={wall.GlobalBasis}\n"
                        + $" rotation={wall.GlobalRotationDegrees}\n"
                        + $" transform={wall.GlobalTransform}\n"
                        + $" size: {wall.GetAabb().Size}"
                );
                GD.Print();
            }
        }

        public override void _Process(double delta)
        {
            Ball.Move();
        }
    }
}
