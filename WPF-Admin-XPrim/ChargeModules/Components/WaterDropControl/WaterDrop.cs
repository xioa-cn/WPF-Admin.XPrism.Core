using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChargeModules.Components.WaterDropControl;

public class WaterDrop : UserControl
{
    private Canvas mainCanvas;
    private Path waterWave1;
    private Path waterWave2;
    private TextBlock? percentText;
    private double waveOffset1 = 0;
    private double waveOffset2 = Math.PI;
    private readonly DispatcherTimer animationTimer;

    public static readonly DependencyProperty WaveSpeedProperty =
        DependencyProperty.Register("WaveSpeed", typeof(double), typeof(WaterDrop),
            new PropertyMetadata(1.0, OnWaveSpeedChanged));

    public double WaveSpeed
    {
        get { return (double)GetValue(WaveSpeedProperty); }
        set { SetValue(WaveSpeedProperty, Math.Max(0.1, Math.Min(5.0, value))); }
    }

    private static void OnWaveSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (WaterDrop)d;
        control.UpdateWaveAnimation();
    }

    public static readonly DependencyProperty PercentageProperty =
        DependencyProperty.Register("Percentage", typeof(double), typeof(WaterDrop),
            new PropertyMetadata(50.0, OnPercentageChanged));

    public double Percentage
    {
        get { return (double)GetValue(PercentageProperty); }
        set { SetValue(PercentageProperty, Math.Max(0, Math.Min(100, value))); }
    }

    private static void OnPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (WaterDrop)d;
        control.UpdateWaveShape();
        control.UpdatePercentText();
    }

    public static readonly DependencyProperty WaterColorProperty =
        DependencyProperty.Register("WaterColor", typeof(Color), typeof(WaterDrop),
            new PropertyMetadata(Color.FromArgb(180, 0, 150, 255)));

    public Color WaterColor
    {
        get { return (Color)GetValue(WaterColorProperty); }
        set { SetValue(WaterColorProperty, value); }
    }

    public WaterDrop()
    {
        // 在控件模板应用后获取Canvas控件
        mainCanvas = null;

        // 创建双层水波
        // 创建外圈边框
        var borderEllipse = new Ellipse
        {
            Stroke = new SolidColorBrush(WaterColor),
            StrokeThickness = 3,
            Width = ActualWidth,
            Height = ActualHeight
        };

        waterWave1 = new Path
        {
            Fill = new SolidColorBrush(Color.FromArgb((byte)(WaterColor.A * 0.8), WaterColor.R, WaterColor.G, WaterColor.B)),
            Effect = new System.Windows.Media.Effects.BlurEffect
            {
                Radius = 3
            },
            Opacity = 0.6
        };

        waterWave2 = new Path
        {
            Fill = new SolidColorBrush(WaterColor),
            Effect = new System.Windows.Media.Effects.BlurEffect
            {
                Radius = 2
            },
            Opacity = 0.4
        };


        // 初始化动画计时器
        animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        animationTimer.Tick += (s, e) => UpdateWaveAnimation();

        Loaded += (s, e) =>
        {
            mainCanvas = GetTemplateChild("PART_MainCanvas") as Canvas;
            waterWave1 = GetTemplateChild("PART_WaterWave1") as Path;
            waterWave2 = GetTemplateChild("PART_WaterWave2") as Path;
            percentText = GetTemplateChild("PART_PercentText") as TextBlock;

            if (mainCanvas != null && waterWave1 != null && waterWave2 != null && percentText != null)
            {
                // 添加外圈边框
                var borderEllipse = new Ellipse
                {
                    Stroke = new SolidColorBrush(WaterColor),
                    StrokeThickness = 3,
                    Width = ActualWidth,
                    Height = ActualHeight
                };
                mainCanvas.Children.Add(borderEllipse);
                InitializeAnimation();
                animationTimer.Start();
            }
        };

        Unloaded += (s, e) =>
        {
            animationTimer.Stop();
        };
    }

    private void InitializeAnimation()
    {
        UpdateWaveShape();
        UpdatePercentText();
    }

    private void UpdateWaveShape()
    {
        if (ActualWidth <= 0 || ActualHeight <= 0) return;

        var radius = Math.Min(ActualWidth, ActualHeight) / 2;
        var centerX = ActualWidth / 2;
        var centerY = ActualHeight / 2;

        // 创建圆形裁剪区域
        var clipGeometry = new EllipseGeometry(new Point(centerX, centerY), radius, radius);
        waterWave1.Clip = clipGeometry;
        waterWave2.Clip = clipGeometry;

        // 计算水位高度，添加偏移使波浪更自然
        var waterHeight = ActualHeight - (Percentage / 100.0 * ActualHeight);
        UpdateWavePath(waterWave1, waterHeight, waveOffset1, 15);
        UpdateWavePath(waterWave2, waterHeight, waveOffset2, 20);
    }

    private void UpdateWavePath(Path wave, double waterHeight, double offset, double amplitude)
    {
        var path = new PathGeometry();
        var figure = new PathFigure();
        figure.StartPoint = new Point(0, waterHeight);

        var points = new List<Point>();
        var step = ActualWidth / 30; // 增加采样点使波浪更平滑

        for (double x = -step; x <= ActualWidth + step; x += step)
        {
            var y = waterHeight + Math.Sin((x / ActualWidth * 3 * Math.PI) + offset) * amplitude;
            points.Add(new Point(x, y));
        }

        // 使用贝塞尔曲线创建平滑的波浪
        var segment = new PolyBezierSegment();
        for (int i = 0; i < points.Count - 3; i++)
        {
            var p1 = points[i];
            var p2 = points[i + 1];
            var p3 = points[i + 2];
            var p4 = points[i + 3];

            // 使用Catmull-Rom样条曲线计算控制点
            var tension = 0.5;
            var cp1 = new Point(
                p2.X + (p3.X - p1.X) * tension / 6,
                p2.Y + (p3.Y - p1.Y) * tension / 6);
            var cp2 = new Point(
                p3.X - (p4.X - p2.X) * tension / 6,
                p3.Y - (p4.Y - p2.Y) * tension / 6);

            segment.Points.Add(cp1);
            segment.Points.Add(cp2);
            segment.Points.Add(p3);
        }

        figure.Segments.Add(segment);
        figure.Segments.Add(new LineSegment(new Point(ActualWidth, ActualHeight), true));
        figure.Segments.Add(new LineSegment(new Point(0, ActualHeight), true));

        path.Figures.Add(figure);
        wave.Data = path;
    }

    private void UpdateWaveAnimation()
    {
        waveOffset1 += 0.05 * WaveSpeed;
        waveOffset2 += 0.08 * WaveSpeed;

        if (waveOffset1 >= Math.PI * 2) waveOffset1 = 0;
        if (waveOffset2 >= Math.PI * 2) waveOffset2 = 0;

        UpdateWaveShape();
    }

    private void UpdatePercentText()
    {
        if (percentText is null) return;
        percentText.Text = $"{Percentage:F0}%";
        Canvas.SetLeft(percentText, (ActualWidth - percentText.ActualWidth) / 2);
        Canvas.SetTop(percentText, (ActualHeight - percentText.ActualHeight) / 2);
    }
}