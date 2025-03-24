using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FlowModules.Models {
    public class FlowConnection {
        private readonly DoubleAnimation? _dashOffsetAnimation = new DoubleAnimation() {
            From = 8,
            To = 0,
            Duration = TimeSpan.FromSeconds(1.5),
            RepeatBehavior = RepeatBehavior.Forever
        };

        public NodePort StartPort { get; }
        public NodePort EndPort { get; }
        public Path? Path { get; set; }

        public FlowConnection(NodePort startPort, NodePort endPort) {
            StartPort = startPort;
            EndPort = endPort;
        }

        public Brush ConnectionColor { get; set; } = new SolidColorBrush(Colors.LightGreen);

        public void UpdatePath() {
            if (Path == null) return;
            Path.StrokeDashArray = new DoubleCollection([4, 4]);
            var startPoint = StartPort.TranslatePoint(
                new Point(StartPort.Width / 2, StartPort.Height / 2),
                Path.Parent as Canvas);

            var endPoint = EndPort.TranslatePoint(
                new Point(EndPort.Width / 2, EndPort.Height / 2),
                Path.Parent as Canvas);

            // 计算控制点
            var controlPoint1 = new Point(
                startPoint.X + 100, // 向右偏移100
                startPoint.Y);

            var controlPoint2 = new Point(
                endPoint.X - 100, // 向左偏移100
                endPoint.Y);

            // 更新路径几何
            var geometry = Path.Data as PathGeometry;
            var figure = geometry.Figures[0];
            var segment = figure.Segments[0] as BezierSegment;

            figure.StartPoint = startPoint;
            if (segment is not null)
            {
                segment.Point1 = controlPoint1;
                segment.Point2 = controlPoint2;
                segment.Point3 = endPoint;
            }

            Path.BeginAnimation(System.Windows.Shapes.Shape.StrokeDashOffsetProperty, _dashOffsetAnimation);
        }
    }
}