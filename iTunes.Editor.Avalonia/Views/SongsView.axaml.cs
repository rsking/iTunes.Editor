namespace ITunes.Editor.Views
{
    using global::Avalonia.Markup.Xaml;
    using ReactiveUI;

    public class SongsView : global::Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.SongsRoutableViewModel>
    {
        public SongsView()
        {
            this.WhenActivated(disposables => { });
            this.InitializeComponent();
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
