using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaZord.Library.Helpers
{
    public static class MZHelperSerialize
    {
        /// <summary>
        /// Retorna um objeto serializado em JSON
        /// </summary>
        /// <typeparam name="T">Tipo do Objeto a ser serializado</typeparam>
        /// <param name="obj">Objeto a ser serializado</param>
        /// <returns>String contendo o objeto</returns>
        public static string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);

        }

        /// <summary>
        /// Deserializa um objeto
        /// </summary>
        /// <typeparam name="T">Tipo do OBjecto</typeparam>
        /// <param name="jsonObject">String Json do OBjeto</param>
        /// <returns>Objeto</returns>
        public static T Deserialize<T>(string jsonObject) where T : class
        {
            return JsonConvert.DeserializeObject<T>(jsonObject);
        }

    }
}
