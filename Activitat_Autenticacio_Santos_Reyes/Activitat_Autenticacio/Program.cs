using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace Activitat_autenticacio
{
    class autenticacio
    {
        static void Main(string[] args)
        {
            //declarem variables.
            string fitxer;
            string opcio = "1";
            string password = "";
            string salt;

            //condicionem la sortida del bucle.
            while (opcio != "0")
            {
                //mostrem el menú.
                Console.WriteLine("----------------Registre/Validació d'usuaris-------------------");
                Console.WriteLine();
                System.Console.WriteLine("Introdueix 'r'(Registrar), 'v'(Validar) o '0'(Sortir): ");

                //llegim la opció.
                opcio = Console.ReadLine();
                //Controlem si l'opció és vàlida.
                while ((opcio != "r") && (opcio != "v") && (opcio != "0"))
                {
                    Console.WriteLine("Error, la opcio introduïda es incorrecte, introdueix una opció vàlida");
                    opcio = Console.ReadLine();
                }
                switch (opcio)
                {

                    case "r":
                        Console.WriteLine("Introdueix el nom d'usuari: ");
                        string usuari = Console.ReadLine();
                        //Comprovem si l'usuari es correcte.
                        if (UsuariCorrecte(usuari))
                        {
                            Console.Write("Entra el password: ");
                            //Guardem el hash del password i la salt en una variable.
                            password = Password();
                            salt = GenerarSalt();
                            
                            //Donem d'alta l'usuari.
                            DonarAltaUsuari(usuari, salt, password);
                        }
                        else
                        {
                            Console.WriteLine("El nom d'usuari és incorrecte");
                        }
                        break;

                    case "v":
                        Console.WriteLine("Introdueix el nom d'usuari a validar: ");
                        usuari = Console.ReadLine();
                        if (UsuariCorrecte(usuari))
                        {
                            Console.Write("Entra el password: ");
                            //Guardem el hash del password en una variable.
                            password = Password();

                            ValidarUsuari(usuari, password);

                        }
                        else
                        {
                            Console.WriteLine("El nom d'usuari és incorrecte");
                        }
                        break;
                }
            }
        }
        //Donar d'alta nou usuari
        private static void DonarAltaUsuari(string usuari, string salt, string password)
        {
            string hashsalt = "," + salt + "," + password;
            Boolean trobat = false;

            if (!File.Exists(@"usuaris.txt"))
            {
                using (System.IO.StreamWriter arxiu = new System.IO.StreamWriter(@"usuaris.txt"))
                {
                    arxiu.WriteLine(usuari + hashsalt);
                    arxiu.Close();//Tanquem el procés un cop s'acaba d'escriure.
                }
            }
            else
            {
                try
                {
                    using (StreamReader sr = new StreamReader(@"usuaris.txt"))
                    {
                        while (sr.Peek() > -1)
                        {
                            string linia = sr.ReadLine();
                            if (!String.IsNullOrEmpty(linia))//Comprovem si es null.
                            {
                                //Separem nom, salt i hash

                                string[] user = linia.Split(',');
                                int i, j;
                                for (i = 0; i < user.Length; i++)
                                {
                                    for (j = 0; j < user.Length; j++)
                                    {
                                        if (user[j].Equals(usuari))
                                        {
                                            trobat = true;//Comprovem sil'usuari existeix.
                                        }
                                    }
                                }
                            }
                        }
                        sr.Close();//Tanquem el procés un cop s'acaba de llegir.
                    }
                    if (trobat == false)
                    {
                        using (System.IO.StreamWriter arxiu = new System.IO.StreamWriter(@"usuaris.txt", true))
                        {
                            arxiu.WriteLine(usuari + hashsalt);
                            arxiu.Close();//Tanquem el procés un cop s'acaba d'escriure.
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error, nom d'usuari existent");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static void ValidarUsuari(string usuari, string password)
        {
            Boolean trobat = false;
            using (StreamReader sr = new StreamReader(@"usuaris.txt"))
            {
                while (sr.Peek() > -1)
                {
                    string linia = sr.ReadLine();
                    if (!String.IsNullOrEmpty(linia))
                    {
                        //Separem nom, salt i hash

                        string[] user = linia.Split(',');
                        int i, j;
                        for (i = 0; i < user.Length; i++)
                        {
                            for (j = 0; j < user.Length; j++)
                            {
                                if (user[j].Equals(usuari))
                                {
                                    if (user[j + 2].Equals(password))
                                    {
                                        trobat = true;//Comprovem si els hash coincideixen.
                                    }
                                }
                            }
                        }
                    }
                }
                sr.Close();//Tanquem el procés un cop s'acaba de llegir.
            }
            if (trobat == true)
            {
                Console.WriteLine("L'usuari és vàlid");
            }
            else
            {
                Console.WriteLine("L'usuari no és vàlid");
            }
        }



        //Generar salt aleatori.
        private static string GenerarSalt()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            return Convert.ToBase64String(salt);
        }

        //Generar Hash.
        private static string GenerarHash(byte[] salt, string password)
        {
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(32);

            return Convert.ToBase64String(hash);
        }

        //Comprovem que l'usuari no tingui comes.
        public static Boolean UsuariCorrecte(string usuari)
        {
            bool valid = true;

            if (usuari.Contains(","))
            {
                valid = false;
            }
            else if (usuari == "")
            {
                valid = false;
            }
            return valid;
        }

        //Ocultar la password
        static string Password()
        {

            Console.WriteLine();
            ConsoleKeyInfo key;
            string password = null;
           
            do
            {
                key = Console.ReadKey(true);
                // Si no es backspace o enter llegeix la tecla.
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                // Esborrem l'útlim '*' al pulsar el backspace.
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password.Remove(password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);
           

            Console.Clear();
            return CalculaHash(password);
        }

        //Calculem el hash.
        static string CalculaHash(string textIn)
        {

            try
            {
                // Convertim l'string a un array de bytes.
                byte[] bytesIn = UTF8Encoding.UTF8.GetBytes(textIn);
                // Instanciar classe per fer hash.
                SHA512Managed SHA512 = new SHA512Managed();
                // Calcular hash.
                byte[] hashResult = SHA512.ComputeHash(bytesIn);

                // Si volem mostrar el hash per pantalla o guardar-lo en un arxiu de text
                // cal convertir-lo a un string.

                //String textOut = BitConverter.ToString(hashResult, 0);
                String textOut=Convert.ToBase64String(hashResult);


                // Eliminem la classe instanciada.
                SHA512.Dispose();
                return textOut;
            }
            catch (Exception)
            {
                Console.WriteLine("Error calculant el hash");
                Console.ReadKey(true);
                return null;
            }
        }
    }
}