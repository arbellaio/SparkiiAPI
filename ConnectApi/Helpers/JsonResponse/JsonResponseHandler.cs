using Newtonsoft.Json;

namespace ConnectApi.Helpers.JsonResponse
{
    public class JsonLoginResponseHandler
    {
        public string Token { get; set; }
        public JsonResponseHandler Response { get; set; }
    }


    public class JsonResponseHandler
    {
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string Warning { get; set; }
      
        // other fields
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}