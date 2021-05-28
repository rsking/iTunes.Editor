// <copyright file="ProgressRing.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Controls
{
    using System;
    using global::Avalonia;

    /// <summary>
    /// Progress ring.
    /// </summary>
    public class ProgressRing : global::Avalonia.Controls.Primitives.TemplatedControl
    {
        /// <summary>
        /// The <see cref="IsActive"/> <see cref="AvaloniaProperty"/>.
        /// </summary>
        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<ProgressRing, bool>(nameof(IsActive), defaultValue: true, notifying: OnIsActiveChanged);

        /// <summary>
        /// The <see cref="MaxSideLength"/> <see cref="AvaloniaProperty"/>.
        /// </summary>
        public static readonly DirectProperty<ProgressRing, double> MaxSideLengthProperty =
            AvaloniaProperty.RegisterDirect<ProgressRing, double>(
               nameof(MaxSideLength),
               o => o.MaxSideLength);

        /// <summary>
        /// The <see cref="EllipseDiameter"/> <see cref="AvaloniaProperty"/>.
        /// </summary>
        public static readonly DirectProperty<ProgressRing, double> EllipseDiameterProperty =
            AvaloniaProperty.RegisterDirect<ProgressRing, double>(
               nameof(EllipseDiameter),
               o => o.EllipseDiameter);

        /// <summary>
        /// The <see cref="EllipseOffset"/> <see cref="AvaloniaProperty"/>.
        /// </summary>
        public static readonly DirectProperty<ProgressRing, Thickness> EllipseOffsetProperty =
            AvaloniaProperty.RegisterDirect<ProgressRing, Thickness>(
               nameof(EllipseOffset),
               o => o.EllipseOffset);

        private const string LargeState = ":large";
        private const string SmallState = ":small";

        private const string InactiveState = ":inactive";
        private const string ActiveState = ":active";

        private double maxSideLength = 10;
        private double ellipseDiameter = 10;
        private Thickness ellipseOffset = new(2);

        /// <summary>
        /// Gets or sets a value indicating whether this is active.
        /// </summary>
        public bool IsActive
        {
            get => this.GetValue(IsActiveProperty);
            set => this.SetValue(IsActiveProperty, value);
        }

        /// <summary>
        /// Gets the maximum side length.
        /// </summary>
        public double MaxSideLength
        {
            get => this.maxSideLength;
            private set => this.SetAndRaise(MaxSideLengthProperty, ref this.maxSideLength, value);
        }

        /// <summary>
        /// Gets the ellipse diameter.
        /// </summary>
        public double EllipseDiameter
        {
            get => this.ellipseDiameter;
            private set => this.SetAndRaise(EllipseDiameterProperty, ref this.ellipseDiameter, value);
        }

        /// <summary>
        /// Gets the ellipse offset.
        /// </summary>
        public Thickness EllipseOffset
        {
            get => this.ellipseOffset;
            private set => this.SetAndRaise(EllipseOffsetProperty, ref this.ellipseOffset, value);
        }

        /// <inheritdoc/>
        public override void Render(global::Avalonia.Media.DrawingContext context)
        {
            base.Render(context);
            this.UpdateVisualStates();
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate(global::Avalonia.Controls.Primitives.TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            double actualMaxSideLength = Math.Min(this.Width, this.Height);
            double actialEllipseDiameter = 0.1 * actualMaxSideLength;
            if (actualMaxSideLength <= 40)
            {
                actialEllipseDiameter++;
            }

            this.EllipseDiameter = actialEllipseDiameter;
            this.MaxSideLength = actualMaxSideLength;
            this.EllipseOffset = new Thickness(0, (actualMaxSideLength * 0.5) - actialEllipseDiameter, 0, 0);
            this.UpdateVisualStates();
        }

        private static void OnIsActiveChanged(IAvaloniaObject obj, bool arg2)
        {
            if (obj is ProgressRing progressRing)
            {
                progressRing.UpdateVisualStates();
            }
        }

        private void UpdateVisualStates()
        {
            this.PseudoClasses.Remove(ActiveState);
            this.PseudoClasses.Remove(InactiveState);
            this.PseudoClasses.Remove(SmallState);
            this.PseudoClasses.Remove(LargeState);
            this.PseudoClasses.Add(this.IsActive ? ActiveState : InactiveState);
            this.PseudoClasses.Add(this.maxSideLength < 60 ? SmallState : LargeState);
        }
    }
}
