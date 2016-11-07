using System.Windows;
using AmbientOTron.Views.Editors.LibraryEditor;
using Core.Audio;
using Core.Dialogs;
using Core.Events;
using Core.Navigation;
using Core.Repository;
using Core.Repository.Models;
using FluentAssertions;
using GongSolutions.Wpf.DragDrop;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prism.Events;
using Prism.Regions;

namespace ClientTests.Views.Editors.LibraryEditor
{
  [TestClass]
  public class DetailViewModelTests
  {
    [TestMethod]
    public void AddingDuplicateFilesIsIgnored()
    {
      var navigationServiceMock = new Mock<INavigationService>();
      var repositoryMock = new Mock<IRepository>();
      var eventAggregator = new Mock<IEventAggregator>();
      var dialogServiceMock = new Mock<IDialogService>();
      var audioServiceMock = new Mock<IAudioService>();

      var mockedUpdateAudioFileModelEvent = new Mock<UpdateModelEvent<AudioFile>>();

      eventAggregator.Setup(x => x.GetEvent<UpdateModelEvent<AudioFile>>()).Returns(mockedUpdateAudioFileModelEvent.Object);

      var sut = new DetailViewModel(navigationServiceMock.Object, repositoryMock.Object, eventAggregator.Object ,dialogServiceMock.Object , audioServiceMock.Object);
      sut.OnNavigatedTo(new NavigationContext(new Mock<IRegionNavigationService>().Object, null));

      repositoryMock.Setup(x => x.GetAudioFileModel(It.IsAny<string>())).Returns<string>(
                      s => new AudioFile
                      {
                        FullPath = s
                      });

      var dropInfoMock = new Mock<IDropInfo>();
      dropInfoMock.SetupGet(x => x.Data).Returns(new DataObject(DataFormats.FileDrop, new[] {"Test", "Test", "Test2"}));

      sut.Drop(dropInfoMock.Object);

      sut.Files.Should().HaveCount(2);
    }
  }
}