using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class Contributors
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Cell { get; set; }
        public DateTime DateJoined { get; set; }
        public bool AlwaysInclude { get; set; }
        public Deposit Deposit { get; set; }

    }
    public class Simcha
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Total { get; set; }



    }
    public class SimchaDonation
    {

        public int ContributorId { get; set; }
        public int Amount { get; set; }
        public int Balance { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool AlwaysInclude { get; set; }
        public bool Included { get; set; }
    }
    public class Deposit
    {
        public int Id { get; set; }
        public int ContributorId { get; set; }
        public int DepositAmount { get; set; }
        public DateTime Date { get; set; }
    }
    public class Donation
    {
        public int Id { get; set; }
        public int SimchaId { get; set; }
        public int DonationAmount { get; set; }
        public DateTime Date { get; set; }

    }

    public class SimchaFundsDB
    {
        private string _connectionString;
        public SimchaFundsDB(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<SimchaDonation> GetContibutorsforDonations(int simchaId)
        {
            IEnumerable<Contributors> contributors = GetContibutors();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM donation WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                List<Donation> donations = new List<Donation>();
                while (reader.Read())
                {
                    Donation d = new Donation
                    {
                        DonationAmount = (int)reader["Amount"],
                        SimchaId = simchaId,
                        Id = (int)reader["ContributorId"]
                    };
                    donations.Add(d);
                }

                List<SimchaDonation> result = new List<SimchaDonation>();
                foreach (Contributors contributor in contributors)
                {
                    SimchaDonation sc = new SimchaDonation();
                    sc.FirstName = contributor.FirstName;
                    sc.LastName = contributor.LastName;
                    sc.AlwaysInclude = contributor.AlwaysInclude;
                    sc.ContributorId = contributor.Id;
                    sc.Balance = GetBalance(sc.ContributorId);
                    Donation donation = donations.FirstOrDefault(c => c.Id == contributor.Id);
                    if (donation != null)
                    {
                        sc.Amount = donation.DonationAmount;
                    }
                    result.Add(sc);
                }

                return result;
            }


            //public int GetContibutorsAmountforDonations(int SimchaId, int ContributorId)
            //{
            //    using (SqlConnection connection = new SqlConnection(_connectionString))
            //    using (SqlCommand cmd = connection.CreateCommand())
            //    {
            //        cmd.CommandText = @"SELECT D.Amount
            //       From Contributor C Left Join Donation D
            //       On C.Id = D.COntributorId WHERE SimchaId = @SimchaId AND ContributorId = @ContributorId";
            //        cmd.Parameters.AddWithValue("@SimchaId", SimchaId);
            //        cmd.Parameters.AddWithValue("@ContributorId", ContributorId);
            //        connection.Open();
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        reader.Read();
            //        int Amount;
            //        if (reader["Amount"] != DBNull.Value)
            //        {
            //            Amount = (int)reader["Amount"];
            //        }
            //        else
            //        {
            //            Amount = 0;
            //        }

            //        return Amount;

            //    }
            //}
        }
        public void AddSimcha(Simcha simcha)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Simcha (Name, Date) " +
                    "VALUES(@Name, @Date)SELECT CAST(SCOPE_IDENTITY() AS INT)";
                cmd.Parameters.AddWithValue("@Name", simcha.Name);
                cmd.Parameters.AddWithValue("@Date", simcha.Date);
                connection.Open();
                simcha.Id = cmd.ExecuteNonQuery();

            }
        }
        public void AddDeposit(Deposit deposit)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"Insert into Deposit (DepositAmount, Date, ContributorId) Values (@DepositAmount, GetDate(), @ContributorId)";
                cmd.Parameters.AddWithValue("@DepositAmount", deposit.DepositAmount);
                cmd.Parameters.AddWithValue("@ContributorId", deposit.ContributorId);
                connection.Open();
                cmd.ExecuteNonQuery();

            }
        }
        public void AddContributor(Contributors newContributor)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Contributor (FirstName,LastName, Date, AlwaysInclude, Cell) " +
                    "VALUES(@FirstName, @LastName, GetDate(), @AlwaysInclude, @Cell)";
                cmd.Parameters.AddWithValue("@FirstName", newContributor.FirstName);
                cmd.Parameters.AddWithValue("@LastName", newContributor.LastName);
                cmd.Parameters.AddWithValue("@AlwaysInclude", newContributor.AlwaysInclude);
                cmd.Parameters.AddWithValue("@Cell", newContributor.Cell);
                connection.Open();
                cmd.ExecuteNonQuery();


            }
        }
        public IEnumerable<Contributors> GetContibutors()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * From Contributor";
                connection.Open();
                List<Contributors> results = new List<Contributors>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new Contributors
                    {
                        Id = (int)reader["Id"],
                        DateJoined = (DateTime)reader["Date"],
                        Cell = (string)reader["Cell"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        AlwaysInclude = (bool)reader["AlwaysInclude"],

                    });
                }
                return results;
            }
        }
        public Contributors GetContributor(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * From Contibutor C Left Join Deposit D On C.@Id = D.ContributorId";
                cmd.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                return new Contributors
                {
                    Id = (int)reader["Id"],
                    DateJoined = (DateTime)reader["Date"],
                    Cell = (string)reader["Cell"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"],
                    AlwaysInclude = (bool)reader["AlwaysInclude"],
                    Deposit = new Deposit
                    {
                        Date = (DateTime)reader["Date"],
                        DepositAmount = (int)reader["DepositAmount"],
                        ContributorId = (int)reader["ContributorId"]

                    }


                };
            }
        }
        public Simcha GetSimcha(int id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT * From Simcha where Id = @Id";
                cmd.Parameters.AddWithValue("@Id", id);
                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                return new Simcha
                {
                    Id = (int)reader["Id"],
                    Date = (DateTime)reader["Date"],
                    Name = (string)reader["Name"],

                };
            }
        }
        public IEnumerable<Simcha> GetAllSimchas()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                List<Simcha> simchas = new List<Simcha>();
                cmd.CommandText = @"SELECT * From Simcha";
                connection.Open();

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var simcha = new Simcha();
                    simcha.Date = (DateTime)reader["Date"];
                    simcha.Name = (string)reader["Name"];
                    simcha.Id = (int)reader["Id"];
                    // simcha.Total = Total(simcha.Id);
                    simchas.Add(simcha);
                }
                return simchas;
            }

        }
        public void Update(Contributors contributor)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"update Contributor Set FirstName = @FirstName, LastName = @LastName, Date = GetDate()," +
                "AlwaysInclude = @AlwaysInclude, Cell = @Cell Where Id = @Id";
                cmd.Parameters.AddWithValue("@FirstName", contributor.FirstName);
                cmd.Parameters.AddWithValue("@LastName", contributor.LastName);
                cmd.Parameters.AddWithValue("@AlwaysInclude", contributor.AlwaysInclude);
                cmd.Parameters.AddWithValue("@Cell", contributor.Cell);
                cmd.Parameters.AddWithValue("@Id", contributor.Id);
                connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public int GetDonationAmount(int contributorId)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT Sum(Amount) from Donation Where ContributorId = @ContributorId";
                cmd.Parameters.AddWithValue("@ContributorId", contributorId);
                connection.Open();
                var Total = cmd.ExecuteScalar();
                int total;
                if (Total == DBNull.Value)
                {
                    total = 0;
                }
                else
                {
                    total = (int)Total;
                }
                return total;

            }



        }
        public IEnumerable<Contributors> GetContributorsForSimcha(int Id)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT FirstName, LastName From Contributor C Join Donation D on C.Id = D.ContributorId where SimchaId = @Id";
                cmd.Parameters.AddWithValue("@Id", Id);
                connection.Open();
                List<Contributors> results = new List<Contributors>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new Contributors
                    {
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                    });
                }
                return results;
            }
        }
        public int Total(int simchaId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT COALESCE(Sum(Amount), 0) from Donation Where SimchaId = @SimchaId";
                cmd.Parameters.AddWithValue("@SimchaId", simchaId);
                connection.Open();
                return (int)cmd.ExecuteScalar();
            }

        }
        public int GetTotalBalance()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT Sum(DepositAMount) FROM Deposit";
                connection.Open();
                var Total = cmd.ExecuteScalar();
                int total;
                if (Total == DBNull.Value)
                {
                    total = 0;
                }
                else
                {
                    total = (int)Total;
                }
                return total;

            }


        }
        public int GetContibutorsCount()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT Count(*) from Contributor";
                connection.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
        public int GetContibutorsCountForSimcha(int simchaId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT COunt(*) From Contributor C Join Donation D on C.Id = D.ContributorId where SimchaId = @SimchaId";
                cmd.Parameters.AddWithValue("@SimchaId", simchaId);
                connection.Open();
                return (int)cmd.ExecuteScalar();
            }
        }
        public int GetDepositAmount(int contributorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT Sum(DepositAmount) from Deposit Where ContributorId = @ContributorId";
                cmd.Parameters.AddWithValue("@ContributorId", contributorId);
                connection.Open();
                var Total = cmd.ExecuteScalar();
                int total;
                if (Total == DBNull.Value)
                {
                    total = 0;
                }
                else
                {
                    total = (int)Total;
                }
                return total;
            }

        }
        public int GetBalance(int contributorId)
        {
            int Donation = GetDonationAmount(contributorId);
            int Balance = GetDepositAmount(contributorId);
            return Balance - Donation;
        }
        public string GetContributorName(int contributorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT FirstName , LastName FROM contributor Where Id = @ContributorId";
                cmd.Parameters.AddWithValue("@ContributorId", contributorId);
                connection.Open();

                return (string)cmd.ExecuteScalar();

            }
        }
        public IEnumerable<Deposit> GetDepositForContributor(int contributorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT DepositAmount , Date From Deposit WHere ContributorId = @ContributorId";
                cmd.Parameters.AddWithValue("@ContributorId", contributorId);
                connection.Open();
                List<Deposit> results = new List<Deposit>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new Deposit
                    {
                        Date = (DateTime)reader["Date"],
                        DepositAmount = (int)reader["DepositAmount"],
                    });
                }
                return results;
            }
        }
        public IEnumerable<Donation> GetDonationForContributor(int contributorId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT Amount , Date From Donation WHere ContributorId = @ContributorId";
                cmd.Parameters.AddWithValue("@ContributorId", contributorId);
                connection.Open();
                List<Donation> results = new List<Donation>();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new Donation
                    {

                        DonationAmount = (int)reader["Amount"],
                        Date = (DateTime)reader["Date"]
                    });
                }
                return results;
            }
        }
        public void UpdateDonations(List<SimchaDonation> donations, int simchaId)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            using (SqlCommand cmd = connection.CreateCommand())
            {
                cmd.CommandText = "Delete from Donation Where SimchaId = @SimchaId";
                cmd.Parameters.AddWithValue("@SimchaId", simchaId);
                connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                foreach (SimchaDonation d in donations)
                {
                    cmd.Parameters.Clear();
                    if (d.Included)
                    {
                        cmd.CommandText = @"Insert into Donation (Amount, SimchaId, ContributorId, Date) Values(@Amount,@SimchaId, @ContributorId, GetDate())";
                        cmd.Parameters.AddWithValue("@Amount", d.Amount);
                        cmd.Parameters.AddWithValue("@SimchaId", simchaId);
                        cmd.Parameters.AddWithValue("@ContributorId", d.ContributorId);
                        cmd.ExecuteNonQuery();
                    }



                }
            }
        }

    }
}




