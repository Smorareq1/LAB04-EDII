using System;
using System.IO;
using System.Text;

namespace LAB_04_EDII
{
    class Program
    {
        static void Main(string[] args)
        {
            Principal();
        }
        
        private static void Principal()
        {
            try
            {
                Console.WriteLine("Ingrese la ruta del archivo de libros: ");
                string archivoLibros = Console.ReadLine();
                
                
                Console.WriteLine("Ingrese la ruta del archivo de busqueda: ");
                string archivoBusqueda = Console.ReadLine();
                    
                
                // Verificar si los archivos existen
                if (!File.Exists(archivoLibros) || !File.Exists(archivoBusqueda))
                {
                    Console.WriteLine($"El archivo {archivoLibros} o {archivoBusqueda} no existe.");
                    return;
                }

                try
                {
                    // Crear la ruta del archivo de resultados en la misma carpeta que el archivo de inserción
                    string carpetaResultados = Path.GetDirectoryName(archivoLibros);
                    string archivoResultados = Path.Combine(carpetaResultados, "resultados-busquedas.txt");

                    // Crear el archivo de resultados si no existe
                    if (!File.Exists(archivoResultados))
                    {
                        File.Create(archivoResultados).Close();
                    }

                    Console.WriteLine($"Archivo de salida generado en {archivoResultados}");
                    Console.WriteLine("Procesando archivos...");

                    GestorDeArchivos.ProcesarArchivoInsertar(archivoLibros);
                    GestorDeArchivos.ProcesarArchivoBusqueda(archivoBusqueda, archivoResultados);

                    // Encrypt the output file using DES
                    string archivoEncriptado = Path.Combine(carpetaResultados, "resultados-busquedas.ENC");
                    GestorDeArchivos.EncryptOutputFileWithDES(archivoResultados, archivoEncriptado);

                    
                    // Decrypt the .ENC file and save it as resultsDeCrypted.txt
                    //string archivoDesencriptado = Path.Combine(carpetaResultados, "resultsDeCrypted.txt");
                    //GestorDeArchivos.DecryptOutputFileWithDES(archivoEncriptado, archivoDesencriptado);

                    Console.WriteLine("Listo :)");
                
                }catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }catch(Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static void prueba1()
        {
            string key = "ok:uo1IN";
            var byteskey = Encoding.ASCII.GetBytes(key);
            string data = "1234567887654321";
            var bytesdata = Encoding.ASCII.GetBytes(data);
            var encrypted = DES.Cifrar(bytesdata, byteskey);
            var decrypted = DES.Descifrar(encrypted, byteskey);
            Console.WriteLine();
            Console.WriteLine($"Encrypt DES: {Convert.ToHexString(encrypted)}");
            Console.WriteLine($"Decrypt DES: {Encoding.ASCII.GetString(decrypted)}");
            bool match = data == Encoding.ASCII.GetString(decrypted);
            Console.WriteLine($"Match = {match}");
        }
        
        public static void DisplayEncryptedFileInBinary(string filePath)
        {
            try
            {
                // Ensure the file exists
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return;
                }

                // Read all bytes from the file
                byte[] fileBytes = File.ReadAllBytes(filePath);

                // Convert each byte to its binary representation without spaces
                StringBuilder binaryStringBuilder = new StringBuilder();
                foreach (byte b in fileBytes)
                {
                    binaryStringBuilder.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                }

                // Display the binary content
                Console.WriteLine($"Content of {filePath} in Binary:");
                Console.WriteLine(binaryStringBuilder.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading the file: {ex.Message}");
            }
        }

        
        public static void DisplayDecryptedTextFromFile(string filePath, string key)
        {
            try
            {
                // Ensure the file exists
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return;
                }

                // Read all bytes from the encrypted file
                byte[] encryptedBytes = File.ReadAllBytes(filePath);

                // Convert the key to bytes
                byte[] keyBytes = Encoding.ASCII.GetBytes(key);

                // Decrypt the file content using the DES algorithm
                byte[] decryptedBytes = DES.Descifrar(encryptedBytes, keyBytes);

                // Convert the decrypted bytes to text
                string decryptedText = Encoding.ASCII.GetString(decryptedBytes);

                // Display the decrypted text
                Console.WriteLine("Decrypted Text:");
                Console.WriteLine(decryptedText);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting the file: {ex.Message}");
            }
        }
        
    }
}