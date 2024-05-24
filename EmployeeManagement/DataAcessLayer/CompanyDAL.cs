using EmployeeManagement.Models;
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


        public bool IsEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("CheckEmail_Company", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }


        public bool IsPhoneNumberExists(string phoneNumber)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("CheckPhoneNumber_Company", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
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
                                    Areas= reader["Area"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Country = reader["Country"].ToString(),
                                   SelectedAreas = new List<string>()
                                };
                                companies.Add(company);
                            }
                            //string areas = reader["Area"].ToString();
/*                            if (!string.IsNullOrEmpty(areas))
                            {
                                string[] areaNames = areas.Split(',');
                                foreach (string areaName in areaNames)
                                {
                                    company.SelectedAreas.Add(areaName.Trim());  // Add area name to the list
                                }
                            }*/

                            /*                      // Add the area to the company's selected areas list
                                                  string areas = reader["Area"].ToString();
                                                  if (!string.IsNullOrEmpty(areas))
                                                  {
                                                      string[] areaIds = areas.Split(','); // Split the areas string into individual area IDs
                                                      foreach (string areaId in areaIds)
                                                      {
                                                          if (int.TryParse(areaId, out int parsedAreaId))
                                                          {
                                                              company.SelectedAreas.Add(parsedAreaId);
                                                          }
                                                      }
                                                  }*/
                            /*   // Add the area to the company's selected areas list
                               int areaId = Convert.ToInt32(reader["Area_Id"]);
                               if (areaId != 0) // Check if the areaId is not null
                               {
                                   company.SelectedAreas.Add(areaId);
                               }*/




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



        public CompanyModel GetCompanyById(int id)
        {
            CompanyModel companyModel = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetCompanyById_New", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            companyModel = new CompanyModel
                            {
                                Id = Convert.ToInt32(reader["CompanyID"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                ContactPerson = reader["ContactPerson"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Address = reader["Address"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Areas = reader["Area"].ToString(),
                                Country = reader["Country"].ToString(),
                                SelectedAreas = new List<string>()

                            };
                        }
                    /*    if (reader.NextResult())//advance the reader to the next result set.
                                                //It returns true if there are more result sets; otherwise, it returns false.
                        {
                            while (reader.Read())
                            {
                                companyModel.SelectedAreas.Add(reader["Area_Id"].ToString());
                            }
                        }*/
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return companyModel;
        }



        public bool UpdateCompany(CompanyModel company, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand command = new SqlCommand("UpdateCompany_New", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@CompanyID", company.Id);
                        command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
                        command.Parameters.AddWithValue("@ContactPerson", company.ContactPerson);
                        command.Parameters.AddWithValue("@Gender", company.Gender);

                        // Create and populate the DataTable for Areas
                        DataTable areasTable = new DataTable();
                        areasTable.Columns.Add("Area_Id", typeof(int));
                        foreach (var areaId in company.SelectedAreas)
                        {
                            areasTable.Rows.Add(areaId);
                        }

                        SqlParameter areasParameter = new SqlParameter("@Area", SqlDbType.Structured)
                        {
                            TypeName = "dbo.AreaTableType",
                            Value = areasTable
                        };

                        command.Parameters.Add(areasParameter);
                        command.Parameters.AddWithValue("@Email", company.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", company.PhoneNumber);
                        command.Parameters.AddWithValue("@Address", company.Address);
                        command.Parameters.AddWithValue("@Country", company.Country);

                        connection.Open();
                        int rowsAffected = command.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                  errorMessage = "An error occurred while updating the company.";
                
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = "An unexpected error occurred.";
                Console.WriteLine(ex.ToString());
                return false;
            }
        }





    }
}











