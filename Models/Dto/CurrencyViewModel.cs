using System.Collections.Generic;

namespace ProgramaYA.Models.Dto
{
    public class CurrencyViewModel
    {
        public Dictionary<string, string> Currencies { get; set; } = new Dictionary<string, string>();
        public string From { get; set; } = "USD";
        public string To { get; set; } = "EUR";
        public decimal Amount { get; set; } = 1;
        public decimal? Result { get; set; }
        public string ErrorMessage { get; set; }
        public string RawApiMessage { get; set; }
    }
}
