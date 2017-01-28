using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AmbientOTron.Views.Dialogs.MessageBox;
using Core.Dialogs;
using Core.Repository;
using Core.Repository.Models;
using Core.WPF;
using GongSolutions.Wpf.DragDrop;
using Prism.Mvvm;
using Prism.Regions;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace AmbientOTron.Views.Gaming.SoundBoard
{
  [Export]
  public class SoundBoardViewModel : BindableBase, IConfirmNavigationRequest, IDropTarget
  {
    private readonly IRepository repository;
    private readonly IDialogService dialogService;
    private readonly ExportFactory<AudioFileViewModel> audioFileViewModelFactory;
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly DragDropHelper dragDropHelper;

    [ImportingConstructor]
    public SoundBoardViewModel(IRepository repository, IDialogService dialogService, ExportFactory<AudioFileViewModel> audioFileViewModelFactory)
    {
      this.repository = repository;
      this.dialogService = dialogService;
      this.audioFileViewModelFactory = audioFileViewModelFactory;
      this.Files = new ObservableCollection<AudioFileViewModel>();

      dragDropHelper = new DragDropHelper
      {
        {
          dropInfo =>
          {
            var dataObject = dropInfo.Data as DataObject;

            return (dataObject?.GetDataPresent(DataFormats.FileDrop) == true);

          },
          dropInfo =>
          {
            var dataObject = dropInfo.Data as DataObject;

            var files = (string[])dataObject.GetData(DataFormats.FileDrop);
            if (files == null) return;

            foreach (var file in files)
            {
              if (Files.Any(x => x.Model.FullPath == file))
                continue;

              Files.Add(CreateFileViewModel(this.repository.GetAudioFileModel(file)));
            }
          }
        },
        {
          dropInfo => dropInfo.Data is AudioFileViewModel,
          dropInfo =>
          {
            Files.Move(Files.IndexOf(dropInfo.Data as AudioFileViewModel),dropInfo.InsertIndex);
          },
          _ => DragDropEffects.Move
        }
      };
    }

    public bool IsDirty { get; set; }

    private ObservableCollection<AudioFileViewModel> files;
    private Core.Repository.Models.SoundBoard model;

    public ObservableCollection<AudioFileViewModel> Files
    {
      get { return files; }
      set { SetProperty(ref files, value); }
    }

    private void LoadFromModel()
    {
      Name = model.Name;
      Files.Clear();
      Files.AddRange(model.Sounds.Select(CreateFileViewModel));
      IsDirty = false;
    }

    private string name;

    public string Name
    {
      get { return name; }
      set { SetProperty(ref name, value); }
    }

    private AudioFileViewModel CreateFileViewModel(AudioFile model)
    {
      var export = audioFileViewModelFactory.CreateExport();
      disposables.Add(export);

      var result = export.Value;
      result.SetModel(model);
      return result;
    }

    #region Implementation of INavigationAware

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
      var id = navigationContext.Parameters?["id"] as string;
      this.model = repository.LoadSoundBoard(id) ?? new Core.Repository.Models.SoundBoard();

      LoadFromModel();
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
      return true;
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
    }

    public async void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
    {
      if (IsDirty)
      {
        var result =
          await
          dialogService.ShowMessageBox(
            "Do you want to leave this page and loose your changes?",
            MessageBoxButtons.YesNo,
            DialogResult.No);

        continuationCallback(result == DialogResult.Yes);
      }
      else
      {
        continuationCallback(true);
      }
    }

    #endregion

    #region Implementation of IDropTarget

    public void DragOver(IDropInfo dropInfo)
    {
      dragDropHelper.DragOver(dropInfo);
    }

    public void Drop(IDropInfo dropInfo)
    {
      dragDropHelper.Drop(dropInfo);
    }

    #endregion
  }
}
