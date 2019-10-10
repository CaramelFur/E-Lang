using System;

namespace E_Lang
{
  public class ELangException : Exception
  {
    public ELangException()
    : base() { }

    public ELangException(string message)
        : base(message) { }

    public ELangException(string format, params object[] args)
        : base(string.Format(format, args)) { }

    public ELangException(string message, Exception innerException)
        : base(message, innerException) { }

    public ELangException(string format, Exception innerException, params object[] args)
        : base(string.Format(format, args), innerException) { }
  }

}