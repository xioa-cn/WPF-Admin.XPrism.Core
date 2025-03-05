using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PersonalityComponentModules.Views;

public partial class ParticleClock : Page {
    private readonly Random _random = new Random();
    private readonly List<Ellipse> _particles = new List<Ellipse>();
    private readonly DispatcherTimer _timer = new DispatcherTimer();
    private readonly int _particleCount = 1000; // 粒子总数
    private readonly double _transitionSpeed = 0.5; // 粒子过渡速度
    
    // 数字显示的点阵定义
    private readonly Dictionary<char, bool[,]> _digitMatrices = new Dictionary<char, bool[,]>();
    
    // 当前时间
    private DateTime _currentTime;
    
    // 在类的成员变量部分添加
    private List<Point> _currentTimePoints = new List<Point>();
    
    public ParticleClock() {
        InitializeComponent();
        InitializeDigitMatrices();
        CreateParticles();
        
        // 设置定时器
        _timer.Interval = TimeSpan.FromMilliseconds(900);
        _timer.Tick += Timer_Tick;

        this.Loaded += ParticleClock_Loaded;

        this.Unloaded += ParticleClock_Unloaded;
        
        // 初始更新时间
        UpdateClock();
    }

    private void ParticleClock_Unloaded(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
    }

    private void ParticleClock_Loaded(object sender, RoutedEventArgs e)
    {
        _timer.Start();
    }

    private void InitializeDigitMatrices()
    {
        // 0-9数字的点阵定义
        _digitMatrices['0'] = new bool[5, 3]
        {
            { true, true, true },
            { true, false, true },
            { true, false, true },
            { true, false, true },
            { true, true, true }
        };
        
        _digitMatrices['1'] = new bool[5, 3]
        {
            { false, true, false },
            { false, true, false },
            { false, true, false },
            { false, true, false },
            { false, true, false }
        };
        
        _digitMatrices['2'] = new bool[5, 3]
        {
            { true, true, true },
            { false, false, true },
            { true, true, true },
            { true, false, false },
            { true, true, true }
        };
        
        _digitMatrices['3'] = new bool[5, 3]
        {
            { true, true, true },
            { false, false, true },
            { true, true, true },
            { false, false, true },
            { true, true, true }
        };
        
        _digitMatrices['4'] = new bool[5, 3]
        {
            { true, false, true },
            { true, false, true },
            { true, true, true },
            { false, false, true },
            { false, false, true }
        };
        
        _digitMatrices['5'] = new bool[5, 3]
        {
            { true, true, true },
            { true, false, false },
            { true, true, true },
            { false, false, true },
            { true, true, true }
        };
        
        _digitMatrices['6'] = new bool[5, 3]
        {
            { true, true, true },
            { true, false, false },
            { true, true, true },
            { true, false, true },
            { true, true, true }
        };
        
        _digitMatrices['7'] = new bool[5, 3]
        {
            { true, true, true },
            { false, false, true },
            { false, false, true },
            { false, false, true },
            { false, false, true }
        };
        
        _digitMatrices['8'] = new bool[5, 3]
        {
            { true, true, true },
            { true, false, true },
            { true, true, true },
            { true, false, true },
            { true, true, true }
        };
        
        _digitMatrices['9'] = new bool[5, 3]
        {
            { true, true, true },
            { true, false, true },
            { true, true, true },
            { false, false, true },
            { true, true, true }
        };
        
        // 冒号的点阵定义
        _digitMatrices[':'] = new bool[5, 1]
        {
            { false },
            { true },
            { false },
            { true },
            { false }
        };
    }
    
    private void CreateParticles()
    {
        for (int i = 0; i < _particleCount; i++)
        {
            // 创建随机位置
            Point randomPosition = new Point(
                _random.Next(0, (int)ParticleCanvas.Width), 
                _random.Next(0, (int)ParticleCanvas.Height)
            );
            
            // 使用更小的粒子，增加清晰度
            Ellipse particle = new Ellipse
            {
                Width = 3,
                Height = 3,
                Fill = new SolidColorBrush(Color.FromArgb(100, 200, 200, 200)), // 默认为浅灰色
                Tag = randomPosition
            };
            
            // 设置粒子的初始位置
            Canvas.SetLeft(particle, randomPosition.X);
            Canvas.SetTop(particle, randomPosition.Y);
            
            _particles.Add(particle);
            ParticleCanvas.Children.Add(particle);
        }
    }
    
    private void Timer_Tick(object sender, EventArgs e)
    {
        UpdateClock();
    }
    
    private void UpdateClock()
    {
        _currentTime = DateTime.Now;
        string timeString = _currentTime.ToString("HH:mm:ss");
        
        // 更新可选的数字显示
        TimeDisplay.Text = timeString;
        
        // 计算粒子目标位置
        _currentTimePoints = CalculateTimePoints(timeString);
        
        // 为每个粒子分配目标位置
        AssignTargetsToParticles(_currentTimePoints);
        
        // 动画移动粒子
        AnimateParticles();
    }
    
    private List<Point> CalculateTimePoints(string timeString)
    {
        List<Point> points = new List<Point>();
        double digitWidth = 30;  // 减小数字宽度，使数字更紧凑
        double digitHeight = 50; // 减小数字高度
        double spacing = 10;     // 减小间距
        double startX = (ParticleCanvas.Width - (timeString.Length * (digitWidth + spacing) - spacing)) / 2;
        double startY = (ParticleCanvas.Height - digitHeight) / 2;
        
        double currentX = startX;
        
        foreach (char c in timeString)
        {
            if (_digitMatrices.ContainsKey(c))
            {
                bool[,] matrix = _digitMatrices[c];
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                
                // 增加点阵密度，使数字更清晰
                int densityFactor = 2; // 每个点生成2个粒子
                
                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        if (matrix[row, col])
                        {
                            // 为每个点生成多个粒子，增加密度
                            for (int d = 0; d < densityFactor; d++)
                            {
                                // 添加微小的随机偏移，使数字看起来更自然
                                double offsetX = _random.NextDouble() * 2 - 1;
                                double offsetY = _random.NextDouble() * 2 - 1;
                                
                                double x = currentX + col * (digitWidth / cols) + offsetX;
                                double y = startY + row * (digitHeight / rows) + offsetY;
                                points.Add(new Point(x, y));
                            }
                        }
                    }
                }
                
                currentX += digitWidth + spacing;
            }
        }
        
        return points;
    }
    
    private void AssignTargetsToParticles(List<Point> targetPoints)
    {
        // 不随机打乱目标点，确保时间显示的粒子能够准确显示
        
        // 为显示时间的粒子分配目标位置
        int timeParticlesCount = Math.Min(targetPoints.Count, _particles.Count);
        for (int i = 0; i < timeParticlesCount; i++)
        {
            _particles[i].Tag = targetPoints[i];
        }
        
        // 为剩余粒子分配随机位置
        for (int i = timeParticlesCount; i < _particles.Count; i++)
        {
            _particles[i].Tag = new Point(
                _random.Next(0, (int)ParticleCanvas.Width),
                _random.Next(0, (int)ParticleCanvas.Height)
            );
        }
    }
    
    private void AnimateParticles()
    {
        // 获取动态资源
        var primaryBrush = (SolidColorBrush)Application.Current.Resources["Text.Primary.Brush"];
        var primaryColor = primaryBrush.Color;
        
        // 计算需要显示时间的粒子数量
        int timeParticlesCount = Math.Min(_currentTimePoints.Count, _particles.Count);
        
        // 先处理显示时间的粒子
        for (int i = 0; i < timeParticlesCount; i++)
        {
            Ellipse particle = _particles[i];
            Point target = _currentTimePoints[i];
            
            // 时间显示的粒子使用主题颜色，并增大尺寸
            particle.Width = 4;
            particle.Height = 4;
            
            // 使用动态绑定的颜色
            particle.Fill = new SolidColorBrush(Color.FromArgb(255, primaryColor.R, primaryColor.G, primaryColor.B));
            
            // 创建动画
            DoubleAnimation animX = new DoubleAnimation
            {
                To = target.X,
                Duration = TimeSpan.FromSeconds(_transitionSpeed * 0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            
            DoubleAnimation animY = new DoubleAnimation
            {
                To = target.Y,
                Duration = TimeSpan.FromSeconds(_transitionSpeed * 0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            
            // 应用动画
            particle.BeginAnimation(Canvas.LeftProperty, animX);
            particle.BeginAnimation(Canvas.TopProperty, animY);
        }
        
        // 处理剩余的粒子
        for (int i = timeParticlesCount; i < _particles.Count; i++)
        {
            Ellipse particle = _particles[i];
            Point target = (Point)particle.Tag;
            
            // 非时间显示的粒子使用半透明的主题颜色，并减小尺寸
            particle.Width = 2;
            particle.Height = 2;
            
            // 使用半透明的动态绑定颜色
            particle.Fill = new SolidColorBrush(Color.FromArgb(50, primaryColor.R, primaryColor.G, primaryColor.B));
            
            // 创建动画
            DoubleAnimation animX = new DoubleAnimation
            {
                To = target.X,
                Duration = TimeSpan.FromSeconds(_transitionSpeed + _random.NextDouble() * 0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            
            DoubleAnimation animY = new DoubleAnimation
            {
                To = target.Y,
                Duration = TimeSpan.FromSeconds(_transitionSpeed + _random.NextDouble() * 0.5),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            
            // 应用动画
            particle.BeginAnimation(Canvas.LeftProperty, animX);
            particle.BeginAnimation(Canvas.TopProperty, animY);
        }
    }
}