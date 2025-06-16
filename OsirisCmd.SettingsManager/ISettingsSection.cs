using Avalonia.Controls;

public interface ISettingsSection {

    string SectionName { get; }

    UserControl SettingsTabContent { get; }

}
