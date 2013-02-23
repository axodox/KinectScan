using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.IO;
namespace ExcaliburSecurityServer
{
    class Program
    {
        static void Main(string[] args)
        {
            LicenseServer LS = new LicenseServer();
            LS.AddUser("rr", "uuu", "k", 6);
            LS.AddComputerToUser("rr", "qq", 10);
            LS.CheckLicense("rr", 10);
            LS.CheckTrial(78);
            Console.WriteLine(LS.UserCount);
            Console.ReadLine();
            LS.Dispose();
        }
    }

    class LicenseServer
    {
        SqlCeConnection DBConnection;

        public LicenseServer()
        {
            string dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DBConnection = new SqlCeConnection("Data Source=" + dir + @"\Licenses.sdf");
            DBConnection.Open();
        }

        public bool AddUser(string username, string firstname, string lastname, int maxcomputers)
        {
            using (SqlCeCommand C = DBConnection.CreateCommand())
            {
                C.CommandText = string.Format("INSERT INTO Users([Username], [FirstName], [LastName], [MaxComputers]) VALUES('{0}', '{1}', '{2}', {3});", new object[] { username, firstname, lastname, maxcomputers });
                try
                {
                    C.ExecuteNonQuery();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool AddComputerToUser(string username, string computername, long dna)
        {
            using (SqlCeCommand C = DBConnection.CreateCommand())
            {
                SqlCeDataReader R;
                C.CommandText = string.Format("SELECT [Id], [MaxComputers] FROM Users WHERE [Username] = '{0}'", username);
                int userId, maxComputers;
                using (R = C.ExecuteReader())
                {
                    if (!R.Read()) return false;
                    userId = (int)R["Id"];
                    maxComputers = (int)R["MaxComputers"];
                }

                C.CommandText = string.Format("SELECT COUNT(*) FROM Computers WHERE [User] = {0}", userId);
                int count = (int)C.ExecuteScalar();
                if (count >= maxComputers) return false;

                C.CommandText = string.Format("DELETE FROM Computers WHERE [DNA] = {0}", dna);
                C.ExecuteNonQuery();

                C.CommandText = string.Format("INSERT INTO Computers([Name], [User], [DNA], [RegistrationTime]) VALUES('{0}', {1}, {2}, {3});", new object[] { computername, userId, dna, DateTime.Now.ToBinary() });
                C.ExecuteNonQuery();
                return true;
            }
        }

        const int TrialDays = 5;
        public int CheckTrial(long dna)
        {
            using (SqlCeCommand C = DBConnection.CreateCommand())
            {
                C.CommandText = string.Format("SELECT [RegistrationTime] FROM Computers WHERE [DNA] = '{0}'", dna);
                object R = C.ExecuteScalar();
                if (R == null)
                {
                    C.CommandText = string.Format("INSERT INTO Computers([DNA], [RegistrationTime]) VALUES({0}, {1});", new object[] { dna, DateTime.Now.ToBinary() });
                    C.ExecuteNonQuery();
                    return TrialDays;
                }
                else
                {
                    return (DateTime.Now - DateTime.FromBinary((long)R)).Days;
                }
            }
        }

        public bool CheckLicense(string username, long dna)
        {
            using (SqlCeCommand C = DBConnection.CreateCommand())
            {
                C.CommandText = string.Format("SELECT [Id] FROM Users WHERE [Username] = '{0}'", username);
                object R = C.ExecuteScalar();
                if (R == null) return false;
                int userId = (int)R;
                C.CommandText = string.Format("SELECT COUNT(*) FROM Computers WHERE [User] = {0} AND [DNA] = {1}", userId, dna);
                return (int)C.ExecuteScalar() == 1;
            }
        }

        public int UserCount
        {
            get
            {
                using (SqlCeCommand C = DBConnection.CreateCommand())
                {
                    C.CommandText = "SELECT COUNT(*) FROM Users";
                    return (int)C.ExecuteScalar();
                }
            }
        }

        public void Dispose()
        {
            DBConnection.Close();
            DBConnection.Dispose();
        }
    }
}
