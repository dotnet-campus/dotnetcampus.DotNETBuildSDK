using System;

namespace Packaging.DebUOS.Exceptions;

public class PackagingException : Exception
{
    public PackagingException()
    {
    }

    public PackagingException(string? message) : base(message)
    {
    }

    public PackagingException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
