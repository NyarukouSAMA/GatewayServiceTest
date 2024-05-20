namespace ExtService.GateWay.API.Helpers
{
    public static class HttpClientExtentions
    {
        public static async Task<string> GetErrorMessageAsync(this HttpResponseMessage httpResponse,
            string messageHeader)
        {
            string errorResponseMessage = null;

            try
            {
                errorResponseMessage = await httpResponse.Content.ReadAsStringAsync();
            }
            catch { }

            if (string.IsNullOrEmpty(errorResponseMessage))
            {
                errorResponseMessage = httpResponse.ReasonPhrase;
            }

            return string.IsNullOrEmpty(errorResponseMessage)
                ? $@"{messageHeader}
Status code: 
            {httpResponse.StatusCode}"
                : $@"{messageHeader}
Status code: {httpResponse.StatusCode}
ErrorMessage: {errorResponseMessage}";
        }
    }
}
