using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using NAudio.Wave;
using Prism.Commands;
using Prism.Mvvm;

namespace AmbientOTron.ViewModels.Tabs
{
    [Export]
    public class SoundEffectsViewModel : BindableBase
    {
        private readonly WaveMixerStream32 outStream;
        private readonly DelegateCommand<SoundEffectViewModel> deleteItemCommand;

        public ObservableCollection<BindableBase> Items { get; }

        [ImportingConstructor]
        public SoundEffectsViewModel(WaveMixerStream32 outStream)
        {
            this.outStream = outStream;
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
            var dialog = new OpenFileDialog
            {
                Title = "Add sound file",
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "MP3 sounds|*.mp3",
                FilterIndex = 0,
            };

            var typeReaders = new Func<string, WaveStream>[]
            {
                f => new WaveFormatConversionStream(new WaveFormat(), new Mp3FileReader(f)),
            };

            var result = dialog.ShowDialog();

            if (!result.HasValue || !result.Value)
                return;

            var readerFactory = typeReaders[dialog.FilterIndex - 1];

            try
            {
                using (readerFactory(dialog.FileName)) {}
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open file.\n{ex.Message}");
                return;
            }

            var file = new FileInfo(dialog.FileName);

            Items.Insert(Items.Count - 1, new SoundEffectViewModel
            {
                Name = file.Name,
                PlayCommand = new DelegateCommand(() => Play(readerFactory(dialog.FileName))),
                DeleteCommand = deleteItemCommand
            });
        }

        [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
        private void Play(WaveStream stream)
        {
            NotifyingChannel channel = null;

            channel = new NotifyingChannel(stream, () =>
            {

                if (channel == null)
                    return;

                outStream.RemoveInputStream(channel);
            });

            outStream.AddInputStream(channel);
            channel.Position = 0;
        }
    }

    public class NotifyingChannel : WaveChannel32
    {
        private readonly Action callback;
        private long offset = 0;

        public NotifyingChannel(WaveStream sourceStream, float volume, float pan, Action callback) : base(sourceStream, volume, pan)
        {
            this.callback = callback;
        }

        public NotifyingChannel(WaveStream sourceStream, Action callback) : base(sourceStream)
        {
            this.callback = callback;
        }

        public override int Read(byte[] destBuffer, int offset, int numBytes)
        {
            var result = base.Read(destBuffer, offset, numBytes);

            if (result < numBytes)
            {
                callback();
            }

            return result;
        }

        public override long Position
        {
            get { return base.Position; }
            set
            {
                var pos = value + offset;
                pos = Math.Max(pos, 0);
                pos = Math.Min(pos, Length - 1);

                base.Position = pos;
            }
        }
    }
}