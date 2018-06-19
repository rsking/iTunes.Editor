// -----------------------------------------------------------------------
// <copyright file="CommandConverter.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Converters
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Command converter.
    /// </summary>
    public class CommandConverter : Avalonia.Markup.IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // see if this is a func
            if (typeof(System.Windows.Input.ICommand).IsAssignableFrom(targetType) && value is Delegate d && d.Method.GetParameters().Length <= 1)
            {
                var execute = d;

                var method = d.Method;
                var target = d.Target;

                Delegate canExecute = default;
                System.ComponentModel.INotifyPropertyChanged inpc = default;
                string propertyName = default;
                System.Reflection.MethodInfo canExecuteMethodInfo;
                var canExecutePropertyInfo = target.GetType().GetProperty("Can" + method.Name);
                if (canExecutePropertyInfo?.PropertyType == typeof(bool))
                {
                    canExecuteMethodInfo = canExecutePropertyInfo.GetGetMethod();
                    inpc = target as System.ComponentModel.INotifyPropertyChanged;
                    propertyName = canExecutePropertyInfo.Name;
                }
                else
                {
                    canExecuteMethodInfo = target.GetType().GetMethod("Can" + method.Name);
                }

                if (canExecuteMethodInfo?.ReturnType == typeof(bool) && canExecuteMethodInfo.GetParameters().Length <= 1)
                {
                    var parameterInfo = canExecuteMethodInfo.GetParameters().FirstOrDefault();
                    canExecute = parameterInfo == null
                        ? Delegate.CreateDelegate(typeof(Func<bool>), target, canExecuteMethodInfo)
                        : Delegate.CreateDelegate(typeof(Func<,>).MakeGenericType(parameterInfo.ParameterType, typeof(bool)), target, canExecuteMethodInfo);
                }

                return new DelegateCommand(execute, canExecute, inpc, propertyName);
            }

            return new Avalonia.Data.BindingNotification(new InvalidCastException($"Could not convert '{value}' to '{targetType.Name}'."), Avalonia.Data.BindingErrorType.Error);
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => this.Convert(value, targetType, parameter, culture);

        private class DelegateCommand : System.Windows.Input.ICommand
        {
            private readonly Delegate execute;

            private readonly Delegate canExecute;

            private readonly System.Reflection.ParameterInfo executeParameterInfo;

            private readonly System.Reflection.ParameterInfo canExecuteParameterInfo;

            public DelegateCommand(Delegate execute, Delegate canExecute, System.ComponentModel.INotifyPropertyChanged inpc, string propertyName)
            {
                this.execute = execute;
                this.executeParameterInfo = this.execute.Method.GetParameters().FirstOrDefault();

                this.canExecute = canExecute;
                this.canExecuteParameterInfo = this.canExecute?.Method.GetParameters().FirstOrDefault();

                if (inpc != null)
                {
                    inpc.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == propertyName)
                        {
                            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                        }
                    };
                }
            }

            public event EventHandler CanExecuteChanged;

            public bool CanExecute(object parameter)
            {
                if (this.canExecute == null)
                {
                    return true;
                }

                if (this.canExecuteParameterInfo == null)
                {
                    return (bool)this.canExecute.DynamicInvoke();
                }

                Avalonia.Utilities.TypeUtilities.TryConvert(this.canExecuteParameterInfo.ParameterType, parameter, CultureInfo.CurrentCulture, out object convertedParameter);
                return (bool)this.canExecute.DynamicInvoke(convertedParameter);
            }

            public async void Execute(object parameter)
            {
                object returnValue;

                if (this.executeParameterInfo == null)
                {
                    returnValue = this.execute.DynamicInvoke();
                }
                else
                {
                    Avalonia.Utilities.TypeUtilities.TryConvert(this.executeParameterInfo.ParameterType, parameter, CultureInfo.CurrentCulture, out object convertedParameter);
                    returnValue = this.execute.DynamicInvoke(convertedParameter);
                }

                // see if it is awaitable
                if (returnValue is System.Threading.Tasks.Task task)
                {
                    await task;
                }
            }
        }
    }
}
