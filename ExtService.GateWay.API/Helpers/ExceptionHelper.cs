namespace ExtService.GateWay.API.Helpers
{
    public static class ExceptionHelper
    {
        public static string BuildExceptionMessage(this Exception exception, string headerMessage)
        {
            return $"{headerMessage}.\nException: {exception.GetExceptionInfo()}";
        }

        private static string GetExceptionInfo(this Exception exception)
        {
            if (exception.InnerException != null)
            {
                return $"\n\tMessage: {exception.Message}\n\tStackTrace: {exception.StackTrace}\nInnerException: {exception.InnerException.GetExceptionInfo()}";
            }
            else
            {
                return $"\n\tMessage: {exception.Message}\n\tStackTrace: {exception.StackTrace}";
            }
        }
    }
}
