using Core.Extensions;
using NAudio.Wave;
using Prism.Events;

namespace Core.Audio.ModelSpecificWaveProviders
{
  public abstract class ModelSpecificWaveProvider<TModel> : IWaveProvider
  {
    private readonly IEventAggregator eventAggregator;
    protected TModel Model;

    protected ModelSpecificWaveProvider(IEventAggregator eventAggregator)
    {
      this.eventAggregator = eventAggregator;
    }

    public abstract int Read(byte[] buffer, int offset, int count);
    public WaveFormat WaveFormat { get; protected set; }

    public virtual void SetModel(TModel model)
    {
      Model = model;
      UpdateFromModel(model);
      eventAggregator.OnModelUpdate(model, UpdateFromModel);
    }

    protected virtual void UpdateFromModel(TModel model)
    {
      // NOP
    }
  }
}