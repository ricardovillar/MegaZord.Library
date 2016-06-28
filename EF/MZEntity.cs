using System.Runtime.Serialization;
using MegaZord.Library.Interfaces;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;
using MegaZord.Library.DataAnnotation;

namespace MegaZord.Library.EF
{
    [JsonObject(IsReference = true)]
    public class MZEntity : IMZEntity
    {
        [DataMember]
        public long ID { get; set; }

        [JsonIgnore]
        public bool IsNew
        {
            get { return ID == 0; }
        }
    }

    #region Entidade Básicas do MegaZord

    [Table("Parametros", Schema = "MZ")]
    [JsonObject(IsReference = true)]
    public class MZParametro : MZEntity
    {
        [MZRequired("Nome do Parâmetro")]
        public string NomeParametro { get; set; }

        [MZRequired("Valor do Parâmetro")]
        public string ValorParametro { get; set; }
    }
    #endregion
}

