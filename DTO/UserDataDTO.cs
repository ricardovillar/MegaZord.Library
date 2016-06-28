using System;

namespace MegaZord.Library.DTO
{
    [Serializable]
    public class UserDataDTO 
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Roles { get; set; }
        public long Pessoa_ID { get; set; }
    }
}
