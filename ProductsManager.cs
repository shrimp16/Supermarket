using MySqlConnector;

namespace Products
{
    class ProductsManager
    {
        MySqlConnectionStringBuilder builder;

        public ProductsManager()
        {
            builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Database = "supermarket",
                UserID = "root",
                Password = ""
            };
        }

        public ProductsManager(string database)
        {
            builder = new MySqlConnectionStringBuilder
            {
                Server = "localhost",
                Database = database,
                UserID = "root",
                Password = ""
            };
        }

        public async Task<Product[]> GetAllProducts()
        {
            List<Product> productsList = new List<Product>();
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {

                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM inventory";

                    command.Parameters.AddWithValue("@id", 4);

                    using (var reader = await command.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            productsList.Add(new Product(reader.GetString(1), reader.GetInt32(2), reader.GetString(3)));
                        }
                    }
                }
            }
            return productsList.ToArray();
        }

        public async Task<Product> GetProduct(string product_name)
        {
            Product product;
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM inventory WHERE name = @product_name;";
                    command.Parameters.AddWithValue("@product_name", product_name);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        await reader.ReadAsync();
                        product = new Product(reader.GetString(1), reader.GetInt32(2), reader.GetString(3));
                    }
                }
            }
            return product;
        }

        public async Task<Product[]> GetProductsByType(string type)
        {
            List<Product> productsList = new List<Product>();
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM inventory WHERE type = @type";
                    command.Parameters.AddWithValue("@type", type);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            productsList.Add(new Product(reader.GetString(1), reader.GetInt32(2), reader.GetString(3)));
                        }
                    }
                }
            }
            return productsList.ToArray();
        }

        public async Task CreateProduct(string name, string type)
        {
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO inventory (name, quantity, type) VALUES (@name, 0, @type);";
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@type", type);
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                    }
                    catch (MySqlException e)
                    {
                        if (e.ErrorCode.ToString() == "DuplicateKeyEntry")
                        {
                            return;
                        }
                        else throw;
                    }
                }
            }
        }

        public async Task CreateProduct(string name, int quantity, string type)
        {
            await CreateProduct(name, type);
            await UpdateProduct(name, quantity);
        }

        public async Task CreateProduct(Product product)
        {
            await CreateProduct(product.Name, product.Type);
            await UpdateProduct(product.Name, product.Quantity);
        }

        public async Task UpdateProduct(string product, int new_quantity)
        {
            Product prod = await GetProduct(product);
            Console.WriteLine(prod.Quantity + new_quantity);
            if (prod.Quantity + new_quantity < 0) return;

            int quantity = prod.Quantity + new_quantity;

            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE inventory SET quantity = @quantity WHERE name = @product;";
                    command.Parameters.AddWithValue("@quantity", quantity);
                    command.Parameters.AddWithValue("@product", product);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteProduct(string product)
        {
            using (var connection = new MySqlConnection(builder.ConnectionString))
            {
                await connection.OpenAsync();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "DELETE FROM inventory WHERE name = @product";
                    command.Parameters.AddWithValue("@product", product);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}