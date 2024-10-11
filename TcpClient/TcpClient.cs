using System;
using System.Net.Sockets;
using System.Text;

class TcpClientProgram
{
    public static void Main(string[] args)
    {
        try
        {
            // Connect to the server
            TcpClient client = new TcpClient("127.0.0.1", 12001); // Change IP if needed
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            string methodChoice = "";
            bool validMethod = false;

            // Keep asking for a valid method until accepted by the server
            while (!validMethod)
            {
                // Ask the user for the method choice
                Console.WriteLine("Choose a method: random, add, or subtract");
                methodChoice = Console.ReadLine().Trim().ToLower();

                // Send the method to the server
                byte[] dataToSend = Encoding.ASCII.GetBytes(methodChoice);
                stream.Write(dataToSend, 0, dataToSend.Length);

                // Read the server's response
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string serverResponse = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(serverResponse);

                // Check if the server accepted the method
                if (!serverResponse.Contains("Invalid method"))
                {
                    validMethod = true;
                }
            }

            // Loop to get valid numbers from the user and send them to the server
            bool validNumbers = false;

            while (!validNumbers)
            {
                Console.Write("Enter the first number: ");
                string firstNumber = Console.ReadLine().Trim();

                Console.Write("Enter the second number: ");
                string secondNumber = Console.ReadLine().Trim();

                // Send the two numbers to the server in the format "x y"
                string numbersToSend = $"{firstNumber} {secondNumber}";
                byte[] dataToSend = Encoding.ASCII.GetBytes(numbersToSend);
                stream.Write(dataToSend, 0, dataToSend.Length);

                // Read the server's response
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string serverResponse = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Server response: {serverResponse}");

                // Check if the server accepted the numbers
                if (!serverResponse.Contains("Invalid"))
                {
                    validNumbers = true;
                }
                // If the server sends an error about invalid numbers, the loop continues to prompt for new numbers
            }

            // Close the connection
            stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }
}
