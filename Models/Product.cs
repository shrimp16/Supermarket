namespace Products
{
    class Product
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string Type {get; set;}

        public Product(string name, int quantity, string type){
            this.Name = name;
            this.Quantity = quantity;
            this.Type = type;
        }

        public override string ToString()
        {
            return $"Product name: {this.Name}, Type: {this.Type} Quantity: {this.Quantity}";
        }
    }
    
}