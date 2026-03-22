namespace Eticaret.Core.DTOs.ChatBot
{
    public class FidanChatState
    {
        public string CurrentStep { get; set; } = "Start";
        public string SelectedCity { get; set; } = string.Empty;
        public string SelectedSun { get; set; } = string.Empty;
        public string SelectedWater { get; set; } = string.Empty;
        public string SelectedPurpose { get; set; } = string.Empty; // "Meyve verimi", "Hızlı büyüme" vb.
    }
}