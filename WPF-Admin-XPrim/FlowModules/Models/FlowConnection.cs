using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace FlowModules.Models
{
    public class FlowConnection
    {
        private readonly DoubleAnimation? _dashOffsetAnimation = new DoubleAnimation()
        {
            From = 8,
            To = 0,
            Duration = TimeSpan.FromSeconds(1.5),
            RepeatBehavior = RepeatBehavior.Forever
        };

        public NodePort StartPort { get; set; }
        public NodePort EndPort { get; set; }
        public Path? Path { get; set; }

        public string StartPortId { get; set; }
        public string EndPortId { get; set; }


        public FlowConnection(NodePort startPort, NodePort endPort)
        {
            StartPort = startPort;
            EndPort = endPort;
        }

        public Brush ConnectionColor { get; set; } = new SolidColorBrush(Colors.GreenYellow);

        public void UpdatePath()
        {
            if (Path == null) return;
            Path.StrokeDashArray = new DoubleCollection([4, 4]);
            var startPoint = StartPort.TranslatePoint(
                new Point(
                    (StartPort.Width == 0 ? 20 : StartPort.Width) / 2,
                (StartPort.Height == 0 ? 20 : StartPort.Height) / 2),
                Path.Parent as Canvas);

            var endPoint = EndPort.TranslatePoint(
                new Point((StartPort.Width == 0 ? 20 : StartPort.Width) / 2,
                (StartPort.Height == 0 ? 20 : StartPort.Height) / 2),
                Path.Parent as Canvas);

            // 计算控制点
            // 计算控制点，使用起点和终点的距离来动态调整控制点的位置
            var distance = Math.Abs(endPoint.X - startPoint.X);
            var xOffset = Math.Min(distance * 0.5, 100); // 根据距离动态调整偏移量

            var controlPoint1 = new Point(
                startPoint.X + xOffset,
                startPoint.Y);

            var controlPoint2 = new Point(
                endPoint.X - xOffset,
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