using Products;

namespace Views
{

    class ProductViews
    {

        public bool onProduct { get; set; }

        public ProductViews() {
            this.onProduct = false;
        }

        public void ShowAllProducts(Product[] products, int current){
            Console.Clear();
            for(int i = 0; i < products.Length; i++){
                if(i == current){
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
                Console.WriteLine(products[i].Name);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public void ShowSingleProduct(Product product){
            Console.Clear();
            onProduct = true;
            Console.WriteLine("Product Name: {0}", product.Name);
            Console.WriteLine("Product Quantity: {0}", product.Quantity);
            Console.WriteLine("Product Type: {0}", product.Type);
        }

        public bool isOnProduct(){
            return onProduct;
        }

    }
}