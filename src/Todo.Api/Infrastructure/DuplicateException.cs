namespace Todo.Api.Infrastructure;

public class DuplicateException : Exception
{
    public DuplicateException(Exception ex) : base(ex.Message, ex)
    {
    }
}