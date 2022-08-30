using Keyboard;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("ON");
        KeyboardControls keyboardControls = new KeyboardControls();
        await keyboardControls.Start();
    }

}