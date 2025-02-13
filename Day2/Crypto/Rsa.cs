using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

class Program1
{
    static Dictionary<string, string> adminData = new Dictionary<string, string>();
    static List<string> patientRecords = new List<string>();
    static int failedLoginAttempts = 0;
    const int maxFailedAttempts = 3;
    const string dataFilePath = "adminData.txt";
    static RSAParameters publicKey;
    static RSAParameters privateKey;

    static void Main(string[] args)
    {
        LoadAdminData();
        GenerateRSAKeys();

        while (true)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Add Patient Record");
            Console.WriteLine("4. View Patient Records");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RegisterAdmin();
                    break;
                case "2":
                    if (LoginAdmin())
                    {
                        // Session key is not needed for RSA encryption
                    }
                    break;
                case "3":
                    if (publicKey.Modulus != null)
                    {
                        AddPatientRecord();
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case "4":
                    if (publicKey.Modulus != null)
                    {
                        ViewPatientRecords();
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void RegisterAdmin()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter passcode: ");
        string passcode = Console.ReadLine();

        if (adminData.ContainsKey(username))
        {
            Console.WriteLine("Username already exists. Please choose a different username.");
            return;
        }

        string salt = BCrypt.Net.BCrypt.GenerateSalt(15);
        string hashedPasscode = BCrypt.Net.BCrypt.HashPassword(passcode, salt);

        adminData[username] = hashedPasscode;
        SaveAdminData();

        Console.WriteLine("Admin registered successfully.");
    }

    static bool LoginAdmin()
    {
        if (failedLoginAttempts >= maxFailedAttempts)
        {
            Console.WriteLine("Too many failed login attempts. Please try again later.");
            return false;
        }

        Console.Write("Enter username: ");
        string username = Console.ReadLine();
        Console.Write("Enter passcode: ");
        string passcode = Console.ReadLine();

        if (adminData.ContainsKey(username) && BCrypt.Net.BCrypt.Verify(passcode, adminData[username]))
        {
            Console.WriteLine("Login successful.");
            failedLoginAttempts = 0;
            return true;
        }

        Console.WriteLine("Invalid username or passcode.");
        failedLoginAttempts++;
        return false;
    }

    static void AddPatientRecord()
    {
        Console.Write("Enter patient name: ");
        string name = Console.ReadLine();
        Console.Write("Enter patient age: ");
        string age = Console.ReadLine();
        Console.Write("Enter patient email: ");
        string email = Console.ReadLine();
        Console.Write("Enter patient SSN: ");
        string ssn = Console.ReadLine();
        Console.Write("Enter patient history of illness: ");
        string history = Console.ReadLine();

        string patientData = $"Name: {name}, Age: {age}, Email: {email}, SSN: {ssn}, History: {history}";
        byte[] encryptedData = Encrypt(patientData, publicKey);
        patientRecords.Add(Convert.ToBase64String(encryptedData));

        Console.WriteLine("Patient record added successfully.");
    }

    static void ViewPatientRecords()
    {
        foreach (var record in patientRecords)
        {
            Console.WriteLine(record);
        }
    }

    static void LoadAdminData()
    {
        if (File.Exists(dataFilePath))
        {
            string[] lines = File.ReadAllLines(dataFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    adminData[parts[0]] = parts[1];
                }
            }
        }
    }

    static void SaveAdminData()
    {
        List<string> lines = new List<string>();
        foreach (var kvp in adminData)
        {
            lines.Add($"{kvp.Key}:{kvp.Value}");
        }
        File.WriteAllLines(dataFilePath, lines);
    }

    static void GenerateRSAKeys()
    {
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            rsa.PersistKeyInCsp = false;
            publicKey = rsa.ExportParameters(false);
            privateKey = rsa.ExportParameters(true);
        }
    }

    static byte[] Encrypt(string plaintext, RSAParameters publicKey)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(publicKey);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
            return rsa.Encrypt(plainBytes, false);
        }
    }

    static string Decrypt(byte[] encryptedData, RSAParameters privateKey)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            byte[] decryptedBytes = rsa.Decrypt(encryptedData, false);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}

class DoctorApp
{
    static RSAParameters privateKey;

    static void Main(string[] args)
    {
        LoadPrivateKey();

        Console.Write("Enter encrypted patient data: ");
        string encryptedData = Console.ReadLine();
        byte[] encryptedBytes = Convert.FromBase64String(encryptedData);

        string decryptedData = Decrypt(encryptedBytes, privateKey);
        Console.WriteLine("Decrypted patient data: " + decryptedData);
    }

    static void LoadPrivateKey()
    {
        // Load the private key from a secure location
        // For demonstration, we generate a new key pair
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            rsa.PersistKeyInCsp = false;
            privateKey = rsa.ExportParameters(true);
        }
    }

    static string Decrypt(byte[] encryptedData, RSAParameters privateKey)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            byte[] decryptedBytes = rsa.Decrypt(encryptedData, false);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}
