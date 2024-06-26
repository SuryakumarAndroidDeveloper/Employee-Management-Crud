﻿using EmployeeManagement.Models;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EmployeeManagement.DataAcessLayer
{
    public class EmployeDAL
    {
      

        private readonly IConfiguration _configuration;
       
        public EmployeDAL(IConfiguration configuration)
        {
            this._configuration = configuration;
        }



        public List<CompanyModel> GetCompanyNameData()
        {
            List<CompanyModel> companyData = new List<CompanyModel>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("GetAllCompanyName_New", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CompanyModel model = new CompanyModel();
                        model.Id = Convert.ToInt32(reader["CompanyID"]);
                        model.CompanyName = reader["CompanyName"].ToString();
                        companyData.Add(model);
                    }
                }
            }

            return companyData;
        }


        public bool IsEmailExists(string email)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("CheckEmail", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Email", email);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        public bool IsMobileExists(string mobile)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                SqlCommand command = new SqlCommand("CheckMobile", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Mobile", mobile);
                connection.Open();
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }










        public bool InsertEmployee(EmployeModel employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("InsertEmployee_New", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CompanyName", employee.CompanyName);
                        cmd.Parameters.AddWithValue("@Name", employee.Name);
                        cmd.Parameters.AddWithValue("@Email", employee.Email);
                        cmd.Parameters.AddWithValue("@Mobile", employee.Mobile);
                        cmd.Parameters.AddWithValue("@Address", employee.Address);
                        cmd.Parameters.AddWithValue("@City", employee.City);
                        cmd.Parameters.AddWithValue("@Pincode", employee.Pincode);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public List<EmployeModel> GetAllEmployees()
        {
            List<EmployeModel> employees = new List<EmployeModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetAllEmployee_New", connection))
                   
                    {
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            EmployeModel employee = new EmployeModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = Convert.ToInt32(reader["Pincode"])
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return employees;
        }





/*        public List<EmployeModel> GetActiveEmployees()
        {
            List<EmployeModel> employees = new List<EmployeModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetActiveEmployees", connection))

                    {
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            EmployeModel employee = new EmployeModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = Convert.ToInt32(reader["Pincode"])
                            };
                            employees.Add(employee);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return employees;
        }*/






            public List<EmployeModel> GetActiveEmployees()
            {
                List<EmployeModel> activeEmployees = new List<EmployeModel>();

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand command = new SqlCommand("GetActiveEmployees_New", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            EmployeModel employee = new EmployeModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = Convert.ToInt32(reader["Pincode"])
                            };
                            activeEmployees.Add(employee);
                        }
                    }
                }

                return activeEmployees;
            }







    public void DeactivateEmployee(int id)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    SqlCommand command = new SqlCommand("DeactivateEmployee_New", connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    command.Parameters.AddWithValue("@Id", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        








    public bool DeleteEmployee(int employeeId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("DeleteEmployee_New", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", employeeId);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }




        public EmployeModel GetById(int id)
        {
            EmployeModel employeModel = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetEmployById_New", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            employeModel = new EmployeModel
                            {   
                                Id = Convert.ToInt32(reader["Id"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = Convert.ToInt32(reader["Pincode"]),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return employeModel;
        }

        public EmployeModel GetEmpById(int id)
        {
            EmployeModel employeModel = null;
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("GetEmployeeById_New", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            employeModel = new EmployeModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                //CompanyID = Convert.ToInt32(reader["CompanyID"]),
                                CompanyName = reader["CompanyName"].ToString(),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                Mobile = reader["Mobile"].ToString(),
                                Address = reader["Address"].ToString(),
                                City = reader["City"].ToString(),
                                Pincode = Convert.ToInt32(reader["Pincode"]),
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return employeModel;
        }



        public bool UpdateEmployee(EmployeModel employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    using (SqlCommand cmd = new SqlCommand("UpdateEmployee_New", connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Id", employee.Id);
                        cmd.Parameters.AddWithValue("@CompanyName", employee.CompanyName);
                        cmd.Parameters.AddWithValue("@Name", employee.Name);
                        cmd.Parameters.AddWithValue("@Email", employee.Email);
                        cmd.Parameters.AddWithValue("@Mobile", employee.Mobile);
                        cmd.Parameters.AddWithValue("@Address", employee.Address);
                        cmd.Parameters.AddWithValue("@City", employee.City);
                        cmd.Parameters.AddWithValue("@Pincode", employee.Pincode);

                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }


        public void DeleteEmployees(List<int> employeeIds)
        {
            if (employeeIds == null || employeeIds.Count == 0)
            {
                throw new ArgumentNullException(nameof(employeeIds), "The list of employee IDs cannot be null or empty.");
            }
            string idList = string.Join(",", employeeIds);

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                using (SqlCommand cmd = new SqlCommand("DeleteEmployeesByIds", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Id", idList);
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Helper method to convert list of IDs to DataTable
/*        private DataTable ConvertToDataTable(List<int> employeeIds)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Id", typeof(int));
            foreach (int id in employeeIds)
            {
                table.Rows.Add(id);
            }
            return table;
        }*/





    }
}
    

