namespace StuDin.Database;

using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using StuDin.Models;

public class DataRepository
{
     SqlConnection _connection = null;
     SqlCommand _command = null;

   private static IConfiguration Configuration { get; set; }
    //Call the Connection String

    private string GetConnectionString()
    {
        var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        Configuration = builder.Build();
        return Configuration.GetConnectionString("StuDinDatabase");
    }

    //method for models

    public List<CompanyModel> GetAll()
    {

       List<CompanyModel> companyModels = new List<CompanyModel>();

        using(_connection = new SqlConnection(GetConnectionString()))
        {
            _command = _connection.CreateCommand();
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = "[DBO].[usp_Get_Company]";
            _connection.Open();
            SqlDataReader reader = _command.ExecuteReader();
            while (reader.Read())
            {
                CompanyModel company = new CompanyModel();

                company.Id = Convert.ToInt32(reader["ID"]);
                company.CompanyName = reader["CompanyName"].ToString();
                company.ContactPerson = reader["ContactPerson"].ToString();
                company.Email = reader["Email"].ToString();
                company.PhoneNumber = Convert.ToDouble(reader["PhoneNumber"]);
                company.Address = reader["Address"].ToString();
                company.Country = reader["Country"].ToString();

                companyModels.Add(company);
            }
            _connection.Close();

        }
        return companyModels;
        
    }
    public bool  insert123(CompanyModel model)
    {
        int id = 3;
        using (_connection = new SqlConnection(GetConnectionString()))
        {
            _command = _connection.CreateCommand();
            _command.CommandType = CommandType.StoredProcedure;
            _command.CommandText = "[DBO].[usp_Insert_Company]";
            _command.Parameters.AddWithValue("@CompanyName", model.CompanyName);
            _command.Parameters.AddWithValue("@ContactPerson", model.ContactPerson);
            _command.Parameters.AddWithValue("@Email", model.Email);
            _command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
            _command.Parameters.AddWithValue("@Address", model.Address);

            try
            {
                _command.Parameters.AddWithValue("@Country", model.Country);
                _connection.Open();
                id = _command.ExecuteNonQuery();
                Console.WriteLine("Company inserted successfully.");
                _connection.Close();
            }
            catch (SqlException ex) {
                Console.WriteLine("SQL Error: " + ex.Message);
            }

        }
        return id>0 ? true : false; 
    }

 
     
    }
