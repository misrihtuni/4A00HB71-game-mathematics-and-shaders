using System;
using Godot;

namespace MathAndShaders
{
    public partial class ThickWalls : Node3D
    {
        public class Hit
        {
            public Vector3 Point { get; set; }
            public Vector3 Normal { get; set; }
            public Vector3 Penetration { get; set; }
        }

        // Funny materials for funny balls.
        private Material matX = GD.Load<Material>("res://materialX.tres");
        private Material matY = GD.Load<Material>("res://materialY.tres");
        private Material matZ = GD.Load<Material>("res://materialZ.tres");
        private Material matO = GD.Load<Material>("res://materialO.tres");

        // Walls.
        [Export]
        private MeshInstance3D[] _walls;

        // Ball related stuff.
        [ExportGroup("Ball")]
        [Export]
        private Node3D _ball;

        [Export]
        private float _ballSpeed = 5f;

        [Export]
        private Vector3 _ballDirection = Vector3.Right;

        [Export]
        private float _ballRadius = 0f;

        public override void _Ready()
        {
            _ballDirection = _ballDirection.Normalized();
        }

        public override void _Process(double delta)
        {
            Vector3 velocity = _ballSpeed * _ballDirection;
            Vector3 movement = velocity * (float)delta;
            Vector3 newPosition = _ball.GlobalPosition + movement;

            foreach (MeshInstance3D wall in _walls)
            {
                Hit hit = Intersects(wall, newPosition);

                if (hit != null)
                {
                    _ballDirection = Bounce(_ballDirection, hit.Normal).Normalized();
                    Vector3 penetration = _ballDirection * hit.Penetration;
                    newPosition = hit.Point + hit.Normal * penetration;
                    break;
                }
            }

            _ball.GlobalPosition = newPosition;
        }

        private Hit Intersects(MeshInstance3D box, Vector3 point)
        {
            Aabb localAabb = box.GetAabb();
            Transform3D globalTransform = box.GlobalTransform;

            // Extents in world space.
            Vector3 localExtents = localAabb.Size / 2;
            Vector3 globalScale = globalTransform.Basis.Scale;
            Vector3 globalExtents = localExtents * globalScale.Abs();

            // Center in world space.
            Vector3 localCenter = localAabb.Position + (globalExtents);
            Vector3 globalCenter = globalTransform * localCenter;

            Vector3 delta = point - globalCenter;

            float penX = globalExtents.X - Mathf.Abs(delta.X);
            float penY = globalExtents.Y - Mathf.Abs(delta.Y);
            float penZ = globalExtents.Z - Mathf.Abs(delta.Z);

            if (penX < 0 || penY < 0 || penZ < 0)
            {
                return null;
            }

            Vector3 normal;
            Vector3 penetration;
            Vector3 collisionPoint;

            float smallest = Mathf.Min(penX, penY);
            smallest = Mathf.Min(smallest, penZ);

            if (Mathf.IsEqualApprox(smallest, penX))
            {
                float signX = Mathf.Sign(delta.X);
                normal = new Vector3(signX, 0, 0);
                penetration = new Vector3(penX * signX, 0, 0);
                collisionPoint = new Vector3(globalCenter.X + (globalExtents.X * signX), point.Y, point.Z);
            }
            else if (Mathf.IsEqualApprox(smallest, penY))
            {
                float signY = Mathf.Sign(delta.Y);
                normal = new Vector3(0, signY, 0);
                penetration = new Vector3(0, penY * signY, 0);
                collisionPoint = new Vector3(point.X, globalCenter.Y + (globalExtents.Y * signY), point.Z);
            }
            else if (Mathf.IsEqualApprox(smallest, penZ))
            {
                float signZ = Mathf.Sign(delta.Z);
                normal = new Vector3(0, 0, signZ);
                penetration = new Vector3(0, 0, penZ * signZ);
                collisionPoint = new Vector3(point.X, point.Y, globalCenter.Z + (globalExtents.Z * signZ));
            }
            else
            {
                // TODO: Think about this case later.
                throw new NotImplementedException();
            }

            // if (penZ < penX || penZ < penY)
            // {
            //     // Closer to back/front.
            //     if (penZ < penX) { }
            //     if (penZ < penY) { }
            // }
            // else if (penZ < penX || penX < penY)
            // {
            //     // Closer to left/right.
            //     if (penZ < penX) { }
            //     if (penX < penY) { }
            // }
            // else if (penY < penX || penY < penZ)
            // {
            //     // Closer to top/bottom.
            //     if (penY < penX) { }
            //     if (penY < penZ) { }
            // }

            return new Hit()
            {
                Normal = normal,
                Penetration = penetration,
                Point = collisionPoint,
            };
        }

        private Vector3 Bounce(Vector3 direction, Vector3 normal)
        {
            Vector3 dx = direction.Dot(normal) * normal;
            Vector3 dy = direction - dx;
            return dy - dx;
        }

        private MeshInstance3D CreateMarker(Material color, string name)
        {
            return new MeshInstance3D()
            {
                Mesh = new SphereMesh() { Radius = .1f, Height = .2f },
                MaterialOverlay = color,
                Name = name,
            };
        }

        private void AddMarker(Node3D marker, Node3D target, Vector3 pos)
        {
            target.AddChild(marker);
            marker.GlobalPosition = pos;
        }

        private void AddMarkers()
        {
            foreach (var wall in _walls)
            {
                Vector3 origin = wall.GlobalTransform.Origin;
                Vector3 X = wall.GlobalBasis.X.Normalized();
                Vector3 Y = wall.GlobalBasis.Y.Normalized();
                Vector3 Z = wall.GlobalBasis.Z.Normalized();
                AddMarker(CreateMarker(matO, "Origin"), wall, origin);
                AddMarker(CreateMarker(matX, "X"), wall, origin + X);
                AddMarker(CreateMarker(matY, "Y"), wall, origin + Y);
                AddMarker(CreateMarker(matZ, "Z"), wall, origin + Z);
            }
        }
    }
}
