using System;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Prism.Commands;

namespace AmbientOTron.ViewModels
{
    [Export]
    public class SoundEffectViewModel : Prism.Mvvm.BindableBase
    {
        private string name;
        private DisplayMode mode;

        public enum DisplayMode
        {
            StandardMode,
            SettingsMode
        }

        public SoundEffectViewModel()
        {
            EnterSettingsModeCommand = new DelegateCommand(() => Mode = DisplayMode.SettingsMode);
            CancelCommand = new DelegateCommand(CancelOperation);
            Mode = DisplayMode.StandardMode;
        }


        public DisplayMode Mode
        {
            get { return mode; }
            set { SetProperty(ref mode, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public ICommand CancelCommand { get; }

        public ICommand EnterSettingsModeCommand { get; }


        public ICommand DeleteCommand { get; set; }
        public ICommand PlayCommand { get; set; }

        private void CancelOperation()
        {
            switch (Mode)
            {
                case DisplayMode.SettingsMode:
                    Mode = DisplayMode.StandardMode;
                    break;
            }
        }

    }
}