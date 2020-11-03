using System;
using System.Windows;

using Microsoft.AspNet.Identity;

namespace LocalUserCreator {
    class Program {
        [STAThread]
        static void Main(string[] args) {
            PasswordHasher hasher = new PasswordHasher();
            Console.WriteLine("Kennwort:");
            string readLine = Console.ReadLine();
            while (!string.IsNullOrWhiteSpace(readLine)) {
                string hash = hasher.HashPassword(readLine);
                Clipboard.SetText(hash);
                Console.WriteLine(hash);

                Console.WriteLine("Kennwort:");
                readLine = Console.ReadLine();
            }
        }
    }
}