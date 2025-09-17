using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace POET.Models;

public partial class Question : INotifyPropertyChanged
{
    public int QuestionId { get; set; }

    public int TestId { get; set; }

    public string QuestionText { get; set; } = null!;

    public string OptionA { get; set; } = null!;

    public string OptionB { get; set; } = null!;

    public string OptionC { get; set; } = null!;

    public string OptionD { get; set; } = null!;

    private string _correctOption;
    public string CorrectOption
    {
        get => _correctOption;
        set
        {
            _correctOption = value;
            OnPropertyChanged(); // Notify UI when this property changes
        }
    }

    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual Test Test { get; set; } = null!;

    [NotMapped]
    public string UserAnswer { get; set; }
}
