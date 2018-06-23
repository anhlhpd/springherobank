using System;

namespace SpringHeroBank.entity
{
    public class SpringHeroTransactionException: Exception
    {
        public SpringHeroTransactionException(string message) : base(message)
        {
        }
    }
}