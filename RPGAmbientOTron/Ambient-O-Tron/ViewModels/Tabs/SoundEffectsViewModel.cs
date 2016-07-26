using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Prism.Commands;
using Prism.Mvvm;

namespace AmbientOTron.ViewModels.Tabs
{
    [Export]
    public class SoundEffectsViewModel : BindableBase
    {
        private DelegateCommand<SoundEffectViewModel> deleteItemCommand;

        public ObservableCollection<BindableBase> Items { get; } 

        public SoundEffectsViewModel()
        {
            Items = new ObservableCollection<BindableBase>
            {
                new AddItemViewModel
                {
                    AddCommand = new DelegateCommand(AddSoundEffect)
                }
            };

            deleteItemCommand = new DelegateCommand<SoundEffectViewModel>(DeleteSoundEffect);
        }

        private void DeleteSoundEffect(SoundEffectViewModel item)
        {
            Items.Remove(item);
        }

        private void AddSoundEffect()
        {
            Items.Insert(Items.Count - 1, new SoundEffectViewModel
            {
                Name = "Blah!",
                DeleteCommand = deleteItemCommand
            });
        }
    }
}