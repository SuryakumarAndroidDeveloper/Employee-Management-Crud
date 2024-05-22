﻿using EmployeeManagement.Models;
using System.Data;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmployeeManagement.DataAcessLayer
{
    public class CompanyDAL
    {
        private readonly IConfiguration _configuration;

        public CompanyDAL(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
            public List<CountryModel> GetCountryData()
            {
                List<CountryModel> countryData = new List<CountryModel>();

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand command = new SqlCommand("GetAllCountry", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                        CountryModel model = new CountryModel();
                            model.Id = Convert.ToInt32(reader["Id"]);
                            model.Country = reader["Country"].ToString();
                            countryData.Add(model);
                        }
                    }
                }

                return countryData;
            }

        public List<AreaModel> GetAreaData()
        {
            List<AreaModel> areaData = new List<AreaModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("GetAllArea", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AreaModel model = new AreaModel();
                        model.Area_Id = Convert.ToInt32(reader["Area_Id"]);
                        model.Area = reader["Area"].ToString();
                        areaData.Add(model);
                    }
                }
            }

            return areaData;
        }

        public bool IsCompanyExists(string companyName)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("CheckCompany_New", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@CompanyName", companyName);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public bool SaveCompany(CompanyModel model, out string errorMessage)
            {
            errorMessage = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand command = new SqlCommand("SaveCompany_New", connection);
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CompanyName", model.CompanyName);
                    command.Parameters.AddWithValue("@ContactPerson", model.ContactPerson);
                    command.Parameters.AddWithValue("@Gender", model.Gender);
                    // Create and populate the DataTable for Areas
                    DataTable areasTable = new DataTable();
                    areasTable.Columns.Add("Area_Id", typeof(int));
                    foreach (var areaId in model.SelectedAreas)
                    {
                        areasTable.Rows.Add(areaId);
                    }

                    SqlParameter areasParameter = new SqlParameter("@Area", SqlDbType.Structured)
                    {
                        TypeName = "dbo.AreaTableType",
                        Value = areasTable
                    };

                    command.Parameters.Add(areasParameter);

                    command.Parameters.AddWithValue("@Email", model.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                    command.Parameters.AddWithValue("@Address", model.Address);
                    command.Parameters.AddWithValue("@Country", model.Country);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                return true;
            }
            catch(SqlException ex) 
            {
                if (ex.Number == 2627) // Unique constraint error number
                {
                    errorMessage = "A company with this name already exists.";
                    
                }
                else
                {
                    errorMessage = "An error occurred while saving the company.";
                }

                return false;

            }
            }


        public List<CompanyModel> GetAllCompanyWithAreas()
        {
            List<CompanyModel> companies = new List<CompanyModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAllCompanyWithAreas", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            int companyId = Convert.ToInt32(reader["CompanyID"]);
                            // Check if the company already exists in the list
                            CompanyModel company = companies.FirstOrDefault(c => c.Id == companyId);

                            // If the company doesn't exist, create a new one and add it to the list
                            if (company == null)
                            {
                                company = new CompanyModel
                                {
                                    Id = companyId,
                                    CompanyName = reader["CompanyName"].ToString(),
                                    ContactPerson = reader["ContactPerson"].ToString(),
                                    Gender = reader["Gender"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Country = reader["Country"].ToString(),
                                    SelectedAreas = new List<int>()
                                };
                                companies.Add(company);
                            }

                            // Add the area to the company's selected areas list
                            int areaId = Convert.ToInt32(reader["Area_Id"]);
                            if (areaId != 0) // Check if the areaId is not null
                            {
                                company.SelectedAreas.Add(areaId);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return companies;
        }









    }
}











