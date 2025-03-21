namespace WPFAdmin.Test;

public class CircleIntersection {
    public class Circle
    {
        public decimal X { get; set; }  // 改用decimal类型提高精度
        public decimal Y { get; set; }
        public decimal R { get; set; }

        public Circle(decimal x, decimal y, decimal r)
        {
            X = x;
            Y = y;
            R = r;
        }

        public static (decimal? x1, decimal? y1, decimal? x2, decimal? y2) GetIntersectionPoints(Circle c1, Circle c2)
        {
            // 使用更高精度的计算
            const decimal EPSILON = 0.0000000001m; // 定义误差容限

            // 计算两个圆心之间的距离
            decimal d = (decimal)Math.Sqrt((double)(
                (c2.X - c1.X) * (c2.X - c1.X) + 
                (c2.Y - c1.Y) * (c2.Y - c1.Y)
            ));

            // 检查圆是否重合或不相交，加入误差容限
            if (d > c1.R + c2.R + EPSILON || 
                d < Math.Abs(c1.R - c2.R) - EPSILON || 
                d < EPSILON)
            {
                return (null, null, null, null);
            }

            // 使用余弦定理计算交点
            decimal a = (c1.R * c1.R - c2.R * c2.R + d * d) / (2 * d);
            decimal h = (decimal)Math.Sqrt((double)(c1.R * c1.R - a * a));

            // 计算交点坐标
            decimal x2 = c1.X + a * (c2.X - c1.X) / d;
            decimal y2 = c1.Y + a * (c2.Y - c1.Y) / d;

            decimal x3 = x2 + h * (c2.Y - c1.Y) / d;
            decimal y3 = y2 - h * (c2.X - c1.X) / d;
            decimal x4 = x2 - h * (c2.Y - c1.Y) / d;
            decimal y4 = y2 + h * (c2.X - c1.X) / d;

            return (x3, y3, x4, y4);
        }

        // 添加验证方法
        public bool IsPointOnCircle(decimal x, decimal y)
        {
            const decimal EPSILON = 0.0000000001m;
            decimal distance = (decimal)Math.Sqrt((double)(
                (x - X) * (x - X) + 
                (y - Y) * (y - Y)
            ));
            return Math.Abs(distance - R) < EPSILON;
        }
    }
    [Fact]
    public  void Example()
    {
        // 创建两个圆
        Circle circle1 = new Circle(0, 0, 5);  
        Circle circle2 = new Circle(3, 4, 6);  

        var (x1, y1, x2, y2) = Circle.GetIntersectionPoints(circle1, circle2);

        if (x1.HasValue)
        {
            Console.WriteLine($"交点1: ({x1:F2}, {y1:F2})");
            Console.WriteLine($"交点2: ({x2:F2}, {y2:F2})");
        }
        else
        {
            Console.WriteLine("这两个圆没有交点");
        }
    }
    
    
  

    
}