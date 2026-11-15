using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace workout_app.PageModels;

public partial class MedicationPageModel(
    DatabaseService databaseService,
    MedicationReminderService reminderService,
    ILogger<MedicationPageModel> logger) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasMedications))]
    [NotifyPropertyChangedFor(nameof(ShowEmptyHint))]
    [NotifyPropertyChangedFor(nameof(ShowPlan))]
    private ObservableCollection<MedicationData> medications = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditButtonText))]
    [NotifyPropertyChangedFor(nameof(ShowEmptyHint))]
    [NotifyPropertyChangedFor(nameof(ShowPlan))]
    private bool isEditing;

    public bool HasMedications => Medications.Count > 0;
    public bool ShowPlan => HasMedications || IsEditing;
    public bool ShowEmptyHint => !ShowPlan;
    public string EditButtonText => IsEditing ? AppResources.button_done : AppResources.button_edit;

    public async Task LoadAsync()
    {
        try
        {
            var plan = await databaseService.GetMedicationsAsync();
            Medications = new ObservableCollection<MedicationData>(plan);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Loading medication plan failed");
            Medications = [];
        }
    }

    [RelayCommand]
    private async Task ToggleEdit()
    {
        if (IsEditing)
            await SaveAsync();

        IsEditing = !IsEditing;
    }

    [RelayCommand]
    private async Task AddMedication()
    {
        var entry = new MedicationData { Timestamp = DateTime.Now };

        try
        {
            await databaseService.AddMedicationAsync(entry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Adding medication failed");
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_save_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        Medications.Add(entry);
        NotifyListChanged();
    }

    [RelayCommand]
    private async Task DeleteMedication(MedicationData entry)
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            AppResources.displayalert_want_to_delete, entry.Name,
            AppResources.button_yes, AppResources.button_no);

        if (!confirm) return;

        try
        {
            await databaseService.DeleteAsync(entry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Deleting medication failed");
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_delete_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        Medications.Remove(entry);
        NotifyListChanged();
        await reminderService.RescheduleAsync();
    }

    // Works in view and edit mode and is saved right away, since it's a quick everyday action
    [RelayCommand]
    private async Task ToggleActive(MedicationData entry)
    {
        entry.IsInactive = !entry.IsInactive;

        try
        {
            await databaseService.UpdateAsync(entry);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Toggling medication failed");
            entry.IsInactive = !entry.IsInactive;
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_save_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        await reminderService.RescheduleAsync();
    }

    // Also called when leaving the page, so edits in the entry fields are not lost without pressing "Done".
    public async Task SaveAsync()
    {
        try
        {
            await databaseService.UpdateMedicationsAsync(Medications);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Saving medication plan failed");
            await Shell.Current.DisplayAlertAsync(AppResources.displayalert_save_failed, AppResources.displayalert_error_message, AppResources.button_ok);
            return;
        }

        await reminderService.RescheduleAsync();
    }

    private void NotifyListChanged()
    {
        OnPropertyChanged(nameof(HasMedications));
        OnPropertyChanged(nameof(ShowPlan));
        OnPropertyChanged(nameof(ShowEmptyHint));
    }
}
