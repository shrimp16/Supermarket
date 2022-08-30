using Xunit;
using Products;
using MySqlConnector;
using Keyboard;

public class CrudOpsTests
{

    public async Task setup()
    {
        MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
        {
            Server = "localhost",
            Database = "supermarket-test",
            UserID = "root",
            Password = ""
        };

        using (var connection = new MySqlConnection(builder.ConnectionString))
        {
            await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DROP TABLE IF EXISTS inventory;";
                await command.ExecuteNonQueryAsync();
                command.CommandText = "CREATE TABLE inventory (id serial PRIMARY KEY, name VARCHAR(50), quantity INTEGER, type VARCHAR(50));";
                await command.ExecuteNonQueryAsync();

                command.CommandText = @"INSERT INTO inventory (name, quantity, type) VALUES 
                (@name1, @quantity1, @type1),
                (@name2, @quantity2, @type2),
                (@name3, @quantity3, @type3);";

                command.Parameters.AddWithValue("@name1", "Carrots");
                command.Parameters.AddWithValue("@quantity1", 50);
                command.Parameters.AddWithValue("@type1", "Foods");
                command.Parameters.AddWithValue("@name2", "Cookies");
                command.Parameters.AddWithValue("@quantity2", 50);
                command.Parameters.AddWithValue("@type2", "Foods");
                command.Parameters.AddWithValue("@name3", "Coca-Cola");
                command.Parameters.AddWithValue("@quantity3", 50);
                command.Parameters.AddWithValue("@type3", "Drinks");

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    [Fact]
    public async void TestGetProducts()
    {
        await setup();
        ProductsManager productsManager = new ProductsManager("supermarket-test");
        Product[] products = await productsManager.GetAllProducts();
        Product product = await productsManager.GetProduct("Coca-Cola");

        Assert.Equal(3, products.Length);
        Assert.Equal("Coca-Cola", product.Name);
        Assert.Equal(50, product.Quantity);
        Assert.Equal("Drinks", product.Type);
    }

    [Fact]
    public async void TestCreateProduct()
    {
        await setup();
        string newProductName = "Apple";
        string newProductType = "Fruit";
        ProductsManager productsManager = new ProductsManager("supermarket-test");
        Product[] old_products = await productsManager.GetAllProducts();
        await productsManager.CreateProduct(newProductName, newProductType);
        Product[] new_products = await productsManager.GetAllProducts();

        Assert.False(old_products.Length == new_products.Length);
        Assert.False(Array.Exists(old_products, product => product.Name == newProductName));
        Assert.True(new_products[new_products.Length - 1].Type == newProductType);
        Assert.True(new_products[new_products.Length - 1].Name == newProductName);
    }

    [Fact]
    public async void TestRemoveProduct()
    {
        await setup();
        string productToRemove = "Coca-Cola";
        ProductsManager productsManager = new ProductsManager("supermarket-test");
        Product[] OldProducts = await productsManager.GetAllProducts();
        await productsManager.DeleteProduct(productToRemove);
        Product[] NewProducts = await productsManager.GetAllProducts();

        Assert.True(Array.Exists(OldProducts, product => product.Name == productToRemove));
        Assert.False(Array.Exists(NewProducts, product => product.Name == productToRemove));
        Assert.Equal(3, OldProducts.Length);
        Assert.Equal(2, NewProducts.Length);
    }

    [Fact]
    public async void TestAddStock()
    {
        await setup();
        ProductsManager productsManager = new ProductsManager("supermarket-test");
        Product product;

        await productsManager.UpdateProduct("Coca-Cola", 150);
        product = await productsManager.GetProduct("Coca-Cola");
        Assert.Equal(200, product.Quantity);
    }

    [Fact]
    public async void TestRemoveStock()
    {
        await setup();
        ProductsManager productsManager = new ProductsManager("supermarket-test");
        Product product;

        await productsManager.UpdateProduct("Coca-Cola", -25);
        product = await productsManager.GetProduct("Coca-Cola");
        Assert.Equal(25, product.Quantity);

        await productsManager.UpdateProduct("Coca-Cola", -50);
        product = await productsManager.GetProduct("Coca-Cola");
        Assert.Equal(25, product.Quantity);

        await productsManager.UpdateProduct("Coca-Cola", -200);
        product = await productsManager.GetProduct("Coca-Cola");
        Assert.Equal(25, product.Quantity);
    }

    [Fact]
    public void TestDecrementPointer()
    {
        KeyboardControls keyboardControls = new KeyboardControls();

        keyboardControls.DecrementPointer();

        Assert.Equal(0, keyboardControls.pointer);

        keyboardControls.pointer = 2;

        keyboardControls.DecrementPointer();

        Assert.Equal(1, keyboardControls.pointer);
    }

    [Fact]
    public async void TestIncrementPointer()
    {
        KeyboardControls keyboardControls = new KeyboardControls();

        keyboardControls.productsManager = new ProductsManager("supermarket-test");

        await keyboardControls.UpdateProducts();

        keyboardControls.IncrementPointer();

        Assert.Equal(1, keyboardControls.pointer);

        keyboardControls.pointer = 2;

        keyboardControls.IncrementPointer();

        Assert.Equal(keyboardControls.products.Length - 1, keyboardControls.pointer);
    }

}