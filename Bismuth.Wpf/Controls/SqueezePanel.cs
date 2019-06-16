using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class SqueezePanel : Panel
    {
        protected override Size MeasureOverride(Size availableSize)
        {
            double totalChildrenWidth = 0;
            double maxHeight = 0;

            var list = new List<double>(InternalChildren.Count);
            foreach (UIElement child in InternalChildren)
            {
                child.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                totalChildrenWidth += child.DesiredSize.Width;

                list.Add(child.DesiredSize.Width);

                if (child.DesiredSize.Height > maxHeight)
                    maxHeight = child.DesiredSize.Height;
            }

            if (totalChildrenWidth > availableSize.Width)
            {
                double truncationThreshold = ComputeTruncationThreshold(list, availableSize.Width);

                foreach (UIElement child in InternalChildren)
                {
                    if (child.DesiredSize.Width > truncationThreshold)
                    {
                        child.Measure(new Size(truncationThreshold, availableSize.Height));
                    }
                }

                return new Size(availableSize.Width, maxHeight);
            }
            else
            {
                return new Size(totalChildrenWidth, maxHeight);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double totalChildrenWidth = 0;

            var list = new List<double>(InternalChildren.Count);
            foreach (UIElement child in InternalChildren)
            {
                totalChildrenWidth += child.DesiredSize.Width;
                list.Add(child.DesiredSize.Width);
            }

            double truncationThreshold = double.PositiveInfinity;
            if (totalChildrenWidth > finalSize.Width)
            {
                truncationThreshold = ComputeTruncationThreshold(list, finalSize.Width);
            }

            double position = 0;
            foreach (UIElement child in InternalChildren)
            {
                double width = child.DesiredSize.Width;
                if (child.DesiredSize.Width > truncationThreshold)
                    width = truncationThreshold;

                child.Arrange(new Rect(position, 0, width, finalSize.Height));
                position += width;
            }

            return finalSize;
        }

        private double ComputeTruncationThreshold(List<double> values, double availableSize)
        {
            values.Sort();
            double remainingSize = availableSize;
            double truncationThreshold = 0;
            for (int i = 0; i < values.Count; i++)
            {
                truncationThreshold = remainingSize / (values.Count - i);

                if (values[i] > truncationThreshold)
                    remainingSize -= truncationThreshold;
                else
                    remainingSize -= values[i];
            }

            return truncationThreshold;
        }
    }
}
