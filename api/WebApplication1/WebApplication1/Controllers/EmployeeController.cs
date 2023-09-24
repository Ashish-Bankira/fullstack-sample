using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Models;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json.Serialization;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }


        //We write an API method to get all data from the Employee table

        [HttpGet]

        public JsonResult Get()
        {
            //Writing raw queries, instead you can use stored procedure or Entity Framework

            string query = @"
                            select EmployeeId, EmployeeName, Department,
                            convert(varchar(10), DateOfJoining,120) as DateOfJoining, PhotoFileName
                            from
                            dbo.Employee
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpPost]
        public JsonResult Post(Employee emp)    //We will send Employee Object to our Post method in our FORM body
        {
            //Writing raw queries, instead you can use stored procedure or Entity Framework

            string query = @"
                            insert into dbo.Employee
                            (EmployeeName,Department,DateOfJoining,PhotoFileName)
                            values (@EmployeeName,@Department,@DateOfJoining,@PhotoFileName)
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Added Succcesfully");   //Returning message which can be seen in Postman
        }




        [HttpPut]
        public JsonResult Put(Employee emp)    //Put method to update records
        {
            //Writing raw queries, instead you can use stored procedure or Entity Framework

            string query = @"
                            update dbo.Employee
                            set EmployeeName= @EmployeeName,
                            Department = @Department,
                            DateOfJoining = @DateOfJoining,
                            PhotoFileName = @PhotoFileName
                            where EmployeeId=@EmployeeId
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    myCommand.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommand.Parameters.AddWithValue("@Department", emp.Department);
                    myCommand.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommand.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Updated Successfully");  //Returning message which can be seen in Postman
        }



        [HttpDelete("{id}")]   // To accept the id in URL to delete
        public JsonResult Delete(int id)    //Delete method to delete records
        {
            //Writing raw queries, instead you can use stored procedure or Entity Framework

            string query = @"
                            delete from dbo.Employee
                            where EmployeeId=@EmployeeId
                            ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@EmployeeId", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult("Deleted Successfully");   //Returning message which can be seen in Postman
        }





        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;  //Using dependency injection to get the application path to the photos folder


                using(var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }


                return new JsonResult(filename);
            }
            catch (Exception) 
            {

                return new JsonResult("anonymous.png");
            }
        }



    }
}
