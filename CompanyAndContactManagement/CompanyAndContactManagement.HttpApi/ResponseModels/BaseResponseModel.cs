using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAndContactManagement.HttpApi.ResponseModels
{
    public class BaseResponseModel
    {
        public BaseResponseModel()
        {
            Success =true;
        }
        public bool Success { get; set; }
        public String Message { get; set; }

        public void SetException(Exception exception)
        { 
            Success = false; 
            Message = exception.Message;
        }
    }
}
