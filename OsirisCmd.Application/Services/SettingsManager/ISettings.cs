using System.Collections.ObjectModel;
using Application.Core.Models;

namespace Application.Core.Services.SettingsManager;

public interface ISettings
{
    public ObservableCollection<SettingItem> Settings { get; }
    
}