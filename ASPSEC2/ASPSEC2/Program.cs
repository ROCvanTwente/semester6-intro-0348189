using System;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        string connectionString = "Server=.;Database=ASPSEC2;Trusted_Connection=True;Encrypt=False";
        string firstName = "John";
        string lastName = "Doe";
        string street = "Main St";
        string houseNumber = "123";
        string postalCode = "12345";
        string city = "Hometown";
        string creditCardNumber = "1234-5678-9012-3456";
        string key = "0123456789abcdef"; // 16 bytes key for AES-128

        string encryptedCreditCardNumber = Encrypt(creditCardNumber, key);

        SavePersonData(connectionString, firstName, lastName, street, houseNumber, postalCode, city, encryptedCreditCardNumber);
    }

    static string Encrypt(string plainText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // Initialization vector with 16 bytes of zeros

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    static void SavePersonData(string connectionString, string firstName, string lastName, string street, string houseNumber, string postalCode, string city, string encryptedCreditCardNumber)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Persons (FirstName, LastName, Street, HouseNumber, PostalCode, City, CreditCardNumber) VALUES (@FirstName, @LastName, @Street, @HouseNumber, @PostalCode, @City, @CreditCardNumber)";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Street", street);
                command.Parameters.AddWithValue("@HouseNumber", houseNumber);
                command.Parameters.AddWithValue("@PostalCode", postalCode);
                command.Parameters.AddWithValue("@City", city);
                command.Parameters.AddWithValue("@CreditCardNumber", encryptedCreditCardNumber);

                command.ExecuteNonQuery();
            }
        }
    }
}