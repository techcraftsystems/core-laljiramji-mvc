using Core.Models;

namespace Core.ViewModel
{
    public class VatCalculatorViewModel
    {
        public PurchasesVat Diesel { get; set; }
        public PurchasesVat Super { get; set; }
        public PurchasesVat Vpower { get; set; }
        public PurchasesVat Kerosene { get; set; }
        
        public VatCalculatorViewModel()
        {
            Diesel = new PurchasesVat();
            Super = new PurchasesVat();
            Vpower = new PurchasesVat();
            Kerosene = new PurchasesVat();
        }
    }
}
