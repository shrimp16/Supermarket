using Views;
using Products;

namespace Keyboard
{

    class KeyboardControls
    {

        public int pointer { get; set; }
        public ProductViews productViews { get; set; }
        public Product[] products { get; set; }
        public ProductsManager productsManager { get; set; }

        public KeyboardControls()
        {
            pointer = 0;
            productsManager = new ProductsManager();
            productViews = new ProductViews();
        }

        public KeyboardControls(ProductsManager productsManager){
            pointer = 0;
            this.productsManager = productsManager;
            productViews = new ProductViews();
        }

        public async Task Start()
        {
            await UpdateProducts();
            productViews.ShowAllProducts(products, pointer);
            while (!productViews.isOnProduct())
            {
                KeyPress(Console.ReadKey(true).Key);
            }
        }

        public async Task UpdateProducts()
        {
            this.products = await productsManager.GetAllProducts();
        }
        public async void KeyPress(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.UpArrow:
                    DecrementPointer();
                    productViews.ShowAllProducts(products, pointer);
                    break;
                case ConsoleKey.DownArrow:
                    IncrementPointer();
                    productViews.ShowAllProducts(products, pointer);
                    break;
                case ConsoleKey.Enter:
                    productViews.ShowSingleProduct(products[pointer]);
                    while (productViews.isOnProduct())
                    {
                        KeyPress(Console.ReadKey(true).Key);
                    }
                    break;
                case ConsoleKey.Escape:
                    productViews.ShowAllProducts(products, pointer);
                    break;
                case ConsoleKey.LeftArrow:
                    await Start();
                    break;
                case ConsoleKey.N:
                    Console.Clear();
                    Console.Write("Name: ");
                    string name = Console.ReadLine();
                    Console.Write("\nType: ");
                    string type = Console.ReadLine();
                    await productsManager.CreateProduct(name, type);
                    await Start();
                    break;
                case ConsoleKey.S:
                    Console.Clear();
                    Console.Write("Stock update: ");
                    UpdateStock();
                    break;
                case ConsoleKey.R:
                    await RemoveProduct();
                    break;
                case ConsoleKey.F:
                    await FindByType();
                    break;
            }

        }

        public void DecrementPointer()
        {
            if (pointer == 0)
            {
                return;
            }
            pointer--;
        }

        public void IncrementPointer()
        {
            if (pointer == products.Length - 1)
            {
                return;
            }
            pointer++;
        }

        public async void UpdateStock()
        {
            int update;
            Int32.TryParse(Console.ReadLine(), out update);
            if (products[pointer].Quantity + update < 0)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("You can't remove more than you have!");
                Console.WriteLine("Press backspace to exit...");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey();
                productViews.ShowAllProducts(products, pointer);
                return;
            }
            await productsManager.UpdateProduct(products[pointer].Name, products[pointer].Quantity + update);
            await UpdateProducts();
            productViews.ShowAllProducts(products, pointer);
        }

        public async Task RemoveProduct()
        {
            Console.Clear();

            if (products.Length == 0)
            {
                return;
            }

            await productsManager.DeleteProduct(products[pointer].Name);

            if (pointer - 1 > 0)
            {
                pointer--;
            }

            await Start();
        }

        public async Task FindByType()
        {
            Console.Clear();
            pointer = 0;
            Console.Write("Product type: ");
            string typeToSearch = Console.ReadLine();
            Console.WriteLine(products.Length);
            products = await productsManager.GetProductsByType(typeToSearch);
            if (products.Length == 0)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("There are currently no products from that category!");
                Console.WriteLine("Press backspace to exit...");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ReadKey(true);
                await Start();
                return;
            }
            productViews.ShowAllProducts(products, pointer);
        }
    }

}