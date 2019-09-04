using Core.Models;

namespace Core.DataModel
{
    public class ProductsTransferCompare {
        public int Error { get; set; }
        public ProductsTransfer Load { get; set; }
        public ProductsTransfer Delv { get; set; }

        public ProductsTransferCompare() {
            Error = 0;
            Load = new ProductsTransfer();
            Delv = new ProductsTransfer();
        }
    }
}
