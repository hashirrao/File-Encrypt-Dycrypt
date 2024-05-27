using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Encrypt_Dycrypt
{
    class EDC
    {
        private static byte[] abc;
        private static byte[,] table;

        public EDC()
        {
            abc = new byte[256];
            for (int i = 0; i < 256; i++)
                abc[i] = Convert.ToByte(i);

            table = new byte[256, 256];
            for (int i = 0; i < 256; i++)
                for (int j = 0; j < 256; j++)
                {
                    table[i, j] = abc[(i + j) % 256];
                }
        }

        public static byte[] Encrypt(String filePath, String encryptionKey)
        {
            try
            {
                byte[] fileContent = File.ReadAllBytes(filePath);
                byte[] passwordTmp = Encoding.ASCII.GetBytes(encryptionKey);
                byte[] keys = new byte[fileContent.Length];
                for (int i = 0; i < fileContent.Length; i++)
                    keys[i] = passwordTmp[i % passwordTmp.Length];

                // Encrypt
                byte[] result = new byte[fileContent.Length];
                for (int encryptionTime = 1; encryptionTime <= 2; encryptionTime++)
                {
                    for (int i = 0; i < fileContent.Length; i++)
                    {
                        byte value = fileContent[i];
                        byte key = keys[i];
                        int valueIndex = -1, keyIndex = -1;
                        for (int j = 0; j < 256; j++)
                            if (abc[j] == value)
                            {
                                valueIndex = j;
                                break;
                            }
                        for (int j = 0; j < 256; j++)
                            if (abc[j] == key)
                            {
                                keyIndex = j;
                                break;
                            }
                        result[i] = table[keyIndex, valueIndex];
                    }
                }
                return result;
            }
            catch
            {
                MessageBox.Show("File is in use.\nClose other program is using this file and try again.");
                return null;
            }
        }

        public static byte[] Decrypt(String filePath, String encryptionKey)
        {
            try
            {
                byte[] fileContent = File.ReadAllBytes(filePath);
                byte[] passwordTmp = Encoding.ASCII.GetBytes(encryptionKey);
                byte[] keys = new byte[fileContent.Length];
                for (int i = 0; i < fileContent.Length; i++)
                    keys[i] = passwordTmp[i % passwordTmp.Length];

                byte[] result = new byte[fileContent.Length];

                for (int i = 0; i < fileContent.Length; i++)
                {
                    byte value = fileContent[i];
                    byte key = keys[i];
                    int valueIndex = -1, keyIndex = -1;
                    for (int j = 0; j < 256; j++)
                        if (abc[j] == key)
                        {
                            keyIndex = j;
                            break;
                        }
                    for (int j = 0; j < 256; j++)
                        if (table[keyIndex, j] == value)
                        {
                            valueIndex = j;
                            break;
                        }
                    result[i] = abc[valueIndex];
                }
                return result;
            }
            catch
            {
                MessageBox.Show("File is in use.\nClose other program is using this file and try again.");
                return null;
            }
        }
    }
}
