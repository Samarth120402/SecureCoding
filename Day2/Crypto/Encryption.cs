using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program1
{
    static Dictionary<string, string> adminData = new Dictionary<string, string>();
    static List<string> patientRecords = new List<string>();
    static int failedLoginAttempts = 0;
    const int maxFailedAttempts = 3;
    const string dataFilePath = "adminData.txt";
    static byte[] sessionKey;

    static void Main(string[] args)
    {
        LoadAdminData();

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
                        sessionKey = GenerateKey();
                    }
                    break;
                case "3":
                    if (sessionKey != null)
                    {
                        AddPatientRecord();
                    }
                    else
                    {
                        Console.WriteLine("Please login first.");
                    }
                    break;
                case "4":
                    if (sessionKey != null)
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
        byte[] encryptedData = Encrypt(patientData, sessionKey);
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

    // Generate a 256-bit random key
    static byte[] GenerateKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] key = new byte[32]; // 32 bytes = 256 bits for AES-256
            rng.GetBytes(key);
            return key;
        }
    }

    static byte[] Encrypt(string plaintext, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.GenerateIV(); // Generate a random IV

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                // Prepend the IV to the encrypted data
                byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
                Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                Buffer.BlockCopy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

                return result;
            }
        }
    }

    static string Decrypt(byte[] encryptedData, byte[] key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;

            // Extract the IV from the beginning of the encrypted data
            byte[] iv = new byte[16]; // AES block size is 16 bytes
            byte[] ciphertext = new byte[encryptedData.Length - iv.Length];
            Buffer.BlockCopy(encryptedData, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(encryptedData, iv.Length, ciphertext, 0, ciphertext.Length);

            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(ciphertext, 0, ciphertext.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}

