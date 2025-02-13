/* -------------------------------------------------------------------------------------------------
   Restricted. Copyright (C) Siemens Healthineers AG, 2025. All rights reserved.
   ------------------------------------------------------------------------------------------------- */
   
using System;
using System.Collections.Generic;
using System.IO;
using BCrypt.Net;

class Program1
{
    static Dictionary<string, string> adminData = new Dictionary<string, string>();
    static int failedLoginAttempts = 0;
    const int maxFailedAttempts = 3;
    const string dataFilePath = "adminData.txt";

    static void Main(string[] args)
    {
        LoadAdminData();

        while (true)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
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
                        AccessPatientManagement();
                    }
                    break;
                case "3":
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
        else
        {
            Console.WriteLine("Invalid username or passcode.");
            failedLoginAttempts++;
            return false;
        }
    }

    static void AccessPatientManagement()
    {
        Console.WriteLine("Accessing patient management functions...");
        // Implement patient management functions here
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
}
