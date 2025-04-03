using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace PictureModules.Components;

public partial class PicControl : UserControl
{
    private List<Border> _cards = new List<Border>();

    public PicControl()
    {
        InitializeComponent();
    }

    private void PicControl_Loaded(object sender, RoutedEventArgs e)
    {
        // 控件加载完成后直接创建3D效果
        if (_cards.Count > 0)
        {
            Create3DCarousel();
        }
    }

    public void LoadImages(List<string> imagePaths)
    {
        // 清空现有图片
        _cards.Clear();

        // 创建图片但不添加到UI
        foreach (var path in imagePaths)
        {
            try
            {
                // 创建图片
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(path)),
                    Stretch = Stretch.UniformToFill
                };

                // 创建边框
                var border = new Border
                {
                    Width = 350,  
                    Height = 250, 
                    BorderThickness = new Thickness(2),
                    BorderBrush = Brushes.White,
                    CornerRadius = new CornerRadius(15), 
                    Child = image
                };

                // 添加阴影效果
                border.Effect = new DropShadowEffect
                {
                    ShadowDepth = 0,
                    Color = Colors.White,
                    Opacity = 0.5,
                    BlurRadius = 15
                };

                // 只添加到列表，不添加到UI
                _cards.Add(border);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图片失败: {ex.Message}");
            }
        }

        // 如果控件已加载，创建3D效果
        if (IsLoaded)
        {
            Create3DCarousel();
        }
    }

    private void Create3DCarousel()
    {

        ScaleTransform scaleTransform = new ScaleTransform
        {
            ScaleY = 0.5,  // Y轴缩放比例，值越小"倾斜"越明显
            CenterY = 300  // 设置变换中心点，使其看起来更自然
        };

        CardCanvas.RenderTransform = scaleTransform;




        // 清空Canvas
        CardCanvas.Children.Clear();

        // 确保Canvas有足够的尺寸
        if (CardCanvas.ActualWidth < 10 || CardCanvas.ActualHeight < 10)
        {
            CardCanvas.Width = ActualWidth;
            CardCanvas.Height = ActualHeight;
        }

        // 获取Canvas的中心点
        double centerX = CardCanvas.ActualWidth / 2;
        double centerY = CardCanvas.ActualHeight / 2;

        // 计算角度步长 - 如果是6张图片，则为60度
        int totalCards = _cards.Count;
        double angleStep = 360.0 / totalCards;
        double radius = 200; // 圆环半径

        // 创建一个旋转的容器 
        Canvas carousel = new Canvas
        {
            Width = radius * 2, // 设置容器宽度为直径
            Height = radius * 2 // 设置容器高度为直径
        };

        // 设置容器的位置为中心，减去半径，使容器的中心与Canvas的中心对齐
        Canvas.SetLeft(carousel, centerX - radius);
        Canvas.SetTop(carousel, centerY - radius);

        // 添加容器到Canvas
        CardCanvas.Children.Add(carousel);

        // 创建旋转动画 
        RotateTransform carouselRotate = new RotateTransform();

        // 设置旋转中心点为容器的中心
        carouselRotate.CenterX = radius;
        carouselRotate.CenterY = radius;

        carousel.RenderTransform = carouselRotate;

        DoubleAnimation rotationAnimation = new DoubleAnimation
        {
            From = 0,
            To = 360,
            Duration = TimeSpan.FromSeconds(20),
            RepeatBehavior = RepeatBehavior.Forever
        };

        // 启动动画
        carouselRotate.BeginAnimation(RotateTransform.AngleProperty, rotationAnimation);

        // 为每张卡片设置3D变换
        for (int i = 0; i < totalCards; i++)
        {
            var originalCard = _cards[i];




            // 创建一个新的边框
            Border newCard = new Border
            {
                Width = 120,
                Height = 180,
                BorderThickness = originalCard.BorderThickness,
                BorderBrush = originalCard.BorderBrush,
                CornerRadius = originalCard.CornerRadius,

                Background = new ImageBrush(((Image)originalCard.Child).Source)
            };

            // 复制阴影效果
            if (originalCard.Effect is DropShadowEffect originalEffect)
            {
                newCard.Effect = new DropShadowEffect
                {
                    ShadowDepth = originalEffect.ShadowDepth,
                    Color = originalEffect.Color,
                    Opacity = originalEffect.Opacity,
                    BlurRadius = originalEffect.BlurRadius
                };
            }

            // 计算角度
            double angle = i * angleStep;
            double radians = angle * Math.PI / 180.0;

            // 计算卡片在圆上的位置
            double x = radius + radius * Math.Cos(radians) - newCard.Width / 2;
            double y = radius + radius * Math.Sin(radians) - newCard.Height / 2;

            // 设置卡片位置
            Canvas.SetLeft(newCard, x);
            Canvas.SetTop(newCard, y);

            // 设置Z顺序
            Canvas.SetZIndex(newCard, 1000 - i);

            // 创建反向旋转动画，使图片保持正向
            RotateTransform cardRotate = new RotateTransform();
            newCard.RenderTransform = cardRotate;
            newCard.RenderTransformOrigin = new Point(0.5, 0.5); // 设置旋转中心为图片中心

            DoubleAnimation cardRotationAnimation = new DoubleAnimation
            {
                From = 0,
                To = -360, // 反向旋转
                Duration = TimeSpan.FromSeconds(20), // 与容器旋转动画持续时间相同
                RepeatBehavior = RepeatBehavior.Forever
            };

            // 启动图片反向旋转动画
            cardRotate.BeginAnimation(RotateTransform.AngleProperty, cardRotationAnimation);

            // 添加到旋转容器
            carousel.Children.Add(newCard);
        }
    }
}