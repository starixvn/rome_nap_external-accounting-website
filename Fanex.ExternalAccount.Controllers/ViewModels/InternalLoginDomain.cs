namespace Fanex.ExternalAccount.Controllers.ViewModels
{
    using System;

    [Serializable]
    public class InternLoginDomain
    {
        public string Domain { get; set; }

        public bool IsInUsed { get; set; }
    }
}