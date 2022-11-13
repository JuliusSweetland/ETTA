// Copyright (c) 2022 OPTIKEY LTD (UK company number 11854839) - All Rights Reserved
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Properties;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JuliusSweetland.OptiKey.UI.Controls
{
    public class ProgressIndicator : Canvas
    {
        public ProgressIndicator()
        {
            ClipToBounds = true;
            SizeChanged += (sender, args) => Render();
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(ProgressIndicator),
                new PropertyMetadata(default(double), PropertyChangedCallback));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(ProgressIndicator),
                new PropertyMetadata(100.0, PropertyChangedCallback));

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(ProgressIndicator),
                new PropertyMetadata(default(Brush), PropertyChangedCallback));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        private static void PropertyChangedCallback(
            DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var progressPie = dependencyObject as ProgressIndicator;
            if (progressPie != null)
            {
                progressPie.Render();
            }
        }

        private void Render()
        {
            Children.Clear();
            var centreX = ActualWidth / 2;
            var centreY = ActualHeight / 2;
            var radius = Math.Sqrt(centreX * centreX + centreY * centreY);

            switch (Settings.Default.ProgressIndicatorBehaviour)
            {
                case ProgressIndicatorBehaviours.Fade:
                    Children.Add(CreateRectangle(0, 0, ActualWidth, ActualHeight));
                    Opacity = Value;
                    break;
                case ProgressIndicatorBehaviours.FillIn:
                    Children.Add(CreateEllipse(centreX, centreY, radius, 2 * Value * radius));
                    break;
                case ProgressIndicatorBehaviours.FillOut:
                    Children.Add(CreateEllipse(centreX, centreY, 1, 2 * Value * radius));
                    break;
                case ProgressIndicatorBehaviours.FillRight:
                    var width = ActualWidth * Value;
                    Children.Add(CreateRectangle(0, 0, width, ActualHeight));
                    break;
                case ProgressIndicatorBehaviours.FillUp:
                    var height = ActualHeight * Value;
                    Children.Add(CreateRectangle(0, ActualHeight - height, ActualWidth, height));
                    break;
                default:
                    FillPie(centreX, centreY);
                    break;
            }
        }

        private Path CreateRectangle(double x, double y, double width, double height)
        {
            return new Path
            {
                Fill = Fill,
                Data = new GeometryGroup
                {
                    Children = new GeometryCollection
                    {
                        new RectangleGeometry
                        {
                            Rect = new Rect(x, y, width, height)
                        }
                    }
                }
            };
        }

        private Path CreateEllipse(double x, double y, double radius, double thickness)
        {
            return new Path
            {
                Stroke = Fill,
                StrokeThickness = thickness,
                Data = new GeometryGroup
                {
                    Children = new GeometryCollection
                    {
                        new EllipseGeometry
                        {
                            Center = new Point(x, y),
                            RadiusX = radius,
                            RadiusY = radius
                        }
                    }
                }
            };
        }

        private void FillPie(double centreX, double centreY)
        {
            var sizeFactor = 1d;

            if (Settings.Default.ProgressIndicatorBehaviour == ProgressIndicatorBehaviours.Shrink
                && Settings.Default.ProgressIndicatorResizeStartProportion > Settings.Default.ProgressIndicatorResizeEndProportion)
            {
                var range = (Settings.Default.ProgressIndicatorResizeStartProportion - Settings.Default.ProgressIndicatorResizeEndProportion) / 100d;
                var reducingValue = MaxValue - Value;
                sizeFactor = (reducingValue * range) + (Settings.Default.ProgressIndicatorResizeEndProportion / 100d);
            }
            else if (Settings.Default.ProgressIndicatorBehaviour == ProgressIndicatorBehaviours.Grow
                && Settings.Default.ProgressIndicatorResizeStartProportion < Settings.Default.ProgressIndicatorResizeEndProportion)
            {
                var range = (Settings.Default.ProgressIndicatorResizeEndProportion - Settings.Default.ProgressIndicatorResizeStartProportion) / 100d;
                sizeFactor = (Value * range) + (Settings.Default.ProgressIndicatorResizeStartProportion / 100d);
            }

            var angle = Settings.Default.ProgressIndicatorBehaviour == ProgressIndicatorBehaviours.FillPie ? (Value / MaxValue) * 360 : 360;
            var radius = Math.Min(centreX, centreY) * sizeFactor;

            Path piePath;

            if (angle >= 360)
            {
                piePath = CreateEllipse(centreX, centreY, 1, 2 * radius);
            }
            else
            {
                var innerArcStartPoint = OffsetEx(ComputeCartesianCoordinate(0, 0), centreX, centreY);
                var innerArcEndPoint = OffsetEx(ComputeCartesianCoordinate(angle, 0), centreX, centreY);
                var outerArcStartPoint = OffsetEx(ComputeCartesianCoordinate(0, radius), centreX, centreY);
                var outerArcEndPoint = OffsetEx(ComputeCartesianCoordinate(angle, radius), centreX, centreY);

                bool largeArc = angle > 180.0;
                var outerArcSize = new Size(radius, radius);
                var innerArcSize = new Size(0, 0);

                piePath = new Path
                {
                    Fill = this.Fill,
                    //Stroke = this.Stroke,
                    //StrokeThickness = 1,
                    Data = new PathGeometry
                    {
                        Figures = new PathFigureCollection
                        {
                            new PathFigure
                            {
                                StartPoint = innerArcStartPoint,
                                Segments = new PathSegmentCollection
                                {
                                    new LineSegment
                                    {
                                        Point = outerArcStartPoint
                                    },
                                    new ArcSegment
                                    {
                                        Point = outerArcEndPoint,
                                        Size = outerArcSize,
                                        IsLargeArc = largeArc,
                                        SweepDirection = SweepDirection.Clockwise,
                                        RotationAngle = 0
                                    },
                                    new LineSegment
                                    {
                                        Point = innerArcEndPoint
                                    },
                                    new ArcSegment
                                    {
                                        Point = innerArcStartPoint,
                                        Size = innerArcSize,
                                        IsLargeArc = largeArc,
                                        SweepDirection = SweepDirection.Counterclockwise,
                                        RotationAngle = 0
                                    }
                                }
                            }
                        }
                    }
                };
            }

            Children.Add(piePath);
        }

        /// <summary>
        /// Converts a coordinate from the polar coordinate system to the cartesian coordinate system.
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private static Point ComputeCartesianCoordinate(double angle, double radius)
        {
            // convert to radians
            double angleRad = (Math.PI / 180.0) * (angle - 90);

            double x = radius * Math.Cos(angleRad);
            double y = radius * Math.Sin(angleRad);

            return new Point(x, y);
        }

        private static Point OffsetEx(Point point, double x, double y)
        {
            return new Point(point.X + x, point.Y + y);
        }
    }
}
