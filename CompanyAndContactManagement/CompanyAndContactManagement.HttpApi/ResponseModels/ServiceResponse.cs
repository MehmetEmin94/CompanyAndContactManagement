using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyAndContactManagement.HttpApi.ResponseModels
{
    public class ServiceResponse<T>: BaseResponseModel
    {
        public T Value { get; set; }
    }
}
