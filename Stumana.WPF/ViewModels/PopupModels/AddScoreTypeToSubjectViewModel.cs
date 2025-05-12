using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Stumana.DataAccess.Services;
using Stumana.DataAcess.Models;
using Stumana.WPF.Commands;
using Stumana.WPF.Stores;

namespace Stumana.WPF.ViewModels.PopupModels;

public class AddScoreTypeToSubjectViewModel : BaseViewModel
{
    public ObservableCollection<ScoreTypeTableRow> SubjectScoreTypeTable { get; set; } = new();
    private ScoreTypeTableRow? _selectedSubjectScoreType;

    public ScoreTypeTableRow? SelectedSubjectScoreType
    {
        get => _selectedSubjectScoreType;
        set
        {
            _selectedSubjectScoreType = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddSubjectScoreTypeCommand { get; set; }
    public ICommand DeleteSubjectScoreTypeCommand { get; set; }
    public ICommand CancelCommand { get; set; }

    private readonly Subject CurSubject;
    public EventHandler? OnAddedData { get; set; }
    private List<ScoreType> ScoreTypes { get; set; } = new List<ScoreType>();
    private List<SubjectScoreType> SubjectScoreTypes { get; set; } = new List<SubjectScoreType>();

    public AddScoreTypeToSubjectViewModel(Subject subject)
    {
        CurSubject = subject;
        OnAddedData += UpdateTable;

        AddSubjectScoreTypeCommand = new NavigateModalCommand(() => new AddSubjectScoreTypeViewModel(subject, OnAddedData));
        DeleteSubjectScoreTypeCommand = new RelayCommand(DeleteSubjectScoreType);
        CancelCommand = new RelayCommand(ModalNavigationStore.Instance.Close);

        LoadSubjectScoreTypeTable();
    }

    private void UpdateTable(object? sender, EventArgs e)
    {
        LoadSubjectScoreTypeTable();
    }

    private async void LoadSubjectScoreTypeTable()
    {
        ScoreTypes = (await GenericDataService<ScoreType>.Instance.GetManyAsync(st => st.YearId == CurSubject.YearId)).ToList();
        SubjectScoreTypes = (await GenericDataService<SubjectScoreType>.Instance.GetManyAsync(sst => sst.SubjectId == CurSubject.Id,
                                                                                              query => query.Include(sst => sst.ScoreType))).ToList();

        SubjectScoreTypeTable.Clear();
        foreach (var subjectScoreType in SubjectScoreTypes)
        {
            ScoreTypeTableRow newRow = new ScoreTypeTableRow
            {
                SubjectScoreTypeId = subjectScoreType.Id,
                ScoreTypeId = subjectScoreType.ScoreTypeId,
                SubjectId = subjectScoreType.SubjectId,
                ScoreTypeName = subjectScoreType.ScoreType.Name,
                Coefficient = subjectScoreType.ScoreType.Coefficient,
                Amount = (uint)subjectScoreType.Amount
            };

            SubjectScoreTypeTable.Add(newRow);
        }
    }

    private async void DeleteSubjectScoreType()
    {
        if (SelectedSubjectScoreType == null)
        {
            ToastMessageViewModel.ShowErrorToast("Hãy chọn loại điểm để xóa");
            return;
        }

        await GenericDataService<SubjectScoreType>.Instance.DeleteOneAsync(sst => sst.Id == SelectedSubjectScoreType.SubjectScoreTypeId);
        SubjectScoreTypeTable.Remove(SelectedSubjectScoreType);
    }

    public sealed class ScoreTypeTableRow
    {
        public string SubjectScoreTypeId { get; set; }
        public string ScoreTypeId { get; set; }
        public string SubjectId { get; set; }
        public string ScoreTypeName { get; set; }
        public double Coefficient { get; set; }
        public uint Amount { get; set; }
    }
}