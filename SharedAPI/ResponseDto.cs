using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SharedAPI
{
    public class ResponseDto<T> where T : class
    {
        public bool IsSuccessful { get; set; }

        public int StatusCode { get; set; }

        public T Data { get; set; }

        public ICollection<string>? Errors { get; set; }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
