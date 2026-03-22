// Eticaret.Data.Entities / ChatOption.cs
using Eticaret.Core.Entities;

public class ChatOption : IEntity
{
    public int Id { get; set; }
    public string Scenario { get; set; } // Hangi senaryoya ait? (FidanOnerisi, Bakim vb.)
    public int StepIndex { get; set; } // Hangi adımda görünecek?
    public string ButtonText { get; set; } // Butonun üzerinde ne yazacak? (Örn: "Meyve Üretimi")
    public string ActionValue { get; set; } // Backend'e gidecek değer (Örn: "Meyve")
    public int NextStepIndex { get; set; } // Bu butona basınca kaçıncı adıma geçilsin?
}