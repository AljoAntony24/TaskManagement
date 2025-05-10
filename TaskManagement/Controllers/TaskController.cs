using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagement.Data;
using TaskManagement.Model;
using TaskManagement.Model.Common;
using TaskManagement.Services;

namespace TaskManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly DataCon _datacon;
        private readonly JwtHandler _jwtHandler;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskController( DataCon datcon, JwtHandler jwthandler, IHttpContextAccessor httpContextAccessor)
        {
            _datacon = datcon;
            _jwtHandler = jwthandler;
            _httpContextAccessor = httpContextAccessor;
        }
        #region Login

        [HttpPost("Login")]
        public async Task<string> Login(string PhoneNumber, string Password)
        {
            try
            {
                if (PhoneNumber != null && Password != null)
                {
                    var CheckIsCorrect = _datacon.Task_User.Where(x => x.Phone == PhoneNumber && x.Password == Password).Select(x => new
                    {
                        x.User_ID,
                        x.User_Role
                    }).FirstOrDefault();
                    if (CheckIsCorrect != null)
                    {

                        var User_Settings = new User_Settings()
                        {
                            User_ID = CheckIsCorrect.User_ID.ToString(),
                            Role = CheckIsCorrect.User_Role.ToString(),
                            Role_Type = CheckIsCorrect.User_Role == 1 ? "Admin" : "User"
                        };

                        string token = _jwtHandler.Generatetoken(User_Settings);
                        var UpdUser = _datacon.Task_User.Where(y => y.User_ID == CheckIsCorrect.User_ID ).FirstOrDefault();
                        if (UpdUser != null)
                        {
                            UpdUser.Token = token;
                            UpdUser.EnteredAt = DateTime.Now;
                            _datacon.SaveChanges();
                        }
                        var res = new
                        {
                            Status = (int)HttpStatusCode.OK,
                            Message = "Login Successfully",
                            Token = token
                        };
                        return JsonConvert.SerializeObject(res);
                    }
                    else
                    {
                        var res = new
                        {
                            Status = (int)HttpStatusCode.BadRequest,
                            Message = "Login Failed"
                        };
                        return JsonConvert.SerializeObject(res);
                    }
                }
                else
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Please Provide Mobile and Pass"
                    };
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
                var res = new
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"{ex}"
                };
                return JsonConvert.SerializeObject(res);
            }
        }
        #endregion

        #region AddTaskDetails
        // This api for using AddTaskDetails
        [HttpPost("AddTaskDetails")]
        public async Task<string>AddTask(string TaskName,string TaskDetails)
        {
            try
            {
                ClaimsIdentity? claimsIdentity = _httpContextAccessor.HttpContext?.User.Identity as ClaimsIdentity;
                string? userID = claimsIdentity?.FindFirst("User_ID")?.Value;
                string? role_id = claimsIdentity?.FindFirst("Role_Type")?.Value;
                if (TaskName != null)
                {
                    var AddTask = new Task_Master()
                    {
                        TaskName = TaskName,
                        TaskDetails = TaskDetails,
                        EnteredAt = DateTime.Now,
                        EnteredBy = long.Parse(userID)
                    };

                    await _datacon.AddAsync(AddTask);
                    await _datacon.SaveChangesAsync();

                    var res = new
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Task Details Added Succsessfully"
                    };
                    return JsonConvert.SerializeObject(res);
                }
                else
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Enter The TaskName"
                    };
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch(Exception ex)
            {
                var res = new
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"{ex}"
                };
                return JsonConvert.SerializeObject(res);
            }
        }
        #endregion

        #region GetTaskDetails
        // This Api for using If Which task is you Selected To Assign
        [HttpGet("GetTaskDetails")]
        public async Task<string>GetTaskDetails()
        {
            try
            {
                var GetDet = _datacon.Task_Master.Select(x => new
                {
                    x.TaskID,
                    x.TaskName
                }).ToList();
                if (GetDet.Count > 0)
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Details Fetched Successfully",
                        Data = GetDet
                    };
                    return JsonConvert.SerializeObject(res);
                }
                else
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Not Found",
                        Data = GetDet
                    };
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch(Exception ex)
            {
                var res = new
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"{ex}"
                };
                return JsonConvert.SerializeObject(res);
            }
        }
        #endregion

        #region GetUserDet
        // This Api for using Which User You Selected To Assigned the Task
        [HttpGet("GetUserDetails")]
        public async Task<string>GetUserDetails()
        {
            try
            {

                var GetUser = _datacon.Task_User.Where(x => x.User_Role == 2).Select(x => new
                {
                    x.User_ID,
                    x.User_Name
                }).ToList();
                if(GetUser.Count > 0)
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Details Fetched Successfully",
                        Data = GetUser
                    };
                    return JsonConvert.SerializeObject(res);
                }
                else
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Not Found"
                    };
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
                var res = new
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"{ex}"
                };
                return JsonConvert.SerializeObject(res);
            }
        }
        #endregion

        #region AssignUser
        // This Api For using Assigned Tasks to User
        [HttpPut("AssignedTask")]
        public async Task<string> AssignUser(int TaskID,int AssignedID,DateTime DueDate)
        {
            try
            {
                var AssignUser = _datacon.Task_Master.Where(x => x.TaskID == TaskID).FirstOrDefault();
                if(AssignUser != null)
                {
                    AssignUser.AssignedTo = AssignedID;
                    AssignUser.Status = 1;
                    AssignUser.DueDate = DueDate;

                    await _datacon.SaveChangesAsync();
                }
                var res = new
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = "Task Assigned Successfully"
                };
                return JsonConvert.SerializeObject(res);
            }
            catch(Exception ex)
            {
                var res = new
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"{ex}"
                };
                return JsonConvert.SerializeObject(res);
            }
        }
        #endregion

        #region GetTaskDetils By ID
        // This Api For using Get Task Details By TaskID
        [HttpGet("{id}")]
        public async Task<string>GetAssignedTaskDetails(int id)
        {
            try
            {
                var GetDet = _datacon.Task_Master.Where(x => x.TaskID == id).Select(x => new
                {
                    x.TaskName,
                    x.TaskDetails,
                    AssignedTo = _datacon.Task_User.FirstOrDefault(y => y.User_ID == x.AssignedTo).User_Name,
                    Status = x.Status == 1 ? "Pending" : x.Status == 2 ? "Started" : "Completed",
                    DueDate = Convert.ToDateTime(x.DueDate).ToString("yyyy-MM-dd"),
                    EnteredBy = _datacon.Task_User.FirstOrDefault(y => y.User_ID == x.EnteredBy).User_Name,
                    x.EnteredAt
                }).FirstOrDefault();
                if(GetDet != null)
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Details Fetched Successfully",
                        Data = GetDet
                    };
                    return JsonConvert.SerializeObject(res);
                }
                else
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Not Found"
                    };
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
                var res = new
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"{ex}"
                };
                return JsonConvert.SerializeObject(res);
            }
        }
        #endregion

        #region Get tasks assigned to a specific user
        // This Api for using Get Task Assigned To a Specific User
        [HttpGet("user/{userId}")]
        public async Task<string>GetTaskDetailInSpecificUser(int userId)
        {
            try
            {
                var GetTask = _datacon.Task_Master.Where(x => x.AssignedTo == userId).Select(x => new
                {
                    x.TaskName,
                    x.TaskDetails,
                    Status = x.Status == 1 ? "Pending" : x.Status == 2 ? "Started" : "Completed",
                    DueDate = Convert.ToDateTime(x.DueDate).ToString("yyyy-MM-dd"),
                    EnteredBy = _datacon.Task_User.FirstOrDefault(y => y.User_ID == x.EnteredBy).User_Name,
                    x.EnteredAt
                }).ToList();
                if(GetTask.Count > 0)
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "Details Fetched Successfully",
                        Data = GetTask
                    };
                    return JsonConvert.SerializeObject(res);
                }
                else
                {
                    var res = new
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Message = "Not Found"
                    };
                    return JsonConvert.SerializeObject(res);
                }
            }
            catch (Exception ex)
            {
                var res = new
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = $"{ex}"
                };
                return JsonConvert.SerializeObject(res);
            }
        }
        #endregion
    }
}
