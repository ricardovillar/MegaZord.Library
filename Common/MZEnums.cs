using System;
namespace MegaZord.Library.Common
{
    public enum MZAuditLogOperation
    {
        InsertAction = 'I',
        UpdateAction = 'U',
        DeleteAction = 'D',

    }


    public enum LogType
    {
        Debug = 2,
        Error = 4,
        SQL = 8,
        Audit = 16,
        Generic = 32

    }


    public enum TipoPessoa 
    {
       
        Fisica = 1,
        Juridia = 2
    }

   
    public enum PapelPessoa 
    {
        Administrador = 1,
        Comprador = 2,
        Fornecedor = 4,
        CompradorFornecedor = 8
    }
}
