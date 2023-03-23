using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Bussiness.common
{
    public static class ResponseMessage
    {
        public static string Success = "Success";
        public static string UserOrPasswordIncorrect = "Username or password is incorrect";
        public static string UsernameDuplicate = "Username is already taken";
        public static string UserNotFound = "Username not found";
        public static string RegisterSuccess = "Register Success";
        public static string RegisterFail = "Register Fail";
        public static string InvalidUserName = "Invalid user name";
    }
}
