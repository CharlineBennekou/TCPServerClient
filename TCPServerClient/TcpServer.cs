using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpServer
{
    public static void Main(string[] args)
    {
        TcpListener server = null;

        try
        {
            // Create a TCP/IP socket and bind it to a port
            server = new TcpListener(IPAddress.Any, 12001);
            server.Start();
            Console.WriteLine("Server is up and running...");

            while (true)
            {
                // Accept client connections
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected.");
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
        finally
        {
            server?.Stop();
        }
    }

    private static void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            bool validMethod = false;
            string method = "";

            // Loop until we get a valid method
            while (!validMethod)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string clientInput = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                Console.WriteLine($"Received method: {clientInput}");

                // Validate the method
                if (clientInput == "random" || clientInput == "add" || clientInput == "subtract")
                {
                    method = clientInput;
                    validMethod = true;
                    string response = $"Method {method} is valid. Now enter two numbers.";
                    stream.Write(Encoding.ASCII.GetBytes(response), 0, response.Length);
                }
                else
                {
                    string response = "Invalid method. Please choose random, add, or subtract.";
                    stream.Write(Encoding.ASCII.GetBytes(response), 0, response.Length);
                }
            }

            // Loop until we get valid numbers
            bool validNumbers = false;

            while (!validNumbers)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string clientInput = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                Console.WriteLine($"Received numbers: {clientInput}");

                // Split and validate the numbers
                string[] numbers = clientInput.Split(' ');
                if (numbers.Length == 2 && int.TryParse(numbers[0], out int num1) && int.TryParse(numbers[1], out int num2))
                {
                    if (method == "random" && num1 > num2)
                    {
                        string response = "For the random method, the first number must be less than or equal to the second.";
                        stream.Write(Encoding.ASCII.GetBytes(response), 0, response.Length);
                    }
                    else
                    {
                        validNumbers = true;
                        string result = PerformOperation(method, num1, num2);
                        stream.Write(Encoding.ASCII.GetBytes(result), 0, result.Length);
                    }
                }
                else
                {
                    string response = "Invalid numbers. Please enter two valid numbers in the format 'x y'.";
                    stream.Write(Encoding.ASCII.GetBytes(response), 0, response.Length);
                }
            }

            // Close the connection
            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"An error occurred: {e.Message}");
        }
    }

    private static string PerformOperation(string method, int num1, int num2)
    {
        switch (method)
        {
            case "random":
                Random rnd = new Random();
                int randomNum = rnd.Next(num1, num2 + 1); // Generates a random number between num1 and num2 (inclusive)
                return $"Random number between {num1} and {num2}: {randomNum}";
            case "add":
                return $"Sum of {num1} and {num2}: {num1 + num2}";
            case "subtract":
                return $"Difference between {num1} and {num2}: {num1 - num2}";
            default:
                return "Unknown operation.";
        }
    }
}
