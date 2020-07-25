using System;
namespace Project.Common
{
    public class ResultMessage
    {
        public bool Status { get; set; }
        public string Message { get; set; }


        public ResultMessage()
        {

        }
        public ResultMessage(bool status, string message)
        {
            Status = status;
            Message = message;
        }
    }
}
